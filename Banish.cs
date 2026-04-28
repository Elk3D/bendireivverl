using DG.Tweening;
using UnityEngine;

public static class Banish
{
	public static void DOBanish(GameObject gameObject, float duration = 5.25f)
	{
		GameManager.Instance.GameCamera.BanishParticles.Play();
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
			foreach (Material material in materials)
			{
				if (material.GetFloat("_DeathPower") < 1f)
				{
					material.SetFloat("_DeathPower", 0.25f);
					sequence.Insert(0f, material.DOFloat(1f, "_DeathPower", duration).SetEase(Ease.InSine));
				}
			}
		}
		sequence.OnComplete(GameManager.Instance.GameCamera.BanishParticles.Stop);
	}
}
