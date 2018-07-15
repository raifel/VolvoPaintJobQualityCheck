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
    using IC6.Xamarin.PictureUpload;
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
                Button analyze = FindViewById<Button>(Resource.Id.button1);
                analyze.Click += Analyze_Click;

            }

        }

        private void Analyze_Click(object sender, EventArgs e)
        {
            CheckBox checkBoxReference = FindViewById<CheckBox>(Resource.Id.checkBoxReference);
            try
            {
                var batch = "reference";
                if (checkBoxReference.Checked == false)
                {
                    batch = DateTime.Now.Ticks.ToString();
                }
                var filestream = System.IO.File.OpenRead(App._file.AbsolutePath);
                ApiService ap = new ApiService();
                var x = ap.UploadImageAsync(filestream, "front.jpg", checkBoxReference.Checked, batch, 1);

                var filestream2 = System.IO.File.OpenRead(App._file2.AbsolutePath);
                ApiService ap2 = new ApiService();
                var m = ap2.UploadImageAsync(filestream2, "side.jpg", checkBoxReference.Checked, batch, 2);

                var message = "Images are uploaded sucussfully. Wait for result";
                Toast.MakeText(ApplicationContext, message, ToastLength.Long).Show();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void CreateDirectoryForPictures()
        {
            //Intent intent = new Intent();
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
