using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FidgetSpace.Features.ConnectDots.Models
{
    /// <summary>
    /// 这个类表示“连点游戏里的一颗点”
    /// 保存这颗点的：位置、颜色、是否被选中、是否显示、是不是 Power 点
    /// 实现 INotifyPropertyChanged 是为了让 UI 在属性变化时自动刷新
    /// </summary>
    public class ConnectDotsCell : INotifyPropertyChanged
    {
        // 每颗点的编号（方便调试或以后扩展）
        public int Id { get; set; }

        // 点在第几行
        public int Row { get; set; }

        // 点在第几列
        public int Col { get; set; }

        // ================= 下面是和 UI 绑定的属性 =================

        // 点的颜色（绑定到 Frame.BackgroundColor）
        private string colorName; // use for store data
        public string ColorName     // use for acess and API
        {
            get { return colorName; }
            set
            {
                if (colorName != value)
                {
                    colorName = value;
                    OnPropertyChanged();   // 通知 UI：颜色变了
                }
            }
        }

        // 是否被选中（选中时 UI 会画一个白色边框）
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();   // 通知 UI：选中状态变了
                }
            }
        }

        // 是否可见（配对成功后我们会把它设为 false，让点在 UI 上消失）
        private bool isVisible = true; // 默认都显示
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged();   // 通知 UI：是否显示改变
                }
            }
        }

        // 是否是 Power 点（★），点击时会清除所有同色点
        private bool isPower;
        public bool IsPower
        {
            get { return isPower; }
            set
            {
                if (isPower != value)
                {
                    isPower = value;
                    OnPropertyChanged();   // 通知 UI：是否是 Power 点改变
                }
            }
        }

        // ================= INotifyPropertyChanged 标准写法 =================

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 触发属性变化事件
        /// [CallerMemberName]：如果不传参数，会自动使用当前属性名
        /// </summary>
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
