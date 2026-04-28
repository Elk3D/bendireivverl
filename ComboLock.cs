using System;
using UnityEngine;

public class ComboLock : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Code")]
	[SerializeField]
	private int m_Code = 414;

	[Header("Combo Lock")]
	[SerializeField]
	private InteractableComboLock m_Content;

	public int ID => m_ID;

	public InteractableComboLock Content => m_Content;

	public bool IsComplete { get; private set; }

	public event EventHandler OnComplete;

	public void Initialize()
	{
		m_Content.Initialize(m_Code);
		m_Content.OnSuccess += HandleComboLockOnSuccess;
	}

	public void InitializeCode(int code)
	{
		m_Content.InitializeCode(code, m_Code);
		m_Content.OnSuccess += HandleComboLockOnSuccess;
	}

	private void HandleComboLockOnSuccess(object sender, EventArgs e)
	{
		m_Content.OnSuccess -= HandleComboLockOnSuccess;
		IsComplete = true;
		this.OnComplete.Send(this);
	}

	public void ForceComplete()
	{
		m_Content.ForceComplete(m_Code);
		IsComplete = true;
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		m_Content.OnSuccess -= HandleComboLockOnSuccess;
		base.OnDisposed();
	}
}
