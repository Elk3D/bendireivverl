using System;
using UnityEngine;

public abstract class JMonoBehaviour : MonoBehaviour, IDisposable
{
	private Transform m_Transform;

	private GameObject m_GameObject;

	public bool IsDisposed { get; private set; }

	public bool IsDestroyed { get; private set; }

	public new Transform transform
	{
		get
		{
			if (!Application.isPlaying)
			{
				return base.transform;
			}
			return m_Transform ?? (m_Transform = base.transform);
		}
	}

	public new GameObject gameObject
	{
		get
		{
			if (!Application.isPlaying)
			{
				return base.gameObject;
			}
			return m_GameObject ?? (m_GameObject = base.gameObject);
		}
	}

	public virtual void Awake()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
	}

	protected virtual void OnDisposed()
	{
	}

	public void SetParent(Transform parent)
	{
		transform.SetParent(parent);
	}

	public void SetParentAndCenter(Transform parent)
	{
		SetParent(parent);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
	}

	public void SetParentAndCenterAndScale(Transform parent)
	{
		SetParentAndCenter(parent);
		transform.localScale = Vector3.one;
	}

	public void SetLayerRecursive(int layer)
	{
		SetLayerRecursive(gameObject, layer);
	}

	public void SetLayerRecursive(GameObject _gameObject, int _layer)
	{
		_gameObject.layer = _layer;
		foreach (Transform item in _gameObject.transform)
		{
			SetLayerRecursive(item.gameObject, _layer);
		}
	}

	public void Dispose()
	{
		if (!IsDisposed)
		{
			OnDisposed();
			IsDisposed = true;
			GC.SuppressFinalize(this);
			if (!IsDestroyed)
			{
				IsDestroyed = true;
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	public void OnDestroy()
	{
		if (!IsDestroyed)
		{
			IsDestroyed = true;
			Dispose();
		}
	}
}
