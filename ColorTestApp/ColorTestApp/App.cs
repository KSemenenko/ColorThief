using System;
using System.Threading.Tasks;
using ColorThiefDotNet;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
#if ANDROID
using Android.Graphics;
#endif
#if WINDOWS_UWP
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using ColorThiefDotNet;
using Plugin.Media.Abstractions;
#endif
#if __IOS__
using UIKit;
#endif

namespace ColorTestApp
{
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
            var loadImage = new Button { Text = "loadImage" };
            var image = new Image();

            loadImage.Clicked += async (sender, args) =>
            {
                var source = ImageSource.FromResource(ResourcePrefix + "test1.jpg");
                await Forms(source);
            };

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


                Forms(image.Source);

    

                return;

#if ANDROID

                var bitmap1 = BitmapFactory.DecodeStream(file.GetStream());
                var ct = new ColorThief();
                var ctColor = ct.GetColor(bitmap1);               
                MainPage.BackgroundColor = Color.FromHex(ctColor.Color.ToHexString());
                var a = 5;
#elif WINDOWS_UWP

                UWP(file);
#elif __IOS__

                var bitmap1 = UIImage.FromFile(file.Path);
                ColorThief ct = new ColorThief();
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
                        loadImage,
                        image
                    }
                }
            };

        }

        private async Task Forms(ImageSource file)
        {
            var ctColor = await ColorThiefDotNet.Forms.CrossColorThief.Current.GetColor(file);
            MainPage.BackgroundColor = Color.FromHex(ctColor.Color.ToHexString());
        }

#if WINDOWS_UWP
        private async Task UWP(MediaFile file)
        {
            try
            {
                using(IRandomAccessStream stream = file.GetStream().AsRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var ct = new ColorThief();
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