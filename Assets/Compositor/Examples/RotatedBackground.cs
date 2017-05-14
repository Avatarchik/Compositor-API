/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

using UnityEngine;
using UnityEngine.UI;

namespace CompositorU.Examples {

	public class RotatedBackground : MonoBehaviour {

        public RawImage rawImage;
		public Texture2D photo;
        public Layer[] layers;

        // Use this for initialization
		void Start () {
            // Create a compositor // Swap width and height because we are rotating by 90 degrees
			using (var compositor = new RenderCompositor(photo.height, photo.width)) {
                // Add the photo texture
                compositor.AddLayer(new Layer(photo, Point.zero, 90f, Vector2.one, null));
                // Add layers
				foreach (var layer in layers) compositor.AddLayer (layer);
				// Composite and display the result
				compositor.Composite (result => rawImage.texture = result);
            }
        }
    }
}