namespace ClubCore.Utilities
{
    /// <summary>
    /// Lightweight console logging helper for development and CLI tools.
    /// This avoids pulling in a full logging framework until the architecture
    /// is ready for it.
    /// </summary>
    public static class Log
    {
        public static void Info(string message)
            => Console.WriteLine($"[INFO] {message}");

        public static void Warn(string message)
            => Console.WriteLine($"[WARN] {message}");

        public static void Error(string message)
            => Console.WriteLine($"[ERROR] {message}");
    }
}