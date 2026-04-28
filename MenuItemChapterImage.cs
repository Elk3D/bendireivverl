using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemChapterImage : JMonoBehaviour
{
	[SerializeField]
	private Image m_Overlay;

	[SerializeField]
	private Image m_Keys;

	[SerializeField]
	private Localize m_LockLbl;

	public void Init(bool isUnlocked)
	{
		m_Overlay.enabled = !isUnlocked;
		m_Keys.enabled = !isUnlocked;
		m_LockLbl.gameObject.SetActive(!isUnlocked);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
