/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    public class PixelCompositor : ICompositor {

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