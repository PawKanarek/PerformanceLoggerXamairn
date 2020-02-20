using AdbPortable;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace LogcatOutput
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class LogcatOutput
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("2bfe5ff2-cf9e-4730-8c8b-71e2b58280ee");

        private readonly AsyncPackage package;
        private Adb adb;
        private Guid outputWindowGuid;
        private IVsOutputWindow outputWindow;
        private IVsOutputWindowPane pane;
        private State state;

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

        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            switch (this.state)
            {
                case State.Disabled:

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
                    this.pane.OutputString("State: ADB Active\n");
                    this.state = State.Active;

                    this.adb = new Adb();
                    this.adb.LogReceived += this.Adb_LogReceived;

                    break;
                case State.Active:

                    this.adb.FilterName = "DevLogger";

                    this.pane.Clear();
                    this.pane.OutputString("State: ADB Active with Filter: 'DevLogger'\n");

                    this.state = State.ActiveWithFilter;

                    break;
                case State.ActiveWithFilter:

                    this.adb.LogReceived -= this.Adb_LogReceived;
                    this.adb.Dispose();
                    this.adb = null;

                    this.pane.Clear();
                    this.pane.OutputString("State: ADB Disabled \n");

                    this.state = State.Disabled;

                    break;
            }
        }

        private async void Adb_LogReceived(object sender, string e)
        {
            if (this.outputWindow == null)
            {
                return;
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Retrieve the new pane.
            this.pane.OutputString(e + "\n");
        }

        private enum State
        {
            Disabled,
            Active,
            ActiveWithFilter
        }
    }
}
