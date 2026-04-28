using System;
using UnityEngine;

public class PuzzleSubmit : JMonoBehaviour
{
	[Header("Code")]
	[SerializeField]
	private int m_Code = 3645;

	[Header("Lever")]
	[SerializeField]
	private Lever m_Lever;

	[Header("Wheels")]
	[SerializeField]
	private PuzzleWheel m_Wheel1;

	[SerializeField]
	private PuzzleWheel m_Wheel2;

	[SerializeField]
	private PuzzleWheel m_Wheel3;

	[SerializeField]
	private PuzzleWheel m_Wheel4;

	[Header("Effects")]
	[SerializeField]
	private LightFixture m_SubmitEmission;

	[SerializeField]
	private GameObject m_Electricity;

	[SerializeField]
	private LightFlicker[] m_LightFlickers;

	[Header("Nav Marker")]
	[SerializeField]
	private Transform m_Navigation;

	[Header("Cutscene")]
	[SerializeField]
	private Cutscene m_SuccessCutscene;

	[SerializeField]
	private Cutscene m_FailCutscene;

	public event EventHandler OnPowerUp;

	public event EventHandler OnPowerDown;

	public override void Start()
	{
		RemoveListeners();
		AddListeners();
		SetActiveFail(active: false);
		m_Electricity.SetActive(value: false);
		string text = m_Code.ToString();
		m_Wheel1.Initialize(text[0].ToString().ParseInt());
		m_Wheel2.Initialize(text[1].ToString().ParseInt());
		m_Wheel3.Initialize(text[2].ToString().ParseInt());
		m_Wheel4.Initialize(text[3].ToString().ParseInt());
	}

	public void DisableFailCutscene()
	{
		m_FailCutscene.SetActive(active: false);
	}

	private void SetActiveSuccess(bool active)
	{
		m_SuccessCutscene.SetActive(active);
		m_SuccessCutscene.gameObject.SetActive(active);
	}

	private void SetActiveFail(bool active)
	{
		m_FailCutscene.SetActive(active);
		m_FailCutscene.gameObject.SetActive(active);
	}

	private void SetActiveEffects(bool active)
	{
		if (active)
		{
			for (int i = 0; i < m_LightFlickers.Length; i++)
			{
				m_LightFlickers[i].TurnOn();
			}
			m_Electricity.SetActive(value: true);
			m_SubmitEmission.SetEmission(1f);
			GameManager.Instance.ShowNavigation(m_Navigation);
			this.OnPowerUp.Send(this);
		}
		else
		{
			for (int j = 0; j < m_LightFlickers.Length; j++)
			{
				m_LightFlickers[j].TurnOff();
			}
			m_Electricity.SetActive(value: false);
			m_SubmitEmission.SetEmission(0f);
			GameManager.Instance.ClearNavigation();
			this.OnPowerDown.Send(this);
		}
	}

	private void SetActiveWheels(bool active)
	{
		m_Wheel1.SetActive(active);
		m_Wheel2.SetActive(active);
		m_Wheel3.SetActive(active);
		m_Wheel4.SetActive(active);
	}

	private void HandleSwitchOnActivated(object sender, EventArgs e)
	{
		m_Lever.Content.Disable();
		SetActiveEffects(active: true);
		SetActiveWheels(active: false);
		if (string.Concat(string.Concat(string.Concat("" + m_Wheel1.ID, m_Wheel2.ID.ToString()), m_Wheel3.ID.ToString()), m_Wheel4.ID.ToString()).ParseInt() == m_Code)
		{
			SetActiveFail(active: false);
			SetActiveSuccess(active: true);
		}
		else
		{
			SetActiveSuccess(active: false);
			SetActiveFail(active: true);
		}
	}

	private void HandleSuccessCutsceneOnComplete(object sender, EventArgs e)
	{
		m_SuccessCutscene.OnComplete -= HandleSuccessCutsceneOnComplete;
		m_Lever.Content.Disable();
		SetActiveEffects(active: false);
	}

	private void HandleFailCutsceneOnComplete(object sender, EventArgs e)
	{
		ResetSwitch();
	}

	public void ResetSwitch()
	{
		m_Lever.Content.ForceDeactivateComplete();
		m_Lever.Content.Enable();
		SetActiveEffects(active: false);
		SetActiveWheels(active: true);
		SetActiveSuccess(active: false);
		m_FailCutscene.SetActive(active: false);
	}

	private void AddListeners()
	{
		m_Lever.OnActivated += HandleSwitchOnActivated;
		m_SuccessCutscene.OnComplete += HandleSuccessCutsceneOnComplete;
		m_FailCutscene.OnComplete += HandleFailCutsceneOnComplete;
	}

	private void RemoveListeners()
	{
		m_Lever.OnActivated -= HandleSwitchOnActivated;
		m_SuccessCutscene.OnComplete -= HandleSuccessCutsceneOnComplete;
		m_FailCutscene.OnComplete -= HandleFailCutsceneOnComplete;
	}

	protected override void OnDisposed()
	{
		this.OnPowerUp = null;
		this.OnPowerDown = null;
		RemoveListeners();
		base.OnDisposed();
	}
}
