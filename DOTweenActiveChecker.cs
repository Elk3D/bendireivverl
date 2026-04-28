using DG.Tweening;
using UnityEngine;

public class DOTweenActiveChecker : JMonoBehaviour
{
	[SerializeField]
	private DOTweenAnimation m_DOTweenAnimation;

	public override void OnEnable()
	{
		if (!(m_DOTweenAnimation == null) && m_DOTweenAnimation.autoPlay)
		{
			m_DOTweenAnimation.DOPlay();
		}
	}

	public override void OnDisable()
	{
		if (!(m_DOTweenAnimation == null) && m_DOTweenAnimation.autoPlay)
		{
			m_DOTweenAnimation.DOPause();
		}
	}
}
