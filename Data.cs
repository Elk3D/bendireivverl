using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data<Key, Value> : Data where Key : IConvertible where Value : IDataObject<Key>
{
	[SerializeField]
	private string m_Name = string.Empty;

	[SerializeField]
	private List<Key> m_Keys = new List<Key>();

	[SerializeField]
	private List<Value> m_Values = new List<Value>();

	public override string Name => m_Name;

	public List<Key> Keys => m_Keys;

	public List<Value> Values => m_Values;

	public override int Count => m_Keys.Count;

	public Data(string name)
	{
		m_Name = name;
	}

	public override bool Add(object key, object value)
	{
		return InternalAdd((Key)key, (Value)value);
	}

	private bool InternalAdd(Key key, Value value)
	{
		bool flag = key != null && value != null;
		if (flag && !m_Keys.Contains(key))
		{
			m_Keys.Add(key);
			m_Keys.Sort();
			m_Values.Insert(m_Keys.IndexOf(key), value);
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public override void Remove(object value)
	{
		InternalRemove((Value)value);
	}

	private void InternalRemove(Value value)
	{
		if (m_Values.Contains(value))
		{
			int index = m_Values.IndexOf(value);
			m_Keys.RemoveAt(index);
			m_Values.RemoveAt(index);
		}
	}

	public override bool ChangeValue(object key, object value)
	{
		return InternalChangeValue((Key)key, (Value)value);
	}

	private bool InternalChangeValue(Key key, Value value)
	{
		if (key == null || value == null)
		{
			return false;
		}
		if (m_Keys.Contains(key))
		{
			int index = m_Keys.IndexOf(key);
			m_Values[index] = value;
			return true;
		}
		return InternalAdd(key, value);
	}

	public override object GetValue(object key)
	{
		return InternalGetValue((Key)key);
	}

	private Value InternalGetValue(Key key)
	{
		Value result = default(Value);
		if (m_Keys.Contains(key))
		{
			int index = m_Keys.IndexOf(key);
			return m_Values[index];
		}
		return result;
	}

	public override object GetValueOfIndex(int index)
	{
		return InternalGetValueOfIndex(index);
	}

	private Value InternalGetValueOfIndex(int index)
	{
		Value result = default(Value);
		if (index < m_Values.Count)
		{
			return m_Values[index];
		}
		return result;
	}

	public override bool IndexOf(object value, out int index)
	{
		return InternalIndexOf((Value)value, out index);
	}

	private bool InternalIndexOf(Value value, out int index)
	{
		bool num = value != null;
		if (num)
		{
			index = m_Values.IndexOf(value);
			return num;
		}
		index = 0;
		return num;
	}

	public override bool ContainsKey(object key)
	{
		return InternalContainsKey((Key)key);
	}

	private bool InternalContainsKey(Key key)
	{
		return m_Keys.Contains(key);
	}

	public override bool ContainsValue(object value)
	{
		return InternalContainsValue((Value)value);
	}

	private bool InternalContainsValue(Value value)
	{
		return m_Values.Contains(value);
	}

	public override void Clear()
	{
		m_Keys.Clear();
		m_Values.Clear();
	}
}
[Serializable]
public abstract class Data
{
	public abstract string Name { get; }

	public abstract int Count { get; }

	public abstract bool Add(object key, object value);

	public abstract void Remove(object value);

	public abstract bool ChangeValue(object key, object value);

	public abstract object GetValue(object key);

	public abstract object GetValueOfIndex(int index);

	public abstract bool IndexOf(object value, out int index);

	public abstract bool ContainsKey(object key);

	public abstract bool ContainsValue(object value);

	public abstract void Clear();
}
