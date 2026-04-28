using UnityEngine;
using UnityEngine.Events;

public class HittableEnemy : Hittable
{
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private UnityEvent m_UnityEvent;

	protected override bool InternalHit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink", 5f).transform.position = hit.point;
		m_UnityEvent?.Invoke();
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
			GameManager.Instance.ClearInteraction();
		}
		m_CharacterContent.SetAnimationTrigger("Death");
		return true;
	}
}
