#Color Thief

A code for grabbing the color palette from an image. Uses C# and .NET to make it happen.

This is a ported project of [ColorThief](http://lokeshdhakar.com/projects/color-thief/) 
Many thanks for C# code [UWP Version](https://gist.github.com/zumicts/c5050a36e4ba742dc244)

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
