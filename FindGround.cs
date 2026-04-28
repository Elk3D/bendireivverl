using UnityEngine;

public class FindGround : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	private void Update()
	{
		if (!(m_Content == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Vector3 vector = base.transform.position + Vector3.up * 5f;
			Vector3 end = vector + Vector3.down * 10f;
			if (Physics.Linecast(vector, end, out var hitInfo, ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("InvisibleCollider"))), QueryTriggerInteraction.Ignore))
			{
				Vector3 point = hitInfo.point;
				point += Vector3.up * 0.01f;
				m_Content.position = point;
			}
		}
	}
}
