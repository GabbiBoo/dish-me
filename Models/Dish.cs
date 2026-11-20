using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DishMeAPP.Models
{
    /// <summary>
    /// Dish data model 菜品数据模型
    /// </summary>
    public class Dish : INotifyPropertyChanged
    {
        private int _quantity;
        /// <summary>
        /// Dish unique identifier 菜品唯一标识符
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dish name 菜品名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Dish title for display 显示标题
        /// </summary>
        public string Title => Name;

        /// <summary>
        /// Dish image path 菜品图片路径
        /// </summary>
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// Dish category 菜品分类
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Dish description/ingredients 菜品描述/用料
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Dish ingredients 菜品用料
        /// </summary>
        public string Ingredients { get; set; } = string.Empty;



        /// <summary>
        /// Whether dish is available 是否可用
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Cooking time in minutes 制作时间（分钟）
        /// </summary>
        public int CookingTimeMinutes { get; set; }

        /// <summary>
        /// Dish tags 菜品标签
        /// </summary>
        public string Tags { get; set; } = string.Empty;

        /// <summary>
        /// Creation time 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Quantity in cart 购物车中的数量
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsInCart));

                }
            }
        }

        /// <summary>
        /// Whether dish is in cart 是否在购物车中
        /// </summary>
        public bool IsInCart => Quantity > 0;



        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
