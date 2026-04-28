using UnityEngine;

public class PlayerModelLayers : JMonoBehaviour
{
	[Header("Cut Scene")]
	[SerializeField]
	[LayerSelector]
	private string m_CutsceneLayer;

	[SerializeField]
	private GameObject[] m_CutsceneLayers;

	[Header("First Person")]
	[SerializeField]
	[LayerSelector]
	private string m_MirrorLayer;

	[SerializeField]
	private GameObject[] m_MirrorLayers;

	[Header("3rd Person")]
	[SerializeField]
	[LayerSelector]
	private string m_DefaultLayer;

	[SerializeField]
	private GameObject[] m_AllLayers;

	[Header("Head")]
	[SerializeField]
	[LayerSelector]
	private string m_HeadLayer;

	[SerializeField]
	private GameObject[] m_HeadLayers;

	[Header("Ability Arm")]
	[SerializeField]
	[LayerSelector]
	private string m_AbilityLayer;

	[SerializeField]
	private GameObject[] m_AbilityLayers;

	[Header("Weapon Arm")]
	[SerializeField]
	[LayerSelector]
	private string m_WeaponLayer;

	[SerializeField]
	private GameObject[] m_WeaponLayers;

	public void DisableHead()
	{
		SetLayer(ref m_HeadLayers, m_HeadLayer);
	}

	public void DisableAbility()
	{
		SetLayer(ref m_AbilityLayers, m_DefaultLayer);
	}

	public void DisableWeapon()
	{
		SetLayer(ref m_WeaponLayers, m_DefaultLayer);
	}

	public void EnableAll()
	{
		SetLayer(ref m_AllLayers, m_DefaultLayer);
	}

	public void EnableCutscene()
	{
		SetLayer(ref m_CutsceneLayers, m_CutsceneLayer);
	}

	public void EnableFirstPerson()
	{
		SetLayer(ref m_MirrorLayers, m_MirrorLayer);
	}

	public void EnableAbility()
	{
		SetLayer(ref m_AbilityLayers, m_AbilityLayer);
	}

	public void EnableWeapon()
	{
		SetLayer(ref m_WeaponLayers, m_WeaponLayer);
	}

	private void SetLayer(ref GameObject[] gameObjects, string layer)
	{
		for (int i = 0; i < gameObjects.Length; i++)
		{
			gameObjects[i].layer = LayerMask.NameToLayer(layer);
		}
	}
}
