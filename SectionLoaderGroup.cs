using System;

[Serializable]
public class SectionLoaderGroup
{
	public SectionID ID;

	public EventTrigger Loader;

	public EventTrigger Unloader;

	public Requirements Requirements;

	public event EventHandler OnLoad;

	public event EventHandler OnUnload;

	public void Initialize()
	{
		RemoveListeners();
		if (!CheckStatus())
		{
			if (Loader != null)
			{
				Loader.SetActive(active: false);
			}
			if (Unloader != null)
			{
				Unloader.SetActive(active: false);
			}
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
		}
		else
		{
			Enable();
		}
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

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SectionLoaderGroup :: HandleOnObjectiveComplete", null, JDebug.JDebugType.Objectives);
		if (CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			RemoveListeners();
			Enable();
		}
	}

	private bool CheckStatus()
	{
		bool result = true;
		if (Requirements != null)
		{
			result = Requirements.IsComplete();
		}
		return result;
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
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
	}

	public void Clear()
	{
		this.OnLoad = null;
		this.OnUnload = null;
		RemoveListeners();
	}
}
