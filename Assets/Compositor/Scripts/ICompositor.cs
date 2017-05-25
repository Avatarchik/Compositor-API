/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    using System;

    public interface ICompositor : IDisposable {

        #region --Properties--
        int Width {get;}
        int Height {get;}
        #endregion

        #region --Client API--
        void AddLayer (Layer layer);
        void Composite (CompositeCallback callback);
        #endregion
    }
}