// Static helpers for triggering camera effects from anywhere in the project.
// Routed through FPCamera.Instance so no scene reference is needed.
public static class FPCameraEffects
{
    public static void ShakeRotation(float duration, float strength = 10f, int vibrato = 10,
                                     float randomness = 90f, bool fadeOut = true, bool vibrate = false)
        => FPCamera.Instance?.ShakeCamera(duration, strength, vibrato, randomness, fadeOut, vibrate);
}
