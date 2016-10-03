using System;
using System.Diagnostics;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;


namespace ColorTestApp
{
    //http://stackoverflow.com/questions/28123300/xamarin-forms-c-sharp-find-dominant-color-of-image-or-image-byte-array
    //http://stackoverflow.com/questions/7807360/how-to-get-pixel-colour-in-android

    public class App : Application
    {
        public App ()
        {
            // The root page of your application

            var takePhoto = new Button() { Text = "GetPhoto"};
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
                    file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg"
                    });
                }


                if(file == null)
                    return;

               

                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    //file.Dispose();
                    return stream;
                });

               



#if ANDROID

                var bitmap1 = Android.Graphics.BitmapFactory.DecodeStream(file.GetStream());
                //var bitmap1 = Android.Graphics.BitmapFactory.DecodeFile(file.Path);
                ColorThief.ColorThief ct = new ColorThief.ColorThief();
                var xxxx = ct.GetColor(bitmap1);
                int a = 5;
#endif

            };


            MainPage = new ContentPage {
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        },
                        takePhoto,
                        image
                    }
                }
            };
        }


        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using(MemoryStream ms = new MemoryStream())
            {
                int read;
                while((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }



        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}
