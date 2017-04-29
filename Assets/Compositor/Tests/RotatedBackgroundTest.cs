/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

using UnityEngine;
using UnityEngine.UI;

namespace CompositorU.Tests {

	public class RotatedBackgroundTest : MonoBehaviour {

        public RawImage rawImage;
		public Texture2D photo;
        public Layer[] layers;

        // Use this for initialization
		void Start () {
            // Create a compositor
			using (var compositor = new RenderCompositor()) {
                // Add a spoof layer
                compositor.AddLayer(new Layer(
                    new Texture2D(photo.height, photo.width),
                    Point.zero,
                    0f,
                    Vector2.one,
                    Layer.Release
                ));
                // Add the photo texture
                compositor.AddLayer(new Layer(
                    photo,
                    Point.zero,
                    90f,
                    Vector2.one,
                    null
                ));
                // Add layers
				foreach (var layer in layers) compositor.AddLayer (layer);
				// Composite and display the result
				compositor.Composite (result => rawImage.texture = result);
            }
        }
    }
}