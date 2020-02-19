using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PerformanceLoggerXamairn.Droid
{
    public class AndroidLogger : ILoggerProvider
    {
        private const string LogerTag = "Koptytko";
        private const string startStr = "Start ";
        private const string stepStr = "Step ";
        private const string stoppStr = "Stop ";
        private const string msStr = " ms.";
        private static readonly Dictionary<string, Stopwatch> stopwatches = new Dictionary<string, Stopwatch>();

        public void Start(string reference, string message, string path, string member, int? lineNumber)
        {
            this.WriteLine(startStr + message, path, member, lineNumber);
            stopwatches[reference] = Stopwatch.StartNew();
        }

        public void Step(string reference, string message, string path, string member, int? lineNumber)
        {
            if (stopwatches.TryGetValue(reference, out var stopwatch))
            {
                this.WriteLine(stepStr + stopwatch.ElapsedMilliseconds.ToString() + msStr + message, path, member, lineNumber);
            }
        }

        public void Stop(string reference, string message, string path, string member, int? lineNumber)
        {
            if (stopwatches.TryGetValue(reference, out var stopwatch))
            {
                stopwatch.Stop();
                this.WriteLine(stoppStr + stopwatch.ElapsedMilliseconds.ToString() + msStr + message, path, member, lineNumber);
                stopwatches.Remove(reference);
            }
        }

        public void WriteLine(string message, string path, string member, int? lineNumber)
        {
            Android.Util.Log.Debug(LogerTag, $"{DateTimeOffset.Now.ToString("HH:mm:ss.FFF"),-12}, T:{Thread.CurrentThread.ManagedThreadId,-3} {path} {member} {lineNumber} {message}");
        }
    }
}