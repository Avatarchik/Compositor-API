/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    public sealed class PixelCompositor : ICompositor {

        #region --Properties--
        public int Width {get; private set;}
		public int Height {get; private set;}
        #endregion

        #region --Op vars--
        private readonly bool immediate;
        #endregion


        #region --Client API--
        
        public PixelCompositor (bool immediate = true) {
            this.immediate = immediate;
        }

        public void AddLayer (Layer layer) {

        }

        public void Composite (CompositeCallback callback) {

        }
        #endregion


        #region --Operations--

        private void Composite (Layer layer) {

        }
        #endregion


        #region --IDisposable--

        public void Dispose () {

        }
        #endregion


        #region --Utility--

        #endregion
    }
}