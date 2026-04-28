using UnityEngine;

public class SetScale : JComponent
{
	[SerializeField]
	private Vector3 m_WorldScale = Vector3.one;

	public override void Start()
	{
		Transform parent = base.transform.parent;
		base.transform.SetParent(null);
		base.transform.localScale = m_WorldScale;
		base.transform.SetParent(parent);
	}
}
