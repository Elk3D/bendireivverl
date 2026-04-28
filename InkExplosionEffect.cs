using System;
using DG.Tweening;
using UnityEngine;

public class InkExplosionEffect : JMonoBehaviour
{
	private const string MELT_PERCENTAGE = "_MeltPercentage";

	[SerializeField]
	private ParticleSystem m_Explosion;

	[SerializeField]
	private ParticleSystem m_Drops;

	private Renderer m_Renderer;

	public event EventHandler OnExplode;

	public override void Awake()
	{
		base.Awake();
		ParticleSystem.CollisionModule collision = m_Drops.collision;
		collision.quality = ParticleSystemCollisionQuality.High;
		collision.radiusScale = 0.001f;
	}

	public void Birth(Renderer _renderer)
	{
		if (m_Renderer == null)
		{
			m_Renderer = _renderer;
		}
		Explode(10, 10);
	}

	public void ExplodeOnly()
	{
		Explode(10, 10);
	}

	public void Activate(Renderer _renderer, float meltStart = 2f, float meltDuration = 2f)
	{
		if (m_Renderer == null)
		{
			m_Renderer = _renderer;
		}
		Sequence s = DOTween.Sequence();
		float num = meltStart;
		if (m_Renderer.material.HasProperty("_MeltPercentage"))
		{
			s.Insert(num, m_Renderer.material.DOFloat(1f, "_MeltPercentage", meltDuration).SetEase(Ease.Linear));
		}
		num += meltDuration;
		s.InsertCallback(num, DeathExplode);
		num += 2f;
		s.InsertCallback(num, SendOnExplode);
	}

	private void DeathExplode()
	{
		Explode(20, 10);
		m_Renderer.enabled = false;
	}

	private void Explode(int explosionCount, int dropCount)
	{
		base.transform.SetParent(null);
		ParticleSystem.ShapeModule shape = m_Explosion.shape;
		shape.skinnedMeshRenderer = m_Renderer as SkinnedMeshRenderer;
		m_Explosion.Emit(explosionCount);
		ParticleSystem.ShapeModule shape2 = m_Drops.shape;
		shape2.skinnedMeshRenderer = m_Renderer as SkinnedMeshRenderer;
		m_Drops.Emit(dropCount);
	}

	private void SendOnExplode()
	{
		this.OnExplode.Send(this);
		Dispose();
	}

	public void Reset()
	{
		this.OnExplode = null;
		m_Renderer.material.SetFloat("_MeltPercentage", 0f);
		m_Renderer.enabled = true;
	}

	protected override void OnDisposed()
	{
		m_Renderer = null;
		this.OnExplode = null;
		base.OnDisposed();
	}
}
