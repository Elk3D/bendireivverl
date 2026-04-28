using System;
using System.Collections.Generic;
using UnityEngine;

public class DummyManager : JDisposable
{
	public const int ACTIVE_DUMMY_LIMIT = 20;

	public List<Dummy> Dummies = new List<Dummy>();

	public bool CanSpawn => Dummies.Count < 20;

	public static DummyManager Create()
	{
		return new DummyManager();
	}

	public void AddDummy(Dummy dummy)
	{
		if (!Dummies.Contains(dummy))
		{
			dummy.OnDeath -= HandleDummyOnDeath;
			dummy.OnDeath += HandleDummyOnDeath;
			Dummies.Add(dummy);
		}
	}

	private void HandleDummyOnDeath(object sender, EventArgs e)
	{
		Dummy dummy = sender as Dummy;
		dummy.OnDeath -= HandleDummyOnDeath;
		RemoveDummy(dummy);
		dummy.Dispose();
	}

	public void RemoveDummy(Dummy dummy)
	{
		if (Dummies.Contains(dummy))
		{
			Dummies.Remove(dummy);
		}
	}

	public void RemoveDummiesOf(Transform transform)
	{
		for (int num = Dummies.Count - 1; num >= 0; num--)
		{
			Dummy dummy = Dummies[num];
			if (dummy.Target == transform)
			{
				dummy.ForceKil();
			}
		}
	}

	public Dummy GetClosest(Transform transform)
	{
		Dummy result = null;
		float num = float.PositiveInfinity;
		Vector3 position = transform.position;
		foreach (Dummy dummy in Dummies)
		{
			if (!(dummy == null) && !(dummy.transform == transform) && !dummy.IsDisposed)
			{
				float num2 = Vector3.Distance(dummy.transform.position, position);
				if (num2 < num)
				{
					result = dummy;
					num = num2;
				}
			}
		}
		return result;
	}

	public void KillAll()
	{
		for (int num = Dummies.Count - 1; num >= 0; num--)
		{
			Dummies[num].ForceKil();
		}
	}

	protected override void OnDisposed()
	{
		if (Dummies != null)
		{
			Dummies.Clear();
			Dummies = null;
		}
		base.OnDisposed();
	}
}
