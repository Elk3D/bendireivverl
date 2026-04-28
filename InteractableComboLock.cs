using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class InteractableComboLock : JMonoBehaviour
{
	[Serializable]
	public class NumberWheel
	{
		public Interactable Interactable;

		public Transform Wheel;

		public int MaxIndex = 5;

		private const int DEFAULT_START_INDEX = 3;

		public int CurrentIndex { get; private set; }

		public void Initialize(int code)
		{
			for (CurrentIndex = code; CurrentIndex == code; CurrentIndex = UnityEngine.Random.Range(1, MaxIndex))
			{
			}
			int num = 3;
			while (num != CurrentIndex)
			{
				num++;
				if (num > MaxIndex)
				{
					num = 1;
				}
				Wheel.localEulerAngles += WHEEL_ROTATION;
			}
			AddListener();
		}

		public void InitializeCode(int code)
		{
			CurrentIndex = code;
			int num = 3;
			while (num != CurrentIndex)
			{
				num++;
				if (num > MaxIndex)
				{
					num = 1;
				}
				Wheel.localEulerAngles += WHEEL_ROTATION;
			}
			AddListener();
		}

		public void ForceComplete(int code)
		{
			CurrentIndex = code;
			int num = 3;
			while (num != CurrentIndex)
			{
				num++;
				if (num > MaxIndex)
				{
					num = 1;
				}
				Wheel.localEulerAngles += WHEEL_ROTATION;
			}
		}

		private void HandleInteractableOnInteract(object sender, EventArgs e)
		{
			RemoveListener();
			GameManager.Instance.Player.Interaction.ResetInteraction();
			DoRotate().OnComplete(delegate
			{
				CurrentIndex++;
				if (CurrentIndex > MaxIndex)
				{
					CurrentIndex = 1;
				}
				AddListener();
				Interactable.ResetAction();
				Interactable.SetActive(active: true);
			});
		}

		public Tweener DoRotate()
		{
			ClearTweens();
			return Wheel.DOLocalRotate(WHEEL_ROTATION, 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack);
		}

		public void ClearTweens()
		{
			Wheel.DOKill();
		}

		private void AddListener()
		{
			Interactable.OnInteract += HandleInteractableOnInteract;
		}

		public void RemoveListener()
		{
			Interactable.OnInteract -= HandleInteractableOnInteract;
		}
	}

	private static Vector3 WHEEL_ROTATION = new Vector3(0f, 0f, -72f);

	[Header("Light")]
	[SerializeField]
	private GameObject m_Light;

	[Header("Button")]
	[SerializeField]
	private Interactable m_Button;

	[SerializeField]
	private Transform m_ButtonPushLocation;

	[Header("Number Wheels")]
	[SerializeField]
	private List<NumberWheel> m_NumberWheels = new List<NumberWheel>();

	[Space]
	[Header("S13Content")]
	[SerializeField]
	private S13ActionGroupReference onWrongCodeAction;

	[SerializeField]
	private S13ActionGroupReference onCorrectCodeAction;

	private Sequence m_ButtonSequence;

	private Vector3 m_ButtonDefaultPosition;

	private int m_Code = -1;

	public List<NumberWheel> NumberWheels => m_NumberWheels;

	public event EventHandler OnSuccess;

	public void Initialize(int code)
	{
		m_ButtonDefaultPosition = m_Button.transform.localPosition;
		string text = code.ToString();
		for (int i = 0; i < m_NumberWheels.Count; i++)
		{
			NumberWheel numberWheel = m_NumberWheels[i];
			numberWheel.Interactable.SetActive(active: false);
			numberWheel.Initialize(text[i].ToString().ParseInt());
		}
		m_Button.SetActive(active: false);
		m_Code = code;
		AddListeners();
		m_Button.SetActive(active: true);
		for (int j = 0; j < m_NumberWheels.Count; j++)
		{
			m_NumberWheels[j].Interactable.SetActive(active: true);
		}
	}

	public void InitializeCode(int code, int finalCode)
	{
		m_ButtonDefaultPosition = m_Button.transform.localPosition;
		string text = code.ToString();
		for (int i = 0; i < m_NumberWheels.Count; i++)
		{
			NumberWheel numberWheel = m_NumberWheels[i];
			numberWheel.Interactable.SetActive(active: false);
			numberWheel.InitializeCode(text[i].ToString().ParseInt());
		}
		m_Button.SetActive(active: false);
		m_Code = finalCode;
		AddListeners();
		m_Button.SetActive(active: true);
		for (int j = 0; j < m_NumberWheels.Count; j++)
		{
			m_NumberWheels[j].Interactable.SetActive(active: true);
		}
	}

	public void ForceComplete(int code)
	{
		string text = code.ToString();
		for (int i = 0; i < m_NumberWheels.Count; i++)
		{
			NumberWheel numberWheel = m_NumberWheels[i];
			numberWheel.Interactable.SetActive(active: false);
			numberWheel.ForceComplete(text[i].ToString().ParseInt());
		}
		m_Light.SetActive(value: false);
		m_Button.SetActive(active: false);
		m_Button.transform.localPosition = m_ButtonPushLocation.localPosition;
	}

	private void HandleButtonOnInteract(object sender, EventArgs e)
	{
		RemoveListeners();
		m_Button.SetActive(active: false);
		string text = "";
		for (int i = 0; i < m_NumberWheels.Count; i++)
		{
			text += m_NumberWheels[i].CurrentIndex;
		}
		EnterCode(text.ParseInt());
	}

	private void EnterCode(int code)
	{
		if (code == m_Code)
		{
			Success();
		}
		else
		{
			Fail();
		}
	}

	private void Success()
	{
		for (int i = 0; i < m_NumberWheels.Count; i++)
		{
			NumberWheel numberWheel = m_NumberWheels[i];
			numberWheel.ClearTweens();
			numberWheel.RemoveListener();
			numberWheel.Interactable.SetActive(active: false);
		}
		m_Light.SetActive(value: false);
		ResetButtonSequence();
		m_ButtonSequence.Insert(0f, m_Button.transform.DOLocalMove(m_ButtonPushLocation.localPosition, 0.2f).SetEase(Ease.OutBack));
		m_ButtonSequence.OnComplete(SuccessOnComplete);
		S13OnCorrectCode();
	}

	private void SuccessOnComplete()
	{
		this.OnSuccess.Send(this);
	}

	private void Fail()
	{
		ResetButtonSequence();
		m_ButtonSequence.Insert(0f, m_Button.transform.DOLocalMove(m_ButtonPushLocation.localPosition, 0.1f).SetEase(Ease.Linear));
		m_ButtonSequence.Insert(0.1f, m_Button.transform.DOLocalMove(m_ButtonDefaultPosition, 0.1f).SetEase(Ease.OutSine));
		m_ButtonSequence.OnComplete(FailOnComplete);
		S13OnWrongCode();
	}

	private void FailOnComplete()
	{
		GameManager.Instance.Player.Interaction.ResetInteraction();
		AddListeners();
		m_Button.SetActive(active: true);
	}

	private void S13OnCorrectCode()
	{
		onCorrectCodeAction?.Execute();
	}

	private void S13OnWrongCode()
	{
		onWrongCodeAction?.Execute();
	}

	private void ResetButtonSequence()
	{
		KillButtonSequence();
		m_ButtonSequence = DOTween.Sequence();
	}

	private void KillButtonSequence()
	{
		if (m_ButtonSequence != null)
		{
			m_ButtonSequence.Kill();
			m_ButtonSequence = null;
		}
	}

	private void AddListeners()
	{
		m_Button.OnInteract += HandleButtonOnInteract;
	}

	private void RemoveListeners()
	{
		m_Button.OnInteract -= HandleButtonOnInteract;
	}

	protected override void OnDisposed()
	{
		this.OnSuccess = null;
		RemoveListeners();
		KillButtonSequence();
		for (int i = 0; i < m_NumberWheels.Count; i++)
		{
			NumberWheel numberWheel = m_NumberWheels[i];
			numberWheel.ClearTweens();
			numberWheel.RemoveListener();
		}
		base.OnDisposed();
	}
}
