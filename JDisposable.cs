using System;

public abstract class JDisposable : IDisposable
{
	public bool IsDisposed { get; private set; }

	protected virtual void OnDisposed()
	{
	}

	public void Dispose()
	{
		if (!IsDisposed)
		{
			OnDisposed();
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	~JDisposable()
	{
		Dispose();
	}
}
