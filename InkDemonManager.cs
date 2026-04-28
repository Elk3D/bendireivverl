using System;
using UnityEngine;

public class InkDemonManager : JMonoBehaviour
{
	private bool m_IsApproaching;

	private float m_Timer;

	private float m_TimerLimit;

	public bool IsActive { get; private set; }

	public float Timer => m_Timer;

	public float TimerLimit => m_TimerLimit;

	public event EventHandler OnApproaching;

	public static InkDemonManager Create()
	{
		return GameManager.Instance.AssetManager.CreateAsset<InkDemonManager>("Controllers/InkDemonManager");
	}

	public override void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (!IsActive || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (m_Timer >= m_TimerLimit)
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide && GameManager.Instance.Player.BattleStatus == BattleStatus.None && !GameManager.Instance.Player.IsUnderWanter && GameManager.Instance.Player.CurrentState == State.Player.Default)
			{
				SetActive(active: false);
				ResetTimer();
				GameManager.Instance.Player.InkDemonTest();
				m_IsApproaching = false;
			}
			return;
		}
		if (!m_IsApproaching && m_Timer >= m_TimerLimit - 20f)
		{
			m_IsApproaching = true;
			if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty == DifficultyLevel.Easy)
			{
				this.OnApproaching.Send(this);
			}
		}
		if (GameManager.Instance.Player.CurrentState != State.Player.Cutscene && GameManager.Instance.Player.CurrentState != State.Player.CutscenePeek && GameManager.Instance.Player.CurrentState != State.Player.Ability && GameManager.Instance.Player.BattleStatus == BattleStatus.None && !GameManager.Instance.Player.IsUnderWanter && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
		{
			m_Timer += Time.deltaTime;
		}
	}

	public void CheckAvailability()
	{
		bool flag = true;
		Section[] allSections = GameManager.Instance.SectionManager.GetAllSections();
		if (allSections != null && allSections.Length != 0)
		{
			foreach (Section section in allSections)
			{
				if (section.IsActive && section.SectionControllers != null)
				{
					SectionController[] sectionControllers = section.SectionControllers;
					for (int j = 0; j < sectionControllers.Length; j++)
					{
						if (sectionControllers[j] is SectionInkDemonController sectionInkDemonController)
						{
							if (!sectionInkDemonController.IsAvailable)
							{
								flag = false;
							}
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
			SetActive(flag);
		}
		else
		{
			SetActive(active: false);
		}
	}

	public void ResetTimer()
	{
		m_Timer = 0f;
		m_TimerLimit = TimerCheck.InkDemon();
	}

	public void SetActive(bool active)
	{
		IsActive = active;
	}

	public void Clear()
	{
		IsActive = false;
		m_IsApproaching = false;
		m_Timer = 0f;
	}

	protected override void OnDisposed()
	{
		this.OnApproaching = null;
		base.OnDisposed();
	}
}
