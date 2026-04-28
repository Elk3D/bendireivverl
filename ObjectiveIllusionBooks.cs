public class ObjectiveIllusionBooks : Objective
{
	private bool m_CanCheck;

	protected override void InternalInitialize()
	{
		m_CanCheck = true;
	}

	private void Update()
	{
		if (m_CanCheck && !base.IsComplete && !base.IsDisposed && !GameManager.Instance.IsPaused && GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.Count >= 24)
		{
			m_CanCheck = false;
			SendOnComplete();
		}
	}
}
