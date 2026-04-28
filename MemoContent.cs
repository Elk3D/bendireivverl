using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoContent : ActionEventContent<MemoContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable MemoContent_LostOne;

		public Interactable MemoContent_JDS;

		public Interactable MemoContent_Gent;
	}

	private Memo m_Memo;

	private UIMemoDataVO m_DataVO;

	protected override void OnInitialize()
	{
		m_Memo = (Memo)base.Connectable;
		if (m_Memo.Data != null && m_Properties != null && m_Properties.Length != 0 && !GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.ContainsKey(m_Memo.Data.ID))
		{
			Properties properties = m_Properties[0];
			properties.ActionEvent = GenerateMemo(properties);
			if (!(properties.ActionEvent == null))
			{
				properties.ActionEvent.transform.SetParent(base.transform);
				properties.ActionEvent.transform.localPosition = Vector3.zero;
				properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
				properties.ActionEvent.transform.localScale = Vector3.one;
			}
		}
	}

	private Interactable GenerateMemo(Properties properties)
	{
		return m_Memo.Data.MemoType switch
		{
			MemoType.LostOne => CreateMemo(properties.MemoContent_LostOne), 
			MemoType.JDS => CreateMemo(properties.MemoContent_JDS), 
			MemoType.GENT => CreateMemo(properties.MemoContent_Gent), 
			_ => null, 
		};
	}

	private Interactable CreateMemo(Interactable interactable)
	{
		return GameManager.Instance.AssetManager.CreateAsset<Interactable>(interactable);
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowInteraction(TextUtility.GetKey(InteractionType.INTERACTION_TAKE.ToString()));
		m_DataVO = new UIMemoDataVO(m_Memo, m_Memo.Data.MemoType, TextUtility.GetKey(m_Memo.Data.ID.ToString()));
		GameManager.Instance.ShowMemo(m_DataVO);
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.ContainsKey(m_Memo.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Add(m_Memo.Data.ID, DataObject<MemoID, MemoDataObject>.Create(m_Memo.Data.ID));
		}
		Array values = Enum.GetValues(typeof(MemoID));
		List<MemoID> list = new List<MemoID>();
		for (int i = 0; i < values.Length; i++)
		{
			object value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((MemoID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.WRITTEN_IN_INK);
		}
	}

	protected override void OnDisposed()
	{
		m_Memo = null;
		m_DataVO = null;
		base.OnDisposed();
	}
}
