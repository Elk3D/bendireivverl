using System;
using System.Collections;
using S13Audio.BATDR;
using UnityEngine;

public class SeasonalTrickOrTreatDoor : JMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	public event EventHandler OnTrick;

	public event EventHandler OnTreat;

	public event EventHandler OnKnock;

	public override void Start()
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		m_Interactable.OnInteract += HandleInteractableOnInteract;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		this.OnKnock.Send(this);
		if (SeasonalCheck.HasHat)
		{
			StartCoroutine(DoTreat());
			this.OnTreat.Send(this);
		}
		else
		{
			StartCoroutine(DoTrick());
			this.OnTrick.Send(this);
		}
	}

	private IEnumerator DoTrick()
	{
		yield return new WaitForSeconds(3f);
		if (GameManager.Instance.Player.Health > 1f)
		{
			GameManager.Instance.Player.Damage(1);
			GameManager.Instance.Player.ShowHealthBar();
		}
		CameraEffects.Damage();
		CameraEffects.ShakeRotation(0.5f, 2f, 10, 90f, fadeOut: false);
		GameManager.Instance.Player.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Hit);
		SeasonalHalloweenAchievement.Trick();
	}

	private IEnumerator DoTreat()
	{
		yield return new WaitForSeconds(3f);
		string[] array = new string[3] { "NOTIFICATION_LOOT_BENDY_BAR", "NOTIFICATION_LOOT_CHIPS", "NOTIFICATION_LOOT_NUTS" };
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_EAT") + " " + TextUtility.GetKey(array[UnityEngine.Random.Range(0, array.Length)]), "Icon/Collectables/Small/UIIcon_Food", 1);
		GameManager.Instance.Player.Heal(1);
		int amount = UnityEngine.Random.Range(1, 3);
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS"), "Icon/Collectables/Small/UIIcon_Slug", amount, 0.15f);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddSlugs(amount);
		SeasonalHalloweenAchievement.Treat();
	}

	protected override void OnDisposed()
	{
		m_Interactable.OnInteract -= HandleInteractableOnInteract;
		base.OnDisposed();
	}
}
