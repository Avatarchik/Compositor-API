/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    using System;

    public interface ICompositor : IDisposable {
        void AddLayer (Layer layer);
        void Composite (CompositeCallback callback);
    }
}