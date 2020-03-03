using System.Runtime.CompilerServices;
using System.Threading;

namespace PerformanceLoggerXamairn
{
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

        public static long Step(string reference, string message = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null, [CallerLineNumber] int? lineNumber = null)
        {
            return Provider?.Step(reference, message, path, member, lineNumber) ?? -1L;
        }

        public static long Stop(string reference, string message = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null, [CallerLineNumber] int? lineNumber = null)
        {
            return Provider?.Stop(reference, message, path, member, lineNumber) ?? -1L;
        }

        public static void WriteLine(string message = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null, [CallerLineNumber] int? lineNumber = null)
        {
            Provider?.WriteLine(message, path, member, lineNumber);
        }
    }
}