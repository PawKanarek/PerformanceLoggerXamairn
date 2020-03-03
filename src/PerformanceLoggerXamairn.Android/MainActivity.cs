
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace PerformanceLoggerXamairn.Droid
{
    [Activity(Label = "PerformanceLoggerXamairn", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Logger.SetProvider(new AndroidLoggerProvider()); // Initalize Logger

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Logger.Start(out var reference); // Start Measuring
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Logger.Step(reference);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Logger.Step(reference);

            this.LoadApplication(new App());
            Logger.Stop(reference); 

            Logger.WriteLine("HelloWriteLine"); // you can also use WriteLine
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}