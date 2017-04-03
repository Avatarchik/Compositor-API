/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CompositeTest : MonoBehaviour {

	public RawImage rawImage;
	public GPUCompositor.Layer[] layers;

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForEndOfFrame ();
		using (var compositor = new GPUCompositor()) {
			foreach (var layer in layers) compositor.AddLayer (layer);
			compositor.Composite (result => rawImage.texture = result);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
