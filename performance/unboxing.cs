using System;

public class App
{
	const int Iterations = 10000000;
	
	public static void Foo(int value)
	{
	}
	
	public static void Main(string[] args)
	{
		for (int i=0; i<1; ++i)
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
		Report("raw unboxing   ", start);
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Foo(UnboxInt32(o));
		}
		Report("boo unboxing   ", start);
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Foo(UnboxInt32X(o));
		}
		Report("boo unboxing(X)", start);
		
		o = 3L;
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Foo(UnboxInt32(o));
		}
		Report("boo long->int  ", start);
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Foo(UnboxInt32X(o));
		}
		Report("boo long->int(X)", start);
		
		o = "3";
		start = DateTime.Now;
		for (int i=0; i<1000; ++i)
		{
			try { Foo((int)o); } catch (InvalidCastException) {} 
		}
		Report("raw failed cast", start, 1000);
		
		start = DateTime.Now;
		for (int i=0; i<1000; ++i)
		{
			try { UnboxInt32X(o); } catch (InvalidCastException) {}
		}
		Report("boo failed cast", start, 1000);		
	}
	
	static void Report(string name, DateTime start)
	{
		Report(name, start, Iterations);
	}
	
	static void Report(string name, DateTime start, int iterations)
	{
		TimeSpan elapsed = DateTime.Now - start;
		Console.WriteLine("{0}:\t{1}\t{2} ops/ms", name, elapsed, iterations/elapsed.TotalMilliseconds);
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
	
	public static Int32 UnboxInt32(object value)
	{
		return CheckNumericPromotion(value).ToInt32(null);
	}
	
	public static Int32 UnboxInt32X(object value)
	{
		if (value is Int32)
		{
			return (Int32)value;
		}
		else
		{
			return CheckNumericPromotion(value).ToInt32(null);
		}
	}
}
