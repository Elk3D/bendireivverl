using DG.Tweening;
using UnityEngine;

public class TimelineCameraEffects : JMonoBehaviour
{
	[SerializeField]
	private float m_EndDuration = 3f;

	[SerializeField]
	private int m_Damage;

	[SerializeField]
	private float m_Delay;

	public void DamageEffect()
	{
		CameraEffects.Damage(m_EndDuration);
		if (m_Damage > 0)
		{
			GameManager.Instance.Player.Damage(m_Damage);
			GameManager.Instance.Player.ShowHealthBar();
		}
	}

	public void TakedownEffect()
	{
		CameraEffects.Takedown(m_EndDuration);
	}

	public void GainPowerEffect()
	{
		CameraEffects.GainPower(m_EndDuration);
	}

	public void VisionOn()
	{
		CameraEffects.VisionOn();
	}

	public void VisionOff()
	{
		CameraEffects.VisionOff();
	}

	public void VisionTransitionOn()
	{
		CameraEffects.VisionTransition(active: true);
	}

	public void VisionTransitionOff()
	{
		CameraEffects.VisionTransition(active: false);
	}

	public void InkDemonEffectOn()
	{
		CameraEffects.InkDemonEffectOn();
	}

	public void InkDemonEffectOff()
	{
		CameraEffects.InkDemonEffectOff();
	}

	public void CameraShake()
	{
		DOTween.Sequence().InsertCallback(m_Delay, delegate
		{
			CameraEffects.ShakeRotation(m_EndDuration, m_Damage);
		});
	}
}
