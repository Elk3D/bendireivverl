using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectionDataObject : DataObject<SectionID, SectionDataObject>, IDataObject<SectionID>, IDataObject
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private bool m_IsComplete;

	[SerializeField]
	private ObjectiveDataGroup m_ObjectiveData = new ObjectiveDataGroup();

	[SerializeField]
	private CutsceneDataGroup m_CutsceneData = new CutsceneDataGroup();

	[SerializeField]
	private DoorDataGroup m_DoorData = new DoorDataGroup();

	[SerializeField]
	private MovableDataGroup m_MovableData = new MovableDataGroup();

	[SerializeField]
	private LootDataGroup m_LootData = new LootDataGroup();

	[SerializeField]
	private LootableEnemyDataGroup m_LootableEnemyData = new LootableEnemyDataGroup();

	[SerializeField]
	private BreakableDataGroup m_BreakableData = new BreakableDataGroup();

	[SerializeField]
	private FoodDataGroup m_FoodData = new FoodDataGroup();

	[SerializeField]
	private GentRechargerDataGroup m_GentRechargeData = new GentRechargerDataGroup();

	[SerializeField]
	private GentBatteryDataGroup m_GentBatteryData = new GentBatteryDataGroup();

	[SerializeField]
	private GentPartsDataGroup m_GentPartsData = new GentPartsDataGroup();

	[SerializeField]
	private SlugDataGroup m_SlugData = new SlugDataGroup();

	[SerializeField]
	private GentToolkitDataGroup m_GentToolkitData = new GentToolkitDataGroup();

	[SerializeField]
	private GentBatteryCasingDataGroup m_GentBatteryCasingData = new GentBatteryCasingDataGroup();

	[SerializeField]
	private GentCardDataGroup m_GentCardData = new GentCardDataGroup();

	[SerializeField]
	private PictureDataGroup m_PictureData = new PictureDataGroup();

	[SerializeField]
	private EnemyDataGroup m_EnemyData = new EnemyDataGroup();

	[SerializeField]
	private CharacterDirectorDataGroup m_CharacterDirectorData = new CharacterDirectorDataGroup();

	[SerializeField]
	private HideDataGroup m_HideData = new HideDataGroup();

	[SerializeField]
	private LadderDataGroup m_LadderData = new LadderDataGroup();

	[SerializeField]
	private ProjectorDataGroup m_ProjectorData = new ProjectorDataGroup();

	[SerializeField]
	private ComboLockDataGroup m_ComboLockData = new ComboLockDataGroup();

	[SerializeField]
	private PeekDataGroup m_PeekData = new PeekDataGroup();

	[SerializeField]
	private CompanionDirectorDataGroup m_CompanionDirectorData = new CompanionDirectorDataGroup();

	private List<Data> m_Groups = new List<Data>();

	public override SectionID ID => m_SectionID;

	public bool IsComplete => m_IsComplete;

	public ObjectiveDataGroup ObjectiveData => m_ObjectiveData;

	public CutsceneDataGroup CutsceneData => m_CutsceneData;

	public DoorDataGroup DoorData => m_DoorData;

	public MovableDataGroup MovableData => m_MovableData;

	public LootDataGroup LootData => m_LootData;

	public LootableEnemyDataGroup LootableEnemyData => m_LootableEnemyData;

	public BreakableDataGroup BreakableData => m_BreakableData;

	public FoodDataGroup FoodData => m_FoodData;

	public GentRechargerDataGroup GentRechargeData => m_GentRechargeData;

	public GentBatteryDataGroup GentBatteryData => m_GentBatteryData;

	public GentPartsDataGroup GentPartsData => m_GentPartsData;

	public SlugDataGroup SlugData => m_SlugData;

	public GentToolkitDataGroup GentToolkitData => m_GentToolkitData;

	public GentBatteryCasingDataGroup GentBatteryCasingData => m_GentBatteryCasingData;

	public GentCardDataGroup GentCardData => m_GentCardData;

	public PictureDataGroup PictureData => m_PictureData;

	public EnemyDataGroup EnemyData => m_EnemyData;

	public CharacterDirectorDataGroup CharacterDirectorData => m_CharacterDirectorData;

	public HideDataGroup HideData => m_HideData;

	public LadderDataGroup LadderData => m_LadderData;

	public ProjectorDataGroup ProjectorData => m_ProjectorData;

	public ComboLockDataGroup ComboLockData => m_ComboLockData;

	public PeekDataGroup PeekData => m_PeekData;

	public CompanionDirectorDataGroup CompanionDirectorData => m_CompanionDirectorData;

	public SectionDataObject()
	{
		m_Groups.Add(ObjectiveData);
		m_Groups.Add(CutsceneData);
		m_Groups.Add(DoorData);
		m_Groups.Add(MovableData);
		m_Groups.Add(LootData);
		m_Groups.Add(LootableEnemyData);
		m_Groups.Add(BreakableData);
		m_Groups.Add(FoodData);
		m_Groups.Add(GentRechargeData);
		m_Groups.Add(GentBatteryData);
		m_Groups.Add(GentPartsData);
		m_Groups.Add(SlugData);
		m_Groups.Add(GentToolkitData);
		m_Groups.Add(GentBatteryCasingData);
		m_Groups.Add(GentCardData);
		m_Groups.Add(PictureData);
		m_Groups.Add(EnemyData);
		m_Groups.Add(CharacterDirectorData);
		m_Groups.Add(HideData);
		m_Groups.Add(LadderData);
		m_Groups.Add(ProjectorData);
		m_Groups.Add(ComboLockData);
		m_Groups.Add(PeekData);
		m_Groups.Add(CompanionDirectorData);
	}

	public void SetComplete()
	{
		m_IsComplete = true;
	}

	public bool GetGroup(string name, out Data group)
	{
		bool result = false;
		group = null;
		for (int i = 0; i < m_Groups.Count; i++)
		{
			group = m_Groups[i];
			if (group.Name == name)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	protected override void Deserialize()
	{
		m_SectionID = m_ID;
	}
}
