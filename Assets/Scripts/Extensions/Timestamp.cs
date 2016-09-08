using System;

public class Timestamp
{
    public static string ImprintLocalTime()
    {
        return string.Format("[{0:HH:mm:ss}]", DateTime.Now);
    }
}
