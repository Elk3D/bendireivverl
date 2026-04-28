using System;
using UnityEngine;

public class CharacterNode : JMonoBehaviour
{
	[DisplayWithoutEdit]
	[SerializeField]
	private int m_ID;

	public int ID => m_ID;

	public Vector3 Position => base.transform.position;

	public virtual bool IsActive { get; protected set; }

	public event EventHandler OnReached;

	public void SetID(int id)
	{
		m_ID = id;
	}

	public void OnNodeReached()
	{
		InternalOnNodeReached();
		this.OnReached.Send(this);
	}

	protected virtual void InternalOnNodeReached()
	{
	}

	public virtual void SetActive(bool active)
	{
		IsActive = active;
	}

	protected override void OnDisposed()
	{
		this.OnReached = null;
		base.OnDisposed();
	}
}
