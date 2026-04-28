using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio.BATDR;

[DefaultExecutionOrder(-20)]
public class S13MixerReference : MonoBehaviour
{
	[SerializeField]
	private AudioMixer masterMixer;

	[SerializeField]
	private AudioMixerGroup placeholderGroup;

	private static S13MixerReference _Instance;

	public static S13MixerReference Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = Object.FindObjectOfType<S13MixerReference>();
				if (!_Instance)
				{
					Debug.LogError("S13MixerReference does not exist within scene. One must be added in this or previously loaded scene to handle references");
				}
			}
			return _Instance;
		}
	}

	public static AudioMixer MasterMixer => Instance.masterMixer;

	public static AudioMixerGroup PlaceholderMixerGroup => Instance.placeholderGroup;
}
