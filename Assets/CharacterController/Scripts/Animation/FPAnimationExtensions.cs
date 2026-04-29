using UnityEngine;

public static class FPAnimationExtensions
{
    public static bool FPContains(this Animator animator, string name)
    {
        foreach (var p in animator.parameters)
            if (p.name == name) return true;
        return false;
    }

    public static void SetMovementState(this Animator a, float v, bool smooth = true, float damp = 0.1f) => FPSetFloat(a, "MovementState", v, smooth, damp);
    public static void SetMovementSpeed(this Animator a, float v, bool smooth = true, float damp = 0.1f) => FPSetFloat(a, "MovementSpeed", v, smooth, damp);
    public static void SetShimmySpeed(this Animator a, float v, bool smooth = true, float damp = 0.1f) => FPSetFloat(a, "ShimmySpeed", v, smooth, damp);
    public static void SetStrafeSpeed(this Animator a, float v, bool smooth = true, float damp = 0.1f) => FPSetFloat(a, "StrafeSpeed", v, smooth, damp);
    public static void SetCrouchState(this Animator a, float v, bool smooth = true, float damp = 0.1f) => FPSetFloat(a, "CrouchState", v, smooth, damp);
    public static void SetCrouchSpeed(this Animator a, float v, bool smooth = true, float damp = 0.1f) => FPSetFloat(a, "CrouchSpeed", v, smooth, damp);

    private static void FPSetFloat(Animator a, string name, float v, bool smooth, float damp)
    {
        if (!a.FPContains(name)) return;
        if (smooth)
        {
            float cur = a.GetFloat(name);
            if (cur > v + 0.01f || cur < v - 0.01f) a.SetFloat(name, v, damp, Time.deltaTime);
            else a.SetFloat(name, v);
        }
        else a.SetFloat(name, v);
    }
}
