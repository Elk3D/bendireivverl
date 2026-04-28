using DG.Tweening;
using UnityEngine;

public class DOTweenActions : JMonoBehaviour
{
	[SerializeField]
	private DOTweenAnimation[] m_DOTweens;

	[SerializeField]
	private Transform[] m_Transforms;

	private Sequence m_Sequence;

	public void DOPlay()
	{
		if (m_Transforms != null && m_Transforms.Length != 0)
		{
			m_Sequence?.Kill();
			m_Sequence = DOTween.Sequence();
			for (int i = 0; i < m_Transforms.Length; i++)
			{
				Transform target = m_Transforms[i];
				m_Sequence.Insert(0f, target.DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.InOutSine));
			}
		}
		else if (m_DOTweens != null && m_DOTweens.Length != 0)
		{
			for (int j = 0; j < m_DOTweens.Length; j++)
			{
				m_DOTweens[j].DOPlay();
			}
		}
	}

	protected override void OnDisposed()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
		if (m_DOTweens != null)
		{
			for (int i = 0; i < m_DOTweens.Length; i++)
			{
				m_DOTweens[i].DOKill();
			}
		}
		base.OnDisposed();
	}
}
