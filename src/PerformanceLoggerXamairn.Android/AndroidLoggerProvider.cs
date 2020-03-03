using Android.Util;
using PerformanceLoggerPortable;
using System.Threading;

namespace PerformanceLoggerXamairn.Droid
{
    public class AndroidLoggerProvider : DefaultLoggerProvider
    {
        public override void WriteLine(string message, string path, string member, int? lineNumber)
        {
            Log.Debug(Constants.Tag, $"T:{Thread.CurrentThread.ManagedThreadId,-3} {GetNicePath(path)}:{lineNumber} {member}() {message}");
        }
    }
}