using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PerformanceLoggerPortable;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "MenuCommand uses event handler without option of async Task.")]
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
        private async void Adb_LogReceived(object sender, string e)
        {
            if (this.outputWindow == null)
            {
                return;
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Retrieve the new pane.
            //this.pane.OutputString(e + "\n");
            this.pane.OutputTaskItemString(e + Environment.NewLine, VSTASKPRIORITY.TP_NORMAL, VSTASKCATEGORY.CAT_BUILDCOMPILE, "MergeUi", 0, "C:\\Bitbucket\\PerformanceLoggerXamairn\\src\\PerformanceLoggerXamairn\\MainPage.xaml.cs", 12, e);
        }
    }
}
