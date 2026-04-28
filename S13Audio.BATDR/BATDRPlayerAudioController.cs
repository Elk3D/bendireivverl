using System;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRPlayerAudioController : MonoBehaviour
{
	public enum FloorMaterials
	{
		NONE = -1,
		Wood = 0,
		Tile = 1,
		Stone = 2,
		Pipe = 3,
		Metal = 4,
		Carpet = 6,
		Vent = 9,
		Body = 10,
		InkPuddle = 7,
		InkShallow = 5,
		InkDeep = 8
	}

	[Serializable]
	private class MatEventPairList : ReorderableArray<MatEventPair>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder(forceExpand = true)]
	private class MatEventPair : IS13HorizontalPropertyHolder
	{
		public FloorMaterials mat;

		public S13ScriptableEvent evt;
	}

	public enum GentPipeImpactType
	{
		Generic,
		Plaster,
		Metal,
		Tile,
		Wood,
		InkSolid,
		InkLiquid,
		Cutout
	}

	[Serializable]
	private class ImpactActionList : ReorderableArray<ImpactActionPair>
	{
	}

	[Serializable]
	private class ImpactActionPair
	{
		public GentPipeImpactType impactType;

		public S13LocalAction action;
	}

	public enum PlayerReaction
	{
		NONE,
		Respawn,
		Dead,
		Hit,
		Land,
		Load,
		FastTravel
	}

	[Serializable]
	private class ReactionList : ReorderableArray<ReactionEntry>
	{
	}

	[Serializable]
	private class ReactionEntry
	{
		public PlayerReaction reaction;

		public S13LocalAction action;
	}

	[Header("Floor Materials")]
	[Reorderable]
	[SerializeField]
	private MatEventPairList matEvents;

	[SerializeField]
	[ReadOnly]
	private FloorMaterials currentFloorMaterial = FloorMaterials.NONE;

	[Header("Wetness")]
	[SerializeField]
	[S13LockableField]
	private string wetAccessorID = string.Empty;

	[SerializeField]
	private float secondsUntilDry = 1f;

	[ReadOnly]
	[SerializeField]
	private float currentFootWetness;

	[Header("Audrey Reactions")]
	[Reorderable(elementNameProperty = "reaction")]
	[SerializeField]
	private ReactionList reactionList;

	[Header("Gent Pipe Impacts")]
	[SerializeField]
	[Reorderable(elementNameProperty = "impactType")]
	private ImpactActionList impactActionList;

	[Header("Cart Move")]
	[SerializeField]
	private S13LocalAction onEnterCartMove;

	[SerializeField]
	private S13LocalAction onExitCartMove;

	[Header("Climb")]
	[SerializeField]
	private S13LocalAction onEnterClimb;

	[SerializeField]
	private float onEnterClimbDelay;

	[SerializeField]
	private S13LocalAction onExitClimb;

	[SerializeField]
	private float onExitClimbDelay;

	[Header("Hide")]
	[SerializeField]
	private S13LocalAction onEnterHide;

	[SerializeField]
	private S13LocalAction onExitHide;

	[Header("Crouch")]
	[SerializeField]
	private S13LocalAction onEnterCrouch;

	[SerializeField]
	private S13LocalAction onExitCrouch;

	[Header("Peek")]
	[SerializeField]
	private S13LocalAction onEnterPeek;

	[SerializeField]
	private S13LocalAction onExitPeek;

	private FloorMaterials _lastFloorMaterial;

	public void React(PlayerReaction reaction)
	{
		if (reaction != PlayerReaction.NONE)
		{
			ReactionEntry reactionEntry = reactionList.FirstOrDefault((ReactionEntry x) => x.reaction == reaction);
			if (reactionEntry != null && reactionEntry.action.IsExecutable)
			{
				S13Debug.Log($"Player received reaction {reaction}", this);
				reactionEntry.action.Execute();
			}
		}
	}

	public void SetFloorMaterial(FloorMaterials newFloorMaterial)
	{
		if (newFloorMaterial == FloorMaterials.InkPuddle)
		{
			StepInInkPuddle();
			return;
		}
		if (newFloorMaterial == FloorMaterials.Body)
		{
			StepInInkPuddle();
		}
		if (currentFloorMaterial != newFloorMaterial)
		{
			_lastFloorMaterial = currentFloorMaterial;
			currentFloorMaterial = newFloorMaterial;
			if (IsInkMaterial(_lastFloorMaterial) && !IsInkMaterial(currentFloorMaterial))
			{
				StepInInkPuddle();
			}
			matEvents.FirstOrDefault((MatEventPair x) => x.mat == newFloorMaterial)?.evt.Raise();
		}
	}

	public void StepInInkPuddle()
	{
		currentFootWetness = 1f;
	}

	public void GentPipeImpact(GentPipeImpactType impactType)
	{
		ImpactActionPair impactActionPair = impactActionList.FirstOrDefault((ImpactActionPair x) => x.impactType == impactType);
		if (impactActionPair != null && impactActionPair.action.IsExecutable)
		{
			impactActionPair.action.Execute();
		}
	}

	public void SetHiding(bool isHiding)
	{
		if (isHiding)
		{
			if (onEnterHide.IsExecutable)
			{
				onEnterHide.Execute();
			}
		}
		else if (onExitHide.IsExecutable)
		{
			onExitHide.Execute();
		}
	}

	public void SetCrouching(bool isCrouching)
	{
		if (isCrouching)
		{
			if (onEnterCrouch.IsExecutable)
			{
				onEnterCrouch.Execute();
			}
		}
		else if (onExitCrouch.IsExecutable)
		{
			onExitCrouch.Execute();
		}
	}

	public void SetPeeking(bool isPeeking)
	{
		if (isPeeking)
		{
			if (onEnterPeek.IsExecutable)
			{
				onEnterPeek.Execute();
			}
		}
		else if (onExitPeek.IsExecutable)
		{
			onExitPeek.Execute();
		}
	}

	public void SetClimbing(bool isClimbing)
	{
		if (isClimbing)
		{
			if (onEnterClimb.IsExecutable)
			{
				if (onEnterClimbDelay > 0f)
				{
					StartCoroutine(S13CoroutineUtil.WaitForDuration(onEnterClimbDelay, ignoreTimeScale: false, onEnterClimb.Execute));
				}
				else
				{
					onEnterClimb.Execute();
				}
			}
		}
		else if (onExitClimb.IsExecutable)
		{
			if (onExitClimbDelay > 0f)
			{
				StartCoroutine(S13CoroutineUtil.WaitForDuration(onExitClimbDelay, ignoreTimeScale: false, onExitClimb.Execute));
			}
			else
			{
				onExitClimb.Execute();
			}
		}
	}

	public void SetCartMove(bool isMoving)
	{
		if (isMoving)
		{
			if (onEnterCartMove.IsExecutable)
			{
				onEnterCartMove.Execute();
			}
		}
		else if (onExitCartMove.IsExecutable)
		{
			onExitCartMove.Execute();
		}
	}

	private void FixedUpdate()
	{
		if (currentFootWetness > 0f)
		{
			currentFootWetness = Mathf.Max(0f, currentFootWetness - Time.fixedDeltaTime * (1f / secondsUntilDry));
			if (!string.IsNullOrEmpty(wetAccessorID))
			{
				S13Manager.SetAccessor(wetAccessorID, currentFootWetness);
			}
		}
	}

	private bool IsInkMaterial(FloorMaterials material)
	{
		if (material != FloorMaterials.InkShallow && material != FloorMaterials.InkDeep)
		{
			return material == FloorMaterials.Body;
		}
		return true;
	}
}
