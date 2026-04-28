using System;
using DG.Tweening;
using UnityEngine;

public class InkDemon : JMonoBehaviour
{
	[Serializable]
	public class DeathAnimations
	{
		public AnimationClip InkDemon;

		public AnimationClip Audrey;
	}

	[Header("Status")]
	public bool IsActive;

	public bool CanUse;

	[Header("Ink Demon")]
	[SerializeField]
	private CharacterContent m_InkDemon;

	[SerializeField]
	private Animator m_Animator;

	[Header("Death Animations")]
	[SerializeField]
	private DeathAnimations[] m_DeathAnimations;

	private Transform m_Parent;

	private LayerMask m_TeleportLayers;

	private RaycastHit m_TargetHit;

	private RaycastHit m_TargetColliderHit;

	private RaycastHit m_TargetColliderToHit;

	private Vector3 m_TargetPosition;

	private Vector3 m_ToTargetPosition;

	private Vector3 m_TargetColliderPosition;

	private Vector3 m_ToTargetColliderPosition;

	private float m_Height = 8f;

	private float m_Radius = 1.25f;

	private float m_Distance = 6f;

	private bool m_IsBehind;

	public event EventHandler OnStart;

	public event EventHandler OnEnd;

	public event EventHandler OnHide;

	public event EventHandler OnBehind;

	public override void Start()
	{
		m_Parent = base.transform.parent;
		m_InkDemon.gameObject.SetActive(value: false);
		m_TeleportLayers = ~(1 << (LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("AI")));
	}

	public void Enable()
	{
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.Active);
		this.OnStart.Send(this);
	}

	public void SetActive(bool active)
	{
		IsActive = active;
	}

	private void FixedUpdate()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused || GameManager.Instance.Player == null)
		{
			return;
		}
		bool flag = false;
		Vector3 forward = GameManager.Instance.Player.transform.forward;
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, UnityEngine.Random.Range(-60, 60), 0f);
		Vector3 vector = GameManager.Instance.Player.transform.position + Vector3.up * 0.5f;
		Vector3 vector2 = Vector3.up * m_Height / 2f;
		Vector3 vector3 = vector + vector2 + Vector3.up * (0f - (m_Height * 0.5f - m_Radius));
		Vector3 p = vector3 + Vector3.up * (m_Height - m_Radius * 2f);
		m_TargetPosition = vector + identity * -forward * m_Distance;
		Color color = Color.green;
		if (!GameManager.Instance.Player.IsGrounded)
		{
			color = Color.red;
		}
		else if (CheckDistance(vector3, p, m_Radius, identity * -forward, m_Distance))
		{
			color = Color.red;
		}
		else
		{
			flag = true;
		}
		DebugUtility.DebugCapsule(m_TargetPosition, m_TargetPosition + Vector3.up * m_Height, color, m_Radius);
		if (!IsActive || GameManager.Instance.Player == null)
		{
			return;
		}
		if (flag)
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
			{
				if (GameManager.Instance.Player.CurrentState == State.Player.Default)
				{
					Death();
				}
			}
			else
			{
				Hidden();
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			Hidden();
		}
		else if (!m_IsBehind)
		{
			m_IsBehind = true;
			this.OnBehind.Send(this);
		}
	}

	private void Hidden()
	{
		Hide();
		IsActive = false;
		m_IsBehind = false;
		if (GameManager.Instance.InkDemonManager == null)
		{
			GameManager.Instance.InkDemonManager = InkDemonManager.Create();
		}
		GameManager.Instance.InkDemonManager.ResetTimer();
		GameManager.Instance.InkDemonManager.CheckAvailability();
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
	}

	private bool CheckDistance(Vector3 p1, Vector3 p2, float radius, Vector3 forward, float maxDistance)
	{
		bool result = false;
		RaycastHit hitInfo;
		if (Physics.CapsuleCast(p1, p2, radius, forward, out m_TargetColliderToHit, maxDistance, m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			m_TargetPosition = new Vector3(m_TargetColliderToHit.point.x, m_TargetPosition.y, m_TargetColliderToHit.point.z);
			result = true;
		}
		else if (!Physics.SphereCast(p2 + forward * maxDistance, 1f, Vector3.down, out hitInfo, m_Height - radius, m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			result = true;
		}
		return result;
	}

	private void Hide()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpdateILife();
		this.OnHide.Send(this);
	}

	private void Death()
	{
		IsActive = false;
		this.OnEnd.Send(this);
		m_IsBehind = false;
		GameManager.Instance.Player.SetDisable(disable: true);
		GameManager.Instance.UIManager.Clear();
		CameraEffects.ShakeRotation(2f, 1f);
		base.transform.SetParent(null);
		m_TargetPosition.y = GameManager.Instance.Player.transform.position.y;
		base.transform.position = m_TargetPosition;
		Vector3 normalized = (GameManager.Instance.Player.transform.position - base.transform.position).normalized;
		base.transform.eulerAngles = Quaternion.LookRotation(normalized).eulerAngles;
		m_InkDemon.gameObject.SetActive(value: true);
		DeathAnimations deathAnimations = m_DeathAnimations[UnityEngine.Random.Range(0, m_DeathAnimations.Length)];
		if (deathAnimations != null)
		{
			deathAnimations.InkDemon.name = "Death";
			m_InkDemon.UpdateClipOverrides(deathAnimations.InkDemon);
			m_Animator.SetTrigger("Death");
			GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
			deathAnimations.Audrey.name = "Interact";
			GameManager.Instance.Player.UpdatePlayerContentClipOverrides(deathAnimations.Audrey);
			GameManager.Instance.Player.EnterInteraction("Interact");
			GameManager.Instance.Player.transform.DORotate(Quaternion.LookRotation(-normalized).eulerAngles, 0.25f).SetEase(Ease.Linear);
		}
	}

	public void Respawn()
	{
		GameManager.Instance.ShowGameOver(GameOverType.InkDemon);
		m_InkDemon.gameObject.SetActive(value: false);
		if (m_Parent != null)
		{
			base.transform.SetParent(m_Parent);
		}
	}

	protected override void OnDisposed()
	{
		this.OnStart = null;
		this.OnEnd = null;
		this.OnHide = null;
		this.OnBehind = null;
		m_Parent = null;
		base.OnDisposed();
	}
}
