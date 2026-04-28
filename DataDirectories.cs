using System;
using UnityEngine;

[Serializable]
public class DataDirectories : DataCategory
{
	[SerializeField]
	private SectionDataDirectory m_SectionDirectory = new SectionDataDirectory();

	[SerializeField]
	private TaskDataDirectory m_TaskDirectory = new TaskDataDirectory();

	[SerializeField]
	private CollectableDataDirectory m_CollectableDirectory = new CollectableDataDirectory();

	public SectionDataDirectory SectionDirectory => m_SectionDirectory;

	public TaskDataDirectory TaskDirectory => m_TaskDirectory;

	public CollectableDataDirectory CollectableDirectory => m_CollectableDirectory;

	public MemoDataDirectory MemoDirectory => m_CollectableDirectory.MemoDirectory;

	public AudioLogDataDirectory AudioLogDirectory => m_CollectableDirectory.AudioLogDirectory;

	public MemoryDataDirectory MemoryDirectory => m_CollectableDirectory.MemoryDirectory;

	public UpgradeDataDirectory UpgradeDirectory => m_CollectableDirectory.UpgradeDirectory;

	public MeatlyDataDirectory MeatlyDirectory => m_CollectableDirectory.MeatlyDirectory;

	public IllusionDataDirectory IllusionDirectory => m_CollectableDirectory.IllusionDirectory;

	public GentSchematicDataDirectory GentSchematicDirectory => m_CollectableDirectory.GentSchematicDirectory;

	public GentLockDataDirectory GentLockDirectory => m_CollectableDirectory.GentLockDataDirectory;

	public GentPowerDataDirectory GentPowerDirectory => m_CollectableDirectory.GentPowerDataDirectory;

	public JackatoyDataDirectory JackatoyDirectory => m_CollectableDirectory.JackatoyDataDirectory;

	public DataDirectories()
	{
		AddData(SectionDirectory);
		AddData(TaskDirectory);
		AddData(MemoDirectory);
		AddData(AudioLogDirectory);
		AddData(MemoryDirectory);
		AddData(UpgradeDirectory);
		AddData(MeatlyDirectory);
		AddData(IllusionDirectory);
		AddData(GentSchematicDirectory);
		AddData(GentLockDirectory);
		AddData(GentPowerDirectory);
		AddData(JackatoyDirectory);
	}
}
