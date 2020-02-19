using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace PerformanceLoggerXamairn.Droid
{
    [Activity(Label = "PerformanceLoggerXamairn", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Logger.SetProvider(new AndroidLogger());
            
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Logger.Start(out var reference);
            base.OnCreate(savedInstanceState);
            Logger.Step(reference);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Logger.Step(reference);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Logger.Step(reference);

            LoadApplication(new App());
            Logger.Stop(reference);

            Logger.WriteLine("HelloWriteLine");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}