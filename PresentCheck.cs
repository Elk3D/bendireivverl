public static class PresentCheck
{
	public static int PresentCount;

	public static void Collect()
	{
		PresentCount++;
	}

	public static void Reset()
	{
		PresentCount = 0;
	}
}
