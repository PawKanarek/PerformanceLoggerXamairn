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
            var guid = new Guid();
            var output = await this.ServiceProvider.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Assumes.Present(output);

            // Create a new pane.
            output.CreatePane(
                ref guid,
                "Pane LogcatOutput Title",
                Convert.ToInt32(true),
                Convert.ToInt32(false));

            // Retrieve the new pane.
            output.GetPane(ref guid, out IVsOutputWindowPane pane);

            pane.OutputString("This is the Created Pane \n");
        }
    }
}
