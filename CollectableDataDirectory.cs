using System;
using UnityEngine;

[Serializable]
public class CollectableDataDirectory
{
	[SerializeField]
	private MemoDataDirectory m_MemoDirectory = new MemoDataDirectory();

	[SerializeField]
	private AudioLogDataDirectory m_AudioLogDirectory = new AudioLogDataDirectory();

	[SerializeField]
	private MemoryDataDirectory m_MemoryDirectory = new MemoryDataDirectory();

	[SerializeField]
	private UpgradeDataDirectory m_UpgradeDirectory = new UpgradeDataDirectory();

	[SerializeField]
	private MeatlyDataDirectory m_MeatlyDirectory = new MeatlyDataDirectory();

	[SerializeField]
	private IllusionDataDirectory m_IllusionDirectory = new IllusionDataDirectory();

	[SerializeField]
	private GentSchematicDataDirectory m_GentSchematicDirectory = new GentSchematicDataDirectory();

	[SerializeField]
	private GentLockDataDirectory m_GentLockDataDirectory = new GentLockDataDirectory();

	[SerializeField]
	private GentPowerDataDirectory m_GentPowerDataDirectory = new GentPowerDataDirectory();

	[SerializeField]
	private JackatoyDataDirectory m_JackatoyDataDirectory = new JackatoyDataDirectory();

	public MemoDataDirectory MemoDirectory => m_MemoDirectory;

	public AudioLogDataDirectory AudioLogDirectory => m_AudioLogDirectory;

	public MemoryDataDirectory MemoryDirectory => m_MemoryDirectory;

	public UpgradeDataDirectory UpgradeDirectory => m_UpgradeDirectory;

	public MeatlyDataDirectory MeatlyDirectory => m_MeatlyDirectory;

	public IllusionDataDirectory IllusionDirectory => m_IllusionDirectory;

	public GentSchematicDataDirectory GentSchematicDirectory => m_GentSchematicDirectory;

	public GentLockDataDirectory GentLockDataDirectory => m_GentLockDataDirectory;

	public GentPowerDataDirectory GentPowerDataDirectory => m_GentPowerDataDirectory;

	public JackatoyDataDirectory JackatoyDataDirectory => m_JackatoyDataDirectory;
}
