using UnityEngine;
using UnityEngine.EventSystems;

public class JInputModule : StandaloneInputModule
{
	private Vector2 m_cursorPos;

	private readonly MouseState m_MouseState = new MouseState();

	protected override MouseState GetMousePointerEventData(int id = 0)
	{
		PointerEventData data;
		bool pointerData = GetPointerData(-1, out data, create: true);
		data.Reset();
		if (pointerData)
		{
			data.position = Input.mousePosition;
		}
		Vector2 vector = Input.mousePosition;
		data.delta = vector - data.position;
		data.position = vector;
		data.scrollDelta = Input.mouseScrollDelta;
		data.button = PointerEventData.InputButton.Left;
		base.eventSystem.RaycastAll(data, m_RaycastResultCache);
		RaycastResult pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
		data.pointerCurrentRaycast = pointerCurrentRaycast;
		m_RaycastResultCache.Clear();
		GetPointerData(-2, out var data2, create: true);
		CopyFromTo(data, data2);
		data2.button = PointerEventData.InputButton.Right;
		GetPointerData(-3, out var data3, create: true);
		CopyFromTo(data, data3);
		data3.button = PointerEventData.InputButton.Middle;
		m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), data);
		m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), data2);
		m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), data3);
		return m_MouseState;
	}

	private void GetSelectState(PointerEventData data, PointerEventData.InputButton inputButton, bool wasPressed, bool wasReleased)
	{
		PointerEventData.FramePressState stateForMouseButton = PointerEventData.FramePressState.NotChanged;
		if (wasPressed)
		{
			stateForMouseButton = PointerEventData.FramePressState.Pressed;
		}
		else if (wasReleased)
		{
			stateForMouseButton = PointerEventData.FramePressState.Released;
		}
		m_MouseState.SetButtonState(inputButton, stateForMouseButton, data);
	}
}
