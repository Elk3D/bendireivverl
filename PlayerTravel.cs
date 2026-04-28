using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using S13Audio.BATDR;
using UnityEngine;

public class PlayerTravel : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Travel Identifier")]
	[SerializeField]
	private TravelID m_TravelID;

	[Header("Travel Section")]
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeReference]
	private TravelID m_TravelToID;

	[SerializeField]
	private List<SectionID> m_ConnectedSections = new List<SectionID>();

	[Header("Interactable")]
	[SerializeField]
	private Interactable m_Interactable;

	[Header("Respawner")]
	[SerializeField]
	private PlayerRespawn m_PlayerRespawn;

	[Header("Emission")]
	[SerializeField]
	private MeshRenderer m_MeshRenderer;

	public TravelID TravelID => m_TravelID;

	public SectionID SectionID => m_SectionID;

	public List<SectionID> ConnectedSections => m_ConnectedSections;

	public PlayerRespawn PlayerRespawn => m_PlayerRespawn;

	public bool HasRequirements { get; private set; }

	public bool IsActive { get; private set; }

	public override void Start()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (!CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += HandleOnObjectiveComplete;
			m_Interactable.OnInteract -= HandleInteractableOnInteract;
			m_Interactable.SetActive(active: false);
		}
	}

	private void Update()
	{
		if ((!HasRequirements || !IsActive) && !GameManager.Instance.IsPaused && !base.IsDisposed)
		{
			if (HasRequirements && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(State.PlayerAbility.FastTravel) && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
			{
				IsActive = true;
				m_Interactable.SetActive(IsActive);
				m_MeshRenderer.material.SetFloat("_Power", 1f);
			}
			else if (IsActive)
			{
				IsActive = false;
				m_Interactable.SetActive(active: false);
				m_MeshRenderer.material.SetFloat("_Power", 0f);
			}
		}
	}

	private void Activate()
	{
		IsActive = GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(State.PlayerAbility.FastTravel) && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active;
		(m_Interactable as InteractableInputDisplay).SetInteractionType(InteractionType.INTERACTION_TRAVEL);
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteract += HandleInteractableOnInteract;
		m_Interactable.SetActive(IsActive);
		if (IsActive)
		{
			m_MeshRenderer.material.SetFloat("_Power", 1f);
		}
		else
		{
			m_MeshRenderer.material.SetFloat("_Power", 0f);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		GameManager.Instance.HideHealthBar();
		GameManager.Instance.HideCrosshair();
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active && GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hide);
		GameManager.Instance.Player.ClearEnemies();
		m_Interactable.ResetAction();
		GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += HandlePlayerOnAnimationComplete;
	}

	private void HandlePlayerOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
		GameManager.Instance.ShowScreenBlocker(0f, 0f, delegate
		{
			GameManager.Instance.Player.transform.position = new Vector3(0f, 2400f, 0f);
			StartCoroutine(LoadSections());
		});
	}

	private IEnumerator LoadSections()
	{
		yield return new WaitForEndOfFrame();
		GameManager.Instance.ShowAsyncLoader();
		if (!GameManager.Instance.SectionManager.Contains(m_SectionID))
		{
			Task sectionTask = GameManager.Instance.SectionManager.LoadSectionAsync(m_SectionID);
			while (!sectionTask.IsCompleted)
			{
				yield return null;
			}
		}
		PlayerTravel playerTravel = GetPlayerTravel(GameManager.Instance.SectionManager.GetSection(m_SectionID));
		foreach (SectionID connectedSection in playerTravel.ConnectedSections)
		{
			Task sectionTask = GameManager.Instance.SectionManager.LoadSectionAsync(connectedSection, initialize: false);
			while (!sectionTask.IsCompleted)
			{
				yield return null;
			}
		}
		GameManager.Instance.ClearAsyncLoader();
		playerTravel.PlayerRespawn.Respawn();
		GameManager.Instance.Player.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.FastTravel);
		yield return new WaitForEndOfFrame();
		Section[] allSections = GameManager.Instance.SectionManager.GetAllSections();
		for (int num = allSections.Length - 1; num >= 0; num--)
		{
			Section section = allSections[num];
			if (section.SectionID != m_SectionID && !playerTravel.ConnectedSections.Contains(section.SectionID))
			{
				GameManager.Instance.SectionManager.Remove(section.SectionID);
			}
		}
		Section componentInParent = GetComponentInParent<Section>(includeInactive: true);
		if (m_SectionID != componentInParent.SectionID && !playerTravel.ConnectedSections.Contains(componentInParent.SectionID))
		{
			GameManager.Instance.SectionManager.Remove(componentInParent.SectionID);
		}
	}

	private PlayerTravel GetPlayerTravel(Section section)
	{
		PlayerTravel playerTravel = null;
		PlayerTravel[] componentsInChildren = section.GetComponentsInChildren<PlayerTravel>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			playerTravel = componentsInChildren[i];
			if (playerTravel.TravelID == m_TravelToID)
			{
				break;
			}
			playerTravel = null;
		}
		return playerTravel;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("PlayerTravel :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag = true;
		if (m_Requirements != null)
		{
			flag = m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
			HasRequirements = true;
			Activate();
		}
		return flag;
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnObjectiveComplete -= HandleOnObjectiveComplete;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= HandlePlayerOnAnimationComplete;
		}
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		base.OnDisposed();
	}
}
