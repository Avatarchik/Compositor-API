/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using Layers = System.Collections.Generic.List<Layer>;

	public sealed class GPUCompositor : IDisposable {

		#region --Op vars--
		private RenderTexture composite;
		private Material material;
		private GraphicsQueue commandQueue;
		private Layers layers;
		#endregion


		#region --Client API--

		public GPUCompositor () {
			// Create the material
			material = new Material (Shader.Find ("Hidden/Compositor2D"));
			// Create the graphic command queue
			commandQueue = new GraphicsQueue();
			// Create the layers collection
			layers = new Layers();
		}

		public void AddLayer (Layer layer) {
			// Check and create the composite target
			composite = composite ?? RenderTexture.GetTemporary(layer.texture.width, layer.texture.height, 0);
			// Prepare the composite for compositing
			if (layers.Count == 0) composite.DiscardContents();
			// Add the layer
			layers.Add(layer);
		}

		public void Composite (CompositeCallback callback) {
			// Null checking
			if (callback == null) {
				Debug.LogError("Compositor: Callback must not be null");
				return;
			}
			// Count checking
			if (layers.Count == 0) {
				Debug.LogError("Compositor: No layers provided");
				return;
			}
			// Composite all layers
			foreach (var layer in layers) Composite(layer);
			// Readback
			Readback(callback);
		}
		#endregion


		#region --Operations--

		private void Composite (Layer layer) {
			commandQueue.Enqueue(() => {
				// Set the material properties
				material.SetVector("_Size", new Vector2(composite.width, composite.height));
				material.SetVector ("_Scale", Vector2.Scale(new Vector2(layer.texture.width, layer.texture.height), layer.scale));
				material.SetVector("_Offset", new Vector2((float)layer.offset.x / composite.width, (float)layer.offset.y / composite.height));
				material.SetFloat ("_Rotation", layer.rotation * Mathf.Deg2Rad);
				// Blit
				Graphics.Blit(layer.texture, composite, material);
				// Invoke composition callback
				// This should be used for resource and memory management because compositing is asynchronous,
				// hence you can free the layer texture once it has been used
				if (layer.callback != null) layer.callback(layer.texture);
			});
		}

		private void Readback (CompositeCallback callback) { // Does not null-check the `callback` parameter
			commandQueue.Enqueue(() => {
				// Create a result texture
				var result = new Texture2D(composite.width, composite.height);
				RenderTexture current = RenderTexture.active;
				RenderTexture.active = composite;
				// Read back
				result.ReadPixels(new Rect(0, 0, composite.width, composite.height), 0, 0);
				result.Apply();
				RenderTexture.active = current;
				// Invoke callback
				callback(result);
			});
		}
		#endregion


		#region --IDisposable--

		public void Dispose () {
			// Enqueue to guarantee that any previous calls to Composite and Readback are completed
			commandQueue.Enqueue(() => {
				// Free the composite texture
				RenderTexture.ReleaseTemporary(composite); 
				// Free the material
				Material.Destroy(material);
				// Dispose the command queue
				commandQueue.Dispose();
				// Clear the layers
				layers.Clear();
			});
		}
		#endregion


		#region --Utility--
		
		/// <summary>
		/// This is a helper class for invoking Graphics jobs at the right time.
		/// On some platforms (like Metal), calls to the Graphics API will fail unless performed
		/// in the onPreRender/onPostRender event.
		/// </summary>
		private sealed class GraphicsQueue : IDisposable {

			private List<Action> jobs;

			public GraphicsQueue () {
				// Create the queue
				jobs = new List<Action>();
				// Register for the post render event
				Camera.onPostRender += Update;
			}

			public void Enqueue (Action job) {
				jobs.Add(job);
			}

			public void Dispose () {
				// Unregister from the post render event
				Camera.onPostRender -= Update;
			}

			private void Update (Camera unused) {
				// Invoke all jobs
				jobs.ForEach(job => job());
				// Clear the queue
				jobs.Clear();
			}
		}
		#endregion
	}
}