using UnityEngine;

public class BoundObject : MonoBehaviour
{
	[SerializeField]
	private bool _includeLightsInBounds = true;

	[HideInInspector]
	public Bounds bounds;

	public bool includeLightsInBounds => _includeLightsInBounds;

	private void OnValidate()
	{
		bounds.center = base.transform.position;
		bounds.size = base.transform.localScale;
	}

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying)
		{
			Gizmos.color = (includeLightsInBounds ? Color.yellow : Color.cyan);
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}
}
