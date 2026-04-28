using DG.Tweening;
using UnityEngine;

public class LootedEnemy : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	private Sequence m_Sequence;

	public void Initialize(Vector3 toPosition)
	{
		m_Content.localScale = Vector3.one * Random.Range(0.9f, 1.03f);
		base.transform.eulerAngles = new Vector3(0f, Random.Range(0, 360), 0f);
		Vector3 end = toPosition + Vector3.down * 10f;
		if (Physics.Linecast(toPosition, end, out var hitInfo, ~(1 << LayerMask.NameToLayer("IgnorePlayer")), QueryTriggerInteraction.Ignore))
		{
			toPosition = hitInfo.point;
		}
		base.transform.position = toPosition;
		ResetSequence();
		m_Sequence.Insert(6f, m_Content.DOScale(0f, 3f).SetEase(Ease.InSine));
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
