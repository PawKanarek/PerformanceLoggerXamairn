using System.Runtime.CompilerServices;
using System.Threading;

namespace PerformanceLoggerXamairn
{
    public interface ILoggerProvider
    {
        void Stop(string reference, string message, string path, string member, int? lineNumber);
        void Step(string reference, string message, string path, string member, int? lineNumber);
        void Start(string reference, string message, string path, string member, int? lineNumber);
        void WriteLine(string message, string path, string member, int? lineNumber);
    }

    public static class Logger
    {
        private static long interlockedReference;

        public static ILoggerProvider Provider { get; private set; }

        public static void SetProvider(ILoggerProvider instance)
        {
            Provider = instance;
        }

        public static void Start(out string reference, string message = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null, [CallerLineNumber] int? lineNumber = null)
        {
            if (Provider == null)
            {
                reference = string.Empty;
                return;
            }

            reference = Interlocked.Increment(ref interlockedReference).ToString();
            Provider.Start(reference, message, path, member, lineNumber);
        }

        public static void Step(string reference, string message = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null, [CallerLineNumber] int? lineNumber = null)
        {
            Provider?.Step(reference, message, path, member, lineNumber);
        }

        public static void Stop(string reference, string message = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null, [CallerLineNumber] int? lineNumber = null)
        {
            Provider?.Stop(reference, message, path, member, lineNumber);
        }

        public static void WriteLine(string message = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null, [CallerLineNumber] int? lineNumber = null)
        {
            Provider?.WriteLine(message, path, member, lineNumber);
        }
    }
}
