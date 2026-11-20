using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DishMeAPP.Models;
using DishMeAPP.Services;

namespace DishMeAPP.ViewModels
{
    /// <summary>
    /// Order page view model 订单页面视图模型
    /// </summary>
    public class OrderViewModel : INotifyPropertyChanged
    {
        private Category? _selectedCategory;
        private List<Dish> _allDishes = new();
        private readonly RecipeStorageService _recipeStorageService;
        private readonly CartService _cartService;
        private readonly UserSettingsService _userSettingsService;

        public OrderViewModel()
        {
            // Initialize collections 初始化集合
            Categories = new ObservableCollection<Category>();
            Dishes = new ObservableCollection<Dish>();
            _recipeStorageService = new RecipeStorageService();
            _cartService = CartService.Instance;
            _userSettingsService = new UserSettingsService();

            // Initialize commands 初始化命令
            SelectCategoryCommand = new Command<Category>(SelectCategory);
            AddToCartCommand = new Command<Dish>(AddToCart);
            RemoveFromCartCommand = new Command<Dish>(RemoveFromCart);
            ViewCartCommand = new Command(ViewCart);

            // Load sample data 加载示例数据
            LoadSampleData();
        }

        #region Properties 属性

        /// <summary>
        /// Dish categories 菜品分类
        /// </summary>
        public ObservableCollection<Category> Categories { get; }

        /// <summary>
        /// Dishes list 菜品列表
        /// </summary>
        public ObservableCollection<Dish> Dishes { get; }

        /// <summary>
        /// Selected category 选中的分类
        /// </summary>
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    // Update previous category selection state 更新之前分类的选中状态
                    if (_selectedCategory != null)
                        _selectedCategory.IsSelected = false;

                    _selectedCategory = value;

                    // Update new category selection state 更新新分类的选中状态
                    if (_selectedCategory != null)
                        _selectedCategory.IsSelected = true;

                    OnPropertyChanged();
                    FilterDishesByCategory();
                }
            }
        }

        #endregion

        #region Commands 命令

        /// <summary>
        /// Select category command 选择分类命令
        /// </summary>
        public ICommand SelectCategoryCommand { get; }

        /// <summary>
        /// Command to add dish to cart 添加菜品到购物车命令
        /// </summary>
        public ICommand AddToCartCommand { get; }

        /// <summary>
        /// Command to remove dish from cart 从购物车移除菜品命令
        /// </summary>
        public ICommand RemoveFromCartCommand { get; }

        /// <summary>
        /// Command to view cart 查看购物车命令
        /// </summary>
        public ICommand ViewCartCommand { get; }

        /// <summary>
        /// Cart service instance 购物车服务实例
        /// </summary>
        public CartService Cart => _cartService;

        /// <summary>
        /// Current role text for display 当前角色显示文本
        /// </summary>
        public string CurrentRoleText => $"Current role: {_userSettingsService.GetCurrentUserRole().ToDisplayString()}";

        /// <summary>
        /// Refresh current role text 刷新当前角色文本
        /// </summary>
        public void RefreshRoleText()
        {
            OnPropertyChanged(nameof(CurrentRoleText));
        }

        #endregion

        #region Cart Methods 购物车方法

        /// <summary>
        /// Add dish to cart 添加菜品到购物车
        /// </summary>
        /// <param name="dish">Dish to add 要添加的菜品</param>
        private void AddToCart(Dish dish)
        {
            if (dish != null)
            {
                _cartService.AddToCart(dish);
                System.Diagnostics.Debug.WriteLine($"Added {dish.Name} to cart. Total items: {_cartService.TotalItems}");
            }
        }

        /// <summary>
        /// Remove dish from cart 从购物车移除菜品
        /// </summary>
        /// <param name="dish">Dish to remove 要移除的菜品</param>
        private void RemoveFromCart(Dish dish)
        {
            if (dish != null)
            {
                _cartService.RemoveFromCart(dish);
                System.Diagnostics.Debug.WriteLine($"Removed {dish.Name} from cart. Total items: {_cartService.TotalItems}");
            }
        }

        /// <summary>
        /// View cart details 查看购物车详情
        /// </summary>
        private async void ViewCart()
        {
            try
            {
                if (!_cartService.HasItems)
                {
                    await SafeDisplayAlert(
                        "Empty Cart",
                        "Your cart is empty. Add some dishes first!",
                        "OK");
                    return;
                }

                // For now, show a simple summary. Later this could navigate to a detailed cart page
                // 目前显示简单摘要，以后可以导航到详细的购物车页面
                var cartSummary = string.Join("\n", _cartService.CartItems.Select(item =>
                    $"{item.Name} x{item.Quantity}"));

                var message = $"Cart Summary:\n\n{cartSummary}";

                var result = await SafeDisplayAlert(
                    "Shopping Cart",
                    message,
                    "Place Order",
                    "Continue Shopping");

                if (result)
                {
                    // Handle order placement 处理下单
                    await PlaceOrder();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error viewing cart: {ex.Message}");
                await SafeDisplayAlert("Error", "Failed to view cart", "OK");
            }
        }

        /// <summary>
        /// Place order 下单
        /// </summary>
        private async Task PlaceOrder()
        {
            try
            {
                // Simulate order processing 模拟订单处理
                await Task.Delay(1000);

                await SafeDisplayAlert(
                    "Order Placed",
                    "Your order has been placed successfully!",
                    "OK");

                // Clear cart after successful order 成功下单后清空购物车
                _cartService.ClearCart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error placing order: {ex.Message}");
                await SafeDisplayAlert("Error", "Failed to place order", "OK");
            }
        }

        #endregion

        #region Helper Methods 辅助方法

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

        #endregion

        #region Private Methods 私有方法

        /// <summary>
        /// Select category 选择分类
        /// </summary>
        private void SelectCategory(Category category)
        {
            if (category != null)
            {
                SelectedCategory = category;
            }
        }

        /// <summary>
        /// Filter dishes by selected category 根据选中分类过滤菜品
        /// </summary>
        private void FilterDishesByCategory()
        {
            Dishes.Clear();

            if (SelectedCategory == null)
            {
                // Show all dishes if no category selected 如果没有选中分类，显示所有菜品
                foreach (var dish in _allDishes)
                {
                    Dishes.Add(dish);
                }
                return;
            }

            // Get category key for filtering 获取分类键用于过滤
            var categoryKey = GetCategoryKey(SelectedCategory.Name);
            var filteredDishes = _allDishes.Where(d => d.Category.Equals(categoryKey, StringComparison.OrdinalIgnoreCase));

            foreach (var dish in filteredDishes)
            {
                Dishes.Add(dish);
            }
        }

        /// <summary>
        /// Get category key from category name 从分类名称获取分类键
        /// </summary>
        private string GetCategoryKey(string categoryName)
        {
            if (categoryName.Contains("Chinese")) return "Chinese";
            if (categoryName.Contains("Western")) return "Western";
            if (categoryName.Contains("Desserts")) return "Desserts";
            if (categoryName.Contains("Soups")) return "Soups";
            if (categoryName.Contains("Vegetarian")) return "Vegetarian";
            if (categoryName.Contains("Others")) return "Others";
            return "Others";
        }

        /// <summary>
        /// Load sample data and saved recipes 加载示例数据和保存的菜谱
        /// </summary>
        private async void LoadSampleData()
        {
            // Load categories 加载分类
            Categories.Clear();
            var sampleCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Chinese", IconPath = "chopsticks.png", IconMargin = new Thickness(0) },
                new Category { Id = 2, Name = "Western", IconPath = "knife_and_fork.png", IconMargin = new Thickness(0) },
                new Category { Id = 3, Name = "Desserts", IconPath = "cake.png", IconMargin = new Thickness(0) },
                new Category { Id = 4, Name = "Soups", IconPath = "soup_bowl.png", IconMargin = new Thickness(0, 0, 0, 3) }, // 底部边距3px用于对齐
                new Category { Id = 5, Name = "Vegetarian", IconPath = "vegetables.png", IconMargin = new Thickness(0) },
                new Category { Id = 6, Name = "Others", IconPath = "plate.png", IconMargin = new Thickness(0) }
            };

            foreach (var category in sampleCategories)
            {
                Categories.Add(category);
            }

            // Set default selected category 设置默认选中分类
            if (Categories.Count > 0)
            {
                SelectedCategory = Categories[0];
            }

            // Load all dishes 加载所有菜品
            await LoadAllDishesAsync();

            // Filter dishes by default category 根据默认分类过滤菜品
            FilterDishesByCategory();
        }

        /// <summary>
        /// Load all dishes including sample dishes and saved recipes 加载所有菜品包括示例菜品和保存的菜谱
        /// </summary>
        private async Task LoadAllDishesAsync()
        {
            try
            {
                // Start with sample dishes 从示例菜品开始
                var allDishes = GetSampleDishes();

                // Load saved recipes and convert to dishes 加载保存的菜谱并转换为菜品
                var savedRecipes = await _recipeStorageService.GetAllRecipesAsync();
                var recipeDishes = savedRecipes.Select(recipe => recipe.ToDish()).ToList();

                // Combine sample dishes and recipe dishes 合并示例菜品和菜谱菜品
                allDishes.AddRange(recipeDishes);

                // Update the all dishes list 更新所有菜品列表
                _allDishes = allDishes;

                System.Diagnostics.Debug.WriteLine($"Loaded {allDishes.Count} total dishes ({savedRecipes.Count} from saved recipes)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dishes: {ex.Message}");
                // Fallback to sample dishes only 回退到仅示例菜品
                _allDishes = GetSampleDishes();
            }
        }

        /// <summary>
        /// Get sample dishes data 获取示例菜品数据
        /// </summary>
        private List<Dish> GetSampleDishes()
        {
            return new List<Dish>
            {
                // Chinese Cuisine 中式菜
                new Dish { Id = 1, Name = "Kung Pao Chicken", Category = "Chinese", Image = "kung_pao_chicken.png" },
                new Dish { Id = 2, Name = "Sweet and Sour Pork", Category = "Chinese", Image = "sweet_and_sour_pork.jpg" },
                new Dish { Id = 3, Name = "Mapo Tofu", Category = "Chinese", Image = "mapo_tofu.jpg" },

                // Western Cuisine 西式菜
                new Dish { Id = 4, Name = "Grilled Steak", Category = "Western", Image = "grilled_steak.jpg" },
                new Dish { Id = 5, Name = "Caesar Salad", Category = "Western", Image = "caesar_salad.jpg" },
                new Dish { Id = 6, Name = "Pasta Carbonara", Category = "Western", Image = "pasta_carbonara.jpg" },

                // Desserts 甜品
                new Dish { Id = 7, Name = "Chocolate Cake", Category = "Desserts", Image = "chocolate_cake.jpg" },
                new Dish { Id = 8, Name = "Tiramisu", Category = "Desserts", Image = "tiramisu.jpg" },

                // Soups 汤品
                new Dish { Id = 9, Name = "Tomato Soup", Category = "Soups", Image = "tomato_soup.jpg" },
                new Dish { Id = 10, Name = "Mushroom Soup", Category = "Soups", Image = "mushroom_soup.jpg" },

                // Vegetarian 素食
                new Dish { Id = 11, Name = "Vegetable Salad", Category = "Vegetarian", Image = "vegetable_salad.jpg" },
                new Dish { Id = 12, Name = "Quinoa Bowl", Category = "Vegetarian", Image = "quinoa_bowl.jpg" },

                // Others 其他
                new Dish { Id = 13, Name = "Fried Rice", Category = "Others", Image = "fried_rice.jpg" },
                new Dish { Id = 14, Name = "Spring Rolls", Category = "Others", Image = "spring_rolls.png" }
            };
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}