using System.Text.Json;
using DishMeAPP.Models;

namespace DishMeAPP.Services
{
    /// <summary>
    /// Recipe storage service using JSON files 使用JSON文件的菜谱存储服务
    /// </summary>
    public class RecipeStorageService
    {
        private readonly string _recipesDirectory;
        private readonly string _recipesIndexFile;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RecipeStorageService()
        {
            _recipesDirectory = Path.Combine(FileSystem.AppDataDirectory, "Recipes");
            _recipesIndexFile = Path.Combine(_recipesDirectory, "recipes_index.json");

            // Ensure directory exists 确保目录存在
            Directory.CreateDirectory(_recipesDirectory);
        }

        /// <summary>
        /// Save recipe to storage 保存菜谱到存储
        /// </summary>
        /// <param name="recipe">Recipe to save 要保存的菜谱</param>
        /// <returns>Saved recipe with ID 带ID的已保存菜谱</returns>
        public async Task<Recipe> SaveRecipeAsync(Recipe recipe)
        {
            try
            {
                // Generate ID if new recipe 如果是新菜谱则生成ID
                if (recipe.Id == 0)
                {
                    recipe.Id = await GetNextIdAsync();
                    recipe.CreatedAt = DateTime.Now;
                }
                recipe.UpdatedAt = DateTime.Now;

                // Save individual recipe file 保存单个菜谱文件
                var recipeFile = Path.Combine(_recipesDirectory, $"recipe_{recipe.Id}.json");
                var json = JsonSerializer.Serialize(recipe, JsonOptions);
                await File.WriteAllTextAsync(recipeFile, json);

                // Update index 更新索引
                await UpdateIndexAsync();

                System.Diagnostics.Debug.WriteLine($"Recipe saved: {recipe.Name} (ID: {recipe.Id})");
                return recipe;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving recipe: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all recipes 获取所有菜谱
        /// </summary>
        /// <returns>List of recipes 菜谱列表</returns>
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            try
            {
                var recipes = new List<Recipe>();
                var recipeFiles = Directory.GetFiles(_recipesDirectory, "recipe_*.json");

                foreach (var file in recipeFiles)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var recipe = JsonSerializer.Deserialize<Recipe>(json, JsonOptions);
                        if (recipe != null)
                        {
                            recipes.Add(recipe);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error reading recipe file {file}: {ex.Message}");
                    }
                }

                // Sort by creation date descending 按创建时间降序排列
                return recipes.OrderByDescending(r => r.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting recipes: {ex.Message}");
                return new List<Recipe>();
            }
        }

        /// <summary>
        /// Get recipes by category 按分类获取菜谱
        /// </summary>
        /// <param name="category">Category name 分类名称</param>
        /// <returns>Filtered recipes 过滤后的菜谱</returns>
        public async Task<List<Recipe>> GetRecipesByCategoryAsync(string category)
        {
            var allRecipes = await GetAllRecipesAsync();
            return allRecipes.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Get recipe by ID 根据ID获取菜谱
        /// </summary>
        /// <param name="id">Recipe ID 菜谱ID</param>
        /// <returns>Recipe or null 菜谱或null</returns>
        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            try
            {
                var recipeFile = Path.Combine(_recipesDirectory, $"recipe_{id}.json");
                if (!File.Exists(recipeFile))
                    return null;

                var json = await File.ReadAllTextAsync(recipeFile);
                return JsonSerializer.Deserialize<Recipe>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting recipe {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Delete recipe 删除菜谱
        /// </summary>
        /// <param name="id">Recipe ID 菜谱ID</param>
        /// <returns>Success status 成功状态</returns>
        public async Task<bool> DeleteRecipeAsync(int id)
        {
            try
            {
                var recipeFile = Path.Combine(_recipesDirectory, $"recipe_{id}.json");
                if (File.Exists(recipeFile))
                {
                    File.Delete(recipeFile);
                    await UpdateIndexAsync();
                    System.Diagnostics.Debug.WriteLine($"Recipe deleted: ID {id}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting recipe {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get next available ID 获取下一个可用ID
        /// </summary>
        /// <returns>Next ID 下一个ID</returns>
        private async Task<int> GetNextIdAsync()
        {
            var recipes = await GetAllRecipesAsync();
            return recipes.Any() ? recipes.Max(r => r.Id) + 1 : 1;
        }

        /// <summary>
        /// Update recipes index file 更新菜谱索引文件
        /// </summary>
        private async Task UpdateIndexAsync()
        {
            try
            {
                var recipes = await GetAllRecipesAsync();
                var index = recipes.Select(r => new { r.Id, r.Name, r.Category, r.CreatedAt }).ToList();
                var json = JsonSerializer.Serialize(index, JsonOptions);
                await File.WriteAllTextAsync(_recipesIndexFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating index: {ex.Message}");
            }
        }

        /// <summary>
        /// Get storage statistics 获取存储统计信息
        /// </summary>
        /// <returns>Storage info 存储信息</returns>
        public async Task<(int RecipeCount, long StorageSize)> GetStorageStatsAsync()
        {
            try
            {
                var recipes = await GetAllRecipesAsync();
                var files = Directory.GetFiles(_recipesDirectory);
                var totalSize = files.Sum(f => new FileInfo(f).Length);

                return (recipes.Count, totalSize);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}
