using System;
using S13Audio.BATDR;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWeaponGentPipe : PlayerWeaponState
{
	private bool m_IsCharged;

	private bool m_IsOvercharged;

	private static Shader FPSToonShader = Shader.Find("JDS/FPS Toon Character Shader");

	private static Shader FPSParticleShader = Shader.Find("JDS/Particles/FPS Standard Unlit");

	private Renderer[] renderers;

	private Renderer[] particleRenderers;

	public override bool IsCharged => m_IsCharged;

	public override bool IsOvercharged => m_IsOvercharged;

	public PlayerWeaponGentPipe(Player player, WeaponData data)
		: base(player, data)
	{
	}

	public override void OnInitialize()
	{
		if (m_Weapon == null)
		{
			m_Weapon = GameManager.Instance.AssetManager.CreateAsset<Weapon>("Weapons/Weapon_GentPipe", base.Data);
			m_Weapon.SetParentAndCenter(base.Actor.WeaponParent);
		}
		else
		{
			m_Weapon.gameObject.SetActive(value: true);
		}
		Renderer[] componentsInChildren = m_Weapon.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		renderers = componentsInChildren;
		componentsInChildren = m_Weapon.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
		particleRenderers = componentsInChildren;
		SetActive(active: true);
	}

	public void SwapShaderToFirstPerson()
	{
		Renderer[] array = renderers;
		foreach (Renderer obj in array)
		{
			obj.material.shader = FPSToonShader;
			obj.shadowCastingMode = ShadowCastingMode.Off;
		}
		array = particleRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			Material[] materials = array[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].shader = FPSParticleShader;
			}
		}
	}

	public void SetFirstPersonFlag(bool active)
	{
		Renderer[] array;
		if (active)
		{
			array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material.EnableKeyword("FIRST_PERSON");
			}
			array = particleRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				Material[] materials = array[i].materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j].EnableKeyword("FIRST_PERSON");
				}
			}
			return;
		}
		array = renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.DisableKeyword("FIRST_PERSON");
		}
		array = particleRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			Material[] materials = array[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].DisableKeyword("FIRST_PERSON");
			}
		}
	}

	protected override bool InternalCheckOvercharge()
	{
		if (m_IsOvercharged || !m_IsCharged || base.Data.WeaponType != WeaponType.LEVEL_4)
		{
			return false;
		}
		bool result = false;
		GentPipePowerIndicator componentInChildren = m_Weapon.GetComponentInChildren<GentPipePowerIndicator>();
		if (componentInChildren != null && componentInChildren.Power >= 1)
		{
			result = true;
		}
		return result;
	}

	protected override bool InternalOvercharge()
	{
		GentPipePowerIndicator componentInChildren = base.Actor.WeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true);
		if (componentInChildren != null)
		{
			SetPowerIndicator(componentInChildren, base.Actor.WeaponParent, hasListeners: true, 1);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(componentInChildren.Power);
		}
		SetPowerIndicator(base.Actor.BodyWeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true), base.Actor.BodyWeaponParent, hasListeners: false, 1);
		m_IsOvercharged = true;
		OverchargeShake();
		return true;
	}

	protected override bool InternalCheckCharge()
	{
		if (m_IsCharged || base.Data.WeaponType == WeaponType.NONE || base.Data.WeaponType == WeaponType.LEVEL_1 || base.Data.WeaponType == WeaponType.LEVEL_2)
		{
			return false;
		}
		bool result = false;
		GentPipePowerIndicator componentInChildren = m_Weapon.GetComponentInChildren<GentPipePowerIndicator>();
		if (componentInChildren != null && componentInChildren.Power >= 2)
		{
			result = true;
		}
		return result;
	}

	protected override bool InternalCharge()
	{
		GentPipePowerIndicator componentInChildren = base.Actor.WeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true);
		if (componentInChildren != null)
		{
			SetPowerIndicator(componentInChildren, base.Actor.WeaponParent, hasListeners: true, 2);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(componentInChildren.Power);
		}
		SetPowerIndicator(base.Actor.ActiveWeaponDuplicate.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true), base.Actor.ActiveWeaponDuplicate.transform, hasListeners: false, 2);
		SetPowerIndicator(base.Actor.ActiveWeaponMirrorDuplicate.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true), base.Actor.ActiveWeaponMirrorDuplicate.transform, hasListeners: false, 2);
		m_IsCharged = true;
		ChargeShake();
		return true;
	}

	private void SetPowerIndicator(GentPipePowerIndicator powerIndicator, Transform weaponParent, bool hasListeners, int powerAmount)
	{
		if (!(powerIndicator != null))
		{
			return;
		}
		int power = powerIndicator.Power;
		power -= powerAmount;
		if (power <= 0)
		{
			power = 0;
		}
		powerIndicator.SetPowerLevel(power);
		GentPipeChargeEffects componentInChildren = weaponParent.GetComponentInChildren<GentPipeChargeEffects>();
		if (componentInChildren != null)
		{
			if (hasListeners)
			{
				componentInChildren.OnEnd -= HandleChargedEffectsOnEnd;
				componentInChildren.OnEnd += HandleChargedEffectsOnEnd;
			}
			componentInChildren.Enable();
		}
	}

	private void HandleChargedEffectsOnEnd(object sender, EventArgs e)
	{
		(sender as GentPipeChargeEffects).OnEnd -= HandleChargedEffectsOnEnd;
		m_IsCharged = false;
		m_IsOvercharged = false;
	}

	protected override bool InternalCheckUse()
	{
		return true;
	}

	protected override bool InternalUse()
	{
		return true;
	}

	protected override void InternalOnAttack()
	{
		Transform transform = GameManager.Instance.GameCamera.transform;
		Vector3 origin = transform.position - transform.forward;
		if (Physics.SphereCast(origin, GetAttackRadius(), transform.forward, out var hitInfo, 9f, LayerMaskUtility.GetWeaponAttack, QueryTriggerInteraction.Ignore))
		{
			bool flag = false;
			IHittable componentInParent = hitInfo.transform.gameObject.GetComponentInParent<IHittable>();
			if (componentInParent == null)
			{
				Character component = hitInfo.transform.gameObject.GetComponent<Character>();
				int num = 0;
				if (component != null)
				{
					HitShake();
					component.OnHit(hitInfo);
					Collider[] array = Physics.OverlapSphere(hitInfo.point, 2f, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore);
					for (int i = 0; i < array.Length; i++)
					{
						if (num > 2)
						{
							break;
						}
						Collider collider = array[i];
						if (collider.gameObject.layer == LayerMask.NameToLayer("AI") || collider.gameObject.layer == LayerMask.NameToLayer("IgnoreAI"))
						{
							Character component2 = collider.gameObject.GetComponent<Character>();
							if (component2 != null && component2 != component)
							{
								component2.OnHit(hitInfo);
								num++;
							}
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			else if (componentInParent.IsPlayerBreakable && !componentInParent.IsBroken)
			{
				HitShake();
				componentInParent.Hit(hitInfo);
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				if (Physics.SphereCast(origin, 0.1f, transform.forward, out var hitInfo2, 9f, LayerMaskUtility.GetWeaponAttack, QueryTriggerInteraction.Ignore))
				{
					GetHitDecal(hitInfo2);
				}
				else
				{
					MissShake();
				}
			}
		}
		else
		{
			MissShake();
		}
	}

	private float GetAttackRadius()
	{
		float result = 0.5f;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 0.75f;
			break;
		case DifficultyLevel.Hard:
			result = 0.25f;
			break;
		case DifficultyLevel.Impossible:
			result = 0.1f;
			break;
		}
		return result;
	}

	private void MissShake()
	{
		CameraEffects.ShakeRotation(0.2f, 2f, 5, 90f, fadeOut: false);
	}

	private void HitShake()
	{
		CameraEffects.ShakeRotation(0.2f, 4f, 10, 90f, fadeOut: false);
	}

	private void ChargeShake()
	{
		CameraEffects.ShakeRotation(0.5f, 5f, 15);
	}

	private void OverchargeShake()
	{
		CameraEffects.ShakeRotation(0.75f, 10f, 20);
	}

	private void GetHitDecal(RaycastHit hit)
	{
		string text = "Impacts/Impact_Generic";
		FloorMaterial floor;
		if (CheckImpactMaterial(hit, out var impact))
		{
			text = GetImpact(impact);
		}
		else if (CheckFloorMaterial(hit, out floor))
		{
			text = GetFloor(floor);
		}
		if (text != string.Empty)
		{
			GameManager.Instance.PoolingManager.GetFromPool(text, 4f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		}
		HitShake();
		if (IsCharged)
		{
			text = "Impacts/Impact_Electric";
			GentPipeChargeEffects componentInChildren = m_Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				componentInChildren.Hit();
			}
			GameManager.Instance.PoolingManager.GetFromPool(text, 4f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		}
	}

	private bool CheckImpactMaterial(RaycastHit hit, out ImpactMaterial impact)
	{
		bool result = true;
		impact = hit.transform.gameObject.GetComponent<ImpactMaterial>();
		if (impact == null)
		{
			result = false;
		}
		return result;
	}

	private string GetImpact(ImpactMaterial impactMaterial)
	{
		string result = "Impacts/Impact_Generic";
		if (impactMaterial.ImpactType == ImpactType.Dirt)
		{
			result = "Impacts/Impact_Dirt";
		}
		else if (impactMaterial.ImpactType == ImpactType.Paper)
		{
			result = "Impacts/Impact_Paper";
		}
		else if (impactMaterial.ImpactType == ImpactType.Brick)
		{
			result = "Impacts/Impact_Brick";
		}
		else if (impactMaterial.ImpactType == ImpactType.Fabric)
		{
			result = "Impacts/Impact_Fabric";
		}
		else if (impactMaterial.ImpactType == ImpactType.Metal)
		{
			result = "Impacts/Impact_Metal";
		}
		else if (impactMaterial.ImpactType == ImpactType.Spark)
		{
			result = "Impacts/Impact_Sparks";
		}
		else if (impactMaterial.ImpactType == ImpactType.Stone)
		{
			result = "Impacts/Impact_Stone";
		}
		else if (impactMaterial.ImpactType == ImpactType.Tile)
		{
			result = "Impacts/Impact_Tile";
		}
		else if (impactMaterial.ImpactType == ImpactType.Wood)
		{
			result = "Impacts/Impact_Wood";
		}
		else if (impactMaterial.ImpactType == ImpactType.Squeak)
		{
			result = "Impacts/Impact_Squeak";
		}
		else if (impactMaterial.ImpactType == ImpactType.Generic)
		{
			result = "Impacts/Impact_Generic";
		}
		else if (impactMaterial.ImpactType == ImpactType.Ink)
		{
			result = "Impacts/Impact_Ink";
		}
		else if (impactMaterial.ImpactType == ImpactType.InkColor)
		{
			result = "Impacts/Impact_Ink_Color";
		}
		else if (impactMaterial.ImpactType == ImpactType.NONE)
		{
			result = string.Empty;
		}
		return result;
	}

	private bool CheckFloorMaterial(RaycastHit hit, out FloorMaterial floor)
	{
		bool result = true;
		floor = hit.transform.gameObject.GetComponent<FloorMaterial>();
		if (floor == null)
		{
			result = false;
		}
		return result;
	}

	private string GetFloor(FloorMaterial floorMaterial)
	{
		string result = "Impacts/Impact_Generic";
		if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Metal)
		{
			result = "Impacts/Impact_Metal";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Stone)
		{
			result = "Impacts/Impact_Stone";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Tile)
		{
			result = "Impacts/Impact_Tile";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Wood)
		{
			result = "Impacts/Impact_Wood";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Vent)
		{
			result = "Impacts/Impact_Sparks";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Pipe)
		{
			result = "Impacts/Impact_Sparks";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkDeep || floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkPuddle || floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkShallow)
		{
			result = "Impacts/Impact_Ink";
		}
		return result;
	}

	protected override void InternalOnAttackComplete()
	{
		SetActive(active: true);
	}

	protected override void InternalReset()
	{
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalOnStateExit()
	{
		if (m_Weapon != null)
		{
			m_Weapon.gameObject.SetActive(value: false);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
