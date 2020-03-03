using Android.Util;
using PerformanceLoggerPortable;
using System;
using System.Text;
using System.Threading;

namespace PerformanceLoggerXamairn.Droid
{
    public class AndroidLoggerProvider : DefaultLoggerProvider
    {
        public override void WriteLine(string message, string path, string member, int? lineNumber)
        {
            var sb = new StringBuilder(Constants.FileTag);
            sb.Append(path);
            sb.Append(Constants.LineTag);
            sb.Append(lineNumber);
            sb.Append(Constants.FileTag);
            sb.Append(member);
            sb.Append("() T:");
            sb.Append(Thread.CurrentThread.ManagedThreadId);
            sb.Append(". ");
            sb.Append(message);

            Log.Debug(Constants.Tag, sb.ToString());
        }
    }
}