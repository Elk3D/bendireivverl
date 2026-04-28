using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShipAhoyPhaseTwo : BossPhase
{
	[Header("ShipAhoy")]
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private AnimationClip m_IdleAnimationClip;

	[SerializeField]
	private AnimationClip m_SwingLoopAnimationClip;

	[Header("Gear")]
	[SerializeField]
	private Transform m_ThrowGearLocation;

	[SerializeField]
	private AnimationClip m_ThrowGearsAnimationClip;

	[Header("Anchor")]
	[SerializeField]
	private ThrowObject m_Anchor;

	[SerializeField]
	private AnimationClip m_PickUpAnchorClip;

	[SerializeField]
	private AnimationClip m_PullAnimationClip;

	[Header("Controllers")]
	[SerializeField]
	private GentRechargerInstant m_GentRechargerInstant;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroups;

	public bool IsActive { get; private set; }

	public event EventHandler OnAnimationEvent;

	public event EventHandler OnAnimationEvent2;

	public event EventHandler OnAnimationComplete;

	protected override void InternalInitialize()
	{
		m_GentRechargerInstant.SetActive(active: true);
		SetIdle();
		for (int i = 0; i < m_AnimationClipOverrideGroups.Length; i++)
		{
			m_AnimationClipOverrideGroups[i].Initialize();
		}
		m_CharacterContent.GenericAnimationEvents.SetReciever(this);
		SetActive(active: true);
	}

	protected override void InternalComplete()
	{
		SetIdle();
		SetActive(active: false);
	}

	private void SetIdle()
	{
		m_IdleAnimationClip.name = "Idle";
		m_CharacterContent.UpdateClipOverrides(m_IdleAnimationClip);
	}

	public void SetActive(bool active)
	{
		IsActive = active;
		if (IsActive)
		{
			ThrowObject();
		}
	}

	private void ThrowObject()
	{
		RemoveListeners();
		AddListeners();
		m_ThrowGearsAnimationClip.name = "Interact";
		m_CharacterContent.UpdateClipOverrides(m_ThrowGearsAnimationClip);
		m_CharacterContent.SetAnimationTrigger("Interact");
		m_CharacterContent.transform.eulerAngles = Vector3.zero;
	}

	private void HandleThrowObjectOnAnimationEvent(object sender, EventArgs e)
	{
		Vector3 vector = GameManager.Instance.GameCamera.transform.position + Vector3.down;
		Vector3 direction = vector - m_ThrowGearLocation.position;
		if (Physics.Raycast(m_ThrowGearLocation.position, direction, out var hitInfo, float.PositiveInfinity, LayerMaskUtility.GetInvisibleColliders, QueryTriggerInteraction.Ignore))
		{
			vector = hitInfo.point;
		}
		ThrowObject component = GameManager.Instance.PoolingManager.GetFromPool("Projectiles/Throwable_Gear", 4f).GetComponent<ThrowObject>();
		component.Initialize();
		component.Throw(m_ThrowGearLocation.position, vector);
	}

	private void HandleThrowObjectOnAnimationEvent2(object sender, EventArgs e)
	{
		Vector3 forward = new Vector3(GameManager.Instance.Player.transform.position.x, 0f, GameManager.Instance.Player.transform.position.z) - new Vector3(m_CharacterContent.transform.position.x, 0f, m_CharacterContent.transform.position.z);
		m_CharacterContent.transform.rotation = Quaternion.LookRotation(forward);
	}

	private void HandleThrowObjectOnAnimationComplete(object sender, EventArgs e)
	{
		RemoveListeners();
		if (IsActive)
		{
			ThrowAnchor();
		}
	}

	public void AnimationEvent()
	{
		this.OnAnimationEvent.Send(this);
	}

	public void AnimationEvent2()
	{
		this.OnAnimationEvent2.Send(this);
	}

	public void AnimationComplete()
	{
		this.OnAnimationComplete.Send(this);
	}

	private void ThrowAnchor()
	{
		m_SwingLoopAnimationClip.name = "Interact";
		m_CharacterContent.UpdateClipOverride(m_SwingLoopAnimationClip);
		m_CharacterContent.SetAnimationTrigger("Interact");
	}

	public void Land()
	{
		m_Anchor.Land();
	}

	public void Drag()
	{
		m_Anchor.Drag();
	}

	public void Throw()
	{
		m_Anchor.Throw();
	}

	public void DragLong()
	{
		m_Anchor.DragLong();
	}

	public void PickUp()
	{
		m_Anchor.DisableParticles();
	}

	public void ThrowComplete()
	{
		ThrowObject();
	}

	public void Roar()
	{
		CameraEffects.ShakeRotation(1f, 1f);
	}

	public void RoarComplete()
	{
		m_CharacterContent.transform.DOKill();
		Vector3 position = GameManager.Instance.Player.transform.position;
		Vector3 forward = new Vector3(position.x, 0f, position.z) - new Vector3(m_CharacterContent.transform.position.x, 0f, m_CharacterContent.transform.position.z);
		bool flag = true;
		float num = Vector3.Distance(position, m_CharacterContent.transform.position);
		if (num > 40f)
		{
			UpdateAnimationClips("Far");
		}
		else if (num > 30f)
		{
			UpdateAnimationClips("Medium");
		}
		else if (num > 14f)
		{
			UpdateAnimationClips("Close");
		}
		else
		{
			flag = false;
		}
		if (flag)
		{
			m_CharacterContent.transform.DORotate(Quaternion.LookRotation(forward).eulerAngles, 0.25f).SetEase(Ease.InSine).SetDelay(1f);
			m_CharacterContent.SetAnimationTrigger("Interact");
			m_Anchor.Initialize();
		}
		else
		{
			m_CharacterContent.transform.DORotate(Quaternion.LookRotation(forward).eulerAngles, 0.25f).SetEase(Ease.InSine);
			UpdateAnimationClips("TooClose");
			m_CharacterContent.SetAnimationTrigger("Interact");
		}
	}

	public void AttackClose()
	{
		Vector3 vector = m_CharacterContent.transform.position + Vector3.up * 3.5f;
		float num = 15f;
		int damage = 1;
		if (Physics.OverlapSphereNonAlloc(vector + m_CharacterContent.transform.forward * (num / 2f), results: new Collider[1], radius: num / 2f, layerMask: LayerMaskUtility.Player(), queryTriggerInteraction: QueryTriggerInteraction.Ignore) > 0 && GameManager.Instance.Player != null)
		{
			RaycastHit hit = new RaycastHit
			{
				point = m_CharacterContent.transform.position
			};
			GameManager.Instance.Player.Hit(hit, null, damage);
			Vector3 force = m_CharacterContent.transform.forward * 15f;
			force.y += 2f;
			GameManager.Instance.Player.AddForce(force);
		}
	}

	public void AttackCloseComplete()
	{
		ThrowAnchor();
		Recenter();
	}

	public void Recenter()
	{
		m_CharacterContent.transform.DOKill();
		m_CharacterContent.transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.InSine);
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list = new List<AnimationClip>();
		for (int i = 0; i < m_AnimationClipOverrideGroups.Length; i++)
		{
			AnimationClipOverrideGroup animationClipOverrideGroup = m_AnimationClipOverrideGroups[i];
			if (animationClipOverrideGroup.Name == name)
			{
				for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
				{
					list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
				}
				break;
			}
		}
		m_CharacterContent.UpdateClipOverrides(list.ToArray());
	}

	private void AddListeners()
	{
		OnAnimationEvent += HandleThrowObjectOnAnimationEvent;
		OnAnimationEvent2 += HandleThrowObjectOnAnimationEvent2;
		OnAnimationComplete += HandleThrowObjectOnAnimationComplete;
	}

	private void RemoveListeners()
	{
		OnAnimationEvent -= HandleThrowObjectOnAnimationEvent;
		OnAnimationEvent2 -= HandleThrowObjectOnAnimationEvent2;
		OnAnimationComplete -= HandleThrowObjectOnAnimationComplete;
	}

	protected override void OnDisposed()
	{
		this.OnAnimationEvent = null;
		this.OnAnimationEvent2 = null;
		this.OnAnimationComplete = null;
		RemoveListeners();
		if (m_CharacterContent != null)
		{
			m_CharacterContent.transform.DOKill();
		}
		base.OnDisposed();
	}
}
