public class GenericLoaderDataVO : JDisposable
{
	public string SceneName;

	public GenericLoaderDataVO(string sceneName)
	{
		SceneName = sceneName;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
