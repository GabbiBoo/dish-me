namespace DishMeAPP.Models
{
    /// <summary>
    /// Recipe data model class | 菜谱数据模型类
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Recipe unique identifier | 菜谱唯一标识符
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Recipe name | 菜谱名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Cooking steps | 制作步骤
        /// </summary>
        public string Steps { get; set; } = string.Empty;

        /// <summary>
        /// Ingredients list | 用料清单
        /// </summary>
        public string Ingredients { get; set; } = string.Empty;

        /// <summary>
        /// Recipe category | 菜谱分类
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Recipe image path | 菜谱图片路径
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Creation time | 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Last modified time | 最后修改时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Estimated cooking time in minutes | 预计制作时间（分钟）
        /// </summary>
        public int? CookingTimeMinutes { get; set; }

        /// <summary>
        /// Difficulty level (1-5 stars) | 难度等级（1-5星）
        /// </summary>
        public int DifficultyLevel { get; set; } = 1;

        /// <summary>
        /// Number of servings | 份数
        /// </summary>
        public int Servings { get; set; } = 1;

        /// <summary>
        /// Whether this is a favorite recipe | 是否为收藏菜谱
        /// </summary>
        public bool IsFavorite { get; set; } = false;

        /// <summary>
        /// Recipe tags (comma separated) | 菜谱标签（用逗号分隔）
        /// </summary>
        public string? Tags { get; set; }

        /// <summary>
        /// Nutrition information notes | 营养信息备注
        /// </summary>
        public string? NutritionInfo { get; set; }

        /// <summary>
        /// Convert Recipe to Dish for display in OrderPage 将菜谱转换为菜品以在点餐页面显示
        /// </summary>
        /// <returns>Dish object 菜品对象</returns>
        public Dish ToDish()
        {
            return new Dish
            {
                Id = Id,
                Name = Name,
                Category = GetDishCategory(),
                Image = GetDisplayImagePath(),
                Description = $"{Ingredients}\n\n{Steps}".Trim()
            };
        }

        /// <summary>
        /// Get dish category mapping 获取菜品分类映射
        /// </summary>
        /// <returns>Dish category 菜品分类</returns>
        private string GetDishCategory()
        {
            return Category switch
            {
                "Chinese Cuisine" => "Chinese",
                "Western Cuisine" => "Western",
                "Desserts" => "Desserts",
                "Soups" => "Soups",
                "Vegetarian" => "Vegetarian",
                "Others" => "Others",
                _ => "Others"
            };
        }

        /// <summary>
        /// Get display image path for UI 获取用于UI显示的图片路径
        /// </summary>
        /// <returns>Image path 图片路径</returns>
        private string GetDisplayImagePath()
        {
            // If no image, return a default placeholder
            // 如果没有图片，返回默认占位符
            if (string.IsNullOrEmpty(ImagePath) || !File.Exists(ImagePath))
            {
                return "recipe_placeholder.png"; // 需要添加默认图片
            }

            return ImagePath;
        }
    }

    /// <summary>
    /// Recipe categories enumeration | 菜谱分类枚举
    /// </summary>
    public static class RecipeCategories
    {
        public static readonly List<string> Categories = new List<string>
        {
            "Chinese Cuisine",
            "Western Cuisine",
            "Desserts",
            "Soups",
            "Vegetarian",
            "Others"
        };

        /// <summary>
        /// Get default category | 获取默认分类
        /// </summary>
        public static string GetDefaultCategory() => Categories.First();

        /// <summary>
        /// Validate if category is valid | 验证分类是否有效
        /// </summary>
        public static bool IsValidCategory(string category)
        {
            return Categories.Contains(category);
        }
    }
}
