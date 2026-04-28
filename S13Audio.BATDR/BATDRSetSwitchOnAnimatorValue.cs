using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRSetSwitchOnAnimatorValue : MonoBehaviour
{
	public Animator animator;

	public string animatorParamName = string.Empty;

	[Header("Values")]
	[ReadOnly]
	public float floatValue;

	[Header("SwitchSettings")]
	[SerializeField]
	private S13SetSwitchOnParameter setSwitchReference;

	private int paramIndex = -1;

	private void Start()
	{
		if (!animator)
		{
			base.enabled = false;
			return;
		}
		AnimatorControllerParameter[] parameters = animator.parameters;
		for (int i = 0; i < parameters.Length; i++)
		{
			if (parameters[i].name == animatorParamName)
			{
				paramIndex = i;
				break;
			}
		}
	}

	private void Update()
	{
		if (paramIndex >= 0)
		{
			floatValue = animator.GetFloat(animatorParamName);
			if (setSwitchReference != null)
			{
				setSwitchReference.SetValue(floatValue);
			}
		}
	}
}
