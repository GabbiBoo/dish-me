using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DishMeAPP.Models;

namespace DishMeAPP.Services
{
    /// <summary>
    /// Shopping cart service 购物车服务
    /// </summary>
    public class CartService : INotifyPropertyChanged
    {
        private static CartService? _instance;
        private readonly ObservableCollection<Dish> _cartItems;

        /// <summary>
        /// Singleton instance 单例实例
        /// </summary>
        public static CartService Instance => _instance ??= new CartService();

        private CartService()
        {
            _cartItems = new ObservableCollection<Dish>();
            _cartItems.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CartItems));
                OnPropertyChanged(nameof(TotalItems));

                OnPropertyChanged(nameof(HasItems));
            };
        }

        /// <summary>
        /// Cart items 购物车项目
        /// </summary>
        public ObservableCollection<Dish> CartItems => _cartItems;

        /// <summary>
        /// Total number of items in cart 购物车中的总项目数
        /// </summary>
        public int TotalItems => _cartItems.Sum(item => item.Quantity);



        /// <summary>
        /// Whether cart has items 购物车是否有项目
        /// </summary>
        public bool HasItems => _cartItems.Any(item => item.Quantity > 0);

        /// <summary>
        /// Add item to cart 添加项目到购物车
        /// </summary>
        /// <param name="dish">Dish to add 要添加的菜品</param>
        public void AddToCart(Dish dish)
        {
            try
            {
                var existingItem = _cartItems.FirstOrDefault(item => item.Id == dish.Id);

                if (existingItem != null)
                {
                    // Increase quantity of existing item 增加现有项目的数量
                    existingItem.Quantity++;
                    System.Diagnostics.Debug.WriteLine($"Increased quantity of {dish.Name} to {existingItem.Quantity}");
                }
                else
                {
                    // Add new item to cart 添加新项目到购物车
                    var cartItem = new Dish
                    {
                        Id = dish.Id,
                        Name = dish.Name,
                        Image = dish.Image,
                        Category = dish.Category,
                        Description = dish.Description,
                        Ingredients = dish.Ingredients,
                        Quantity = 1
                    };

                    _cartItems.Add(cartItem);
                    System.Diagnostics.Debug.WriteLine($"Added {dish.Name} to cart");
                }

                // Update the original dish quantity for UI display
                // 更新原始菜品数量以供UI显示
                dish.Quantity = _cartItems.FirstOrDefault(item => item.Id == dish.Id)?.Quantity ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding to cart: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove item from cart 从购物车中移除项目
        /// </summary>
        /// <param name="dish">Dish to remove 要移除的菜品</param>
        public void RemoveFromCart(Dish dish)
        {
            try
            {
                var existingItem = _cartItems.FirstOrDefault(item => item.Id == dish.Id);

                if (existingItem != null)
                {
                    if (existingItem.Quantity > 1)
                    {
                        // Decrease quantity 减少数量
                        existingItem.Quantity--;
                        System.Diagnostics.Debug.WriteLine($"Decreased quantity of {dish.Name} to {existingItem.Quantity}");
                    }
                    else
                    {
                        // Remove item completely 完全移除项目
                        _cartItems.Remove(existingItem);
                        System.Diagnostics.Debug.WriteLine($"Removed {dish.Name} from cart");
                    }

                    // Update the original dish quantity for UI display
                    // 更新原始菜品数量以供UI显示
                    dish.Quantity = _cartItems.FirstOrDefault(item => item.Id == dish.Id)?.Quantity ?? 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing from cart: {ex.Message}");
            }
        }

        /// <summary>
        /// Clear all items from cart 清空购物车
        /// </summary>
        public void ClearCart()
        {
            try
            {
                _cartItems.Clear();
                System.Diagnostics.Debug.WriteLine("Cart cleared");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing cart: {ex.Message}");
            }
        }

        /// <summary>
        /// Get quantity of specific dish in cart 获取购物车中特定菜品的数量
        /// </summary>
        /// <param name="dishId">Dish ID 菜品ID</param>
        /// <returns>Quantity 数量</returns>
        public int GetQuantity(int dishId)
        {
            return _cartItems.FirstOrDefault(item => item.Id == dishId)?.Quantity ?? 0;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
