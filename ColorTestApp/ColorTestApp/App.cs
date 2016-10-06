using System;
using Android.Graphics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;

#if WINDOWS_UWP
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#endif
#if __IOS__
using UIKit;
#endif

namespace ColorTestApp
{
    //http://stackoverflow.com/questions/28123300/xamarin-forms-c-sharp-find-dominant-color-of-image-or-image-byte-array
    //http://stackoverflow.com/questions/7807360/how-to-get-pixel-colour-in-android

    public class App : Application
    {
#if __IOS__
        public static string ResourcePrefix = "ColorTestApp.iOS.";
#endif
#if ANDROID
        public static string ResourcePrefix = "ColorTestApp.Droid.";
#endif
#if WINDOWS_UWP
        public static string ResourcePrefix = "ColorTestApp.UWP.";
#endif

        public App()
        {
            // The root page of your application

            var takePhoto = new Button {Text = "GetPhoto"};
            var image = new Image();

            takePhoto.Clicked += async (sender, args) =>
            {
                MediaFile file;

                if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    file = await CrossMedia.Current.PickPhotoAsync();
                }
                else
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg"
                    });
                }

                if(file == null)
                {
                    return;
                }

                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    //file.Dispose();
                    return stream;
                });

#if ANDROID

                var bitmap1 = BitmapFactory.DecodeStream(file.GetStream());
                //var bitmap1 = Android.Graphics.BitmapFactory.DecodeFile(file.Path);
                var ct = new ColorThief.ColorThief();
                var ctColor = ct.GetColor(bitmap1);               
                MainPage.BackgroundColor = Color.FromHex(ctColor.Color.ToHexString());
                var a = 5;
#elif WINDOWS_UWP

                UWP(file);
#elif __IOS__

                var bitmap1 = UIImage.FromFile(file.Path);
                ColorThief.ColorThief ct = new ColorThief.ColorThief();
                var ctColor = ct.GetColor(bitmap1);
                MainPage.BackgroundColor = Color.FromHex(ctColor.Color.ToHexString());
                int a = 5;
#endif
            };

            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        },
                        takePhoto,
                        image
                    }
                }
            };

        }

#if WINDOWS_UWP
        private async Task UWP(MediaFile file)
        {
            try
            {
                using(IRandomAccessStream stream = file.GetStream().AsRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var ct = new ColorThief.ColorThief();
                    var ctColor = await ct.GetColor(decoder);
                    MainPage.BackgroundColor = Color.FromHex(ctColor.Color.ToHexString());
                    var a = 5;
                }
            }
            catch(Exception ex)
            {
                var a = 5;
            }
        }
#endif
    }
}