using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SubwayCarTravel : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Event Trigger")]
	[SerializeField]
	private EventTrigger m_SubwayCarEnter;

	[SerializeField]
	private EventTrigger m_SubwayCarDeath;

	[Header("Section Controllers")]
	[SerializeField]
	private Door m_Door;

	[SerializeField]
	private Door m_DoorCar;

	[Header("Subway")]
	[SerializeField]
	private EventTrigger m_SubwayEventTrigger;

	[SerializeField]
	private Transform m_SubwayCarParent;

	[SerializeField]
	private Transform m_SubwayCarPlayerParent;

	[SerializeField]
	private Transform m_Subway;

	[SerializeField]
	private Transform m_SubwayStart;

	[SerializeField]
	private Transform m_SubwayEnd;

	[SerializeField]
	private Transform m_SubwayLeave;

	[SerializeField]
	private Transform m_SubwayTravel;

	[Header("Tunnel")]
	[SerializeField]
	private Transform m_Tunnel;

	[SerializeField]
	private Transform m_TunnelStart;

	[SerializeField]
	private Transform m_TunnelEnd;

	[SerializeField]
	private GameObject m_TunnelCollider;

	[Header("City")]
	[SerializeField]
	private EventTrigger m_CityEnter;

	[SerializeField]
	private Transform m_CityStart;

	[SerializeField]
	private Transform m_CityEnd;

	[SerializeField]
	private Transform m_CityLeave;

	[Header("Section")]
	[SerializeField]
	private SectionID m_CurrentSectionID;

	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private SectionID[] m_UninitializedSections;

	[Header("DOTween Animations")]
	[SerializeField]
	private DOTweenAnimation[] m_Handles;

	private Section m_Section;

	private Sequence m_TunnelSequence;

	private Sequence m_Sequence;

	private bool m_IsActive;

	private bool m_HasArrived;

	private bool m_IsTraveled;

	private bool m_CanReturn;

	private int m_LoopIndex;

	private int m_LoopMax = 10;

	private float m_Timer;

	private const float m_TimerMax = 15f;

	private const float m_TimerReturnMax = 30f;

	public event EventHandler OnArrive;

	public event EventHandler OnLeave;

	public event EventHandler OnTravelStart;

	public event EventHandler OnTravelEnd;

	public event EventHandler OnArriveVO;

	public event EventHandler OnLeaveVO;

	public event EventHandler OnHorn;

	public override void Start()
	{
		m_Section = GetComponentInParent<Section>(includeInactive: true);
		m_Subway.gameObject.SetActive(value: false);
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
	}

	private void Update()
	{
		if (!(m_Section == null) && m_Section.IsInitialized && m_Section.IsActive && m_IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			float num = (m_CanReturn ? 30f : 15f);
			if (!m_HasArrived && m_Timer >= num)
			{
				Arrive();
			}
			else if (m_HasArrived && m_CanReturn && m_Timer >= 15f)
			{
				Return();
			}
			else
			{
				m_Timer += Time.deltaTime;
			}
		}
	}

	private void SendOnHorn()
	{
		this.OnHorn.Send(this);
	}

	private void SendOnArrive()
	{
		this.OnArrive.Send(this);
	}

	private void Return()
	{
		if (!(GameManager.Instance.Player != null) || !(Vector3.Distance(GameManager.Instance.Player.transform.position, m_Door.transform.position) < 10f))
		{
			m_IsActive = false;
			m_HasArrived = false;
			m_Timer = 0f;
			m_SubwayCarEnter.SetActive(active: false);
			m_SubwayCarEnter.OnEnter -= HandleSubwayCarEnterOnEnter;
			m_SubwayCarDeath.OnEnter -= HandleSubwayCarDeathOnEnter;
			m_SubwayCarDeath.OnEnter += HandleSubwayCarDeathOnEnter;
			m_SubwayCarDeath.SetActive(active: true);
			m_Door.Content.ForceDeactivate();
			ResetSequence();
			m_Sequence.Insert(1f, m_Subway.DOMove(m_SubwayLeave.position, 3f).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed));
			m_Sequence.OnComplete(ReturnComplete);
			this.OnLeave.Send(this);
		}
	}

	private void ReturnComplete()
	{
		m_SubwayCarDeath.OnEnter -= HandleSubwayCarDeathOnEnter;
		m_Subway.gameObject.SetActive(value: false);
		m_IsActive = true;
	}

	private void Arrive()
	{
		m_IsActive = false;
		m_Subway.gameObject.SetActive(value: true);
		m_SubwayCarEnter.SetActive(active: false);
		m_SubwayCarDeath.OnEnter -= HandleSubwayCarDeathOnEnter;
		m_SubwayCarDeath.OnEnter += HandleSubwayCarDeathOnEnter;
		m_SubwayCarDeath.SetActive(active: true);
		float num = 5f;
		ResetSequence();
		m_Sequence.InsertCallback(num, SendOnHorn);
		num += 3f;
		m_Sequence.InsertCallback(num, SendOnArrive);
		num += 2.5f;
		m_Sequence.InsertCallback(num, EnableSubwayCar);
		m_Sequence.Insert(num, m_Subway.DOMove(m_SubwayEnd.position, 4f).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed));
		num += 4f;
		m_Sequence.InsertCallback(num, RetreatInitializeOnComplete);
	}

	private void EnableSubwayCar()
	{
		m_Subway.position = m_SubwayStart.position;
		KillTunnelSequence();
	}

	private void HandleSubwayCarDeathOnEnter(object sender, EventArgs e)
	{
		m_SubwayCarDeath.OnEnter -= HandleSubwayCarDeathOnEnter;
		GameManager.Instance.Player.ForceDeath();
		this.OnHorn.Send(this);
	}

	private void RetreatInitializeOnComplete()
	{
		m_DoorCar.Data.SetStatus(DoorStatus.Closed);
		m_DoorCar.Content.ForceDeactivateComplete();
		m_DoorCar.Content.Enable();
		m_Door.Content.ForceActivate();
		m_SubwayCarDeath.SetActive(active: false);
		m_SubwayCarDeath.OnEnter -= HandleSubwayCarDeathOnEnter;
		m_SubwayCarEnter.OnEnter -= HandleSubwayCarEnterOnEnter;
		m_SubwayCarEnter.OnEnter += HandleSubwayCarEnterOnEnter;
		m_SubwayCarEnter.SetActive(active: true);
		m_Timer = 0f;
		m_IsActive = true;
		m_HasArrived = true;
		m_CanReturn = true;
	}

	private void HandleSubwayCarEnterOnEnter(object sender, EventArgs e)
	{
		m_SubwayCarEnter.OnEnter -= HandleSubwayCarEnterOnEnter;
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.Subway);
		m_IsActive = false;
		m_CanReturn = false;
		m_Timer = 0f;
		GameManager.Instance.Player.ForceAbilitiesCancel();
		GameManager.Instance.Player.ForceAbilitiesCooldown();
		GameManager.Instance.Player.SetState(State.Player.Default);
		GameManager.Instance.Player.LockAbilities();
		GameManager.Instance.DisableTeleport();
		if ((bool)m_SubwayCarPlayerParent)
		{
			GameManager.Instance.Player.transform.SetParent(m_SubwayCarPlayerParent);
		}
		else
		{
			GameManager.Instance.Player.transform.SetParent(m_SubwayCarParent);
		}
		if (m_TunnelCollider != null)
		{
			m_TunnelCollider.SetActive(value: false);
		}
		m_Door.Content.ForceDeactivate();
		m_Door.OnDeactivated -= HandleSubwayCarOnLeave;
		m_Door.OnDeactivated += HandleSubwayCarOnLeave;
	}

	private void HandleSubwayCarOnLeave(object sender, EventArgs e)
	{
		m_Door.OnDeactivated -= HandleSubwayCarOnLeave;
		CameraEffects.ShakeRotation(2f, 0.5f, 10, 90f, fadeOut: false);
		ResetSequence();
		m_Sequence.Insert(1f, m_Subway.DOMove(m_SubwayLeave.position, 2f).SetEase(Ease.InSine));
		m_Sequence.InsertCallback(1.25f, HandlesPlay);
		m_Sequence.OnComplete(Travel);
		this.OnLeaveVO.Send(this);
	}

	private void HandlesPlay()
	{
		for (int i = 0; i < m_Handles.Length; i++)
		{
			Transform target = m_Handles[i].transform;
			target.DOKill();
			target.DOShakeRotation(0.25f, new Vector3(10f, 0f, 0f), 10, 90f, fadeOut: false).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
		}
	}

	private void HandlesStop()
	{
		for (int i = 0; i < m_Handles.Length; i++)
		{
			m_Handles[i].transform.DOKill();
		}
	}

	private void Travel()
	{
		m_Subway.position = m_SubwayTravel.position;
		Loop();
		this.OnTravelStart.Send(this);
		StartCoroutine(LoadSections());
	}

	private IEnumerator LoadSections()
	{
		SectionID[] uninitializedSections = m_UninitializedSections;
		foreach (SectionID sectionID in uninitializedSections)
		{
			Task uninitializedTask = GameManager.Instance.SectionManager.LoadSectionAsync(sectionID, initialize: false);
			while (!uninitializedTask.IsCompleted)
			{
				yield return null;
			}
		}
		Task task = GameManager.Instance.SectionManager.LoadSectionAsync(m_SectionID);
		while (!task.IsCompleted)
		{
			yield return null;
		}
		Traveled();
	}

	private void Traveled()
	{
		m_IsTraveled = true;
	}

	private void Loop()
	{
		CameraEffects.ShakeRotation(1.5f, 0.5f, 10, 90f, fadeOut: false);
		ResetTunnelSequence();
		if (m_LoopIndex >= m_LoopMax)
		{
			if (m_IsTraveled)
			{
				m_Subway.position = m_CityStart.position;
				m_Subway.eulerAngles = m_CityStart.eulerAngles;
				ResetSequence();
				this.OnTravelEnd.Send(this);
				this.OnArriveVO.Send(this);
				m_Sequence.Insert(0f, m_Subway.DOMove(m_CityEnd.position, 4f).SetEase(Ease.OutSine));
				m_Sequence.OnComplete(TraveledComplete);
			}
			else
			{
				LoopTravel();
			}
		}
		else
		{
			LoopTravel();
		}
	}

	private void TraveledComplete()
	{
		HandlesStop();
		CameraEffects.ShakeRotation(0.5f, 5f);
		GameManager.Instance.Player.UnlockAbilities();
		GameManager.Instance.EnableTeleport();
		GameManager.Instance.Player.transform.SetParent(null);
		UnityEngine.Object.DontDestroyOnLoad(GameManager.Instance.Player.gameObject);
		GameManager.Instance.Player.ResetRotation();
		m_Door.Content.ForceActivate();
		m_CityEnter.OnEnter -= HandleCityOnEnter;
		m_CityEnter.OnEnter += HandleCityOnEnter;
		m_CityEnter.SetActive(active: true);
	}

	private void HandleCityOnEnter(object sender, EventArgs e)
	{
		m_CityEnter.OnEnter -= HandleCityOnEnter;
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		Leave();
	}

	private void LoopTravel()
	{
		m_LoopIndex++;
		m_TunnelSequence.InsertCallback(0f, delegate
		{
			m_Tunnel.position = m_TunnelStart.position;
		});
		m_TunnelSequence.Insert(0f, m_Tunnel.DOMove(m_TunnelEnd.position, 1.5f).SetEase(Ease.Linear));
		m_TunnelSequence.OnComplete(Loop);
	}

	public void Leave()
	{
		m_Door.Content.ForceDeactivate();
		ResetSequence();
		m_Sequence.Insert(1f, m_Subway.DOMove(m_CityLeave.position, 3f).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed));
		m_Sequence.OnComplete(LeaveComplete);
		m_SubwayCarEnter.SetActive(active: false);
		m_SubwayCarDeath.OnEnter -= HandleSubwayCarDeathOnEnter;
		m_SubwayCarDeath.OnEnter += HandleSubwayCarDeathOnEnter;
		m_SubwayCarDeath.SetActive(active: true);
		this.OnLeave.Send(this);
	}

	private void LeaveComplete()
	{
		GameManager.Instance.SectionManager.Remove(m_CurrentSectionID);
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			m_SubwayEventTrigger.OnEnter -= HandleSubwayEventTriggerOnEnter;
			m_SubwayEventTrigger.OnEnter += HandleSubwayEventTriggerOnEnter;
			m_SubwayEventTrigger.SetActive(active: true);
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SubwayCarTravel :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		CheckStatus();
	}

	private void HandleSubwayEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_SubwayEventTrigger.OnEnter -= HandleSubwayEventTriggerOnEnter;
		m_IsActive = true;
	}

	private void ResetTunnelSequence()
	{
		KillTunnelSequence();
		m_TunnelSequence = DOTween.Sequence();
	}

	private void KillTunnelSequence()
	{
		if (m_TunnelSequence != null)
		{
			m_TunnelSequence.Kill();
			m_TunnelSequence = null;
		}
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

	private void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		m_SubwayEventTrigger.OnEnter -= HandleSubwayEventTriggerOnEnter;
		m_SubwayCarEnter.OnEnter -= HandleSubwayCarEnterOnEnter;
		m_Door.OnDeactivated -= HandleSubwayCarOnLeave;
		m_SubwayCarDeath.OnEnter -= HandleSubwayCarDeathOnEnter;
	}

	protected override void OnDisposed()
	{
		this.OnArrive = null;
		this.OnLeave = null;
		this.OnTravelStart = null;
		this.OnTravelEnd = null;
		this.OnArriveVO = null;
		this.OnLeaveVO = null;
		this.OnHorn = null;
		m_Section = null;
		RemoveListeners();
		KillTunnelSequence();
		KillSequence();
		base.OnDisposed();
	}
}
