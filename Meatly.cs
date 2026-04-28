using System;
using System.Collections.Generic;
using UnityEngine;

public class Meatly : JMonoBehaviour
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Meatly Identifier")]
	[SerializeField]
	protected MeatlyID m_MeatlyID;

	[Header("Meatly Content")]
	[SerializeField]
	private MeatlyContent m_Content;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Requirement Complete")]
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public MeatlyContent Content { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MeatlyDirectory.ContainsKey(m_MeatlyID))
		{
			Content = GameManager.Instance.AssetManager.CreateAsset<MeatlyContent>(m_Content);
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
		else if (m_ActiveSetter != null)
		{
			m_ActiveSetter.SetActive(active: true);
		}
	}

	private void HandleActivatorOnActivated(object sender, EventArgs e)
	{
		JDebug.Log("Meatly :: HandleActivatorOnActivated", this, JDebug.JDebugType.Collectables);
		Content.Activator.OnActivated -= HandleActivatorOnActivated;
		GameManager.Instance.GameData.CurrentSave.DataDirectories.MeatlyDirectory.Add(m_MeatlyID, DataObject<MeatlyID, MeatlyDataObject>.Create(m_MeatlyID));
		Content.Director.OnComplete -= HandleDirectorOnComplete;
		Content.Director.OnComplete += HandleDirectorOnComplete;
		Content.Director.Play();
		Array values = Enum.GetValues(typeof(MeatlyID));
		List<MeatlyID> list = new List<MeatlyID>();
		for (int i = 0; i < values.Length; i++)
		{
			object value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((MeatlyID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MeatlyDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.FAMILIAR_FACES);
		}
		this.OnActivated.Send(this);
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		JDebug.Log("Meatly :: HandleDirectorOnComplete", this, JDebug.JDebugType.Collectables);
		RemoveListeners();
		this.OnComplete.Send(this);
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("Meatly :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Collectables);
		CheckStatus();
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
			}
			if (m_ActiveSetter != null)
			{
				m_ActiveSetter.SetActive(active: true);
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
