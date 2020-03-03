using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PerformanceLoggerPortable;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Constants = PerformanceLoggerPortable.Constants;
using Task = System.Threading.Tasks.Task;

namespace PerformanceLoggerVsix
{
    internal sealed class LogcatOutput
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("2bfe5ff2-cf9e-4730-8c8b-71e2b58280ee");

        private readonly AsyncPackage package;
        private Adb adb;
        private Guid outputWindowGuid;
        private IVsOutputWindow outputWindow;
        private IVsOutputWindowPane pane;

        private LogcatOutput(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static LogcatOutput Instance
        {
            get;
            private set;
        }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new LogcatOutput(package, commandService);
        }

        [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "MenuCommand uses event handler without option of async Task.")]
        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (this.outputWindowGuid == Guid.Empty)
            {
                this.outputWindowGuid = new Guid();
            }

            if (this.outputWindow == null)
            {
                this.outputWindow = await this.ServiceProvider.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
                Assumes.Present(this.outputWindow);
                this.outputWindow.CreatePane(ref this.outputWindowGuid, "LogcatOutput", Convert.ToInt32(true), Convert.ToInt32(false));
                this.outputWindow.GetPane(ref this.outputWindowGuid, out this.pane);
            }
            this.pane.Clear();
            this.pane.OutputString("LogcatOutput Activated\n");

            this.ReleaseAdbProcess();
            try
            {
                this.adb = new Adb();
                this.adb.LogReceived += this.Adb_LogReceived;
                this.adb.FilterName = "DevLogger";
            }
            catch (AdbInitalizeException ex)
            {
                this.pane.OutputString(ex.Message);
            }
        }

        public void ReleaseAdbProcess()
        {
            if (this.adb == null)
            {
                return;
            }

            this.adb.LogReceived -= this.Adb_LogReceived;
            this.adb.Dispose();
            this.adb = null;
        }

        [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event Listener can be async")]
        private async void Adb_LogReceived(object sender, string log)
        {
            if (this.outputWindow == null)
            {
                return;
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var success = false;
            try
            {
                // well formatted log shoud look like this:
                // *adb prefix messages* *logcat Tag* *FileTag**Filename*;*LineNumber**FileTag* *Thread info* *user message*
                // real world example
                // log = "03-03 22:44:00.856 D/DevLogger(10813): <'File'>C:\App\Activity.cs;19<'File'>OnCreate() T:1. Hello World"
                var logcatMessage = log.Split(new string[] { Constants.FileTag }, StringSplitOptions.None);
                if (logcatMessage.Length == 3)
                {
                    var pathPart = logcatMessage[1].Split(Constants.LineTag);       // "C:\App\Activity.cs;19"                      -> string[] { "C:\App\Activity.cs", "19" }

                    var sb = new StringBuilder(logcatMessage[0].Split(' ')[1]);     // "03-03 22:44:00.856 D/DevLogger(10813): "    -> "22:44:00.856"
                    sb.Append(' ');
                    sb.Append(pathPart[0].Split('\\').Last());                      // "C:\App\Activity.cs"                         -> "Activity.cs"
                    sb.Append(':');
                    sb.Append(pathPart[1]);                                         // "19"
                    sb.Append(' ');
                    sb.Append(logcatMessage[2]);                                    //OnCreate() T:1. Hello World
                    sb.Append(Environment.NewLine);
                    this.pane.OutputTaskItemString(sb.ToString(), VSTASKPRIORITY.TP_NORMAL, VSTASKCATEGORY.CAT_BUILDCOMPILE, string.Empty, 0, pathPart[0], Convert.ToUInt32(pathPart[1]) - 1, logcatMessage[2]);
                    success = true;
                }
            }
            catch (Exception)
            {
                success = false;
            }

            if (!success)
            {
                this.pane.OutputString(log + Environment.NewLine);
            }
        }
    }
}
