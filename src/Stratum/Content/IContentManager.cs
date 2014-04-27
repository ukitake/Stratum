using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit.Content;

namespace Stratum.Content
{
    public interface IContentManager
    {
        SharpDX.Toolkit.Content.ContentManager SharpContent { get; }

        T Load<T>(string filePath);
        //T Load<T>(IMapEntity mapEntity, string filePath) where T : class, IDisposable;
    }
}
