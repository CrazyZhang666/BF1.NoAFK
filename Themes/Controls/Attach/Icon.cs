﻿namespace BF1.NoAFK.Themes.Controls.Attach;

/// <summary>
/// 附加属性-图标
/// </summary>
public class Icon
{
    #region 图标
    public static Geometry GetGeometry(DependencyObject obj)
    {
        return (Geometry)obj.GetValue(GeometryProperty);
    }
    public static void SetGeometry(DependencyObject obj, Geometry value)
    {
        obj.SetValue(GeometryProperty, value);
    }
    /// <summary>
    /// 图标
    /// </summary>
    public static readonly DependencyProperty GeometryProperty =
        DependencyProperty.RegisterAttached("Geometry", typeof(Geometry), typeof(Icon), new PropertyMetadata(default));
    #endregion

    #region 宽度
    public static string GetEmoji(DependencyObject obj)
    {
        return (string)obj.GetValue(EmojiProperty);
    }
    public static void SetEmoji(DependencyObject obj, string value)
    {
        obj.SetValue(EmojiProperty, value);
    }
    /// <summary>
    /// 图标宽度
    /// </summary>
    public static readonly DependencyProperty EmojiProperty =
        DependencyProperty.RegisterAttached("Emoji", typeof(string), typeof(Icon), new PropertyMetadata(default));
    #endregion

    #region 宽度
    public static double GetWidth(DependencyObject obj)
    {
        return (int)obj.GetValue(WidthProperty);
    }
    public static void SetWidth(DependencyObject obj, double value)
    {
        obj.SetValue(WidthProperty, value);
    }
    /// <summary>
    /// 图标宽度
    /// </summary>
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.RegisterAttached("Width", typeof(double), typeof(Icon), new PropertyMetadata(12.0));
    #endregion

    #region 高度
    public static double GetHeight(DependencyObject obj)
    {
        return (double)obj.GetValue(HeightProperty);
    }
    public static void SetHeight(DependencyObject obj, double value)
    {
        obj.SetValue(HeightProperty, value);
    }
    /// <summary>
    /// 图标高度
    /// </summary>
    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.RegisterAttached("Height", typeof(double), typeof(Icon), new PropertyMetadata(12.0));
    #endregion
}
