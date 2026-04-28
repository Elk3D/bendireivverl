using UnityEngine;

public class TourBox : ActionEventController<TourBoxContent, TourBoxData>
{
	[SerializeField]
	private AudioClip m_AudioClip;

	protected override void OnInitialized()
	{
		(m_Content as TourBoxContent).SetAudioClip(m_AudioClip);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.InteractableTransform.position, "TourBox Icon", allowScaling: true);
	}
}
