using System;

namespace Helpers
{
    public sealed class EmptyDisposable : IDisposable
    {
        public static readonly EmptyDisposable Singleton = new ();  
        private EmptyDisposable() { }
        public void Dispose() { }
    }
}
