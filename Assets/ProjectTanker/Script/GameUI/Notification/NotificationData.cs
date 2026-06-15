using UnityEngine;

public struct NotificationData
{
    public string Message;
    public Sprite Icon;
    public Color  AccentColor;
    public float  Duration; // 0 = NotificationManager.defaultDuration を使用

    public static NotificationData Info(string msg, Sprite icon = null) => new()
    {
        Message     = msg,
        Icon        = icon,
        Duration    = 0f,
        AccentColor = new Color(0.302f, 0.722f, 0.910f), // #4DB8E8
    };

    public static NotificationData Tutorial(string msg, Sprite icon = null) => new()
    {
        Message     = msg,
        Icon        = icon,
        Duration    = 6f,
        AccentColor = new Color(0.961f, 0.773f, 0.094f), // #F5C518
    };

    public static NotificationData Warning(string msg, Sprite icon = null) => new()
    {
        Message     = msg,
        Icon        = icon,
        Duration    = 0f,
        AccentColor = new Color(1.000f, 0.624f, 0.263f), // #FF9F43
    };

    public static NotificationData Success(string msg, Sprite icon = null) => new()
    {
        Message     = msg,
        Icon        = icon,
        Duration    = 0f,
        AccentColor = new Color(0.408f, 0.784f, 0.478f), // #68C87A
    };
}
