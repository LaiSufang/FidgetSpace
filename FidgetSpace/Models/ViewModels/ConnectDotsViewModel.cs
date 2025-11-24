using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FidgetSpace.Models;
using Microsoft.Maui.Controls;
using System.Diagnostics;          // 计时器 Stopwatch
using Microsoft.Maui.Storage;      // Preferences 本地存储
using Microsoft.Maui.Dispatching;

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

        // ⭐ 计时相关字段
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _isTimerRunning;

        // 保存总秒数（历史 + 当前已完成局）
        private int totalTimePlayedSeconds;

        // 用于 UI 显示的总时间文本
        private string totalTimePlayedDisplay;
        public string TotalTimePlayedDisplay
        {
            get => totalTimePlayedDisplay;
            set
            {
                if (totalTimePlayedDisplay != value)
                {
                    totalTimePlayedDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        // ⭐ 用于 UI 显示的「本局用时」
        private string sessionTimeDisplay;
        public string SessionTimeDisplay
        {
            get => sessionTimeDisplay;
            set
            {
                if (sessionTimeDisplay != value)
                {
                    sessionTimeDisplay = value;
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

            // ⭐ 从本地读取之前累计的总时长（秒数）
            totalTimePlayedSeconds = Preferences.Get("ConnectDots_TotalTimeSeconds", 0);
            TotalTimePlayedDisplay = TimeSpan
                .FromSeconds(totalTimePlayedSeconds)
                .ToString(@"hh\:mm\:ss");

            // 初始化本局时间为 00:00:00
            SessionTimeDisplay = TimeSpan
                .FromSeconds(0)
                .ToString(@"hh\:mm\:ss");



            GenerateBoard();
        }

        /// <summary>
        /// 页面进入时调用，开始本局计时（现在由第一次点击触发）
        /// </summary>
        public void StartTimer()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            _isTimerRunning = true;

            // 每秒刷新一次 TotalTimePlayedDisplay
            Application.Current.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (!_isTimerRunning)
                    return false; // 停止这个 UI timer

                // 本局已经过去的秒数
                int sessionSeconds = (int)_stopwatch.Elapsed.TotalSeconds;

                // 当前总时间 = 已保存的总秒数 + 本局已经过去的秒数
                int currentTotalSeconds = totalTimePlayedSeconds + sessionSeconds;

                // 更新总时间显示
                TotalTimePlayedDisplay = TimeSpan
                    .FromSeconds(currentTotalSeconds)
                    .ToString(@"hh\:mm\:ss");

                // ⭐ 更新本局时间显示
                SessionTimeDisplay = TimeSpan
                    .FromSeconds(sessionSeconds)
                    .ToString(@"hh\:mm\:ss");

                return true; // 返回 true 表示 1 秒后继续调用（循环）
            });

        }

        /// <summary>
        /// 页面离开时调用，停止并保存本局时间到总时长
        /// </summary>
        public void StopAndSaveTimer()
        {
            // 如果本局根本没开始计时，就不用保存
            if (!_isTimerRunning)
                return;

            _isTimerRunning = false;
            _stopwatch.Stop();

            int sessionSeconds = (int)_stopwatch.Elapsed.TotalSeconds;
            if (sessionSeconds <= 0)
                return;

            // ⭐ 停下来时，把本局最终用时显示出来
            SessionTimeDisplay = TimeSpan
                .FromSeconds(sessionSeconds)
                .ToString(@"hh\:mm\:ss");

            // 累加到总时长
            totalTimePlayedSeconds += sessionSeconds;

            // 保存到本地
            Preferences.Set("ConnectDots_TotalTimeSeconds", totalTimePlayedSeconds);

            // 再更新一次显示（确保停下来的时候是最新值）
            TotalTimePlayedDisplay = TimeSpan
                .FromSeconds(totalTimePlayedSeconds)
                .ToString(@"hh\:mm\:ss");
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

            // ⭐ 第一次真正点击任意一个点时，才开始计时
            if (!_isTimerRunning)
            {
                StartTimer();
            }

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
                // ⭐ 这一局已经结束，先把本局时间停掉并累加到总时长
                StopAndSaveTimer();

                BoardActive = false;

#if ANDROID || WINDOWS
                await Application.Current.MainPage.DisplayAlert(
                    "Great!",
                    "All dots cleared!",
                    "OK");
#endif

                // 如果你希望通关后自动开始下一局，就保留这行；
                // 如果你想让玩家点击“New”再开始，就可以把这行注释掉。
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
