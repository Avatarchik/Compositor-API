/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

using UnityEngine;
using System;
using System.Collections.Generic;
using Layers = System.Collections.Generic.List<GPUCompositor.Layer>;

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
			Debug.LogError("Image Compositor: Callback must not be null");
			return;
		}
		// Count checking
		if (layers.Count == 0) {
			Debug.LogError("Image Compositor: No layers provided");
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
			material.SetVector("_Offset", layer.offset);
			material.SetFloat ("_Rotation", layer.rotation);
			material.SetVector ("_Scale", layer.scale);
			// Blit
			Graphics.Blit(layer.texture, composite, material);
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
		commandQueue.Enqueue(() => {
			// Free the composite texture
			RenderTexture.ReleaseTemporary(composite); 
			// Free the material
			Material.Destroy(material);
			// Dispose the command queue
			commandQueue.Dispose();
			// Clear the layers
			layers.Clear();
			layers = null;
		});
	}
	#endregion


	#region --Value types--

	[Serializable]
	public struct Layer {
		public Texture2D texture;
		public Vector2 offset, scale;
		public float rotation;

		/// <summary>
		/// Create a new composition layer
		/// </summary>
		/// <param name="texture">Layer texture</param>
		/// <param name="offset">Offset of the layer's bottom left corner before any rotation is applied</param>
		/// <param name="rotation">Layer's rotation in degrees</param>
		/// <param name="scale">Layer's scale. To use natural scale, use (1, 1)<param>
		public Layer (Texture2D texture, Vector2 offset = default(Vector2), float rotation = 0f, Vector2 scale = default(Vector2)) {
			this.texture = texture;
			this.offset = offset;
			this.rotation = rotation;
			this.scale = scale;
		}
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
			// Register for the post render event
			Camera.onPostRender += Update;
		}

		public void Enqueue (Action job) {
			jobs.Add(job);
		}

		public void Dispose () {
			// Unregister from the post render event
			Camera.onPostRender -= Update;
			// Clear the queue
			jobs.Clear(); jobs = null;
		}

		private void Update (Camera unused) {
			// Invoke all jobs
			jobs.ForEach(job => job());
			// Clear the queue // Null check here in case Dispose becomes enqueued
			if (jobs != null) jobs.Clear();
		}
	}
	#endregion
}

#region --Callbacks--

public delegate void CompositeCallback (Texture2D composite);
#endregion