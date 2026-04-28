using System;
using UnityEngine;

public class LostOneTiedUpController : JMonoBehaviour
{
	[SerializeField]
	private LostOneTiedUp[] m_LostOnes;

	public override void Start()
	{
		RemoveListeners();
		AddListeners();
	}

	private void HandleLostOneOnInteract(object sender, EventArgs e)
	{
		LostOneTiedUp lostOneTiedUp = sender as LostOneTiedUp;
		for (int i = 0; i < m_LostOnes.Length; i++)
		{
			LostOneTiedUp lostOneTiedUp2 = m_LostOnes[i];
			if (lostOneTiedUp2 != lostOneTiedUp)
			{
				lostOneTiedUp2.Disable();
			}
		}
	}

	private void HandleLostOneOnComplete(object sender, EventArgs e)
	{
		LostOneTiedUp lostOneTiedUp = sender as LostOneTiedUp;
		for (int i = 0; i < m_LostOnes.Length; i++)
		{
			LostOneTiedUp lostOneTiedUp2 = m_LostOnes[i];
			if (lostOneTiedUp2 != lostOneTiedUp)
			{
				lostOneTiedUp2.Enable();
			}
		}
	}

	private void AddListeners()
	{
		for (int i = 0; i < m_LostOnes.Length; i++)
		{
			LostOneTiedUp obj = m_LostOnes[i];
			obj.OnInteract += HandleLostOneOnInteract;
			obj.OnComplete += HandleLostOneOnComplete;
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < m_LostOnes.Length; i++)
		{
			LostOneTiedUp obj = m_LostOnes[i];
			obj.OnInteract -= HandleLostOneOnInteract;
			obj.OnComplete -= HandleLostOneOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
