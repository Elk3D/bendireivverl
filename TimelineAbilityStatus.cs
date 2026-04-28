public class TimelineAbilityStatus : JMonoBehaviour
{
	public void Enable()
	{
		SetAbilityStatus(AbilityStatus.Active);
	}

	public void Disable()
	{
		SetAbilityStatus(AbilityStatus.Inactive);
	}

	private void SetAbilityStatus(AbilityStatus abilityStatus)
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.SetStatus(abilityStatus);
		CameraEffects.GainPower(0.75f);
		switch (abilityStatus)
		{
		case AbilityStatus.Active:
			GameManager.Instance.Player.UnlockAbilities();
			if (!GameManager.Instance.Player.HasTeleport())
			{
				break;
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
			break;
		case AbilityStatus.Inactive:
			GameManager.Instance.Player.LockAbilities();
			if (GameManager.Instance.Player.HasTeleport())
			{
				GameManager.Instance.HideTeleport();
			}
			break;
		}
	}
}
