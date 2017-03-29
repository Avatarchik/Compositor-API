using System;
using UnityEngine;
using Layers = System.Collections.Generic.List<ImageCompositor.Layer>;

public class ImageCompositor : IDisposable {

	#region --Op vars--
	private Layers layers;
	private Material material;
	#endregion


	#region --Constructor--

	public ImageCompositor () {
		// Create the layers collection
		layers = new Layers();
		// Create the material
		material = new Material (Shader.Find ("Hidden/Compositor2D"));
	}
	#endregion


	#region --Client API--

	public void AddLayer (Texture2D texture, Vector2 offset = default(Vector2), float rotation = 0, Vector2 scale = default(Vector2)) {
		// Add the layer
		AddLayer(new Layer(texture, offset, rotation, scale));
	}

	public void AddLayer (Layer layer) {
		// Add the layer
		layers.Add(layer);
	}

	public void Composite (Action<Texture2D> callback) {
		if (callback == null) {
			Debug.LogError("Image Compositor: Callback must not be null");
			return;
		}
		if (layers.Count == 0) {
			Debug.LogError("Image Compositor: No layers provided");
			return;
		}
		// Create a temporary render texture
		var composite = RenderTexture.GetTemporary(layers[0].texture.width, layers[0].texture.height, 0);
		composite.DiscardContents ();
		foreach (var layer in layers) {
			// Set the material properties
			material.SetVector("_Offset", layer.offset);
			material.SetFloat ("_Rotation", layer.rotation);
			material.SetVector ("_Scale", layer.scale);
			// Blit
			Graphics.Blit(layer.texture, composite, material);
		}
		// Create a result texture
		var result = new Texture2D(layers[0].texture.width, layers[0].texture.height);
		RenderTexture current = RenderTexture.active;
		RenderTexture.active = composite;
		// Read back
		result.ReadPixels(new Rect(0, 0, layers[0].texture.width, layers[0].texture.height), 0, 0);
		RenderTexture.active = current;
		RenderTexture.ReleaseTemporary(composite); 
		// Upload the texture data back to the GPU
		result.Apply();
		callback(result);
		((IDisposable)this).Dispose();
	}
	#endregion


	#region --IDisposable--

	void IDisposable.Dispose () {
		// Free the material
		Material.Destroy(material);
	}
	#endregion


	#region --Value types--

	[Serializable]
	public struct Layer {
		public Texture2D texture;
		public Vector2 offset, scale;
		public float rotation;
		public Layer (Texture2D texture, Vector2 offset, float rotation, Vector2 scale) {
			this.texture = texture;
			this.offset = offset;
			this.rotation = rotation;
			this.scale = scale;
		}
	}
	#endregion
}
