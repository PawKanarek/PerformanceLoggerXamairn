using PerformanceLoggerPortable;
using System;
using System.Threading.Tasks;

namespace PerformanceLoggerConsoleApp
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            var adb = new Adb();
            adb.LogReceived += Adb_LogReceived;
            await Task.Delay(10000000);
            adb.LogReceived -= Adb_LogReceived;
            adb.Dispose();
        }

        private static void Adb_LogReceived(object sender, string e)
        {
            Console.WriteLine(e);
        }
    }
}
