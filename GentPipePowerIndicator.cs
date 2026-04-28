using UnityEngine;

public class GentPipePowerIndicator : JMonoBehaviour
{
	[SerializeField]
	private GameObject[] m_PowerLevels;

	public int Power { get; private set; }

	public void SetPowerLevel(int level)
	{
		Power = level;
		if (Power >= m_PowerLevels.Length)
		{
			Power = m_PowerLevels.Length;
		}
		else if (Power <= 0)
		{
			Power = 0;
		}
		for (int i = 0; i < m_PowerLevels.Length; i++)
		{
			GameObject gameObject = m_PowerLevels[i];
			if (i >= level)
			{
				gameObject.SetActive(value: false);
			}
			else
			{
				gameObject.SetActive(value: true);
			}
		}
	}

	public void Initialize(GameObject[] powerLevels)
	{
		m_PowerLevels = powerLevels;
	}
}
