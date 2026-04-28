using System;

[Serializable]
public class SectionSoftLoaderGroup
{
	public SectionID ID;

	public EventTrigger Loader;

	public EventTrigger Unloader;

	public event EventHandler OnLoad;

	public event EventHandler OnUnload;

	public void Initialize()
	{
		RemoveListeners();
		Enable();
	}

	private void Enable()
	{
		EnableLoader();
		EnableUnloader();
	}

	private void EnableLoader()
	{
		if (Loader != null)
		{
			Loader.OnEnter -= HandleLoaderOnEnter;
			Loader.OnEnter += HandleLoaderOnEnter;
			Loader.SetActive(active: true);
		}
	}

	private void EnableUnloader()
	{
		if (Unloader != null)
		{
			Unloader.OnEnter -= HandleUnloaderOnEnter;
			Unloader.OnEnter += HandleUnloaderOnEnter;
			Unloader.SetActive(active: true);
		}
	}

	private void HandleLoaderOnEnter(object sender, EventArgs e)
	{
		this.OnLoad.Send(this);
	}

	private void HandleUnloaderOnEnter(object sender, EventArgs e)
	{
		this.OnUnload.Send(this);
	}

	public void RemoveListeners()
	{
		if (Loader != null)
		{
			Loader.OnEnter -= HandleLoaderOnEnter;
		}
		if (Unloader != null)
		{
			Unloader.OnEnter -= HandleUnloaderOnEnter;
		}
	}

	public void Clear()
	{
		this.OnLoad = null;
		this.OnUnload = null;
		RemoveListeners();
	}
}
