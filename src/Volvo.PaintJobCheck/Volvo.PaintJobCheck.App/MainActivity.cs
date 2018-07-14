namespace CameraAppDemo
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.OS;
    using Android.Provider;
    using Android.Widget;
    using Java.IO;
    using Environment = Android.OS.Environment;
    using Uri = Android.Net.Uri;

    public static class App {
        public static File _file;
        public static File _file2;
        public static File _dir;     
        public static Bitmap bitmap;
        public static Bitmap bitmap2;
    }

    [Activity(Label = "Volvo Paint Job Check", MainLauncher = true)]
    public class MainActivity : Activity
    {
        int piccount = 0;
        private ImageView _imageView;
        private ImageView _imageView2;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery
            if (requestCode == 0)
            {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = _imageView.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    _imageView.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                }
            }
            else
            {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(App._file2);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = _imageView.Height;
                App.bitmap2 = App._file2.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap2 != null)
                {
                    _imageView2.SetImageBitmap(App.bitmap2);
                    App.bitmap2 = null;
                }
            }
            // Display in ImageView. We will resize the bitmap to fit the display
            // Loading the full sized image will consume to much memory 
            // and cause the application to crash.

            

            

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.myButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                _imageView2 = FindViewById<ImageView>(Resource.Id.imageView2);
                button.Click += TakeAPicture;
            }

        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "VolvoPaintJobChecks");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = 
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);

            Intent intent2 = new Intent(MediaStore.ActionImageCapture);

            App._file2 = new File(App._dir, String.Format("myPhoto2_{0}.jpg", Guid.NewGuid()));

            intent2.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file2));
            StartActivityForResult(intent2, 1);

        }
    }
}
