using System;
using System.Collections;

public class JCoroutine : JMonoBehaviour
{
	private Action m_Callback;

	public event EventHandler OnComplete;

	public void StartCoroutine(IEnumerator coroutine, Action callback)
	{
		m_Callback = callback;
		StartCoroutine(Coroutine(coroutine));
	}

	private IEnumerator Coroutine(IEnumerator coroutine)
	{
		yield return coroutine;
		m_Callback?.Invoke();
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		m_Callback = null;
		base.OnDisposed();
	}
}
