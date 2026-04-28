using System;
using UnityEngine;

public class TextureSwapper : MonoBehaviour
{
	[Serializable]
	public class Emotion
	{
		public string name;

		public Texture[] textureFrames;

		public int currentFrame;

		public bool isReverse;
	}

	[SerializeField]
	private SkinnedMeshRenderer m_MeshRenderer;

	[SerializeField]
	private HeadTracker m_HeadTracker;

	[SerializeField]
	private Emotion[] m_Emotions;

	private bool blink;

	private bool sad;

	private bool close;

	private bool open;

	private bool cry;

	private bool shock;

	private void Update()
	{
		if (blink)
		{
			DoEmotion("blink", ref blink);
		}
		if (sad)
		{
			DoEmotion("sad", ref sad);
		}
		if (close)
		{
			DoEmotion("close", ref close);
		}
		if (open)
		{
			DoEmotion("open", ref open);
		}
		if (cry)
		{
			DoEmotion("cry", ref cry);
		}
		if (shock)
		{
			DoEmotion("shock", ref shock);
		}
	}

	public void Shock()
	{
		open = false;
		close = false;
		sad = false;
		cry = false;
		blink = false;
		shock = true;
	}

	public void Blink()
	{
		open = false;
		close = false;
		sad = false;
		cry = false;
		shock = false;
		blink = true;
	}

	public void Sad()
	{
		open = false;
		close = false;
		blink = false;
		cry = false;
		shock = false;
		sad = true;
	}

	public void Open()
	{
		close = false;
		blink = false;
		sad = false;
		cry = false;
		shock = false;
		open = true;
	}

	public void Close()
	{
		open = false;
		blink = false;
		sad = false;
		cry = false;
		shock = false;
		close = true;
	}

	public void Cry()
	{
		open = false;
		blink = false;
		sad = false;
		close = false;
		shock = false;
		cry = true;
	}

	public void DisableHeadTrack()
	{
		if (m_HeadTracker != null)
		{
			m_HeadTracker.SetIgnore(ignore: true);
		}
	}

	public void EnableHeadTrack()
	{
		if (m_HeadTracker != null)
		{
			m_HeadTracker.SetIgnore(ignore: false);
		}
	}

	private void DoEmotion(string emotionType, ref bool boolType)
	{
		Emotion[] emotions = m_Emotions;
		foreach (Emotion emotion in emotions)
		{
			if (!(emotion.name.ToLower() == emotionType))
			{
				continue;
			}
			for (int j = 0; j < m_MeshRenderer.materials.Length; j++)
			{
				if (emotion.currentFrame >= emotion.textureFrames.Length)
				{
					emotion.isReverse = true;
				}
				if (emotion.isReverse)
				{
					emotion.currentFrame--;
				}
				if (emotion.currentFrame < 0)
				{
					emotion.currentFrame = 0;
					boolType = false;
					emotion.isReverse = false;
				}
				m_MeshRenderer.materials[j].mainTexture = emotion.textureFrames[emotion.currentFrame];
				if (!emotion.isReverse & boolType)
				{
					emotion.currentFrame++;
				}
			}
		}
	}
}
