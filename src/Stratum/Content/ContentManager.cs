using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stratum.Content;

namespace Stratum.Content
{
    public class ContentManager : IContentManager
    {
        public ContentManager(IServiceProvider provider)
        {
            sharpContent = new SharpDX.Toolkit.Content.ContentManager(provider);
            sharpContent.Resolvers.Add(new FileSystemContentResolver());
        }

        private SharpDX.Toolkit.Content.ContentManager sharpContent;
        public SharpDX.Toolkit.Content.ContentManager SharpContent
        {
            get { return sharpContent; }
        }

        public T Load<T>(string filePath)
        {
            return sharpContent.Load<T>(filePath);
        }

        //public T Load<T>(IMapEntity mapEntity, string filePath) where T : class, IDisposable
        //{
        //    return null;
        //}
    }
}
