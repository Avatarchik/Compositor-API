/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    using UnityEngine;
    using System;
    using Layers = System.Collections.Generic.List<Layer>;

    /// <summary>
    /// Compositor that uses the SIMD Accelerate framework on iOS
    /// </summary>
    public sealed class AccelerateCompositor : ICompositor {
        
        #region --Properties--
		public int width {get; private set;}
		public int height {get; private set;}
		#endregion


        #region --Client API--

        /// <summary>
		/// Create an Accelerate compositor
		/// </summary>
		/// <param name="width">Composite width</param>
		/// <param name="height">Composite height</param>
        public AccelerateCompositor (int width, int height) {
            
        }

        /// <summary>
		/// Add a layer to be composited
		/// </summary>
		/// <param name="layer">Layer to be composited</param>
        public void AddLayer (Layer layer) {
            
        }

        /// <summary>
		/// Composite layers
		/// </summary>
		/// <param name="callback">Callback to be invoked with the composite texture</param>
        public void Composite (CompositeCallback callback) {
            // Null checking
			if (callback == null) {
				Debug.LogError("Compositor: Callback must not be null");
				return;
			}
            // ...
        }

        public void Dispose () {
            
        }
        #endregion
    }
}