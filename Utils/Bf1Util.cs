namespace BF1.NoAFK.Utils;

public static class Bf1Util
{
    /// <summary>
    /// 判断战地1程序是否运行
    /// </summary>
    /// <returns></returns>
    public static bool IsBf1Run()
    {
        var pArray = Process.GetProcessesByName("bf1");
        if (pArray.Length > 0)
        {
            foreach (var item in pArray)
            {
                if (item.MainWindowTitle.Equals("Battlefield™ 1"))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 计算时间差，即软件运行时间
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public static string ExecDateDiff(DateTime startTime, DateTime endTime)
    {
        var ts1 = new TimeSpan(startTime.Ticks);
        var ts2 = new TimeSpan(endTime.Ticks);

        return ts1.Subtract(ts2).Duration().ToString("c")[..8];
    }

    /// <summary>
    /// 将本程序设为开启自启
    /// </summary>
    /// <param name="onOff">自启开关</param>
    /// <returns></returns>
    public static bool SetMeStart(bool onOff)
    {
        var appName = Process.GetCurrentProcess().MainModule.ModuleName;
        var appPath = Process.GetCurrentProcess().MainModule.FileName;
        return SetAutoStart(onOff, appName, appPath);
    }

    /// <summary>
    /// 将应用程序设为或不设为开机启动
    /// </summary>
    /// <param name="onOff">自启开关</param>
    /// <param name="appName">应用程序名</param>
    /// <param name="appPath">应用程序完全路径</param>
    public static bool SetAutoStart(bool onOff, string appName, string appPath)
    {
        bool isOk = true;
        // 如果从没有设为开机启动设置到要设为开机启动
        if (!IsExistKey(appName) && onOff)
        {
            isOk = SelfRunning(onOff, appName, @appPath);
        }
        // 如果从设为开机启动设置到不要设为开机启动
        else if (IsExistKey(appName) && !onOff)
        {
            isOk = SelfRunning(onOff, appName, @appPath);
        }
        return isOk;
    }

    /// <summary>
    /// 判断注册键值对是否存在，即是否处于开机启动状态
    /// </summary>
    /// <param name="keyName">键值名</param>
    /// <returns></returns>
    private static bool IsExistKey(string keyName)
    {
        try
        {
            RegistryKey local = Registry.LocalMachine;
            RegistryKey runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (runs == null)
            {
                RegistryKey key2 = local.CreateSubKey("SOFTWARE");
                RegistryKey key3 = key2.CreateSubKey("Microsoft");
                RegistryKey key4 = key3.CreateSubKey("Windows");
                RegistryKey key5 = key4.CreateSubKey("CurrentVersion");
                RegistryKey key6 = key5.CreateSubKey("Run");
                runs = key6;
            }
            string[] runsName = runs.GetValueNames();
            foreach (string strName in runsName)
            {
                if (strName.ToUpper() == keyName.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 写入或删除注册表键值对,即设为开机启动或开机不启动
    /// </summary>
    /// <param name="isStart">是否开机启动</param>
    /// <param name="exeName">应用程序名</param>
    /// <param name="path">应用程序路径带程序名</param>
    /// <returns></returns>
    private static bool SelfRunning(bool isStart, string exeName, string path)
    {
        try
        {
            RegistryKey local = Registry.LocalMachine;
            RegistryKey key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null)
            {
                local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
            }
            if (isStart)
            {
                // 若开机自启动则添加键值对
                key.SetValue(exeName, path);
                key.Close();
            }
            else
            {
                // 否则删除键值对
                string[] keyNames = key.GetValueNames();
                foreach (string keyName in keyNames)
                {
                    if (keyName.ToUpper() == exeName.ToUpper())
                    {
                        key.DeleteValue(exeName);
                        key.Close();
                    }
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
