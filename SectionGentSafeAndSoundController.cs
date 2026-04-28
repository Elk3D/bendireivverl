using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentSafeAndSoundController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentSafeAndSoundGroup[] m_Group;

	public SectionID SectionID => m_SectionID;

	public GentSafeAndSoundGroup[] Group => m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < m_Group.Length; i++)
		{
			GentSafeAndSoundGroup gentSafeAndSoundGroup = m_Group[i];
			if (gentSafeAndSoundGroup != null)
			{
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards >= 1)
				{
					gentSafeAndSoundGroup.Controller.Content.Enable();
				}
				else
				{
					gentSafeAndSoundGroup.Controller.Content.Disable();
				}
			}
		}
		yield return null;
	}
}
