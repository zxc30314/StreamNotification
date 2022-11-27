internal static class Log
{
    public static void Stream(string text, bool newLine = true)
    {
        FormatColor(text, ConsoleColor.Green, newLine);
    }

    public static void Info(string text, bool newLine = true)
    {
        FormatColor(text, ConsoleColor.Gray, newLine);
    }

    public static void Warn(string text, bool newLine = true)
    {
        FormatColor(text, ConsoleColor.DarkYellow, newLine);
    }

    public static void Error(string text, bool newLine = true)
    {
        FormatColor(text, ConsoleColor.Red, newLine);
    }
    public static void FormatColor(string text, ConsoleColor consoleColor = ConsoleColor.Gray, bool newLine = true)
    {
        text = $"[{DateTime.Now}] {text}";
        Console.ForegroundColor = consoleColor;
        if (newLine) Console.WriteLine(text);
        else Console.Write(text);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}