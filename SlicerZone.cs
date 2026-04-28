using System;
using DG.Tweening;
using UnityEngine;

public class SlicerZone : ButcherGangSpawner
{
	[SerializeField]
	private Transform m_Test;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private EventTrigger m_EventTriggerHit;

	[SerializeField]
	private AudioSource m_test_audiosource;

	[SerializeField]
	private ParticleSystem m_TrailParticles;

	[SerializeField]
	private AudioClip[] m_Test_AudioClips;

	[SerializeField]
	protected ControllerRumble.RUMBLE_PRESETS m_presetRumble = ControllerRumble.RUMBLE_PRESETS.NONE;

	[SerializeField]
	protected Vector2 m_CustomRumble;

	private int m_Test_AudioClipIndex;

	public ControllerRumble.RUMBLE_PRESETS PresetRumble => m_presetRumble;

	public Vector2 CustomRumble => m_CustomRumble;

	public bool IsTriggered { get; private set; }

	protected override void Enable()
	{
		m_TrailParticles.Stop();
		m_Test.gameObject.SetActive(value: false);
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		m_EventTrigger.ResetAction();
		m_EventTrigger.SetActive(active: true);
	}

	protected override void Disable()
	{
		if (!IsTriggered)
		{
			m_TrailParticles.Stop();
			m_Test.gameObject.SetActive(value: false);
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
			m_EventTrigger.SetActive(active: false);
		}
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		IsTriggered = true;
		if (base.IsActive)
		{
			Sequence s = DOTween.Sequence();
			s.InsertCallback(0f, SetLocation);
			s.InsertCallback(0.75f, Attack);
			s.InsertCallback(1.75f, AttackOnComplete);
		}
		else
		{
			AttackOnComplete();
		}
	}

	private void SetLocation()
	{
		Vector3 forward = GameManager.Instance.Player.transform.forward;
		m_Test.position = base.transform.position + forward * 7.5f;
		m_Test.LookAt(base.transform.position, Vector3.up);
		m_Test.gameObject.SetActive(value: true);
		m_test_audiosource.PlayOneShot(m_Test_AudioClips[m_Test_AudioClipIndex]);
		m_Test_AudioClipIndex++;
		if (m_Test_AudioClipIndex >= m_Test_AudioClips.Length)
		{
			m_Test_AudioClipIndex = 0;
		}
	}

	private void Attack()
	{
		m_EventTriggerHit.OnEnter -= HandleEventTriggerHitOnEnter;
		m_EventTriggerHit.OnEnter += HandleEventTriggerHitOnEnter;
		m_EventTriggerHit.ResetAction();
		m_EventTriggerHit.SetActive(active: true);
		Vector3 forward = m_Test.forward;
		m_Animator.SetTrigger("Interact");
		m_TrailParticles.Play();
		Vector3 endValue = m_Test.position + forward * 15f;
		m_Test.DOMove(endValue, 1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed)
			.OnComplete(delegate
			{
				m_EventTriggerHit.OnEnter -= HandleEventTriggerHitOnEnter;
				m_EventTriggerHit.SetActive(active: false);
				m_TrailParticles.Stop();
				m_Test.gameObject.SetActive(value: false);
			});
		if (m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
		{
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[(int)m_presetRumble]);
		}
		else
		{
			GameManager.Instance.TriggerRumble(m_CustomRumble);
		}
	}

	private void HandleEventTriggerHitOnEnter(object sender, EventArgs e)
	{
		m_EventTriggerHit.OnEnter -= HandleEventTriggerHitOnEnter;
		m_EventTriggerHit.SetActive(active: false);
		RaycastHit hit = new RaycastHit
		{
			point = m_Test.position
		};
		Vector3 force = (GameManager.Instance.Player.transform.position - hit.point).normalized * 30f;
		GameManager.Instance.Player.AddForce(force);
		int damage = 1;
		if (GameManager.Instance.Player.Health <= 1f)
		{
			damage = 0;
		}
		GameManager.Instance.Player.Hit(hit, null, damage);
	}

	private void AttackOnComplete()
	{
		m_Test.gameObject.SetActive(value: false);
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		m_EventTrigger.ResetAction();
		m_EventTrigger.SetActive(active: true);
		IsTriggered = false;
		SendOnReturned();
	}
}
