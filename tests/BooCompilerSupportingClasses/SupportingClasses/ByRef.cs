namespace BooCompiler.Tests.SupportingClasses;

public class ByRef
{
	public static void SetValue(int value, ref int output) => output = value;

	public static void SetRef(object value, ref object output) => output = value;

	public static void ReturnValue(int value, out int output) => output = value;

	public static void ReturnRef(object value, out object output) => output = value;
}

public class RefReturn
{
	private int _value;
	private readonly int[] _items = new int[3];

	public ref int Value => ref _value;

	public ref readonly int ReadOnlyValue => ref _value;

	public ref int Item(int index) => ref _items[index];

	public int Read() => _value;

	public int ReadItem(int index) => _items[index];
}
