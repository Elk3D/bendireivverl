using System;
using UnityEngine;

public abstract class UIController : JMonoBehaviour
{
	[SerializeField]
	protected RectTransform m_Visuals;

	private RectTransform m_RectTransform;

	public RectTransform rectTransform
	{
		get
		{
			if (!(m_RectTransform == null))
			{
				return m_RectTransform;
			}
			return m_RectTransform = GetComponent<RectTransform>();
		}
	}

	public event EventHandler OnPlayInComplete;

	public event EventHandler OnPlayOutComplete;

	public void Initialize(object _data)
	{
		Canvas component = GetComponent<Canvas>();
		Camera camera = (component.worldCamera = GameManager.Instance.UIManager.Camera);
		component.pixelPerfect = false;
		component.planeDistance = Math.Abs(component.transform.parent.position.z - camera.transform.position.z);
		component.sortingOrder = (int)(0f - component.planeDistance);
		OnInitialized(_data);
	}

	protected virtual void OnInitialized(object _data)
	{
	}

	public virtual void PlayIn()
	{
		PlayInComplete();
	}

	public virtual void PlayInComplete()
	{
		this.OnPlayInComplete.Send(this);
	}

	public virtual void PlayOut()
	{
		PlayOutComplete();
	}

	public virtual void PlayOutComplete()
	{
		this.OnPlayOutComplete.Send(this);
		Dispose();
	}

	protected override void OnDisposed()
	{
		this.OnPlayInComplete = null;
		this.OnPlayOutComplete = null;
		m_RectTransform = null;
		base.OnDisposed();
	}
}
