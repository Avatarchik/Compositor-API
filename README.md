# Compositor API
Compositor is a lightweight utility API for compositing images quickly and efficiently in Unity.
Download the unitypackage [here](http://www.google.com).

## How to Use
Compositor treats images as layers. First, you create a compositor:
```csharp
var compositor = new ImageCompositor();
```
Then you add layers:
```csharp
compositor.AddLayer(texture);
```
Then composite, providing a callback to be invoked with the resulting texture:
```csharp
compositor.Composite(result => material.mainTexture = result);
```

## Architecture
*INCOMPLETE*

## Credits
- [Yusuf Olokoba](mailto:olokobayusuf@gmail.com)