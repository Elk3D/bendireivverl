using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Memory : JMonoBehaviour
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Memory Identifier")]
	[SerializeField]
	protected MemoryID m_MemoryID;

	[Header("Memory Content")]
	[SerializeField]
	private MemoryContent m_Content;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	public SectionID SectionID => m_SectionID;

	public MemoryID MemoryID => m_MemoryID;

	public MemoryContent Content { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.ContainsKey(m_MemoryID))
		{
			Content = GameManager.Instance.AssetManager.CreateAsset<MemoryContent>(m_Content);
			Content.transform.SetParent(base.transform);
			Content.transform.localPosition = Vector3.zero;
			Content.transform.localEulerAngles = Vector3.zero;
			Content.Activator.OnActivated -= HandleActivatorOnActivated;
			Content.Activator.OnActivated += HandleActivatorOnActivated;
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			if (!CheckStatus())
			{
				GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
			}
		}
	}

	private void HandleActivatorOnActivated(object sender, EventArgs e)
	{
		JDebug.Log("Memory :: HandleActivatorOnActivated", this, JDebug.JDebugType.Collectables);
		Content.Activator.OnActivated -= HandleActivatorOnActivated;
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.ContainsKey(m_MemoryID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Add(m_MemoryID, DataObject<MemoryID, MemoryDataObject>.Create(m_MemoryID));
		}
		Content.Director.OnComplete -= HandleDirectorOnComplete;
		Content.Director.OnComplete += HandleDirectorOnComplete;
		InternalPreparePlay();
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_MEMORY"));
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Count < 2)
		{
			GameManager.Instance.ClearNotificationText();
			string text = " <color=#FEC97D>";
			string text2 = "<color=#FFFFFF> ";
			string text3 = TextUtility.GetKey("NOTIFICATION_MEMORY_MENU").ToUpper();
			string text4 = TextUtility.GetKey("NOTIFICATION_MENU_01").ToUpper();
			string text5 = TextUtility.GetKey("NOTIFICATION_MENU_02").ToUpper();
			string text6 = TextUtility.GetKey("NOTIFICATION_MENU_03").ToUpper();
			string text7 = TextUtility.GetKey("NOTIFICATION_MENU_04").ToUpper();
			string text8 = text + text3 + text2 + text4 + text + text5 + text2 + text6 + text + text7 + " ";
			GameManager.Instance.ShowNotificationText(text8);
		}
		DirectorOnComplete();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("Memory :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Collectables);
		CheckStatus();
	}

	private void InternalPreparePlay()
	{
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.HideFirstPersonArms();
		GameManager.Instance.Player.CancelMovement();
		GameManager.Instance.Player.ResetAnimation();
		GameManager.Instance.Player.ResetRotation();
		GameManager.Instance.Player.SetState(State.Player.Cutscene);
		Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(Content.StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(Content.StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.OnComplete(InternalPreparePlayOnComplete);
	}

	private void InternalPreparePlayOnComplete()
	{
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		Content.Director.Play();
		Array values = Enum.GetValues(typeof(MemoryID));
		List<MemoryID> list = new List<MemoryID>();
		for (int i = 0; i < values.Length; i++)
		{
			object value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((MemoryID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.SELF_DISCOVERY);
		}
		this.OnActivated.Send(this);
	}

	private void DirectorOnComplete()
	{
		RemoveListeners();
		GameManager.Instance.ShowCrosshair();
		this.OnComplete.Send(this);
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
			if (Content != null)
			{
				Content.Activator.SetActive(active: true);
				Content.Initialize();
			}
		}
		return flag;
	}

	private void RemoveListeners()
	{
		if (Content != null)
		{
			Content.Activator.OnActivated -= HandleActivatorOnActivated;
			Content.Director.OnComplete -= HandleDirectorOnComplete;
		}
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
