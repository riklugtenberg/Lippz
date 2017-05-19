using System;
using Android.App;
using Android.Graphics;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.IO;

namespace Lippz.Droid
{
    [Activity(MainLauncher = true, Label = "RecordActivity")]
    public class RecordActivity : Activity , ISurfaceHolderCallback
    {
        private Android.Hardware.Camera _camera;
        private Android.Hardware.Camera.Parameters _parameters;

        private SurfaceView _surfaceView;
        private ISurfaceHolder _surfaceHolder;
        
        private MediaRecorder _mediaRecorder;

        public int _bestHeight, _bestWidth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.RequestFeature(WindowFeatures.NoTitle);
            Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);

            SetContentView(Resource.Layout.RecordWordView);
            
            _surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            _camera = GetCameraInstance();
            _surfaceHolder = _surfaceView.Holder;
            _surfaceHolder.AddCallback(this);
            _parameters = _camera.GetParameters();
            StartCamera();
        }


        /** A safe way to get an instance of the Camera object. */

        public static Android.Hardware.Camera GetCameraInstance()
        {
            Android.Hardware.Camera c = null;

            try
            {
                c = Android.Hardware.Camera.Open(1); // attempt to get a Camera instance
            }

            catch (Java.Lang.Exception e)
            {
                // Camera is not available (in use or does not exist)
            }

            return c; // returns null if camera is unavailable
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height){}

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            StartCamera();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            if (_camera != null)
            {
                _camera.StopPreview();
                _camera.Release();
                _camera = null;
            }
        }

        private void StartCamera()
        {
            System.Console.WriteLine("wertk");
            GetBestSupportedCameraSize();
            try
            {
                var p = _parameters;
                p.PictureFormat = ImageFormatType.Jpeg;
                p.JpegQuality = 100;
                p.Set("orientation", "portrait");
                p.SetRotation(90);
                p.SetPictureSize(_bestWidth, _bestHeight);
                _camera.SetParameters(p);
                _camera.SetPreviewDisplay(_surfaceHolder);
                _camera.SetDisplayOrientation(90);
                _camera.StartPreview();
            }
            catch (IOException e)
            {
            }
        }

        private void GetBestSupportedCameraSize()
        {
            var p = _parameters;
            foreach (var size in p.SupportedPictureSizes)

                if ((1200 <= size.Width) & (size.Width <= 1920)) // Higer than 1200 and lower than 1920
                {
                    _bestWidth = size.Width;
                    _bestHeight = size.Height;
                    break;
                }
        }
    }
}