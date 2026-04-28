using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Mirror : JMonoBehaviour
{
	public enum AntiAliasingSampleCount
	{
		None = 1,
		Two = 2,
		Four = 4,
		Eight = 8
	}

	public class ReflectionCameraInfo
	{
		public Camera SourceCamera;

		public Camera ReflectionCamera;

		public bool SourceCameraIsReflection;

		public RenderTexture TargetTexture;

		public RenderTexture TargetTexture2;
	}

	[Tooltip("Renderer to draw reflection in")]
	public Renderer ReflectRenderer;

	[Tooltip("Renderer to check visible")]
	public Renderer VisibleRenderer;

	[Tooltip("What layers to reflect")]
	public LayerMask ReflectionMask = -1;

	[Tooltip("Reflection mask for recursion. Set to 0 to match the ReflectionMask property.")]
	public LayerMask ReflectionMaskRecursion = 0;

	[Tooltip("Whether to reflect the skybox")]
	public bool ReflectSkybox;

	[Tooltip("Reflection texture name for shaders to use")]
	public string ReflectionSamplerName = "_ReflectionTex";

	private string ReflectionSamplerName2;

	[Tooltip("Maximum per pixel lights in reflection")]
	[Range(0f, 128f)]
	public int MaximumPerPixelLightsToReflect = 8;

	[Tooltip("Set to greater than 0 to set anti-aliasing samples on the render texture. Ignored for non-forward rendering paths.")]
	public AntiAliasingSampleCount AntiAliasingSamples;

	[Tooltip("Near clip plane offset for reflection")]
	public float ClipPlaneOffset = 0.07f;

	[Tooltip("Render texture size. Based on aspect ratio this will use this size as the width or height.")]
	[Range(64f, 4096f)]
	public int RenderTextureSize = 1024;

	[Tooltip("The reflection camera render path. Set to 'UsePlayerSettings' to take on the observing camera rendering path. DO NOT CHANGE AT RUNTIME.")]
	public RenderingPath ReflectionCameraRenderingPath = RenderingPath.UsePlayerSettings;

	[Tooltip("Whether normal is forward. True for quads, false for planes (up)")]
	public bool NormalIsForward = true;

	[Tooltip("Aspect ratio (width/height) for reflection camera, 0 for default.")]
	[Range(0f, 10f)]
	public float AspectRatio;

	[Tooltip("Field of view for reflection camera, 0 for default.")]
	[Range(0f, 360f)]
	public float FieldOfView;

	[Tooltip("Near plane for reflection camera, 0 for default.")]
	public float NearPlane;

	[Tooltip("Far plane for reflection camera, 0 for default.")]
	public float FarPlane;

	[Tooltip("Recursion limit. Reflections will render off each other up to this many times. Be careful for performance.")]
	[Range(0f, 10f)]
	public int RecursionLimit;

	[Tooltip("Reduce render texture size as recursion increases, formula = Mathf.Pow(RecursionRenderTextureSizeReducerPower, recursionLevel) * RenderTextureSize.")]
	[Range(0.1f, 1f)]
	public float RecursionRenderTextureSizeReducerPower = 0.75f;

	[Tooltip("Render texture format for reflection")]
	public RenderTextureFormat RenderTextureFormat;

	[Tooltip("Stereo separation multiplier, use this if objects are not scaling exactly the way you want.")]
	[Range(0f, 1f)]
	public float StereoSeparationMultiplier = 1f;

	private const string mirrorRecursionLimitKeyword = "MIRROR_RECURSION_LIMIT";

	private readonly List<ReflectionCameraInfo> currentCameras = new List<ReflectionCameraInfo>();

	private readonly List<ReflectionCameraInfo> cameraCache = new List<ReflectionCameraInfo>();

	private readonly List<KeyValuePair<RenderTexture, RenderTexture>> currentRenderTextures = new List<KeyValuePair<RenderTexture, RenderTexture>>();

	private readonly Dictionary<Camera, List<KeyValuePair<RenderTexture, StereoTargetEyeMask>>> sourceCamerasToRenderTextures = new Dictionary<Camera, List<KeyValuePair<RenderTexture, StereoTargetEyeMask>>>();

	private static readonly float[] cullDistances = new float[32];

	private static int renderCount;

	private const int maxRenderCount = 100;

	private const int waterLayerInverse = -17;

	private bool initialized;

	private bool m_IsActive;

	public static int CurrentRecursionLevel { get; private set; }

	public ReflectionCameraInfo QueueReflection(Camera sourceCamera)
	{
		if (ShouldIgnoreCamera(sourceCamera, out var isReflection))
		{
			return null;
		}
		ReflectionCameraInfo reflectionCameraInfo = CreateReflectionCamera(sourceCamera, isReflection);
		RenderReflectionCamera(reflectionCameraInfo);
		return reflectionCameraInfo;
	}

	public Camera CameraRenderingReflection(Camera sourceCamera)
	{
		for (int i = 0; i < currentCameras.Count; i++)
		{
			if (currentCameras[i].SourceCamera == sourceCamera)
			{
				return currentCameras[i].ReflectionCamera;
			}
		}
		return null;
	}

	public static bool CameraIsReflection(Camera cam, out string camName)
	{
		camName = cam.name;
		if (camName.IndexOf("water", StringComparison.OrdinalIgnoreCase) < 0)
		{
			return camName.IndexOf("refl", StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return true;
	}

	public static void RenderReflection(ReflectionCameraInfo info, Transform reflectionTransform, Vector3 reflectionNormal, float clipPlaneOffset, float stereoSeparationMultiplier)
	{
		if (info.SourceCamera.stereoEnabled)
		{
			if (info.SourceCamera.stereoTargetEye == StereoTargetEyeMask.Both || info.SourceCamera.stereoTargetEye == StereoTargetEyeMask.Left)
			{
				RenderReflectionInternal(info, reflectionTransform, reflectionNormal, clipPlaneOffset, StereoTargetEyeMask.Left, info.TargetTexture, stereoSeparationMultiplier);
			}
			if (info.SourceCamera.stereoTargetEye == StereoTargetEyeMask.Both || info.SourceCamera.stereoTargetEye == StereoTargetEyeMask.Right)
			{
				RenderReflectionInternal(info, reflectionTransform, reflectionNormal, clipPlaneOffset, StereoTargetEyeMask.Right, info.TargetTexture2, stereoSeparationMultiplier);
			}
		}
		else
		{
			RenderReflectionInternal(info, reflectionTransform, reflectionNormal, clipPlaneOffset, StereoTargetEyeMask.Both, info.TargetTexture, stereoSeparationMultiplier);
		}
	}

	private static void RenderReflectionInternal(ReflectionCameraInfo info, Transform reflectionTransform, Vector3 reflectionNormal, float clipPlaneOffset, StereoTargetEyeMask eye, RenderTexture targetTexture, float stereoSeparationMultiplier)
	{
		bool invertCulling = GL.invertCulling;
		Vector3 position = reflectionTransform.position;
		if (info.SourceCameraIsReflection && GL.invertCulling)
		{
			reflectionNormal = -reflectionNormal;
		}
		float w = 0f - Vector3.Dot(reflectionNormal, position) - clipPlaneOffset;
		Vector4 plane = new Vector4(reflectionNormal.x, reflectionNormal.y, reflectionNormal.z, w);
		CalculateReflectionMatrix(out var reflectionMat, plane);
		Vector3 position2 = info.SourceCamera.transform.position;
		Vector3 position3 = reflectionMat.MultiplyPoint(position2);
		Matrix4x4 worldToCameraMatrix = info.SourceCamera.worldToCameraMatrix;
		switch (eye)
		{
		case StereoTargetEyeMask.Left:
			worldToCameraMatrix[12] += info.SourceCamera.stereoSeparation * 0.5f * stereoSeparationMultiplier;
			info.ReflectionCamera.projectionMatrix = info.SourceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			break;
		case StereoTargetEyeMask.Right:
			worldToCameraMatrix[12] -= info.SourceCamera.stereoSeparation * 0.5f * stereoSeparationMultiplier;
			info.ReflectionCamera.projectionMatrix = info.SourceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			break;
		default:
			info.ReflectionCamera.projectionMatrix = info.SourceCamera.projectionMatrix;
			break;
		}
		info.ReflectionCamera.worldToCameraMatrix = worldToCameraMatrix * reflectionMat;
		if (info.ReflectionCamera.actualRenderingPath != RenderingPath.DeferredShading)
		{
			Vector4 clipPlane = CameraSpacePlane(info.ReflectionCamera, position, reflectionNormal, clipPlaneOffset, GL.invertCulling ? (-1f) : 1f);
			info.ReflectionCamera.projectionMatrix = info.ReflectionCamera.CalculateObliqueMatrix(clipPlane);
		}
		for (int i = 0; i < cullDistances.Length; i++)
		{
			cullDistances[i] = info.ReflectionCamera.farClipPlane;
		}
		info.ReflectionCamera.layerCullDistances = cullDistances;
		info.ReflectionCamera.layerCullSpherical = true;
		GL.invertCulling = !GL.invertCulling;
		info.ReflectionCamera.transform.position = position3;
		if (++renderCount < 100)
		{
			info.ReflectionCamera.targetTexture = targetTexture;
			info.ReflectionCamera.Render();
			info.ReflectionCamera.targetTexture = null;
		}
		info.ReflectionCamera.transform.position = position2;
		GL.invertCulling = invertCulling;
	}

	private void AddRenderTextureForSourceCamera(Camera sourceCamera, RenderTexture tex, StereoTargetEyeMask eyeMask)
	{
		if (!sourceCamerasToRenderTextures.TryGetValue(sourceCamera, out var value))
		{
			value = (sourceCamerasToRenderTextures[sourceCamera] = new List<KeyValuePair<RenderTexture, StereoTargetEyeMask>>());
		}
		value.Add(new KeyValuePair<RenderTexture, StereoTargetEyeMask>(tex, eyeMask));
	}

	private bool ShouldIgnoreCamera(Camera sourceCamera, out bool isReflection)
	{
		isReflection = CameraIsReflection(sourceCamera, out var _);
		if (isReflection && (sourceCamera.transform.parent == null || sourceCamera.transform.parent.GetComponent<Mirror>() == null))
		{
			return true;
		}
		return false;
	}

	private void CleanupCamera(ReflectionCameraInfo info, bool destroyCamera)
	{
		if (!(info.ReflectionCamera == null) && destroyCamera)
		{
			UnityEngine.Object.Destroy(info.ReflectionCamera.gameObject);
		}
	}

	private void CleanupCameras(bool destroyCameras)
	{
		cameraCache.AddRange(currentCameras);
		currentCameras.Clear();
		for (int num = cameraCache.Count - 1; num >= 0; num--)
		{
			CleanupCamera(cameraCache[num], destroyCameras);
			if (destroyCameras)
			{
				cameraCache.RemoveAt(num);
			}
		}
	}

	private void Update()
	{
		if (GameManager.Instance.GameCamera == null || base.IsDisposed || GameManager.Instance.IsPaused || !(VisibleRenderer != null))
		{
			return;
		}
		if (VisibleRenderer.isVisible)
		{
			if (!m_IsActive && Vector3.Distance(GameManager.Instance.GameCamera.transform.position, ReflectRenderer.transform.position) < GameManager.Instance.GameCamera.Camera.farClipPlane + 10f)
			{
				m_IsActive = true;
				ReflectRenderer.enabled = true;
				OnEnable();
			}
		}
		else if (m_IsActive)
		{
			m_IsActive = false;
			OnDisable();
			ReflectRenderer.enabled = false;
		}
	}

	private void LateUpdate()
	{
		if (!(GameManager.Instance.GameCamera == null) && !base.IsDisposed && !GameManager.Instance.IsPaused && m_IsActive)
		{
			CleanupCameras(destroyCameras: false);
			renderCount = 0;
		}
	}

	public override void OnEnable()
	{
		if (!initialized)
		{
			initialized = true;
			ReflectRenderer = ((ReflectRenderer == null) ? GetComponent<Renderer>() : ReflectRenderer);
			ReflectRenderer.sharedMaterial = ReflectRenderer.material;
			ReflectRenderer.sharedMaterial.DisableKeyword("MIRROR_RECURSION_LIMIT");
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Camera component = base.transform.GetChild(i).GetComponent<Camera>();
				if (component != null)
				{
					cameraCache.Add(new ReflectionCameraInfo
					{
						ReflectionCamera = component
					});
				}
			}
		}
		RemoveListeners();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		ReflectionSamplerName2 = ReflectionSamplerName + "2";
	}

	public override void OnDisable()
	{
		CleanupCameras(destroyCameras: true);
		RemoveListeners();
		sourceCamerasToRenderTextures.Clear();
	}

	private void RemoveListeners()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
	}

	protected override void OnDisposed()
	{
		CleanupCameras(destroyCameras: true);
		RemoveListeners();
		sourceCamerasToRenderTextures.Clear();
		if (ReflectRenderer.sharedMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(ReflectRenderer.sharedMaterial);
		}
		base.OnDisposed();
	}

	private void OnWillRenderObject()
	{
		QueueReflection(Camera.current);
	}

	private void CameraPreCull(Camera camera)
	{
		if (ReflectRenderer != null && ReflectRenderer.sharedMaterial != null)
		{
			KeyValuePair<RenderTexture, RenderTexture> item = new KeyValuePair<RenderTexture, RenderTexture>(ReflectRenderer.sharedMaterial.GetTexture(ReflectionSamplerName) as RenderTexture, ReflectRenderer.sharedMaterial.GetTexture(ReflectionSamplerName2) as RenderTexture);
			currentRenderTextures.Add(item);
		}
	}

	private void CameraPreRender(Camera camera)
	{
	}

	private void CameraPostRender(Camera camera)
	{
		if (currentRenderTextures.Count != 0)
		{
			int index = currentRenderTextures.Count - 1;
			KeyValuePair<RenderTexture, RenderTexture> keyValuePair = currentRenderTextures[index];
			ReflectRenderer.sharedMaterial.SetTexture(ReflectionSamplerName, keyValuePair.Key);
			ReflectRenderer.sharedMaterial.SetTexture(ReflectionSamplerName2, keyValuePair.Value);
			currentRenderTextures.RemoveAt(index);
		}
		for (int num = currentCameras.Count - 1; num >= 0; num--)
		{
			if (currentCameras[num].SourceCamera == camera)
			{
				CleanupCamera(currentCameras[num], destroyCamera: false);
				currentCameras.RemoveAt(num);
			}
		}
		if (!sourceCamerasToRenderTextures.TryGetValue(camera, out var value))
		{
			return;
		}
		StereoTargetEyeMask stereoTargetEyeMask = StereoTargetEyeMask.Both;
		string camName;
		bool flag = CameraIsReflection(camera, out camName);
		for (int num2 = value.Count - 1; num2 >= 0; num2--)
		{
			if (flag || (MirrorUtil.isPresent() && XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.OneEye))
			{
				stereoTargetEyeMask = camera.stereoActiveEye switch
				{
					Camera.MonoOrStereoscopicEye.Left => StereoTargetEyeMask.Left, 
					Camera.MonoOrStereoscopicEye.Right => StereoTargetEyeMask.Right, 
					_ => StereoTargetEyeMask.Both, 
				};
			}
			KeyValuePair<RenderTexture, StereoTargetEyeMask> keyValuePair2 = value[num2];
			if (keyValuePair2.Key != null && (stereoTargetEyeMask & keyValuePair2.Value) != StereoTargetEyeMask.None)
			{
				RenderTexture.ReleaseTemporary(keyValuePair2.Key);
				value.RemoveAt(num2);
			}
		}
	}

	private void SyncCameraSettings(Camera reflectCamera, Camera sourceCamera)
	{
		reflectCamera.nearClipPlane = ((NearPlane <= 0f) ? sourceCamera.nearClipPlane : NearPlane);
		reflectCamera.farClipPlane = ((FarPlane <= 0f) ? sourceCamera.farClipPlane : FarPlane);
		reflectCamera.aspect = ((AspectRatio <= 0f) ? sourceCamera.aspect : AspectRatio);
		if (!reflectCamera.stereoEnabled)
		{
			reflectCamera.fieldOfView = ((FieldOfView <= 0f) ? sourceCamera.fieldOfView : FieldOfView);
		}
		reflectCamera.orthographic = sourceCamera.orthographic;
		reflectCamera.orthographicSize = sourceCamera.orthographicSize;
		reflectCamera.renderingPath = ((ReflectionCameraRenderingPath == RenderingPath.UsePlayerSettings) ? sourceCamera.renderingPath : ReflectionCameraRenderingPath);
		reflectCamera.backgroundColor = Color.black;
		reflectCamera.clearFlags = (ReflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.Color);
		reflectCamera.cullingMask = ((CurrentRecursionLevel == 0 || ReflectionMaskRecursion.value == 0) ? ReflectionMask : ReflectionMaskRecursion);
		reflectCamera.stereoSeparation = sourceCamera.stereoSeparation;
		reflectCamera.allowHDR = sourceCamera.allowHDR;
		reflectCamera.allowMSAA = AntiAliasingSamples > AntiAliasingSampleCount.None;
		reflectCamera.allowDynamicResolution = sourceCamera.allowDynamicResolution;
		reflectCamera.rect = new Rect(0f, 0f, 1f, 1f);
		reflectCamera.transform.rotation = sourceCamera.transform.rotation;
		reflectCamera.transform.position = sourceCamera.transform.position;
		if (ReflectSkybox && (bool)sourceCamera.gameObject.GetComponent(typeof(Skybox)))
		{
			Skybox skybox = (Skybox)reflectCamera.gameObject.GetComponent(typeof(Skybox));
			if (!skybox)
			{
				skybox = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
				skybox.hideFlags = HideFlags.HideAndDontSave;
			}
			skybox.material = ((Skybox)sourceCamera.GetComponent(typeof(Skybox))).material;
		}
	}

	private ReflectionCameraInfo CreateReflectionCamera(Camera sourceCamera, bool sourceCameraIsReflection)
	{
		if (ReflectRenderer == null || !ReflectRenderer.enabled || ReflectRenderer.sharedMaterial == null || sourceCamera == null)
		{
			return null;
		}
		Mirror mirror = ((sourceCameraIsReflection && sourceCamera.transform.parent != null) ? sourceCamera.transform.parent.GetComponent<Mirror>() : null);
		if (sourceCameraIsReflection && (mirror == null || mirror == this))
		{
			ReflectRenderer.sharedMaterial.EnableKeyword("MIRROR_RECURSION_LIMIT");
			return null;
		}
		if (mirror != null && currentCameras.Count >= RecursionLimit)
		{
			ReflectRenderer.sharedMaterial.EnableKeyword("MIRROR_RECURSION_LIMIT");
			return null;
		}
		ReflectionCameraInfo reflectionCameraInfo;
		if (cameraCache.Count == 0)
		{
			GameObject obj = new GameObject("MirrorReflectionCamera");
			obj.hideFlags = HideFlags.HideAndDontSave;
			obj.SetActive(value: false);
			obj.transform.parent = base.transform;
			Camera camera = obj.AddComponent<Camera>();
			camera.enabled = false;
			camera.useOcclusionCulling = false;
			reflectionCameraInfo = new ReflectionCameraInfo
			{
				SourceCamera = sourceCamera,
				ReflectionCamera = camera
			};
		}
		else
		{
			int index = cameraCache.Count - 1;
			reflectionCameraInfo = cameraCache[index];
			cameraCache.RemoveAt(index);
			CleanupCamera(reflectionCameraInfo, destroyCamera: false);
		}
		reflectionCameraInfo.SourceCamera = sourceCamera;
		reflectionCameraInfo.SourceCameraIsReflection = sourceCameraIsReflection;
		int num = Math.Max(32, (int)(Mathf.Pow(RecursionRenderTextureSizeReducerPower, CurrentRecursionLevel) * (float)RenderTextureSize));
		reflectionCameraInfo.TargetTexture = RenderTexture.GetTemporary(num, num, 16, RenderTextureFormat.DefaultHDR);
		reflectionCameraInfo.TargetTexture.wrapMode = TextureWrapMode.Clamp;
		reflectionCameraInfo.TargetTexture.filterMode = FilterMode.Bilinear;
		if (AntiAliasingSamples > AntiAliasingSampleCount.None)
		{
			reflectionCameraInfo.TargetTexture.antiAliasing = (int)AntiAliasingSamples;
		}
		AddRenderTextureForSourceCamera(sourceCamera, reflectionCameraInfo.TargetTexture, StereoTargetEyeMask.Left);
		if (sourceCamera.stereoEnabled)
		{
			reflectionCameraInfo.TargetTexture2 = RenderTexture.GetTemporary(num, num, 16, RenderTextureFormat.DefaultHDR);
			reflectionCameraInfo.TargetTexture2.wrapMode = TextureWrapMode.Clamp;
			reflectionCameraInfo.TargetTexture2.filterMode = FilterMode.Bilinear;
			if (AntiAliasingSamples > AntiAliasingSampleCount.None)
			{
				reflectionCameraInfo.TargetTexture2.antiAliasing = (int)AntiAliasingSamples;
			}
			AddRenderTextureForSourceCamera(sourceCamera, reflectionCameraInfo.TargetTexture2, StereoTargetEyeMask.Right);
		}
		else
		{
			reflectionCameraInfo.TargetTexture2 = reflectionCameraInfo.TargetTexture;
		}
		currentCameras.Add(reflectionCameraInfo);
		return reflectionCameraInfo;
	}

	private void RenderReflectionCamera(ReflectionCameraInfo info)
	{
		if (info == null || info.ReflectionCamera == null || info.SourceCamera == null || ReflectRenderer == null || ReflectRenderer.sharedMaterial == null || !ReflectRenderer.enabled)
		{
			return;
		}
		CurrentRecursionLevel = currentCameras.Count + (info.SourceCameraIsReflection ? 1 : 0);
		renderCount++;
		Camera sourceCamera = info.SourceCamera;
		Camera reflectionCamera = info.ReflectionCamera;
		int pixelLightCount = QualitySettings.pixelLightCount;
		int cullingMask = reflectionCamera.cullingMask;
		bool softParticles = QualitySettings.softParticles;
		int antiAliasing = QualitySettings.antiAliasing;
		ShadowQuality shadows = QualitySettings.shadows;
		SyncCameraSettings(reflectionCamera, sourceCamera);
		if (currentCameras.Count > 1)
		{
			if (currentCameras.Count > 3)
			{
				QualitySettings.shadows = ShadowQuality.Disable;
			}
			QualitySettings.shadows = ShadowQuality.HardOnly;
			QualitySettings.antiAliasing = 0;
			QualitySettings.softParticles = false;
			QualitySettings.pixelLightCount = 0;
			reflectionCamera.cullingMask &= -17;
		}
		else
		{
			QualitySettings.pixelLightCount = MaximumPerPixelLightsToReflect;
		}
		Transform transform = base.transform;
		Vector3 reflectionNormal = (NormalIsForward ? (-transform.forward) : transform.up);
		RenderReflection(info, transform, reflectionNormal, ClipPlaneOffset, StereoSeparationMultiplier);
		reflectionCamera.cullingMask = cullingMask;
		QualitySettings.pixelLightCount = pixelLightCount;
		QualitySettings.softParticles = softParticles;
		QualitySettings.antiAliasing = antiAliasing;
		QualitySettings.shadows = shadows;
		ReflectRenderer.sharedMaterial.SetTexture(ReflectionSamplerName, info.TargetTexture);
		ReflectRenderer.sharedMaterial.SetTexture(ReflectionSamplerName2, info.TargetTexture2);
		ReflectRenderer.sharedMaterial.DisableKeyword("MIRROR_RECURSION_LIMIT");
		currentCameras.Remove(info);
		info.SourceCamera = null;
		info.TargetTexture = null;
		info.TargetTexture2 = null;
		cameraCache.Add(info);
	}

	private static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float clipPlaneOffset, float sideSign)
	{
		Vector3 point = pos + normal * clipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private static void CalculateReflectionMatrix(out Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
