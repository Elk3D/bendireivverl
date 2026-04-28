using UnityEngine;

public class SeasonalHatLoader : JMonoBehaviour
{
	[SerializeField]
	private SeasonalType m_SeasonalType;

	[SerializeField]
	private GameObject m_Hat;

	[SerializeField]
	private bool m_IsPlayer;

	public override void Start()
	{
		if ((!m_IsPlayer || SeasonalCheck.HasHat) && SeasonalCheck.IsSeasonal(out var seasonalType) && GameManager.Instance.PlayerSettings.Seasonal && seasonalType == m_SeasonalType)
		{
			GameObject obj = Object.Instantiate(m_Hat);
			obj.transform.SetParent(base.transform);
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localEulerAngles = Vector3.zero;
		}
	}
}
