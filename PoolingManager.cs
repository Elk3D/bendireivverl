using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : JMonoBehaviour
{
	private Dictionary<int, List<GameObject>> m_PooledObjects = new Dictionary<int, List<GameObject>>();

	public static PoolingManager Create()
	{
		return new GameObject("[PoolManager]").AddComponent<PoolingManager>();
	}

	public override void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public GameObject GetFromPool(string prefab, float delay = 15f)
	{
		int hashCode = prefab.GetHashCode();
		if (!m_PooledObjects.ContainsKey(hashCode))
		{
			m_PooledObjects.Add(hashCode, new List<GameObject>());
		}
		if (m_PooledObjects[hashCode].Count <= 0)
		{
			m_PooledObjects[hashCode].Add(Object.Instantiate(GameManager.Instance.AssetManager.GetAsset<GameObject>(prefab)));
		}
		GameObject gameObject = m_PooledObjects[hashCode][0];
		m_PooledObjects[hashCode].Remove(gameObject);
		gameObject.SetActive(value: true);
		StartCoroutine(DelayPooling(hashCode, gameObject, delay));
		return gameObject;
	}

	private IEnumerator DelayPooling(int hashID, GameObject go, float delay = 15f)
	{
		yield return new WaitForSeconds(delay);
		while (GameManager.Instance.IsPaused)
		{
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		SendToPool(hashID, go);
	}

	private void SendToPool(int hashID, GameObject go)
	{
		if ((bool)go)
		{
			go.SetActive(value: false);
			if (!m_PooledObjects.ContainsKey(hashID))
			{
				m_PooledObjects.Add(hashID, new List<GameObject>());
			}
			m_PooledObjects[hashID].Add(go);
			go.transform.SetParent(base.transform);
		}
	}

	protected override void OnDisposed()
	{
		StopAllCoroutines();
		if (m_PooledObjects != null)
		{
			m_PooledObjects.Clear();
			m_PooledObjects = null;
		}
		base.OnDisposed();
	}
}
