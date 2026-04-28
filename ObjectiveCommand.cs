using System;

[Serializable]
public class ObjectiveCommand : Command<ObjectiveCommandType, Objective>
{
	protected override void InternalExecute()
	{
		m_CommandObject.OnComplete += HandleCommandObjectOnComplete;
		if (m_CommandType != ObjectiveCommandType.None)
		{
			if (m_CommandType == ObjectiveCommandType.Initialize)
			{
				m_CommandObject.Initialize();
			}
			else if (m_CommandType == ObjectiveCommandType.Enable)
			{
				m_CommandObject.Enable();
			}
			else if (m_CommandType == ObjectiveCommandType.Disable)
			{
				m_CommandObject.Disable();
			}
			else if (m_CommandType == ObjectiveCommandType.Inactive)
			{
				m_CommandObject.Inactive();
			}
			else if (m_CommandType == ObjectiveCommandType.ForceComplete)
			{
				m_CommandObject.ForceComplete();
			}
			else
			{
				AbortExecute();
			}
		}
	}

	protected override void InternalForceComplete()
	{
		if (!(m_CommandObject == null))
		{
			m_CommandObject.OnComplete -= HandleCommandObjectOnComplete;
			m_CommandObject.ForceComplete();
		}
	}

	private void HandleCommandObjectOnComplete(object sender, EventArgs e)
	{
		m_CommandObject.OnComplete -= HandleCommandObjectOnComplete;
		SendOnComplete();
	}

	private void AbortExecute()
	{
		JDebug.Log("[Abort Execute] :: " + m_CommandObject.name + " :: " + m_CommandType, m_CommandObject, JDebug.JDebugType.Objectives);
		m_CommandObject.OnComplete -= HandleCommandObjectOnComplete;
		SendOnComplete();
	}
}
