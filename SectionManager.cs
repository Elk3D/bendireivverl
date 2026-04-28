using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectionManager : JDisposable
{
	private Dictionary<SectionID, Section> m_Sections = new Dictionary<SectionID, Section>();

	private HashSet<SectionID> m_SectionsLoading = new HashSet<SectionID>();

	public event EventHandler OnLoaded;

	public static SectionManager Create()
	{
		return new SectionManager();
	}

	public Section GetSection(SectionID sectionID)
	{
		Section result = null;
		if (Contains(sectionID))
		{
			result = m_Sections[sectionID];
		}
		return result;
	}

	public Section[] GetAllSections()
	{
		List<Section> list = new List<Section>();
		foreach (KeyValuePair<SectionID, Section> section in m_Sections)
		{
			if (!list.Contains(section.Value))
			{
				list.Add(section.Value);
			}
		}
		return list.ToArray();
	}

	public bool Contains(SectionID sectionID)
	{
		return m_Sections.ContainsKey(sectionID);
	}

	public void Add(Section section)
	{
		if (!Contains(section.SectionID))
		{
			m_Sections.Add(section.SectionID, section);
		}
	}

	public void Remove(SectionID sectionID)
	{
		if (Contains(sectionID))
		{
			m_Sections.Remove(sectionID);
			SceneManager.UnloadSceneAsync(sectionID.ToString());
		}
	}

	public void InitializeSection(SectionID sectionID)
	{
		if (Contains(sectionID))
		{
			Section section = GetSection(sectionID);
			if (section != null)
			{
				if (!section.IsInitialized)
				{
					section.Initialize();
				}
				else
				{
					section.SetActive(active: true);
				}
				GameManager.Instance.InkDemonManager?.CheckAvailability();
			}
		}
		else if (!m_SectionsLoading.Contains(sectionID))
		{
			LoadSectionAsync(sectionID);
		}
		else
		{
			OnLoaded -= HandleLoadSectionAsyncOnLoaded;
			OnLoaded += HandleLoadSectionAsyncOnLoaded;
		}
	}

	private void HandleLoadSectionAsyncOnLoaded(object sender, EventArgs e)
	{
		OnLoaded -= HandleLoadSectionAsyncOnLoaded;
		Section section = sender as Section;
		if (section != null)
		{
			if (!section.IsInitialized)
			{
				section.Initialize();
			}
			else
			{
				section.SetActive(active: true);
			}
			GameManager.Instance.InkDemonManager?.CheckAvailability();
		}
	}

	public async Task LoadSectionAsync(SectionID sectionID, bool initialize = true)
	{
		if (!Contains(sectionID) && !m_SectionsLoading.Contains(sectionID))
		{
			m_SectionsLoading.Add(sectionID);
			SectionDataObject sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(sectionID);
			if (sectionDataObject == null)
			{
				GameManager.Instance.GameData.CurrentSave.Save(sectionID, DataObject<SectionID, SectionDataObject>.Create(sectionID));
			}
			else
			{
				Debug.Log("SectionDataObject Found: " + sectionDataObject);
			}
			AsyncOperation sectionLoadOp = SceneManager.LoadSceneAsync(sectionID.ToString(), LoadSceneMode.Additive);
			sectionLoadOp.allowSceneActivation = true;
			while (!sectionLoadOp.isDone)
			{
				await Task.Yield();
			}
			Section section = null;
			GameObject[] rootGameObjects = SceneManager.GetSceneByName(sectionID.ToString()).GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				if (!(gameObject.name == sectionID.ToString()))
				{
					continue;
				}
				section = gameObject.GetComponent<Section>();
				section.Load(sectionDataObject);
				if (initialize)
				{
					section.Initialize();
					while (!section.IsReady)
					{
						await Task.Yield();
					}
				}
				GameManager.Instance.SectionManager.Add(section);
				break;
			}
			if (section != null)
			{
				this.OnLoaded.Send(section);
			}
			else
			{
				Debug.LogError("There was an error loading section: " + sectionID);
			}
			m_SectionsLoading.Remove(sectionID);
		}
		else
		{
			if (!Contains(sectionID) || !initialize)
			{
				return;
			}
			Section section = m_Sections[sectionID];
			if (!section.IsInitialized)
			{
				section.Initialize();
				while (!section.IsReady)
				{
					await Task.Yield();
				}
			}
			else
			{
				section.SetActive(active: true);
			}
		}
	}

	public async void LoadSectionsAsync(SectionID[] sectionIDs, SectionID[] initializeSectionIDs, Action callback)
	{
		await Task.WhenAll(sectionIDs.Select((SectionID i) => LoadSectionAsync(i, initialize: false)));
		await Task.WhenAll(initializeSectionIDs.Select((SectionID i) => LoadSectionAsync(i)));
		callback?.Invoke();
	}

	public void Clear()
	{
		if (m_Sections == null)
		{
			return;
		}
		foreach (KeyValuePair<SectionID, Section> section in m_Sections)
		{
			if (section.Value != null)
			{
				SceneManager.UnloadSceneAsync(section.Key.ToString());
			}
		}
		m_Sections.Clear();
	}

	public void Kill()
	{
		Clear();
		m_Sections = null;
	}

	protected override void OnDisposed()
	{
		Kill();
		this.OnLoaded = null;
		base.OnDisposed();
	}
}
