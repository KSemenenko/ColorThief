#Color Thief .NET

A code for grabbing the color palette from an image. Uses C# and .NET to make it happen.

This is a ported project of [ColorThief](http://lokeshdhakar.com/projects/color-thief/) 

Many thanks for C# code [UWP Version](https://gist.github.com/zumicts/c5050a36e4ba742dc244)

###Platforms:
|Platform|Supported|Version|
| ------------------- | :-----------: | :------------------: |
|Xamarin.iOS|Partial|iOS 6+|
|Xamarin.Android|Partial|API 10+|
|Windows Phone 8|Partial|8.0+|
|Windows Phone 8.1|Partial|8.1+|
|Windows Store|Partial|8.1+|
|Windows 10 UWP|Partial|10+|
|Xamarin.Mac|Partial||
|Desktop .NET 4.5|Yes||
|Desktop .NET 4.6|Yes||

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
