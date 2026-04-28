public class UIElementAudioLogDataVO : UIElementDataVO
{
	public AudioLogID ID;

	public UIElementButtonDataVO Button;

	public UIElementAudioLogDataVO(string prefabKey, AudioLogID id, UIElementButtonDataVO button)
		: base(prefabKey)
	{
		ID = id;
		Button = button;
	}
}
