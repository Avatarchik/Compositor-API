# Compositor API
Compositor is a lightweight utility API for compositing images quickly and efficiently in Unity.
Download the unitypackage [here](http://www.google.com).

## Compositor
Compositor treats images as layers. First, you create a compositor:
```csharp
var compositor = new RenderCompositor();
```
Then you add layers:
```csharp
compositor.AddLayer(texture);
```
Then composite, providing a callback to be invoked with the resulting texture:
```csharp
compositor.Composite(result => material.mainTexture = result);
```
Finally, release the compositor's resources:
```csharp
compositor.Dispose();
```
Compositors implement the IDisposable interface, so you can use it in a `using` block:
```csharp
using (var compositor = new RenderCompositor()) {
    // Add layers...
    compositor.Composite(OnComposite);
} // The compositor is automatically freed at the end of this block
```

## RenderCompositor
*INCOMPLETE*

## Credits
- [Yusuf Olokoba](mailto:olokobayusuf@gmail.com)
- [Owen Mundy](omundy@gmail.com)