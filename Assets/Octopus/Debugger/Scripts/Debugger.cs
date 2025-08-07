using UnityEngine;

namespace Core
{
    public static class ConsoleReporter
    {
        private static readonly string DefaultHexColor = "BEBEBEFF"; // Gray

        private static string GetColorHex(Color tone)
        {
            return ColorUtility.ToHtmlStringRGBA(tone == default ? Color.gray : tone);
        }

        private static void Print(string content, string level, Color tone)
        {
            if (!Settings.IsDebug()) return;

            var hex = GetColorHex(tone);
            
            var formatted = $"<color=#{hex}>{content}</color>";

            switch (level)
            {
                case "log":
                    Debug.Log(formatted);
                    break;
                case "warn":
                    Debug.LogWarning(formatted);
                    break;
                case "error":
                    Debug.LogError(formatted);
                    break;
            }
        }

        public static void Info(string msg, Color color = default)
        {
            Print(msg, "log", color);
        }

        public static void Warn(string msg)
        {
            Print(msg, "warn", new Color(1f, 0.85f, 0f));
        }

        public static void Fail(string msg)
        {
            Print(msg, "error", Color.red);
        }
    }
}

