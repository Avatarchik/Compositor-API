/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

using UnityEngine;
using UnityEngine.UI;

namespace CompositorU.Examples {

	public class RotatedBackground : MonoBehaviour {

        public RawImage rawImage;
        public Layer[] layers;

        // Use this for initialization
		void Start () {
            // Size checking
            if (layers.Length == 0) return;
            // Create a compositor // Swap width and height because we are rotating by 90 degrees
			using (var compositor = new RenderCompositor(layers[0].texture.height, layers[0].texture.width)) {
                // Add layers
				foreach (var layer in layers) compositor.AddLayer (layer);
				// Composite and display the result
				compositor.Composite (result => rawImage.texture = result);
            }
        }
    }
}