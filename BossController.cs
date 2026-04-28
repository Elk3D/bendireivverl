using System;
using S13Audio;
using UnityEngine;

public class BossController : JMonoBehaviour
{
	[Header("Cutscene")]
	[SerializeField]
	private Cutscene m_BossCutscene;

	[Header("ShipAhoy")]
	[SerializeField]
	private Transform m_ShipAhoy;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private GameObject m_ShipAhoySparks;

	[SerializeField]
	private HittableCharacter m_HittableCharacter;

	[Header("Phases")]
	[SerializeField]
	private BossPhaseController m_PhaseController;

	[SerializeField]
	private BossPhase m_PhaseOne;

	[SerializeField]
	private BossPhase m_PhaseWave;

	[SerializeField]
	private BossPhase m_PhaseTwo;

	[Header("Chargers")]
	[SerializeField]
	private GentRechargerInstant m_GentRechargerInstant;

	[Header("Towers")]
	[SerializeField]
	private GameObject m_CenterLights;

	[SerializeField]
	private GentPower[] m_GentPowers;

	[SerializeField]
	private Transform[] m_EdisonTowers;

	[SerializeField]
	private SignalTower[] m_SignalTowers;

	[SerializeField]
	private S13ScriptableEvent[] m_S13ScriptableEvents;

	private int m_TowerCount;

	public override void Awake()
	{
		m_ShipAhoy.gameObject.SetActive(value: false);
		m_CenterLights.SetActive(value: false);
		for (int i = 0; i < m_EdisonTowers.Length; i++)
		{
			m_EdisonTowers[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < m_SignalTowers.Length; j++)
		{
			m_SignalTowers[j].TurnOff();
		}
		for (int k = 0; k < m_GentPowers.Length; k++)
		{
			m_GentPowers[k].OnComplete -= HandleActionEventControllerOnInteractionComplete;
			m_GentPowers[k].OnComplete += HandleActionEventControllerOnInteractionComplete;
		}
		m_ShipAhoySparks.SetActive(value: false);
		m_BossCutscene.OnComplete -= HandleBossOnComplete;
		m_BossCutscene.OnComplete += HandleBossOnComplete;
		m_PhaseWave.OnEnd -= HandlePhaseWaveOnEnd;
		m_PhaseWave.OnEnd += HandlePhaseWaveOnEnd;
		m_PhaseController.OnComplete -= HandleBossPhaseControllerOnComplete;
		m_PhaseController.OnComplete += HandleBossPhaseControllerOnComplete;
	}

	private void HandleBossPhaseControllerOnComplete(object sender, EventArgs e)
	{
		if (m_PhaseController != null)
		{
			m_PhaseController.OnComplete -= HandleBossPhaseControllerOnComplete;
		}
		if (m_BossCutscene != null)
		{
			m_BossCutscene.OnComplete -= HandleBossOnComplete;
		}
		if (m_PhaseWave != null)
		{
			m_PhaseWave.OnEnd -= HandlePhaseWaveOnEnd;
		}
		m_ShipAhoySparks.SetActive(value: false);
		m_HittableCharacter.SetActive(active: false);
		m_ShipAhoy.gameObject.SetActive(value: false);
		m_CenterLights.SetActive(value: false);
	}

	private void HandlePhaseWaveOnEnd(object sender, EventArgs e)
	{
		m_ShipAhoySparks.SetActive(value: false);
	}

	private void HandleBossOnComplete(object sender, EventArgs e)
	{
		m_BossCutscene.OnComplete -= HandleBossOnComplete;
		m_ShipAhoy.gameObject.SetActive(value: true);
		m_HittableCharacter.SetActive(active: true);
		m_PhaseController.Initialize();
		m_GentRechargerInstant.SetActive(active: true);
		m_CenterLights.SetActive(value: true);
		for (int i = 0; i < m_EdisonTowers.Length; i++)
		{
			m_EdisonTowers[i].gameObject.SetActive(value: true);
		}
		for (int j = 0; j < m_SignalTowers.Length; j++)
		{
			m_SignalTowers[j].TurnOn();
		}
		if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Easy && GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Normal)
		{
			return;
		}
		for (int k = 0; k < m_GentPowers.Length; k++)
		{
			Sparkles componentInChildren = m_GentPowers[k].GetComponentInChildren<Sparkles>();
			if (componentInChildren != null)
			{
				componentInChildren.Play();
			}
		}
	}

	private void HandleActionEventControllerOnInteractionComplete(object sender, EventArgs e)
	{
		m_TowerCount++;
		GentPower gentPower = (GentPower)sender;
		int num = 0;
		for (int i = 0; i < m_GentPowers.Length; i++)
		{
			if (gentPower.GentPowerID == m_GentPowers[i].GentPowerID)
			{
				num = i;
				break;
			}
		}
		m_EdisonTowers[num].gameObject.SetActive(value: false);
		m_SignalTowers[num].TurnOff();
		m_S13ScriptableEvents[num].Raise();
		if (m_TowerCount == 4)
		{
			SetPhase(m_PhaseTwo);
		}
		else if (m_TowerCount == 2)
		{
			SetPhase(m_PhaseOne);
		}
	}

	public void ActivateSignalTower(int index)
	{
		if (index == 0)
		{
			m_CenterLights.SetActive(value: true);
		}
		m_EdisonTowers[index].gameObject.SetActive(value: true);
		m_SignalTowers[index].TurnOn();
	}

	private void SetPhase(BossPhase phaseComplete)
	{
		phaseComplete.Complete();
		m_GentRechargerInstant.SetActive(active: false);
		m_ShipAhoySparks.SetActive(value: true);
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < m_GentPowers.Length; i++)
		{
			m_GentPowers[i].OnComplete -= HandleActionEventControllerOnInteractionComplete;
		}
		m_BossCutscene.OnComplete -= HandleBossOnComplete;
		m_PhaseWave.OnEnd -= HandlePhaseWaveOnEnd;
		base.OnDisposed();
	}
}
