using System;
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

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
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
               

                if (file == null)
                    return;


                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    return stream;
                });


               // ColorThief.ColorThief ct = new ColorThief.ColorThief();
              

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
