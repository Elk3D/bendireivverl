using System.Collections.Generic;

public class DataCategory
{
	public List<Data> Data { get; private set; }

	public DataCategory()
	{
		Data = new List<Data>();
	}

	protected void AddData(Data data)
	{
		if (!Data.Contains(data))
		{
			Data.Add(data);
		}
	}

	public bool GetData(string name, out Data data)
	{
		bool result = false;
		data = null;
		for (int i = 0; i < Data.Count; i++)
		{
			data = Data[i];
			if (data.Name == name)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
