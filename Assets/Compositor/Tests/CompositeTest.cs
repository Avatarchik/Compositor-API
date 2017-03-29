using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompositeTest : MonoBehaviour {

	public RawImage rawImage;
	public ImageCompositor.Layer[] layers;

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForEndOfFrame ();
		using (var compositor = new ImageCompositor(result => rawImage.texture = result)) {
			foreach (var layer in layers) compositor.AddLayer (layer);
			compositor.Composite ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
