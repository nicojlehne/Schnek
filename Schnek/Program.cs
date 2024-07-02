#pragma warning disable IDE0059
#pragma warning disable IDE0060
#pragma warning disable IDE0079
using System.Text;

static class Constants
{
    public enum Options
    {
        Debug,
        Difficulty
    }

    public enum Difficulty
    {
        None,
        Easy,
        Normal,
        Hard,
        Extreme
    }
}

static class Variables
{
    public static string playerDirection = "right";
    public static int offsetHeight = 0;
    public static int offsetWidth = 0;
    public static ConsoleKey pressedKey = ConsoleKey.None;
}

static class Statistics
{
    public static int Length { get; set; }

}

static class Settings
{
    public static bool Debug { get; set; }
    public static double Difficulty { get; set; }

    public static void DefaultSettings()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Difficulty = (double)Constants.Difficulty.Easy;
    }

    public static void HandleCommand(string command = "")
    {
        string[] arguments = command.Split(' ');
        if (Enum.TryParse(arguments[0], ignoreCase: true, out Constants.Options option))
        {
            switch (option)
            {
                case Constants.Options.Debug:
                    if (arguments.Length > 1)
                        if (int.TryParse(arguments[1], out int debugValue))
                        {
                            Debug = (debugValue == 1);
                        }
                        else
                        {
                            Debug ^= true;
                        }
                    break;
                case Constants.Options.Difficulty:
                    if (arguments.Length > 1)
                        if (int.TryParse(arguments[1], out int difficultyValue))
                        {
                            Difficulty = difficultyValue < Enum.GetNames(typeof(Constants.Difficulty)).Length ? difficultyValue : Enum.GetValues(typeof(Constants.Difficulty)).Cast<int>().Max();
                        }
                        else
                        {
                            CUI.WriteAt("Invalid value for Difficulty option. Use an integer value.", 1, Console.WindowHeight - 2);
                            Console.Read();
                        }
                    break;
                default:
                    CUI.WriteAt("Unknown option.", 1, Console.WindowHeight - 2);
                    Console.Read();
                    break;

            }
        }
    }
}

class CUI
{
    public CUI()
    {
        Console.Clear();
        Frame();
        if (Settings.Debug) DebugInfo(Variables.pressedKey);
    }
    public static void WriteAt(string? value, int windowWidth = 0, int windowHeight = 0, bool returnToOrigin = false)
    {
        (int originX, int originY) = Console.GetCursorPosition();
        Console.SetCursorPosition(windowWidth, windowHeight);
        Console.Write(value);
        if (returnToOrigin)
        {
            Console.SetCursorPosition(originX, originY);
        }
    }

    public static void WriteLineAt(string value = "", int windowWidth = 0, int windowHeight = 0, bool returnToOrigin = false)
    {
        (int originX, int originY) = Console.GetCursorPosition();
        Console.SetCursorPosition(windowWidth, windowHeight);
        value += new String(' ', Console.WindowWidth - value.Length);
        Console.Write(value);
    }

    public static string? ReadAt(int windowWidth = 0, int windowHeight = 0, bool returnToOrigin = false)
    {
        (int originX, int originY) = Console.GetCursorPosition();
        Console.SetCursorPosition(windowWidth, windowHeight);
        return Console.ReadLine();
    }

    public static void Frame(char value = ' ')
    {
        int windowHeight = Console.WindowHeight;
        int windowWidth = Console.WindowWidth;
        for (int i = 0; i < Console.WindowHeight; i++)
        {
            if (i == 0 || i == windowHeight - 1)
            {
                Console.Write(new String('#', windowWidth));
            }
            else
            {
                Console.WriteLine("#" + (new String(' ', windowWidth - 2)) + '#');
            }
        }
    }

    public static void DebugInfo(ConsoleKey pressedKey)
    {
        WriteAt("Difficulty: " + Settings.Difficulty, 2, 1);
        WriteAt("Polling rate (ms): " + 67.0 / Settings.Difficulty, 2, 2);
        WriteAt("Last pressed key: " + pressedKey, 2, 3);
        WriteAt("Offset: (" + Variables.offsetWidth + "|" + Variables.offsetHeight + ")", 2, 4);
        WriteAt("Dimensions: (" + Console.WindowWidth + "|" + Console.WindowHeight + ")", 2, 5);
    }

    public static void HandleKeyPress(ConsoleKey pressedKey, bool shiftDown = false)
    {
        if (shiftDown)
        {
            switch (pressedKey)
            {
                case ConsoleKey.Oem1:
                    WriteLineAt(":", 0, Console.WindowHeight - 1);
                    string str = ReadAt(1, Console.WindowHeight - 1) ?? " ";
                    Settings.HandleCommand(str);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (pressedKey)
            {
                case ConsoleKey.Spacebar:
                    Variables.playerDirection = "none";
                    break;
                case ConsoleKey.LeftArrow:
                    Variables.playerDirection = "left";
                    break;
                case ConsoleKey.RightArrow:
                    Variables.playerDirection = "right";
                    break;
                case ConsoleKey.UpArrow:
                    Variables.playerDirection = "top";
                    break;
                case ConsoleKey.DownArrow:
                    Variables.playerDirection = "bottom";
                    break;
                default:
                    break;
            }
        }
    }
}

class Entity
{
    public class Player
    {
        public Player() {
            DrawEntity();
            HandleDirection();
        }
        public static void DrawEntity()
        {
            bool shiftDown = false;
            CUI.WriteAt(":", Console.WindowWidth / 2 + Variables.offsetWidth, Console.WindowHeight / 2 + Variables.offsetHeight);

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo pressedKeyInfo = Console.ReadKey(true);
                Variables.pressedKey = pressedKeyInfo.Key;

                shiftDown = (pressedKeyInfo.Modifiers & ConsoleModifiers.Shift) != 0;
            }

            CUI.HandleKeyPress(Variables.pressedKey, shiftDown);
            Thread.Sleep((int)Math.Ceiling(67.0 / Settings.Difficulty));

            CUI.WriteAt(" ", Console.WindowWidth / 2 + Variables.offsetWidth, Console.WindowHeight / 2 + Variables.offsetHeight);
        }
        public static void HandleDirection()
        {
            switch (Variables.playerDirection)
            {
                case "none":
                    break;
                case "left":
                    if (Variables.offsetWidth > Console.WindowWidth / 2 * -1 + 1) Variables.offsetWidth--;
                    break;
                case "right":
                    if (Variables.offsetWidth < Console.WindowWidth / 2 - (Console.WindowWidth % 2 == 0 ? 2 : 1)) Variables.offsetWidth++;
                    break;
                case "top":
                    if (Variables.offsetHeight > Console.WindowHeight / 2 * -1 + 1) Variables.offsetHeight--;
                    break;
                case "bottom":
                    if (Variables.offsetHeight < Console.WindowHeight / 2 - (Console.WindowHeight % 2 == 0 ? 2 : 1)) Variables.offsetHeight++;
                    break;
                default:
                    break;
            }
        }
    }
}

class Schnek
{
    static void Main(string[] args)
    {
        Settings.DefaultSettings();
        do
        {
            CUI CUI = new();
            Entity.Player Player = new();
        } while (Variables.pressedKey != ConsoleKey.Escape);
    }
}