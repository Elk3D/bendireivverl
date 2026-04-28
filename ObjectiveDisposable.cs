using UnityEngine;

public class ObjectiveDisposable : Objective
{
	[SerializeField]
	private DisposableObject m_DisposableObject;

	protected override void InternalInitialize()
	{
		m_DisposableObject.Dispose();
		SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		m_DisposableObject.Dispose();
	}
}
