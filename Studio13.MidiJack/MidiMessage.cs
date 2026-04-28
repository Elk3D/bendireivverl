namespace Studio13.MidiJack;

public struct MidiMessage(ulong data)
{
	public uint source = (uint)(data & 0xFFFFFFFFu);

	public byte status = (byte)((data >> 32) & 0xFF);

	public byte data1 = (byte)((data >> 40) & 0xFF);

	public byte data2 = (byte)((data >> 48) & 0xFF);

	public override string ToString()
	{
		return $"s({status:X2}) d({data1:X2},{data2:X2}) from {source:X8}";
	}
}
