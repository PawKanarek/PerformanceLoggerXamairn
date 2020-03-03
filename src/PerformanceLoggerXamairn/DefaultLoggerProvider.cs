using PerformanceLoggerPortable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PerformanceLoggerXamairn
{
    public class DefaultLoggerProvider : ILoggerProvider
    {
        protected static readonly Dictionary<string, Stopwatch> stopwatches = new Dictionary<string, Stopwatch>();

        public virtual void Start(string reference, string message, string path, string member, int? lineNumber)
        {
            this.WriteLine(Constants.StartStr + message, path, member, lineNumber);
            stopwatches[reference] = Stopwatch.StartNew();
        }

        public virtual long Step(string reference, string message, string path, string member, int? lineNumber)
        {
            var elapsed = -1L;
            if (stopwatches.TryGetValue(reference, out var stopwatch))
            {
                elapsed = stopwatch.ElapsedMilliseconds;
                this.WriteLine(Constants.StepStr + elapsed.ToString() + Constants.MsStr + message, path, member, lineNumber);
            }
            return elapsed;
        }

        public virtual long Stop(string reference, string message, string path, string member, int? lineNumber)
        {
            var elapsed = -1L;
            if (stopwatches.TryGetValue(reference, out var stopwatch))
            {
                elapsed = stopwatch.ElapsedMilliseconds;
                stopwatch.Stop();
                this.WriteLine(Constants.StopStr + elapsed.ToString() + Constants.MsStr + message, path, member, lineNumber);
                stopwatches.Remove(reference);
            }
            return elapsed;
        }

        public virtual void WriteLine(string message, string path, string member, int? lineNumber)
        {
            Console.WriteLine($"{Constants.Tag} T:{Thread.CurrentThread.ManagedThreadId,-3} {GetNicePath(path)}:{lineNumber} {member}() {message}");
        }

        protected static string GetNicePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            var splitted = path.Split('\\').Reverse().Take(2).Reverse();
            return string.Join(".", splitted);
        }
    }
}