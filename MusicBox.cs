using UnityEngine;

public class MusicBox : JMonoBehaviour
{
	[SerializeField]
	private BoxCollider m_BoxCollider;

	public void Activate()
	{
		Collider[] array = Physics.OverlapBox(m_BoxCollider.bounds.center, m_BoxCollider.bounds.extents, m_BoxCollider.transform.rotation, 1 << LayerMask.NameToLayer("AI"), QueryTriggerInteraction.Ignore);
		if (array == null)
		{
			return;
		}
		foreach (Collider collider in array)
		{
			if (collider.gameObject.activeInHierarchy)
			{
				CharacterDirector component = collider.GetComponent<CharacterDirector>();
				if (component != null)
				{
					component.TriggerSpecialPath();
				}
			}
		}
	}
}
