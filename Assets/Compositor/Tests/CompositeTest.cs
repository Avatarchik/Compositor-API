/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

using UnityEngine;
using UnityEngine.UI;

namespace CompositorU.Tests {

	public class CompositeTest : MonoBehaviour {

		public RawImage rawImage;
		public Layer[] layers;

		// Use this for initialization
		void Start () {
			// Create a compositor
			using (var compositor = new RenderCompositor()) {
				// Add layers
				foreach (var layer in layers) compositor.AddLayer (layer);
				// Composite and display the result
				compositor.Composite (result => rawImage.texture = result);
			}
		}
	}
}