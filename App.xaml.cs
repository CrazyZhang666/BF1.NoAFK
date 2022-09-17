namespace BF1.NoAFK;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : Application
{
    private Mutex AppMainMutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppMainMutex = new Mutex(true, ResourceAssembly.GetName().Name, out var createdNew);
        if (createdNew)
        {
            base.OnStartup(e);
        }
        else
        {
            MessageBox.Show("请不要重复打开，程序已经运行\n如果一直提示，请到\"任务管理器-详细信息（win7为进程）\"里结束本程序",
                "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            Current.Shutdown();
        }
    }
}
