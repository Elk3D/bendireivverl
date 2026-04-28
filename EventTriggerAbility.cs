using System;
using UnityEngine;

public class EventTriggerAbility : EventTrigger
{
	[Space(10f)]
	[Header("Requirements")]
	[SerializeField]
	private bool m_IsRequirementsComplete;

	[SerializeField]
	private Requirements m_Requirements;

	[Header("Ability Status")]
	[SerializeField]
	private AbilityStatus m_AbilityStatus;

	[Header("Notification")]
	[SerializeField]
	private string m_Label = string.Empty;

	[SerializeField]
	private bool m_IsKey;

	protected override void OnInternalEnter(Collider col)
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == m_AbilityStatus)
		{
			return;
		}
		if (m_IsRequirementsComplete)
		{
			if (CheckStatus())
			{
				SetStatus();
			}
		}
		else if (!CheckStatus())
		{
			SetStatus();
		}
	}

	private void SetStatus()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.SetStatus(m_AbilityStatus);
		if (m_AbilityStatus == AbilityStatus.None)
		{
			return;
		}
		CameraEffects.GainPower(0.75f);
		CameraEffects.ShakeRotation(2f, 1f);
		if (m_AbilityStatus == AbilityStatus.Active)
		{
			GameManager.Instance.Player.UnlockAbilities(playAudio: true);
			if (!GameManager.Instance.Player.HasTeleport())
			{
				return;
			}
			if (GameManager.Instance.Player.Abilities != null)
			{
				foreach (PlayerAbilityState ability in GameManager.Instance.Player.Abilities)
				{
					if (ability != null && ability is PlayerAbilityStateFlow)
					{
						ability.ForceCooldown();
						break;
					}
				}
			}
			GameManager.Instance.DisplayTeleport();
		}
		else
		{
			if (m_AbilityStatus != AbilityStatus.Inactive)
			{
				return;
			}
			GameManager.Instance.Player.LockAbilities(playAudio: true);
			if (GameManager.Instance.Player.HasTeleport())
			{
				GameManager.Instance.HideTeleport();
			}
			if (m_Label != string.Empty)
			{
				if (m_IsKey)
				{
					GameManager.Instance.ShowNotificationText(TextUtility.GetKey(m_Label).GetColorNotification().GetWhite()
						.ToUpper());
				}
				else
				{
					GameManager.Instance.ShowNotificationText(m_Label);
				}
			}
		}
	}

	private bool CheckStatus()
	{
		bool result = true;
		if (m_Requirements != null)
		{
			result = m_Requirements.IsComplete();
		}
		return result;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		CheckStatus();
	}
}
