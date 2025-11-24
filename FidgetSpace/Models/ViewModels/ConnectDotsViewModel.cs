using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
// using FidgetSpace.Features.ConnectDots.Models;
using Microsoft.Maui.Controls;

namespace FidgetSpace.Models.ViewModels
{
    /// <summary>
    /// 连点游戏的核心逻辑（MVVM 的 ViewModel）：
    /// - 生成棋盘
    /// - 处理点击
    /// - 判断配对与结束
    /// </summary>
    public class ConnectDotsViewModel : INotifyPropertyChanged
    {
        // 用于绑定到 CollectionView 的点集合
        public ObservableCollection<ConnectDotsCell> Cells { get; set; }

        // 行数（你可以根据需要调整）
        private int rows = 4;
        public int Rows
        {
            get => rows;
            set
            {
                if (rows != value)
                {
                    rows = value;
                    OnPropertyChanged();
                }
            }
        }

        // 列数（默认 6 列）
        private int cols = 6;
        public int Cols
        {
            get => cols;
            set
            {
                if (cols != value)
                {
                    cols = value;
                    OnPropertyChanged();
                }
            }
        }

        // 剩余可配对的对数（显示在页面顶部）
        private int remainingPairs;
        public int RemainingPairs
        {
            get => remainingPairs;
            set
            {
                if (remainingPairs != value)
                {
                    remainingPairs = value;
                    OnPropertyChanged();
                }
            }
        }

        // 当前棋盘是否可以继续操作
        private bool boardActive = true;
        public bool BoardActive
        {
            get => boardActive;
            set
            {
                if (boardActive != value)
                {
                    boardActive = value;
                    OnPropertyChanged();
                }
            }
        }

        // 新游戏按钮命令
        public ICommand NewGameCommand { get; set; }

        // 点击某个点时触发的命令
        public ICommand TapCommand { get; set; }

        private readonly Random random = new Random();
        private ConnectDotsCell firstSelected;   // 记录第一次选中的点

        // 使用的颜色集合
        private readonly string[] colors = new[]
        {
            "#FF6B6B", // 红
            "#4D96FF", // 蓝
            "#6BCB77"  // 绿
        };

        public ConnectDotsViewModel()
        {
            Cells = new ObservableCollection<ConnectDotsCell>();

            NewGameCommand = new Command(GenerateBoard);
            TapCommand = new Command<ConnectDotsCell>(OnTap);

            GenerateBoard();
        }

        /// <summary>
        /// 生成新的棋盘：颜色成对出现、随机分布
        /// </summary>
        void GenerateBoard()
        {
            Cells.Clear();
            firstSelected = null;
            BoardActive = true;

            int total = Rows * Cols;
            if (total % 2 != 0)
            {
                total -= 1; // 保证是偶数，方便两两配对
            }

            var colorList = new System.Collections.Generic.List<string>();
            int pairCount = total / 2;
            int colorCount = colors.Length;

            // 让每种颜色大致平分总对数
            int basePairs = pairCount / colorCount;
            int extraPairs = pairCount - basePairs * colorCount;

            for (int i = 0; i < colorCount; i++)
            {
                int pairs = basePairs + (i < extraPairs ? 1 : 0);
                for (int p = 0; p < pairs; p++)
                {
                    colorList.Add(colors[i]);
                    colorList.Add(colors[i]); // 同色两颗，形成一对
                }
            }

            // 洗牌（打乱颜色顺序）
            for (int i = colorList.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (colorList[i], colorList[j]) = (colorList[j], colorList[i]);
            }

            int id = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    int index = r * Cols + c;
                    if (index >= colorList.Count)
                        break;

                    var cell = new ConnectDotsCell
                    {
                        Id = id++,
                        Row = r,
                        Col = c,
                        ColorName = colorList[index],
                        IsVisible = true,
                        IsSelected = false
                    };

                    Cells.Add(cell);
                }
            }

            RemainingPairs = Cells.Count(x => x.IsVisible) / 2;
        }

        /// <summary>
        /// 点击某一个点时的处理逻辑：
        /// - 如果是第一次点击：仅选中
        /// - 第二次点击：如果颜色相同 → 两个都消失；否则切换选中
        /// </summary>
        async void OnTap(ConnectDotsCell cell)
        {
            if (!BoardActive) return;
            if (cell == null) return;
            if (!cell.IsVisible) return;

            // 第一次点击：记录并高亮
            if (firstSelected == null)
            {
                firstSelected = cell;
                cell.IsSelected = true;
                return;
            }

            // 再点同一颗：取消选中
            if (firstSelected == cell)
            {
                cell.IsSelected = false;
                firstSelected = null;
                return;
            }

            // 第二次点击：颜色相同 → 消除这一对
            if (firstSelected.ColorName == cell.ColorName)
            {
                firstSelected.IsVisible = false;
                cell.IsVisible = false;

                firstSelected.IsSelected = false;
                cell.IsSelected = false;

                firstSelected = null;

                RemainingPairs = Cells.Count(x => x.IsVisible) / 2;
                await CheckEnd();
            }
            else
            {
                // 颜色不同：切换选中到新的点
                firstSelected.IsSelected = false;
                firstSelected = cell;
                cell.IsSelected = true;
            }
        }

        /// <summary>
        /// 检查是否已经没有可见的点，如果是则提示并重开一局
        /// </summary>
        async Task CheckEnd()
        {
            bool anyVisible = Cells.Any(x => x.IsVisible);
            if (!anyVisible)
            {
                BoardActive = false;

#if ANDROID || WINDOWS
                await Application.Current.MainPage.DisplayAlert(
                    "Great!",
                    "All dots cleared!",
                    "OK");
#endif
                GenerateBoard();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
