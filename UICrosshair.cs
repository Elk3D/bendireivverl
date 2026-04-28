using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICrosshair : UIController
{
	[Header("Crosshairs")]
	[SerializeField]
	private CanvasGroup m_Cavnas;

	[SerializeField]
	private Image m_Default;

	[SerializeField]
	private Image m_Weapon;

	protected override void OnInitialized(object _data)
	{
		m_Cavnas.alpha = 0f;
	}

	public void Show()
	{
		CheckType(GameManager.Instance.Player.CurrentWeapon != null);
		m_Cavnas.gameObject.SetActive(value: true);
		m_Cavnas.DOKill();
		m_Cavnas.DOFade(0.9f, 0.5f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
	}

	public void Hide()
	{
		m_Cavnas.DOKill();
		m_Cavnas.DOFade(0f, 0.5f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true).OnComplete(ForceHide);
	}

	public void ForceHide()
	{
		m_Cavnas.gameObject.SetActive(value: false);
	}

	private void CheckType(bool hasWeapon)
	{
		if (m_Weapon != null)
		{
			m_Weapon.enabled = hasWeapon;
		}
		if (m_Default != null)
		{
			m_Default.enabled = !hasWeapon;
		}
	}

	protected override void OnDisposed()
	{
		m_Cavnas.DOKill();
		base.OnDisposed();
	}
}
