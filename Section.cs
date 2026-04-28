using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class Section : DataMonoBehaviour<SectionID, SectionDataObject>
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Navigation Settings")]
	[SerializeField]
	private bool m_HasNavigation = true;

	[Header("Seasonal")]
	[SerializeField]
	private SeasonalSectionData m_SeasonalSectionData;

	private SectionController[] m_SectionControllers;

	protected override SectionID m_ID => m_SectionID;

	public SectionID SectionID => m_ID;

	public bool HasNavigation => m_HasNavigation;

	public SectionController[] SectionControllers => m_SectionControllers;

	public bool IsActive { get; private set; }

	public bool IsReady { get; private set; }

	public void SetActive(bool active)
	{
		IsActive = active;
	}

	protected override void InternalInitialize()
	{
		JDebug.Log("Section :: InternalInitialize", this, JDebug.JDebugType.SectionController);
		SetActive(active: true);
		StartCoroutine(InitializeControllers());
	}

	private IEnumerator InitializeControllers()
	{
		m_SectionControllers = GetComponentsInChildren<SectionController>(includeInactive: false);
		SectionLootController sectionLootController = null;
		SectionBreakableController sectionBreakableController = null;
		SectionHideController sectionHideController = null;
		SectionCompanionDirectorController sectionCompanionDirectorController = null;
		SewerDrainController sewerDrainController = null;
		for (int i = 0; i < m_SectionControllers.Length; i++)
		{
			SectionController sectionController = m_SectionControllers[i];
			sectionController.SetSection(this);
			if (sectionController is SectionLootController)
			{
				sectionLootController = sectionController as SectionLootController;
			}
			else if (sectionController is SectionBreakableController)
			{
				sectionBreakableController = sectionController as SectionBreakableController;
			}
			else if (sectionController is SectionHideController)
			{
				sectionHideController = sectionController as SectionHideController;
			}
			else if (sectionController is SectionCompanionDirectorController)
			{
				sectionCompanionDirectorController = sectionController as SectionCompanionDirectorController;
			}
			else if (sectionController is SewerDrainController)
			{
				sewerDrainController = sectionController as SewerDrainController;
			}
			else
			{
				yield return sectionController.Initialize();
			}
		}
		if (sewerDrainController != null)
		{
			yield return sewerDrainController.Initialize();
		}
		if (sectionLootController != null)
		{
			yield return sectionLootController.Initialize();
		}
		if (sectionBreakableController != null)
		{
			yield return sectionBreakableController.Initialize();
		}
		if (sectionHideController != null)
		{
			yield return sectionHideController.Initialize();
		}
		if (sectionCompanionDirectorController != null)
		{
			yield return sectionCompanionDirectorController.Initialize();
		}
		if (m_SeasonalSectionData != null && SeasonalCheck.IsSeasonal(out var seasonalType) && seasonalType != SeasonalType.None && GameManager.Instance.PlayerSettings.Seasonal)
		{
			SeasonalData seasonalData = m_SeasonalSectionData.SeasonalDatas.Find((SeasonalData x) => x.SeasonalType == seasonalType);
			if (seasonalData != null && seasonalData.SeasonalObject != null)
			{
				GameObject obj = Object.Instantiate(seasonalData.SeasonalObject);
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localEulerAngles = Vector3.zero;
			}
		}
		IsReady = true;
	}

	protected override void OnDisposed()
	{
		m_SectionControllers = null;
		base.OnDisposed();
	}
}
