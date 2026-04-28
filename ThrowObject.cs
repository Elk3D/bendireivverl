using System;
using DG.Tweening;
using UnityEngine;

public class ThrowObject : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Content;

	[SerializeField]
	private GameObject m_Particles;

	[SerializeField]
	private Collider m_Collider;

	[SerializeField]
	private bool m_DestroyOnImpact;

	private bool m_IsHit;

	private ParticleSystem[] m_ParticleSystems;

	private Sequence m_Sequence;

	public GameObject Content => m_Content;

	public GameObject Particles => m_Particles;

	public Collider Collider => m_Collider;

	public event EventHandler OnDrag;

	public event EventHandler OnLand;

	public event EventHandler OnThrow;

	public event EventHandler OnDragLong;

	public override void Start()
	{
		InitializeParticles();
	}

	public void Initialize()
	{
		m_IsHit = false;
		if (m_Content != null)
		{
			m_Content.SetActive(value: true);
		}
		if (m_Collider != null)
		{
			m_Collider.enabled = true;
		}
		InitializeParticles();
	}

	private void InitializeParticles()
	{
		if (m_Particles != null && m_ParticleSystems == null)
		{
			m_ParticleSystems = m_Particles.GetComponentsInChildren<ParticleSystem>();
		}
		DisableParticles();
	}

	public void Throw(Vector3 startPosition, Vector3 toPosition)
	{
		ResetSequence();
		base.transform.position = startPosition;
		base.transform.eulerAngles = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(20f, 360f);
		Vector3 endValue = new Vector3(360f, 720f, 0f);
		m_Sequence.SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
		m_Sequence.Insert(0f, base.transform.DOLocalRotate(endValue, 0.85f, RotateMode.LocalAxisAdd));
		m_Sequence.Insert(0f, base.transform.DOMove(toPosition, 0.85f));
		m_Sequence.OnComplete(ThrowOnComplete);
	}

	private void ThrowOnComplete()
	{
		if (m_Content != null)
		{
			m_Content.SetActive(value: false);
		}
		EnableParticles();
		Land();
	}

	public void EnableParticles()
	{
		SetParticles(play: true);
	}

	public void DisableParticles()
	{
		SetParticles(play: false);
	}

	private void SetParticles(bool play)
	{
		if (m_ParticleSystems == null)
		{
			return;
		}
		for (int i = 0; i < m_ParticleSystems.Length; i++)
		{
			ParticleSystem particleSystem = m_ParticleSystems[i];
			if (play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	public void Drag()
	{
		CameraEffects.ShakeRotation(0.5f, 1f, 10, 90f, fadeOut: false);
		this.OnDrag.Send(this);
	}

	public void Land()
	{
		if (m_Collider != null)
		{
			m_Collider.enabled = false;
		}
		m_IsHit = true;
		EnableParticles();
		CameraEffects.ShakeRotation(0.5f, 2f);
		this.OnLand.Send(this);
	}

	public void Throw()
	{
		this.OnThrow.Send(this);
	}

	public void DragLong()
	{
		CameraEffects.ShakeRotation(3.5f, 1f);
		this.OnDragLong.Send(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_IsHit && other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			m_IsHit = true;
			RaycastHit hit = new RaycastHit
			{
				point = other.transform.position
			};
			Vector3 point = hit.point;
			point.y = GameManager.Instance.Player.transform.position.y - 0.5f;
			GameManager.Instance.Player.AddForce((point - base.transform.position).normalized * 20f);
			GameManager.Instance.Player.Hit(hit, null, DamageCheck.ThrowableGear());
			if (m_DestroyOnImpact)
			{
				KillSequence();
				ThrowOnComplete();
			}
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			m_Sequence.Kill();
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.OnDrag = null;
		this.OnLand = null;
		this.OnThrow = null;
		this.OnDragLong = null;
		KillSequence();
		base.OnDisposed();
	}
}
