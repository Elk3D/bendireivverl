using DG.Tweening;
using UnityEngine;

public class DummyDeath : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	private Sequence m_Sequence;

	public override void OnEnable()
	{
		base.transform.eulerAngles = new Vector3(0f, Random.Range(0, 360), 0f);
		m_Content.localScale = Vector3.one;
		ResetSequence();
		m_Sequence.Insert(5f, m_Content.DOScale(0f, 2f).SetEase(Ease.InSine));
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
