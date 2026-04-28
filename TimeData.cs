using System;
using UnityEngine;

[Serializable]
public class TimeData
{
	[SerializeField]
	private long m_Date;

	[SerializeField]
	private double m_TimePlayed;

	public long Date => m_Date;

	public double TimePlayed => m_TimePlayed;

	public TimeData()
	{
		m_Date = DateTime.Now.Ticks;
		m_TimePlayed = 0.0;
	}

	public TimeData(TimeData copy)
	{
		m_Date = copy.Date;
		m_TimePlayed = copy.TimePlayed;
	}

	public void Update()
	{
		m_TimePlayed += (DateTime.Now - new DateTime(m_Date)).TotalSeconds;
		m_Date = DateTime.Now.Ticks;
	}
}
