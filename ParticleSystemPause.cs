using System.Collections;
using UnityEngine;

public class ParticleSystemPause : MonoBehaviour
{
	public ParticleSystem ps;

	private void OnValidate()
	{
		if (!ps)
		{
			ps = GetComponent<ParticleSystem>();
		}
	}

	private void Awake()
	{
		if (!ps)
		{
			ps = GetComponent<ParticleSystem>();
		}
	}

	private IEnumerator Start()
	{
		int maxCount = 100;
		SimpleCull sc;
		do
		{
			if (maxCount-- <= 0)
			{
				yield break;
			}
			sc = Object.FindObjectOfType<SimpleCull>();
			yield return null;
		}
		while (sc == null);
		do
		{
			yield return null;
		}
		while (!sc.cullTransform);
		if (Vector3.Distance(base.transform.position, sc.cullTransform.position) > sc.cullingDistance)
		{
			OnBecameInvisible();
		}
	}

	private void OnBecameVisible()
	{
		if ((bool)ps && ps.isPaused)
		{
			ps.Play();
		}
	}

	private void OnBecameInvisible()
	{
		if ((bool)ps && ps.isPlaying)
		{
			ps.Pause();
		}
	}
}
