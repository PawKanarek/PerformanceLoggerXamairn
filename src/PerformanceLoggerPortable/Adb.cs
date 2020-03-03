using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PerformanceLoggerPortable
{
    public class Adb : IDisposable
    {
        private Process logCatProcess;
        public event EventHandler<string> LogReceived;

        /// <summary>
        /// Inializes new adb process
        /// </summary>
        /// <exception cref="AdbInitalizeException">Thrown when couldnt initalize adb process</exception>
        public Adb()
        {
            var adbPath = this.GetAdbPath();
            if (string.IsNullOrEmpty(adbPath))
            {
                throw new AdbInitalizeException($"Could not find any valid path where adb.exe exists. Add directory that contains Android platform-tool adb.exe to PATH or ANDROID_HOME environment variable");
            }

            var logProcessInfo = new ProcessStartInfo();
            logProcessInfo.CreateNoWindow = true;
            logProcessInfo.UseShellExecute = false;
            logProcessInfo.RedirectStandardOutput = true;
            logProcessInfo.RedirectStandardError = true;
            logProcessInfo.StandardOutputEncoding = Encoding.UTF8;
            logProcessInfo.FileName = adbPath;
            logProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
            logProcessInfo.Arguments = "logcat -T 1";

            try
            {
                this.logCatProcess = Process.Start(logProcessInfo);
            }
            catch (FileNotFoundException ex)
            {
                throw new AdbInitalizeException($"Could not start adb process on path {adbPath}", ex);
            }
            catch (Win32Exception ex)
            {
                throw new AdbInitalizeException($"Could not start adb process on path {adbPath}", ex);
            }

            if (this.logCatProcess != null)
            {
                this.logCatProcess.ErrorDataReceived += this.LogCatProcess_ErrorDataReceived;
                this.logCatProcess.OutputDataReceived += this.LogCatProcess_OutputDataReceived;
                this.logCatProcess.BeginErrorReadLine();
                this.logCatProcess.BeginOutputReadLine();
            }
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

            var handler = this.LogReceived;
            handler?.Invoke(this, log);
        }

        private string GetAdbPath()
        {
            var path = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (string.IsNullOrEmpty(path))
            {
                path = @"C:\Program Files (x86)\Android\android-sdk";
            }

            path = this.getPlatformToolsPath(path);

            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                var pathVariables = Environment.GetEnvironmentVariable("PATH");
                foreach (var item in pathVariables.Split(Path.PathSeparator))
                {
                    if (File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        path = this.getPlatformToolsPath(item);
                        if (File.Exists(path))
                        {
                            return path;
                        }
                        else
                        {
                            path = Path.Combine(item, "adb.exe");
                            if (File.Exists(path))
                            {
                                return path;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        private string getPlatformToolsPath(string parentPath)
        {
            return Path.Combine(parentPath, Path.Combine("platform-tools", "adb.exe"));
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

    /// <summary>
    /// Thrown when couldnt initalize adb process
    /// </summary>
    public class AdbInitalizeException : Exception
    {
        public AdbInitalizeException() : base()
        {
        }

        public AdbInitalizeException(string message) : base(message)
        {
        }

        public AdbInitalizeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
