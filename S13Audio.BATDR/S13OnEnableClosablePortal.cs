using UnityEngine;

namespace S13Audio.BATDR;

public class S13OnEnableClosablePortal : S13ClosablePortal
{
	[SerializeField]
	private bool onEnableValue = true;

	private bool _value = true;

	public override bool IsOpen()
	{
		return _value;
	}

	private void OnEnable()
	{
		_value = onEnableValue;
	}

	private void OnDisable()
	{
		_value = !onEnableValue;
	}
}
