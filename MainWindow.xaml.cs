using BF1.NoAFK.Core;
using BF1.NoAFK.Utils;

using Hardcodet.Wpf.TaskbarNotification;

namespace BF1.NoAFK;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow
{
    /// <summary>
    /// 主窗口程序是否在运行
    /// </summary>
    private bool IsMainAppRunning = true;

    /// <summary>
    /// 挂机防踢定时器
    /// </summary>
    private Timer TimerNoAFK = null;

    /// <summary>
    /// 存储软件开始运行的时间
    /// </summary>
    private DateTime Origin_DateTime;

    public MainWindow()
    {
        InitializeComponent();

        CheckBox_RunAtStart.IsChecked = Properties.Settings.Default.IsRunAtStart;
        CheckBox_ShowLogger.IsChecked = Properties.Settings.Default.IsShowLogger;

        if (CheckBox_ShowLogger.IsChecked == true)
        {
            TextBox_Logger.Visibility = Visibility.Visible;
            Window_Main.Height = 400;
        }
        else
        {
            TextBox_Logger.Visibility = Visibility.Collapsed;
            Window_Main.Height = 60;
        }
    }

    private void Window_Main_Loaded(object sender, RoutedEventArgs e)
    {
        // 获取当前时间，存储到对于变量中
        Origin_DateTime = DateTime.Now;

        new Thread(UpdateThread)
        {
            Name = "UpdateThread",
            IsBackground = true
        }.Start();

        TimerNoAFK = new()
        {
            AutoReset = true,
            Interval = TimeSpan.FromSeconds(30).TotalMilliseconds
        };
        TimerNoAFK.Elapsed += TimerNoAFK_Elapsed;

        AppendLogger("欢迎使用战地1暖服机器人自动挂机工具，DS By CrazyZhang666");
    }

    private void Window_Main_Closing(object sender, CancelEventArgs e)
    {
        TaskbarIcon_Main.Dispose();

        IsMainAppRunning = false;
        TimerNoAFK.Stop();
        Bf1Mem.CloseHandle();

        Properties.Settings.Default.Save();
    }

    private void AppendLogger(string log)
    {
        this.Dispatcher.Invoke(() =>
        {
            if (CheckBox_ShowLogger.IsChecked == true)
            {
                if (TextBox_Logger.LineCount >= 1000)
                    TextBox_Logger.Clear();

                TextBox_Logger.AppendText($"[{DateTime.Now:T}] {log}\r\n");
                TextBox_Logger.ScrollToEnd();
            }
        });
    }

    private void UpdateThread()
    {
        while (IsMainAppRunning)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBlock_AppRunTime.Text = $"运行时间 : {Bf1Util.ExecDateDiff(Origin_DateTime, DateTime.Now)}";
            });

            // 判断战地1是否在运行
            if (!Bf1Util.IsBf1Run())
            {
                // 关闭战地1进程句柄
                Bf1Mem.CloseHandle();
                continue;
            }

            if (Bf1Mem.Bf1ProHandle == IntPtr.Zero)
            {
                // 代表刚刚发现战地1进程，尚未初始化
                AppendLogger("发现战地1进程，正在初始化...");

                // 初始化战地1进程
                if (Bf1Mem.Initialize())
                {
                    AppendLogger("战地1内存模块初始化成功");
                    AppendLogger("正在等待玩家进入服务器...");
                    RunNoAFK();
                }
                else
                {
                    Bf1Mem.CloseHandle();
                    AppendLogger("战地1内存模块初始化失败");
                    return;
                }
            }
            else
            {
                // 代表刚刚发现战地1已经被初始化过
                RunNoAFK();
            }

            Thread.Sleep(1000);
        }
    }

    /// <summary>
    /// 挂机防踢
    /// </summary>
    private void RunNoAFK()
    {
        var serverName = Bf1Mem.ReadString(Bf1Mem.Bf1ProBaseAddress + Offsets.ServerName_Offset, Offsets.ServerName, 64);
        if (string.IsNullOrEmpty(serverName))
        {
            if (TimerNoAFK.Enabled)
            {
                // 玩家不在服务器
                TimerNoAFK.Stop();
                AppendLogger("检测到玩家离开服务器，已关闭游戏内挂机防踢功能");
            }
        }
        else
        {
            if (!TimerNoAFK.Enabled)
            {
                // 玩家在服务器
                TimerNoAFK.Start();
                AppendLogger("检测到玩家进入服务器，已启用游戏内挂机防踢功能");
            }
        }
    }

    /// <summary>
    /// 游戏内挂机防踢线程
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void TimerNoAFK_Elapsed(object sender, ElapsedEventArgs e)
    {
        Bf1Mem.SetForegroundWindow();
        Thread.Sleep(50);

        Win32.Keybd_Event(WinVK.TAB, Win32.MapVirtualKey(WinVK.TAB, 0), 0, 0);
        Thread.Sleep(3000);
        Win32.Keybd_Event(WinVK.TAB, Win32.MapVirtualKey(WinVK.TAB, 0), 2, 0);
        Thread.Sleep(50);
    }

    /// <summary>
    /// 开机自启
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckBox_RunAtStart_Click(object sender, RoutedEventArgs e)
    {
        if (CheckBox_RunAtStart.IsChecked == true)
        {
            if (Bf1Util.SetMeStart(true))
            {
                AppendLogger("设置开机自启成功");
            }
            else
            {
                CheckBox_RunAtStart.IsChecked = false;
                AppendLogger("设置开机自启失败");
            }
        }
        else
        {
            if (Bf1Util.SetMeStart(false))
            {
                AppendLogger("关闭开机自启成功");
            }
            else
            {
                CheckBox_RunAtStart.IsChecked = true;
                AppendLogger("关闭开机自启失败");
            }
        }

        Properties.Settings.Default.IsRunAtStart = CheckBox_RunAtStart.IsChecked == true;
    }

    /// <summary>
    /// 显示日志
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckBox_ShowLogger_Click(object sender, RoutedEventArgs e)
    {
        if (CheckBox_ShowLogger.IsChecked == true)
        {
            TextBox_Logger.Visibility = Visibility.Visible;
            Window_Main.Height = 400;
        }
        else
        {
            TextBox_Logger.Visibility = Visibility.Collapsed;
            Window_Main.Height = 60;
        }

        Properties.Settings.Default.IsShowLogger = CheckBox_ShowLogger.IsChecked == true;
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_ShowMainWindow_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
            Activate();
            ShowInTaskbar = true;
        }
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_ExitProcess_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Main_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            ShowInTaskbar = false;
            TaskbarIcon_Main.ShowBalloonTip("提示", "工具已最小化到系统托盘，请使用托盘图标右键退出程序", BalloonIcon.Info);
        }
    }

    private void TaskbarIcon_Main_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
            Activate();
            ShowInTaskbar = true;
        }
    }
}
