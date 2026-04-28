using System;
using System.Collections.Generic;
using UnityEngine;

public class RiverWaves : MonoBehaviour
{
	[Serializable]
	public class WaveContainer
	{
		public Vector3 Center;

		public float Amplitude;

		public float Speed = 2f;

		public float Frequency = 0.04f;

		internal float ScaledDistance;

		internal float Timer;

		public WaveContainer(Vector3 _Position, float _Amplitude = 2f, float _Frequency = 0.04f, float _Speed = 2f)
		{
			Center = _Position;
			Amplitude = _Amplitude;
			Frequency = _Frequency;
			Speed = _Speed;
		}
	}

	public List<WaveContainer> CurrentWaves = new List<WaveContainer>();

	private Mesh WaterMesh;

	private MeshCollider WaterMeshCollider;

	[SerializeField]
	private float waveScale;

	[SerializeField]
	private float Turbulance_A;

	[SerializeField]
	private float Turbulance_B;

	[SerializeField]
	private float Speed;

	private Vector3[] StartingVertPositions;

	private Vector3[] newVertPositions;

	private void Start()
	{
		WaterMesh = GetComponent<MeshFilter>().mesh;
		WaterMeshCollider = GetComponent<MeshCollider>();
		StartingVertPositions = WaterMesh.vertices;
		newVertPositions = (Vector3[])StartingVertPositions.Clone();
	}

	public void AddWave(Vector3 _Position, float _Amplitude, float _Frequency = 0.04f, float _Speed = 2f)
	{
		CurrentWaves.Add(new WaveContainer(_Position, _Amplitude, _Frequency, _Speed));
	}

	private void Update()
	{
		for (int num = CurrentWaves.Count - 1; num >= 0; num--)
		{
			WaveContainer waveContainer = CurrentWaves[num];
			waveContainer.Timer += Time.deltaTime;
			waveContainer.ScaledDistance += Time.deltaTime * 10f;
			waveContainer.Amplitude = Mathf.Lerp(waveContainer.Amplitude, 0f, waveContainer.ScaledDistance / 3000f * waveContainer.Speed);
			waveContainer.Frequency = Mathf.Lerp(waveContainer.Frequency, 0f, waveContainer.ScaledDistance / 3000f * waveContainer.Speed);
			if ((double)waveContainer.Amplitude < 0.01)
			{
				CurrentWaves.Remove(waveContainer);
			}
		}
		for (int i = 0; i < StartingVertPositions.Length; i++)
		{
			Vector3 vector = StartingVertPositions[i];
			Vector3 vector2 = base.transform.TransformPoint(StartingVertPositions[i]);
			vector.y = Mathf.Sin(vector2.x * 1.2f * Turbulance_B + vector2.z * -0.75f * Turbulance_A + Time.time * Speed * Turbulance_A) * waveScale;
			vector.y += Mathf.Sin(vector2.x * -1.16f * Turbulance_A + vector2.z * 1.24f * Turbulance_B + Time.time * Speed * Turbulance_B) * waveScale;
			vector.y += Mathf.Sin(vector2.x + vector2.z) * waveScale;
			for (int j = 0; j < CurrentWaves.Count; j++)
			{
				WaveContainer waveContainer2 = CurrentWaves[j];
				float num2 = Mathf.Clamp01(1f - (vector2 - waveContainer2.Center).magnitude / waveContainer2.ScaledDistance);
				float num3 = (vector2.x - waveContainer2.Center.x) * (vector2.x - waveContainer2.Center.x);
				float num4 = (vector2.z - waveContainer2.Center.z) * (vector2.z - waveContainer2.Center.z);
				vector.y += waveContainer2.Amplitude * (Mathf.Sin((num3 + num4) * waveContainer2.Frequency) + Mathf.Sin(waveContainer2.Timer)) * num2;
			}
			newVertPositions[i] = vector;
		}
		WaterMesh.vertices = newVertPositions;
		WaterMesh.RecalculateNormals();
		if ((bool)WaterMeshCollider)
		{
			WaterMeshCollider.sharedMesh = WaterMesh;
		}
	}
}
