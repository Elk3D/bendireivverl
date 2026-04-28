using System;
using UnityEngine;

[CreateAssetMenu]
public class MouseAndKeyboardInputMappingGraphics : ScriptableObject
{
	[Serializable]
	public class InputToGfxMapping
	{
		public KeyCode keyCode;

		public Sprite gfx;
	}

	[SerializeField]
	private InputToGfxMapping[] mappings;

	public Sprite GetSpriteForInput(KeyCode keyCode)
	{
		InputToGfxMapping[] array = mappings;
		foreach (InputToGfxMapping inputToGfxMapping in array)
		{
			if (keyCode == inputToGfxMapping.keyCode)
			{
				return inputToGfxMapping.gfx;
			}
		}
		return null;
	}
}
