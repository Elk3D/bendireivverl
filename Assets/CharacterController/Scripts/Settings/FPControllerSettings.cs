using UnityEngine;

[CreateAssetMenu(fileName = "FPControllerSettings", menuName = "FP Controller/Settings")]
public class FPControllerSettings : ScriptableObject
{
    [Header("Camera")]
    [Tooltip("Interpolate camera pivot position for smoother headbob.")]
    public bool SmoothCamera = true;

    [Tooltip("Enable procedural camera sway while moving.")]
    public bool ViewSwaying = true;

    [Header("Movement")]
    [Tooltip("Seconds of continuous sprinting before the run locks out (stamina).")]
    public float Stamina = 5f;

    [Header("Input")]
    [Tooltip("Mouse/stick look sensitivity multiplier.")]
    public float Sensitivity = 1f;

    [Tooltip("Invert the vertical look axis.")]
    public bool InvertY = false;
}
