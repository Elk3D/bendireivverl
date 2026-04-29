using System;

namespace CharacterController
{
    public abstract class FPSDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        protected virtual void OnDisposed() { }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                OnDisposed();
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~FPSDisposable()
        {
            Dispose();
        }
    }
}
