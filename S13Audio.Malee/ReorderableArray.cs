using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.Malee;

[Serializable]
public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	[SerializeField]
	private T[] array = new T[0];

	public T this[int index]
	{
		get
		{
			return array[index];
		}
		set
		{
			array[index] = value;
		}
	}

	public int Length => array.Length;

	public bool IsReadOnly => array.IsReadOnly;

	public int Count => array.Length;

	public ReorderableArray()
		: this(0)
	{
	}

	public ReorderableArray(int length)
	{
		array = new T[length];
	}

	public object Clone()
	{
		return array.Clone();
	}

	public bool Contains(T value)
	{
		return Array.IndexOf(array, value) >= 0;
	}

	public int IndexOf(T value)
	{
		return Array.IndexOf(array, value);
	}

	public void Insert(int index, T item)
	{
		List<T> list = new List<T>(array);
		list.Insert(index, item);
		Array.Resize(ref array, array.Length + 1);
		list.CopyTo(array);
	}

	public void RemoveAt(int index)
	{
		List<T> list = new List<T>(array);
		list.RemoveAt(index);
		Array.Resize(ref array, array.Length - 1);
		list.CopyTo(array);
	}

	public void Add(T item)
	{
		Array.Resize(ref array, array.Length + 1);
		array[array.Length - 1] = item;
	}

	public void Clear()
	{
		array = new T[0];
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		((ICollection<T>)this.array).CopyTo(array, arrayIndex);
	}

	public bool Remove(T item)
	{
		List<T> list = new List<T>(array);
		bool num = list.Remove(item);
		if (num)
		{
			Array.Resize(ref array, array.Length - 1);
			list.CopyTo(array);
		}
		return num;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>)array).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<T>)array).GetEnumerator();
	}

	public static implicit operator Array(ReorderableArray<T> reorderableArray)
	{
		return reorderableArray.array;
	}

	public static implicit operator T[](ReorderableArray<T> reorderableArray)
	{
		return reorderableArray.array;
	}
}
