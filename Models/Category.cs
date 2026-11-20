using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DishMeAPP.Models
{
    /// <summary>
    /// Category data model 分类数据模型
    /// </summary>
    public class Category : INotifyPropertyChanged
    {
        private bool _isSelected;

        /// <summary>
        /// Category ID 分类ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Category name 分类名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Category icon 分类图标
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Category icon path 分类图标路径
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Icon margin for alignment adjustment 图标边距用于对齐调整
        /// </summary>
        public Thickness IconMargin { get; set; } = new Thickness(0);

        /// <summary>
        /// Whether category is selected 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackgroundColor));
                    OnPropertyChanged(nameof(TextColor));
                    OnPropertyChanged(nameof(FontWeight));
                    OnPropertyChanged(nameof(IconColor));
                }
            }
        }

        /// <summary>
        /// Background color based on selection state 根据选中状态的背景色
        /// </summary>
        public string BackgroundColor => "White";

        /// <summary>
        /// Text color based on selection state 根据选中状态的文字颜色
        /// </summary>
        public string TextColor => IsSelected ? "#E41088" : "#E41088";

        /// <summary>
        /// Font attributes based on selection state 根据选中状态的字体属性
        /// </summary>
        public FontAttributes FontWeight => IsSelected ? FontAttributes.Bold : FontAttributes.None;

        /// <summary>
        /// Icon opacity for visual effect 图标透明度用于视觉效果
        /// </summary>
        public double IconOpacity => 1.0;

        /// <summary>
        /// Icon color based on selection state 根据选中状态的图标颜色
        /// </summary>
        public Color IconColor => IsSelected ? Color.FromArgb("#E41088") : Color.FromArgb("#E41088");

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


