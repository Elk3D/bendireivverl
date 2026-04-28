using System.Collections;

namespace DG.Tweening;

public class Sequencer : JDisposable
{
	private Sequence m_Sequence;

	private UpdateType m_UpdateType;

	private bool m_IsIndependent;

	public bool IsPlaying
	{
		get
		{
			if (m_Sequence != null)
			{
				if (m_Sequence.IsActive())
				{
					return m_Sequence.IsPlaying();
				}
				return false;
			}
			return false;
		}
	}

	public Sequencer(bool isIndependent = false, UpdateType updateType = UpdateType.Normal)
	{
		m_IsIndependent = isIndependent;
		m_UpdateType = updateType;
	}

	public Sequencer CreateSequence(params Tween[] tweens)
	{
		RestartSequence();
		IEnumerator enumerator = tweens.GetEnumerator();
		while (enumerator.MoveNext())
		{
			m_Sequence?.Insert(0f, enumerator.Current as Tween);
		}
		return this;
	}

	public Sequencer New()
	{
		Kill();
		return this;
	}

	public Sequencer Insert(float atPosition, Tween tween)
	{
		if (m_Sequence == null)
		{
			RestartSequence();
		}
		m_Sequence?.Insert(atPosition, tween);
		return this;
	}

	public Sequencer Insert(float atPosition, TweenCallback callback)
	{
		if (m_Sequence == null)
		{
			RestartSequence();
		}
		m_Sequence?.InsertCallback(atPosition, callback);
		return this;
	}

	public Sequencer OnComplete(TweenCallback action)
	{
		m_Sequence?.OnComplete(action);
		return this;
	}

	public void RestartSequence()
	{
		Kill();
		m_Sequence = DOTween.Sequence().SetUpdate(m_UpdateType, m_IsIndependent);
	}

	public void Kill()
	{
		m_Sequence?.Kill();
		m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		Kill();
		base.OnDisposed();
	}
}
