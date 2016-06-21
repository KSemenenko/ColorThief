#Color Thief .NET

A code for grabbing the color palette from an image. Uses C# and .NET to make it happen.

This is a ported project of [ColorThief](http://lokeshdhakar.com/projects/color-thief/) 

Many thanks for C# code [UWP Version](https://gist.github.com/zumicts/c5050a36e4ba742dc244)

###Platforms:
|Platform|Supported|Version|
| ------------------- | :-----------: | :------------------: |
|Xamarin.iOS|Yes|iOS 6+|
|Xamarin.Android|Yes|API 10+|
|Windows Phone 8|Yes|8.0+|
|Windows Phone 8.1|Yes|8.1+|
|Windows Store|Yes|8.1+|
|Windows 10 UWP|Yes|10+|
|Xamarin.Mac|Partial||
|.NET 4.5|Yes||
|.NET 4.6|Yes||

##How to use

###Get the dominant color from an image
```cs
var colorThief = new ColorThief();
colorThief.GetColor(sourceImage);
```

###Build a color palette from an image

In this example, we build an 8 color palette.

```cs
var colorThief = new ColorThief();
colorThief.GetPalette(sourceImage, 8);
```
