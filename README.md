# PerformanceLoggerXamairn
Light Logger for Xamarin.Forms apps with extension for Visual Stuido

Universal Logger:
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
02-20 03:10:53.058  6617  6617 D DevLogger: T:1   src.PerformanceLoggerXamairn.Android.MainActivity.cs:19 OnCreate() Start 
02-20 03:10:53.177  6617  6617 D DevLogger: T:1   src.PerformanceLoggerXamairn.Android.MainActivity.cs:23 OnCreate() Step 118 ms.
02-20 03:10:53.226  6617  6617 D DevLogger: T:1   src.PerformanceLoggerXamairn.Android.MainActivity.cs:26 OnCreate() Step 167 ms.
02-20 03:10:53.457  6617  6617 D DevLogger: T:1   src.PerformanceLoggerXamairn.Android.MainActivity.cs:29 OnCreate() Stop 398 ms.
02-20 03:10:53.458  6617  6617 D DevLogger: T:1   src.PerformanceLoggerXamairn.Android.MainActivity.cs:31 OnCreate() HelloWriteLine
```
# But hey. There is more! 

I have made Visual Studio Extension that filters results from output window. Debugging was never easier.  
To toggle betwen 3 states use this:


First state shows you full adb logs

Second state shows you all logs from your app (that has been logged with my logger)

Thrid state is disabled
