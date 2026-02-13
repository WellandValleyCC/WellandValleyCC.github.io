using ClubCore.Interfaces;

namespace ClubCore.Utilities
{
    public class DefaultLog : ILog
    {
        public void Info(string message) => Log.Info(message);
        public void Warn(string message) => Log.Warn(message);
        public void Error(string message) => Log.Error(message);
    }
}