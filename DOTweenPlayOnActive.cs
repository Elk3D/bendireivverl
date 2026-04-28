using DG.Tweening;
using UnityEngine;

public class DOTweenPlayOnActive : JMonoBehaviour
{
	[SerializeField]
	private DOTweenAnimation m_Animation;

	public override void OnEnable()
	{
		m_Animation.DOPlay();
	}

	public override void OnDisable()
	{
		m_Animation.DORestart();
		m_Animation.DOPause();
	}
}
