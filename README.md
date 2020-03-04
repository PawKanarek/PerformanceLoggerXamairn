# PerformanceLoggerXamairn
Light & Fast Performance Logger for Xamarin.Forms apps with additional extension for Visual Stuido

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
01:52:34.693 MainActivity.cs:19 OnCreate() T:1. Start 
01:52:34.833 MainActivity.cs:23 OnCreate() T:1. Step 139 ms.
01:52:34.892 MainActivity.cs:26 OnCreate() T:1. Step 198 ms.
01:52:35.150 MainActivity.cs:29 OnCreate() T:1. Stop 456 ms.
01:52:35.151 MainActivity.cs:31 OnCreate() T:1. HelloWriteLine
```
Real life example 

![example](https://github.com/PawKanarek/PerformanceLoggerXamairn/blob/master/images/Preview.png)
##  Visual Studio Extension 
Visual Studio Extension available here:
https://marketplace.visualstudio.com/items?itemName=PawKanarek.v1

To use VisualStudio Extension you need to initalize logger with AndroidLoggerProvider.

## Loggers Example
[DefaultLoggerProvider.cs](https://github.com/PawKanarek/PerformanceLoggerXamairn/blob/master/src/PerformanceLoggerXamairn/DefaultLoggerProvider.cs)

[AndroidLoggerProvider.cs](https://github.com/PawKanarek/PerformanceLoggerXamairn/blob/master/src/PerformanceLoggerXamairn.Android/AndroidLoggerProvider.cs)

# TOD0
ios
