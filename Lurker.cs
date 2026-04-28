using DG.Tweening;
using S13Audio;
using UnityEngine;

public class Lurker : Hittable
{
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private AnimationClip m_LurkerDeathClip;

	[SerializeField]
	private AnimationClip m_AudreyDeathClip;

	[SerializeField]
	private Transform m_DeathLocation;

	[SerializeField]
	private S13AudioHandler m_DeathAudio;

	[SerializeField]
	private Material m_DeathMaterial;

	private int m_HitCount;

	private int m_HitCountMax = 3;

	protected override bool InternalHit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Ink", 5f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		if (GameManager.Instance.Player != null && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			GentPipeChargeEffects componentInChildren = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				componentInChildren.Hit();
				GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Electric", 5f).GetComponent<Impact>().Initialize(hit, isPooled: true);
			}
		}
		m_HitCount++;
		if (m_HitCount >= m_HitCountMax)
		{
			Death();
		}
		return true;
	}

	private void Death()
	{
		AnimationCycler component = base.gameObject.GetComponent<AnimationCycler>();
		if (component != null)
		{
			component.Disable();
		}
		m_DeathAudio.Play();
		GameManager.Instance.Player.SetDisable(disable: true);
		GameManager.Instance.UIManager.Clear();
		m_LurkerDeathClip.name = "Death";
		m_CharacterContent.UpdateClipOverrides(m_LurkerDeathClip);
		m_CharacterContent.Animator.SetTrigger("Death");
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		m_AudreyDeathClip.name = "Interact";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(m_AudreyDeathClip);
		GameManager.Instance.Player.EnterInteraction("Interact");
		GameManager.Instance.Player.transform.DOMove(m_DeathLocation.position, 0.4f).SetEase(Ease.Linear);
		GameManager.Instance.Player.transform.DORotate(m_DeathLocation.eulerAngles, 0.4f).SetEase(Ease.Linear);
		SkinnedMeshRenderer[] componentsInChildren = m_CharacterContent.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = m_DeathMaterial;
		}
	}

	public void GameOver()
	{
		GameManager.Instance.ShowGameOver(GameOverType.Lurker);
	}
}
