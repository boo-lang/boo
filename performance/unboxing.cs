using System;
using System.Globalization;

public class App
{
	const int Iterations = 10000000;
	
	public static void Foo(int value)
	{
	}
	
	public static void Main(string[] args)
	{
		for (int i=0; i<3; ++i)
		{
			Test();
		}
	}
	
	static void Test()
	{
		object o = 3;
		
		DateTime start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Foo((int)o);
		}
		Report("raw unboxing", start);
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Foo(UnboxInt32(o));
		}
		Report("boo unboxing", start);
		
		o = 3L;
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Foo(UnboxInt32(o));
		}
		Report("boo long->int", start);
	}
	
	static void Report(string name, DateTime start)
	{
		TimeSpan elapsed = DateTime.Now - start;
		Console.WriteLine("{0}:\t{1}\t{2} ops/ms", name, elapsed, Iterations/elapsed.TotalMilliseconds);
	}
	
	static IConvertible CheckNumericPromotion(object value)
	{
		IConvertible convertible = (IConvertible)value;
		switch (convertible.GetTypeCode())
		{
			case TypeCode.Byte: return convertible;
			case TypeCode.SByte: return convertible;
			case TypeCode.Int16: return convertible;
			case TypeCode.Int32: return convertible;
			case TypeCode.Int64: return convertible;
			case TypeCode.UInt16: return convertible;
			case TypeCode.UInt32: return convertible;
			case TypeCode.UInt64: return convertible;
			case TypeCode.Single: return convertible;
			case TypeCode.Double: return convertible;
			case TypeCode.Boolean: return convertible;
		}
		throw new InvalidCastException();
	}
	
	static CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
	
	public static Int32 UnboxInt32(object value)
	{
		return CheckNumericPromotion(value).ToInt32(InvariantCulture);
	}
}
