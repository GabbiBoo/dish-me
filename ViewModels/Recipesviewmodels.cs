using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DishMeAPP.Models;
using DishMeAPP.Services;
using Microsoft.Maui.Controls;

namespace DishMeAPP.ViewModels
{
    /// <summary>
    /// Recipe page view model | 菜谱页面视图模型
    /// </summary>
    public class RecipeViewModel : INotifyPropertyChanged
    {
        private Recipe _currentRecipe;
        private string _selectedCategory;
        private bool _isLoading;
        private string _recipeImagePath = string.Empty;
        private readonly RecipeStorageService _storageService;

        public RecipeViewModel()
        {
            _currentRecipe = new Recipe();
            _selectedCategory = RecipeCategories.GetDefaultCategory();
            _storageService = new RecipeStorageService();

            // Initialize commands | 初始化命令
            SaveRecipeCommand = new Command(async () => await SaveRecipeAsync(), CanSaveRecipe);
            SelectCategoryCommand = new Command(async () => await SelectCategoryAsync());

            // Initialize categories list | 初始化分类列表
            Categories = new ObservableCollection<string>(RecipeCategories.Categories);
        }

        #region Properties | 属性

        /// <summary>
        /// Current recipe object | 当前菜谱对象
        /// </summary>
        public Recipe CurrentRecipe
        {
            get => _currentRecipe;
            set
            {
                _currentRecipe = value;
                OnPropertyChanged();
                // Update related properties | 更新相关属性
                OnPropertyChanged(nameof(RecipeName));
                OnPropertyChanged(nameof(RecipeSteps));
                OnPropertyChanged(nameof(RecipeIngredients));
            }
        }

        /// <summary>
        /// Recipe name | 菜谱名称
        /// </summary>
        public string RecipeName
        {
            get => _currentRecipe.Name;
            set
            {
                if (_currentRecipe.Name != value)
                {
                    _currentRecipe.Name = value;
                    OnPropertyChanged();
                    ((Command)SaveRecipeCommand).ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Recipe steps | 制作步骤
        /// </summary>
        public string RecipeSteps
        {
            get => _currentRecipe.Steps;
            set
            {
                if (_currentRecipe.Steps != value)
                {
                    _currentRecipe.Steps = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StepsCharacterCount));
                    ((Command)SaveRecipeCommand).ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Recipe ingredients | 用料清单
        /// </summary>
        public string RecipeIngredients
        {
            get => _currentRecipe.Ingredients;
            set
            {
                if (_currentRecipe.Ingredients != value)
                {
                    _currentRecipe.Ingredients = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IngredientsCharacterCount));
                    ((Command)SaveRecipeCommand).ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Selected category | 选中的分类
        /// </summary>
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    _currentRecipe.Category = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CategoryDisplayText));
                    ((Command)SaveRecipeCommand).ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Category display text | 分类显示文本
        /// </summary>
        public string CategoryDisplayText => string.IsNullOrEmpty(_selectedCategory) ? "Select a category" : _selectedCategory;

        /// <summary>
        /// Categories list | 分类列表
        /// </summary>
        public ObservableCollection<string> Categories { get; }

        /// <summary>
        /// Whether is loading | 是否正在加载
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        /// <summary>
        /// Whether is not loading | 是否未在加载
        /// </summary>
        public bool IsNotLoading => !IsLoading;

        /// <summary>
        /// Steps character count | 制作步骤字符数量
        /// </summary>
        public string StepsCharacterCount => $"{(RecipeSteps?.Length ?? 0)}/2000";

        /// <summary>
        /// Ingredients character count | 用料清单字符数量
        /// </summary>
        public string IngredientsCharacterCount => $"{(RecipeIngredients?.Length ?? 0)}/1000";

        /// <summary>
        /// Recipe image path | 菜谱图片路径
        /// </summary>
        public string RecipeImagePath
        {
            get => _recipeImagePath ?? string.Empty;
            set
            {
                if (_recipeImagePath != value)
                {
                    _recipeImagePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasImage));
                    OnPropertyChanged(nameof(HasNoImage));
                }
            }
        }

        /// <summary>
        /// Whether has image | 是否有图片
        /// </summary>
        public bool HasImage => !string.IsNullOrEmpty(RecipeImagePath);

        /// <summary>
        /// Whether has no image | 是否没有图片
        /// </summary>
        public bool HasNoImage => !HasImage;

        #endregion

        #region Commands | 命令

        /// <summary>
        /// Save recipe command | 保存菜谱命令
        /// </summary>
        public ICommand SaveRecipeCommand { get; }

        /// <summary>
        /// Select category command | 选择分类命令
        /// </summary>
        public ICommand SelectCategoryCommand { get; }

        #endregion

        #region Private Methods | 私有方法

        /// <summary>
        /// Whether can save recipe | 是否可以保存菜谱
        /// </summary>
        private bool CanSaveRecipe()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(RecipeName) &&
                   !string.IsNullOrWhiteSpace(RecipeSteps) &&
                   !string.IsNullOrWhiteSpace(RecipeIngredients) &&
                   !string.IsNullOrWhiteSpace(SelectedCategory);
        }

        /// <summary>
        /// Save recipe | 保存菜谱
        /// </summary>
        private async Task SaveRecipeAsync()
        {
            try
            {
                IsLoading = true;

                // Update current recipe properties | 更新当前菜谱的属性
                _currentRecipe.Name = RecipeName;
                _currentRecipe.Steps = RecipeSteps;
                _currentRecipe.Ingredients = RecipeIngredients;
                _currentRecipe.Category = SelectedCategory;
                _currentRecipe.ImagePath = RecipeImagePath;
                _currentRecipe.UpdatedAt = DateTime.Now;

                if (_currentRecipe.Id == 0)
                {
                    _currentRecipe.CreatedAt = DateTime.Now;
                }

                // Save recipe to storage | 保存菜谱到存储
                var savedRecipe = await _storageService.SaveRecipeAsync(_currentRecipe);

                System.Diagnostics.Debug.WriteLine($"Recipe saved successfully: {savedRecipe.Name} (ID: {savedRecipe.Id})");

                // Show success message | 显示成功信息
                await SafeDisplayAlert("Success", "Recipe saved successfully!", "OK");

                // Reset form | 重置表单
                ResetForm();
            }
            catch (Exception ex)
            {
                await SafeDisplayAlert("Error", $"Save failed: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Select category | 选择分类
        /// </summary>
        private async Task SelectCategoryAsync()
        {
            var result = await SafeDisplayActionSheet(
                "Select a category", "Cancel", null, Categories.ToArray());

            if (!string.IsNullOrEmpty(result) && result != "Cancel")
            {
                SelectedCategory = result;
            }
        }

        /// <summary>
        /// Reset form | 重置表单
        /// </summary>
        private void ResetForm()
        {
            CurrentRecipe = new Recipe();
            SelectedCategory = RecipeCategories.GetDefaultCategory();
            RecipeImagePath = string.Empty;
        }

        /// <summary>
        /// Safely display alert dialog 安全显示警告对话框
        /// </summary>
        private async Task<bool> SafeDisplayAlert(string title, string message, string accept, string? cancel = null)
        {
            try
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    if (cancel != null)
                        return await mainPage.DisplayAlert(title, message, accept, cancel);
                    else
                    {
                        await mainPage.DisplayAlert(title, message, accept);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying alert: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Safely display action sheet 安全显示操作表
        /// </summary>
        private async Task<string?> SafeDisplayActionSheet(string title, string cancel, string? destruction, params string[] buttons)
        {
            try
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    return await mainPage.DisplayActionSheet(title, cancel, destruction, buttons);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying action sheet: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}