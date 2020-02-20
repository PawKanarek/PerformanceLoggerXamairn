# PerformanceLoggerXamairn
Light Logger for Xamarin.Forms apps with extension for Visual Stuido

Example of usage: 
```
Logger.SetProvider(new AndroidLogger()); // Initalize Logger

... 

Logger.Start(out var reference); // Start Measuring time
...
Logger.Step(reference); // Lets make step and see how long its take
...
Logger.Step(reference); // Lets another make step and see how long its take
...
Logger.Stop(reference); // stop measuring

....

Logger.WriteLine("HelloWriteLine"); // you can also use WriteLine
```

This will give you this bjuːtɪfʊl output
```
02-20 03:10:53.058  6617  6617 D DevLogger: T:1   src.Xamairn.Android.MainActivity.cs:19 OnCreate() Start 
02-20 03:10:53.177  6617  6617 D DevLogger: T:1   src.Xamairn.Android.MainActivity.cs:23 OnCreate() Step 118 ms.
02-20 03:10:53.226  6617  6617 D DevLogger: T:1   src.Xamairn.Android.MainActivity.cs:26 OnCreate() Step 167 ms.
02-20 03:10:53.457  6617  6617 D DevLogger: T:1   src.Xamairn.Android.MainActivity.cs:29 OnCreate() Stop 398 ms.
02-20 03:10:53.458  6617  6617 D DevLogger: T:1   src.Xamairn.Android.MainActivity.cs:31 OnCreate() HelloWriteLine
```
# But hey. There is more! 

I have made Visual Studio Extension that filters results from output window. Debugging was never easier.  
To toggle between 3 states use this:

![Toggle between states](https://raw.githubusercontent.com/PawKanarek/PerformanceLoggerXamairn/master/screenshoots/how_to_start.png)

First state shows you full adb logs

![First state](https://raw.githubusercontent.com/PawKanarek/PerformanceLoggerXamairn/master/screenshoots/state_active.png)

Second state shows you all logs from your app that has been logged with my logger

![Second state](https://raw.githubusercontent.com/PawKanarek/PerformanceLoggerXamairn/master/screenshoots/state_filtered.png)

Thrid state is disabled

![Thrid state](https://raw.githubusercontent.com/PawKanarek/PerformanceLoggerXamairn/master/screenshoots/state_disabled.png)

# Loggers Example
```
 public class AndroidLogger : ILoggerProvider
    {
        private const string LogerTag = "DevLogger";
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
            Android.Util.Log.Debug(LogerTag, $"T:{Thread.CurrentThread.ManagedThreadId,-3} {GetNicePath(path)}:{lineNumber} {member}() {message}");
        }

        private static string GetNicePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            var splitted = path.Split('\\').Reverse().Take(3).Reverse();
            return string.Join('.', splitted);
        }
```

# TOD0
ios
