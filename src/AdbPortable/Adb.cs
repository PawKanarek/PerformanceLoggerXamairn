using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AdbPortable
{
    public class Adb : IDisposable
    {
        private const string androidHome = "ANDROID_HOME";
        private Process logCatProcess;
        public event EventHandler<string> LogReceived;

        public Adb()
        {
            var adbPath = this.GetAdbPath();

            var logProcessInfo = new ProcessStartInfo();
            logProcessInfo.CreateNoWindow = true;
            logProcessInfo.UseShellExecute = false;
            logProcessInfo.RedirectStandardOutput = true;
            logProcessInfo.RedirectStandardError = true;
            logProcessInfo.StandardOutputEncoding = Encoding.UTF8;
            logProcessInfo.FileName = adbPath;
            logProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
            logProcessInfo.Arguments = "logcat -T 1";

            this.logCatProcess = Process.Start(logProcessInfo);
            this.logCatProcess.ErrorDataReceived += this.LogCatProcess_ErrorDataReceived;
            this.logCatProcess.OutputDataReceived += this.LogCatProcess_OutputDataReceived;
            this.logCatProcess.BeginErrorReadLine();
            this.logCatProcess.BeginOutputReadLine();
        }

        public string FilterName { get; set; }

        private void LogCatProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data?.Length > 2)
            {
                this.AddLog(e.Data);
            }
        }

        private void LogCatProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data?.Length > 2)
            {
                this.AddLog(e.Data);
            }
        }

        private void AddLog(string log)
        {
            if (!string.IsNullOrWhiteSpace(this.FilterName) && log.IndexOf(this.FilterName, StringComparison.OrdinalIgnoreCase) < 0) //ignoring case without linq
            {
                return;
            }

            EventHandler<string> handler = this.LogReceived;
            handler?.Invoke(this, log);
        }

        private string GetAdbPath()
        {
            var path = Environment.GetEnvironmentVariable(androidHome);
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"There is no {androidHome} environment variable");
            }

            return Path.Combine(path, Path.Combine("platform-tools", "adb"));
        }

        private void StopLogCatProcess()
        {
            if (this.logCatProcess == null)
            {
                return;
            }

            try
            {
                if (!this.logCatProcess.HasExited)
                {
                    this.logCatProcess.Kill();
                }
            }
            catch (InvalidOperationException) { } // Just ignore
            catch (Win32Exception) { } // Just ignore
            catch (NotSupportedException) { } // Just ignore
            finally
            {
                this.logCatProcess.ErrorDataReceived -= this.LogCatProcess_ErrorDataReceived;
                this.logCatProcess.OutputDataReceived -= this.LogCatProcess_OutputDataReceived;
                this.logCatProcess.Dispose();
                this.logCatProcess = null;
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.StopLogCatProcess();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
