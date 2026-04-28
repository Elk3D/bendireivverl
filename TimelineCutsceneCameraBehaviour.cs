using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineCutsceneCameraBehaviour : PlayableBehaviour
{
	public TimelineCutsceneCameraType CutsceneType;

	public Transform Target;

	public Transform StartLocation;

	public Transform EndLocation;

	public bool UnlockWeapons = true;

	public bool EndCrouching;

	public bool IsReal;

	public bool IsSnap;

	private bool m_IsPlayed;

	private GameObject m_Owner;

	private CutsceneDirector m_CutsceneDirector;

	public void SetOwner(GameObject owner)
	{
		m_Owner = owner;
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !m_IsPlayed)
		{
			m_IsPlayed = true;
			if (CutsceneType == TimelineCutsceneCameraType.Enter)
			{
				Enter();
			}
			else if (CutsceneType == TimelineCutsceneCameraType.Exit)
			{
				Exit();
			}
		}
	}

	private void Enter()
	{
		if (GameManager.Instance.BeastBendy == null)
		{
			GameManager.Instance.HideHealthBar();
			GameManager.Instance.HideSprintBar();
			GameManager.Instance.HideCrosshair();
			if (GameManager.Instance.Player.HasTeleport())
			{
				GameManager.Instance.HideTeleport();
			}
			GameManager.Instance.Player.SetCollision(active: false);
			GameManager.Instance.Player.SetState(State.Player.Cutscene);
			GameManager.Instance.Player.SetAllTrackers(active: false);
			GameManager.Instance.Player.HideFirstPersonArms();
			GameManager.Instance.Player.ForceEnemiesFlee();
			GameManager.Instance.GameCamera.Camera.DOFieldOfView(55f, 0.15f);
			if (StartLocation != null)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(StartLocation.position, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.OnComplete(EnterGameCamera);
			}
			else
			{
				EnterGameCamera();
			}
		}
		else if (StartLocation != null)
		{
			Sequence sequence2 = DOTween.Sequence();
			sequence2.Insert(0f, GameManager.Instance.BeastBendy.transform.DOMove(StartLocation.position, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			sequence2.Insert(0f, GameManager.Instance.BeastBendy.transform.DORotate(StartLocation.eulerAngles, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			sequence2.OnComplete(EnterGameCamera);
		}
		else
		{
			EnterGameCamera();
		}
	}

	private void EnterGameCamera()
	{
		Transform transform = GameManager.Instance.GameCamera.InitializeFreeRoamCam(Target);
		transform.DOKill();
		if (!IsSnap)
		{
			transform.DOLocalMove(Vector3.zero, 0.75f).SetEase(Ease.InOutSine);
			transform.DOLocalRotate(Vector3.zero, 0.75f).SetEase(Ease.InOutSine);
		}
		else
		{
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
		}
		if (GameManager.Instance.BeastBendy == null)
		{
			GameManager.Instance.Player.gameObject.SetActive(value: false);
			return;
		}
		GameManager.Instance.BeastBendy.Disable();
		GameManager.Instance.BeastBendy.gameObject.SetActive(value: false);
	}

	private void Exit()
	{
		if (GameManager.Instance.BeastBendy == null)
		{
			if (GameManager.Instance.GameCamera.FreeRoamCam == null)
			{
				return;
			}
			SendToEndLocation();
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			if (m_Owner != null)
			{
				m_CutsceneDirector = m_Owner.GetComponent<CutsceneDirector>();
				if (m_CutsceneDirector != null && m_CutsceneDirector.IsSkipped)
				{
					GameManager.Instance.Player.HeadContainer.localPosition = Vector3.zero;
					GameManager.Instance.Player.HeadContainer.localEulerAngles = Vector3.zero;
				}
			}
			GameManager.Instance.Player.ForceResetAnimation();
			GameManager.Instance.Player.ResetRotation();
			if (EndCrouching)
			{
				GameManager.Instance.Player.ForceCrouch();
			}
			else
			{
				GameManager.Instance.Player.ForceStand();
			}
			GameManager.Instance.Player.SetAllTrackers(active: true);
			if (GameManager.Instance.Player.Health < UpgradeCheck.GetHealth())
			{
				GameManager.Instance.ShowHealthBar(GameManager.Instance.Player.Health, UpgradeCheck.GetHealth());
			}
			if (UnlockWeapons)
			{
				GameManager.Instance.Player.ShowFirstPersonArms();
				GameManager.Instance.ShowCrosshair(IsReal);
			}
			GameManager.Instance.Player.SetHeadContainerSlerp(active: true);
			GameManager.Instance.Player.SetState(State.Player.Default);
			GameManager.Instance.Player.SetCollision(active: true);
			if (!GameManager.Instance.Player.HasTeleport() || GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus != AbilityStatus.Active)
			{
				return;
			}
			if (GameManager.Instance.Player.Abilities != null && m_CutsceneDirector != null && m_CutsceneDirector.PlayableDirector != null)
			{
				foreach (PlayerAbilityState ability in GameManager.Instance.Player.Abilities)
				{
					if (ability != null && ability is PlayerAbilityStateFlow && ability.IsCooldown)
					{
						ability.ForceSetCooldown(ability._Cooldown + (float)m_CutsceneDirector.PlayableDirector.duration);
					}
				}
			}
			GameManager.Instance.DisplayTeleport();
			GameManager.Instance.Player.UnlockAbilities();
		}
		else
		{
			GameManager.Instance.BeastBendy.gameObject.SetActive(value: true);
			GameManager.Instance.BeastBendy.Enable();
		}
	}

	private void SendToEndLocation(bool ignoreOffset = false)
	{
		if (!(EndLocation == null))
		{
			GameManager.Instance.Player.SetHeadContainerSlerp(active: false);
			GameManager.Instance.Player.ResetRotation();
			Vector3 vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			Vector3 vector2 = (ignoreOffset ? Vector3.zero : (EndLocation.forward * 0.5368653f));
			Vector3 position = EndLocation.position - vector2 + vector;
			GameManager.Instance.Player.transform.position = position;
			GameManager.Instance.Player.transform.eulerAngles = EndLocation.eulerAngles;
		}
	}
}
