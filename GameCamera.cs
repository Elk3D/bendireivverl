using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[DefaultExecutionOrder(-95)]
public class GameCamera : JMonoBehaviour
{
	private static readonly int kFirstPersonProjMatrixId = Shader.PropertyToID("_FirstPersonProjMatrix");

	[Header("Post Process")]
	[SerializeField]
	private PostProcessVolumes m_PostProceussVolumes;

	[Header("First Person")]
	[SerializeField]
	private Camera m_FirstPersonCamera;

	[SerializeField]
	private GameObject m_FirstPersonArms;

	[SerializeField]
	private Transform m_ArmsContainer;

	[Header("Particles")]
	[SerializeField]
	private ParticleSystem m_TeleportParticles;

	[SerializeField]
	private ParticleSystem m_BanishParticles;

	[Header("Underwater")]
	[SerializeField]
	private GameObject m_UnderWater;

	private PostProcessLayer m_PostProcessLayer;

	private MotionBlur m_MotionBlur;

	private AmbientOcclusion m_AmbientOcclusion;

	private DepthOfField m_DepthOfField;

	private Bloom m_Bloom;

	private ColorGrading m_ColorGrading;

	private DamageEffect m_DamagePostProcessEffect;

	private Takedown m_TakedownPostProcessEffect;

	private GainPowerEffect m_GainPowerPostProcessEffect;

	private InkDemonEffect m_InkDemonPostProcessEffect;

	private VisionEffect m_VisionPostProcessEffect;

	private Sequence m_DamageSequence;

	private Sequence m_TakedownSequence;

	private Sequence m_GainPowerSequence;

	private Sequence m_InkDemonSequence;

	private Renderer[] renderers;

	private Tweener m_CameraShakeVibration;

	public Camera FirstPersonCamera => m_FirstPersonCamera;

	public Transform ArmsContainer => m_ArmsContainer;

	public ParticleSystem TeleportParticles => m_TeleportParticles;

	public ParticleSystem BanishParticles => m_BanishParticles;

	public bool IsUnderWater
	{
		get
		{
			if (m_UnderWater != null)
			{
				return m_UnderWater.activeSelf;
			}
			return false;
		}
	}

	public Camera Camera { get; private set; }

	public Transform CameraContainer { get; private set; }

	public Transform HeadContainer { get; private set; }

	public Transform FreeRoamCam { get; private set; }

	public Animator FirstPersonArmsAnimator { get; private set; }

	public CameraDepthOfFieldDistanceSetter DOFDistanceSetter { get; private set; }

	public PostProcessVolume ColorPostProcess => m_PostProceussVolumes.ColorPostProcessVolume;

	public PostProcessVolume BasePostProcess => m_PostProceussVolumes.BasePostProcessVolume;

	public bool AA
	{
		get
		{
			return m_PostProcessLayer.antialiasingMode == PostProcessLayer.Antialiasing.TemporalAntialiasing;
		}
		set
		{
			m_PostProcessLayer.antialiasingMode = (value ? PostProcessLayer.Antialiasing.TemporalAntialiasing : PostProcessLayer.Antialiasing.None);
		}
	}

	public bool MotionBlur
	{
		get
		{
			return m_MotionBlur.active;
		}
		set
		{
			m_MotionBlur.active = value;
		}
	}

	public bool DoF
	{
		get
		{
			return m_DepthOfField.active;
		}
		set
		{
			m_DepthOfField.active = value;
			if (DOFDistanceSetter == null)
			{
				DOFDistanceSetter = GetComponent<CameraDepthOfFieldDistanceSetter>();
			}
			DOFDistanceSetter.Initialize(value, m_DepthOfField);
		}
	}

	public bool Bloom
	{
		get
		{
			return m_Bloom.active;
		}
		set
		{
			m_Bloom.active = value;
		}
	}

	public bool AmbientOcclusion
	{
		get
		{
			return m_AmbientOcclusion.active;
		}
		set
		{
			m_AmbientOcclusion.active = value;
		}
	}

	public float Brightness
	{
		get
		{
			return m_ColorGrading.brightness.value;
		}
		set
		{
			int num = (int)(value * 10f);
			if (num <= 0)
			{
				num = 0;
			}
			else if (num >= 10)
			{
				num = 10;
			}
			float num2 = 0f;
			num2 = ((num > 5) ? ((float)(num - 5) * 2.5f) : (0f - (float)(5 - num) * 2.5f));
			float num3 = (GameManager.Instance.IsRealWorld ? 5 : 32);
			m_ColorGrading.brightness.value = num3 + num2;
		}
	}

	public override void Awake()
	{
		if (m_UnderWater != null)
		{
			m_UnderWater.SetActive(value: false);
		}
		Camera = GetComponent<Camera>();
		if (Camera != null)
		{
			Camera.enabled = false;
		}
		if (m_PostProcessLayer == null)
		{
			m_PostProcessLayer = GetComponent<PostProcessLayer>();
		}
		if (m_PostProcessLayer != null)
		{
			AA = GameManager.Instance.PlayerSettings.AA;
		}
		m_TakedownPostProcessEffect = BasePostProcess.profile.GetSetting<Takedown>();
		if (m_TakedownPostProcessEffect != null)
		{
			m_TakedownPostProcessEffect._Power.value = 0f;
		}
		m_GainPowerPostProcessEffect = BasePostProcess.profile.GetSetting<GainPowerEffect>();
		if (m_GainPowerPostProcessEffect != null)
		{
			m_GainPowerPostProcessEffect._Power.value = 0f;
		}
		m_VisionPostProcessEffect = BasePostProcess.profile.GetSetting<VisionEffect>();
		if (m_VisionPostProcessEffect != null)
		{
			m_VisionPostProcessEffect.active = false;
		}
		m_DamagePostProcessEffect = ColorPostProcess.profile.GetSetting<DamageEffect>();
		if (m_DamagePostProcessEffect != null)
		{
			m_DamagePostProcessEffect._Power.value = 0f;
		}
		m_InkDemonPostProcessEffect = ColorPostProcess.profile.GetSetting<InkDemonEffect>();
		if (m_InkDemonPostProcessEffect != null)
		{
			m_InkDemonPostProcessEffect._Power.value = 0f;
		}
		m_MotionBlur = BasePostProcess.profile.GetSetting<MotionBlur>();
		if (m_MotionBlur != null)
		{
			MotionBlur = GameManager.Instance.PlayerSettings.MotionBlur;
		}
		m_AmbientOcclusion = BasePostProcess.profile.GetSetting<AmbientOcclusion>();
		if (m_AmbientOcclusion != null)
		{
			AmbientOcclusion = GameManager.Instance.PlayerSettings.AmbientOcclusion;
		}
		m_DepthOfField = BasePostProcess.profile.GetSetting<DepthOfField>();
		if (m_DepthOfField != null)
		{
			DoF = GameManager.Instance.PlayerSettings.DoF;
		}
		m_Bloom = ColorPostProcess.profile.GetSetting<Bloom>();
		if (m_Bloom != null)
		{
			m_Bloom.active = GameManager.Instance.PlayerSettings.Bloom;
		}
		m_ColorGrading = ColorPostProcess.profile.GetSetting<ColorGrading>();
		if (m_ColorGrading != null)
		{
			Brightness = GameManager.Instance.PlayerSettings.Brightness;
		}
		if (m_FirstPersonArms != null)
		{
			FirstPersonArmsAnimator = m_FirstPersonArms.GetComponent<Animator>();
			renderers = m_FirstPersonArms.GetComponentsInChildren<Renderer>();
		}
	}

	public override void Start()
	{
		Camera.enabled = true;
		FreeRoamCam = new GameObject("FreeRoamCam").transform;
		Object.DontDestroyOnLoad(FreeRoamCam);
	}

	private void LateUpdate()
	{
		if (m_FirstPersonCamera != null)
		{
			Shader.SetGlobalMatrix(kFirstPersonProjMatrixId, m_FirstPersonCamera.projectionMatrix);
		}
	}

	public void Initialize(Transform headContainer, Transform cameraContainer)
	{
		HeadContainer = headContainer;
		CameraContainer = cameraContainer;
		GameManager.Instance.GameCamera = this;
	}

	public void SetFirstPersonArmsActive(bool active)
	{
		if (!(m_FirstPersonArms != null))
		{
			return;
		}
		m_FirstPersonArms.SetActive(active);
		if (active)
		{
			Renderer[] array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material.EnableKeyword("FIRST_PERSON");
			}
		}
		else
		{
			Renderer[] array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material.DisableKeyword("FIRST_PERSON");
			}
		}
	}

	public void SetUnderWater(bool active)
	{
		if (m_UnderWater != null)
		{
			m_UnderWater.SetActive(active);
		}
	}

	public Transform InitializeFreeRoamCam()
	{
		return InitializeFreeRoamCam(null);
	}

	public Transform InitializeFreeRoamCam(Transform parent)
	{
		if (FreeRoamCam == null)
		{
			FreeRoamCam = new GameObject("FreeRoamCam").transform;
		}
		if (HeadContainer != null)
		{
			FreeRoamCam.position = HeadContainer.position;
			FreeRoamCam.rotation = HeadContainer.rotation;
			HeadContainer.SetParent(FreeRoamCam);
		}
		FreeRoamCam.SetParent(parent);
		return FreeRoamCam;
	}

	public void ExitFreeRoamCam()
	{
		if (!(FreeRoamCam == null))
		{
			if (FreeRoamCam != null)
			{
				FreeRoamCam.SetParent(null);
			}
			if (!GameManager.Instance.Player.gameObject.activeSelf)
			{
				GameManager.Instance.Player.gameObject.SetActive(value: true);
			}
			Vector3 zero = Vector3.zero;
			zero.x = FreeRoamCam.eulerAngles.x;
			Vector3 zero2 = Vector3.zero;
			zero2.y = FreeRoamCam.localEulerAngles.y;
			HeadContainer.SetParent(GameManager.Instance.Player.CameraPivot);
			GameManager.Instance.Player.ForceRotation(Quaternion.Euler(zero2), Quaternion.Euler(zero));
			FreeRoamCam.SetParent(null);
		}
	}

	public void ShakeCamera(float duration, float strength = 10f, int vibrato = 10, float randomness = 90f, bool fadeOut = true, bool vibrate = true)
	{
		Camera.transform.DOKill();
		Camera.transform.localEulerAngles = Vector3.zero;
		Camera.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut).OnComplete(delegate
		{
			Camera.transform.localEulerAngles = Vector3.zero;
		});
		if (vibrate)
		{
			GameManager.Instance.TriggerRumble(duration, strength);
		}
	}

	public void Damage(float endDuration = 3f)
	{
		m_DamageSequence?.Kill();
		m_DamageSequence = DOTween.Sequence();
		float value = m_DamagePostProcessEffect._Power.value;
		value += 0.3f;
		if (value > 0.9f)
		{
			value = 0.9f;
		}
		float num = 0.25f;
		m_DamageSequence.Insert(0f, DOTween.To(() => m_DamagePostProcessEffect._Power.value, delegate(float x)
		{
			m_DamagePostProcessEffect._Power.value = x;
		}, value, num).SetEase(Ease.OutSine));
		m_DamageSequence.Insert(num + 0.25f, DOTween.To(() => m_DamagePostProcessEffect._Power.value, delegate(float x)
		{
			m_DamagePostProcessEffect._Power.value = x;
		}, 0f, endDuration).SetEase(Ease.OutSine));
	}

	public void Takedown(float endDuration = 3f)
	{
		m_TakedownSequence?.Kill();
		m_TakedownSequence = DOTween.Sequence();
		float value = m_TakedownPostProcessEffect._Power.value;
		value += 0.3f;
		if (value > 0.9f)
		{
			value = 0.9f;
		}
		float num = 0.25f;
		m_TakedownSequence.Insert(0f, DOTween.To(() => m_TakedownPostProcessEffect._Power.value, delegate(float x)
		{
			m_TakedownPostProcessEffect._Power.value = x;
		}, value, num).SetEase(Ease.OutSine));
		m_TakedownSequence.Insert(num + 0.25f, DOTween.To(() => m_TakedownPostProcessEffect._Power.value, delegate(float x)
		{
			m_TakedownPostProcessEffect._Power.value = x;
		}, 0f, endDuration).SetEase(Ease.OutSine));
	}

	public void GainPower(float endDuration = 3f)
	{
		m_GainPowerSequence?.Kill();
		m_GainPowerSequence = DOTween.Sequence();
		float num = 1f;
		m_GainPowerSequence.Insert(0f, DOTween.To(() => m_GainPowerPostProcessEffect._Power.value, delegate(float x)
		{
			m_GainPowerPostProcessEffect._Power.value = x;
		}, 1f, num).SetEase(Ease.OutSine));
		m_GainPowerSequence.Insert(num + 0.25f, DOTween.To(() => m_GainPowerPostProcessEffect._Power.value, delegate(float x)
		{
			m_GainPowerPostProcessEffect._Power.value = x;
		}, 0f, endDuration).SetEase(Ease.InSine));
	}

	public void InkDemon(float endDuration = 3f)
	{
		m_InkDemonSequence?.Kill();
		m_InkDemonSequence = DOTween.Sequence();
		m_InkDemonSequence.Insert(0f, DOTween.To(() => m_InkDemonPostProcessEffect._Power.value, delegate(float x)
		{
			m_InkDemonPostProcessEffect._Power.value = x;
		}, 1f, 0.25f).SetEase(Ease.OutSine));
		m_InkDemonSequence.Insert(endDuration, DOTween.To(() => m_InkDemonPostProcessEffect._Power.value, delegate(float x)
		{
			m_InkDemonPostProcessEffect._Power.value = x;
		}, 0f, 2f).SetEase(Ease.OutSine));
	}

	public void InkDemonOn()
	{
		m_InkDemonSequence?.Kill();
		m_InkDemonSequence = DOTween.Sequence();
		m_InkDemonSequence.Insert(0f, DOTween.To(() => m_InkDemonPostProcessEffect._Power.value, delegate(float x)
		{
			m_InkDemonPostProcessEffect._Power.value = x;
		}, 1f, 0.25f).SetEase(Ease.OutSine));
	}

	public void InkDemonOff()
	{
		m_InkDemonSequence?.Kill();
		m_InkDemonSequence = DOTween.Sequence();
		m_InkDemonSequence.Insert(0f, DOTween.To(() => m_InkDemonPostProcessEffect._Power.value, delegate(float x)
		{
			m_InkDemonPostProcessEffect._Power.value = x;
		}, 0f, 2f).SetEase(Ease.OutSine));
	}

	public void VisionOn()
	{
		m_VisionPostProcessEffect.active = true;
	}

	public void VisionOff()
	{
		m_VisionPostProcessEffect.active = false;
	}

	public void VisionTransition(bool active)
	{
		m_VisionPostProcessEffect._Transition.value = active;
	}

	public Vector3 WorldToScreenPoint(Vector3 position)
	{
		return Camera.WorldToScreenPoint(position);
	}

	public bool CheckPositionAngle(Vector3 position, float angle = 0f)
	{
		return WorldToScreenPoint(position).z < angle;
	}

	protected override void OnDisposed()
	{
		m_DamageSequence?.Kill();
		m_DamageSequence = null;
		m_TakedownSequence?.Kill();
		m_TakedownSequence = null;
		m_GainPowerSequence?.Kill();
		m_GainPowerSequence = null;
		m_InkDemonSequence?.Kill();
		m_InkDemonSequence = null;
		m_PostProcessLayer = null;
		m_MotionBlur = null;
		m_AmbientOcclusion = null;
		m_DepthOfField = null;
		m_Bloom = null;
		m_DamagePostProcessEffect = null;
		m_TakedownPostProcessEffect = null;
		m_GainPowerPostProcessEffect = null;
		m_VisionPostProcessEffect = null;
		m_InkDemonPostProcessEffect = null;
		Camera = null;
		CameraContainer = null;
		HeadContainer = null;
		FirstPersonArmsAnimator = null;
		DOFDistanceSetter = null;
		renderers = null;
		if (FreeRoamCam != null)
		{
			Object.Destroy(FreeRoamCam.gameObject);
			FreeRoamCam = null;
		}
		base.OnDisposed();
	}
}
