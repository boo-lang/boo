namespace Statsia
import System
import System.Collections.Generic
import System.Linq.Enumerable
import MathNet.Numerics

// Static class that will be imported.
public static class Math:
	
	def Abs(x as decimal):
		return System.Math.Abs(x)

	def Abs(x as double):
		return System.Math.Abs(x)
	
	def Abs(x as Int16):
    	return System.Math.Abs(x)
    
    def Abs(x as Int32):
    	return System.Math.Abs(x)
    
  	def Abs(x as Int64):
  		return System.Math.Abs(x)
  	
  	def Abs(x as SByte):
  		return System.Math.Abs(x)
  
  	def Abs(x as Single):
  		return System.Math.Abs(x)
  		  	
  	def Acos(x as double):
  		return System.Math.Acos(x)
  	
  	def Asin(x as double):
  		return System.Math.Asin(x)
  	
  	def Atan(x as double):
  		return System.Math.Atan(x)
  	
  	def Atan2(y as double, x):
  		return System.Math.Atan2(y, x)

	def BigMul(a as Int32, b as Int32):
		return System.Math.BigMul(a, b)
	
	def Ceiling(x as decimal):
		return System.Math.Ceiling(x)
	
	def Ceiling(x as double):
		return System.Math.Ceiling(x)
	
	def Cos(x as double):
		return System.Math.Cos(x)
	
	def Cosh(x as double):
		return System.Math.Cosh(x)
	
	def DivRem(a as Int32, b as Int32, result as Int32):
		return System.Math.DivRem(a, b, result)
	
	def DivRem(a as Int64, b as Int64, result as Int64):
		return System.Math.DivRem(a, b, result)
	
	def Exp(x as double):
		return System.Math.Exp(x)
	
	def Floor(x as decimal):
		return System.Math.Floor(x)
	
	def Floor(x as double):
		return System.Math.Floor(x)
	
	def IEEERemainer(a as double, b as double):
		return System.Math.IEEERemainder(a, b)
	
	def Log(x as double):
		return System.Math.Log(x)
	
	def Log(x as double, base as double):
		return System.Math.Log(x, base)
	
	def Log10(x as double):
		return System.Math.Log10(x)
	
	def Max(a as byte, b as byte):
		return System.Math.Max(a, b)
	
	def Max(a as decimal, b as decimal):
		return System.Math.Max(a, b)
	
	def Max(a as Int16, b as Int16):
		return System.Math.Max(a, b)
	
	def Max(a as Int32, b as Int32):
		return System.Math.Max(a, b)
		
	def Max(a as Int64, b as Int64):
		return System.Math.Max(a, b)
		
	def Max(a as SByte, b as SByte):
		return System.Math.Max(a, b)
	
	def Max(a as Single, b as Single):
		return System.Math.Max(a, b)
		
	def Max(a as UInt16, b as UInt16):
		return System.Math.Max(a, b)

	def Max(a as UInt32, b as UInt32):
		return System.Math.Max(a, b)

	def Max(a as UInt64, b as UInt64):
		return System.Math.Max(a, b)

	def Min(a as byte, b as byte):
		return System.Math.Min(a, b)
	
	def Min(a as decimal, b as decimal):
		return System.Math.Min(a, b)
	
	def Min(a as Int16, b as Int16):
		return System.Math.Min(a, b)
	
	def Min(a as Int32, b as Int32):
		return System.Math.Min(a, b)
		
	def Min(a as Int64, b as Int64):
		return System.Math.Min(a, b)
		
	def Min(a as SByte, b as SByte):
		return System.Math.Min(a, b)
	
	def Min(a as Single, b as Single):
		return System.Math.Min(a, b)
		
	def Min(a as UInt16, b as UInt16):
		return System.Math.Min(a, b)

	def Min(a as UInt32, b as UInt32):
		return System.Math.Min(a, b)

	def Min(a as UInt64, b as UInt64):
		return System.Math.Min(a, b)
	
	def Pow(x as double, y as double):
		return System.Math.Pow(x, y)
	
	def Round(x as decimal):
		return System.Math.Round(x)
	
	def Round(x as double):
		return System.Math.Round(x)
	
	def Round(x as decimal, decimals as int):
		return System.Math.Round(x, decimals)
	
	def Round(x as double, decimals as int):
		return System.Math.Round(x, decimals)

	def Sign(x as decimal):
		return System.Math.Sign(x)
	
	def Sign(a as double):
		return System.Math.Sign(a)
		
	def Sign(x as Int16):
		return System.Math.Sign(x)
		
	def Sign(x as Int32):
		return System.Math.Sign(x)
		
	def Sign(a as Int64):
		return System.Math.Sign(a)
		
	def Sign(a as SByte):
		return System.Math.Sign(a)
	
	def Sign(a as Single):
		return System.Math.Sign(a)
	
	def Sin(x as double):
		return System.Math.Sin(x)
	
	def Sinh(x as double):
		return System.Math.Sinh(x)
	
	def Sqrt(x as double):
		return System.Math.Sqrt(x)

	def Tan(x as double):
		return System.Math.Tan(x)
	
	def Tanh(x as double):
		return System.Math.Tanh(x)
	
	def Truncate(d as decimal):
		return System.Math.Truncate(d)
	
	def Truncate(x as double):
		return System.Math.Truncate(x)
	
	public final PI as double = System.Math.PI	

	public final Inf as double = double.PositiveInfinity
	
	public final NaN as double = double.NaN
	
	// def special functions	
	// random functions
	def runiform():
		return Statsia.Control.RandomSource.NextDouble()
	
	def Beta(a as double, b as double) as Statsia.Random.IRandomVariable[of double]:
		return Statsia.Random.Beta(a, b)

	def Beta(n as int, a as double, b as double) as IList[of Statsia.Random.IRandomVariable[of double]]:
		result = List[of Statsia.Random.IRandomVariable[of double]]()
		for i in Boo.Lang.Builtins.range(n):
			result.Add(Statsia.Random.Beta(a, b))
		return result
	
	[Extension]
	def Var(source as IEnumerable[of double]):
		avg = source.Average()
		d = source.Aggregate(0.0, {total, next | total += System.Math.Pow(next - avg, 2) })
		return d / (source.Count() - 1)
	
	def runiform(min as double, max as double):
		return runiform()*(max - min) + min
	
	// [min, max) exclusive.
	def rint(min as int, max as int):
		return Statsia.Control.RandomSource.Next(min, max)
	
	// Some list extensions.
	[Extension]
	def Sample[of T](x as IList[of T]) as T:
		if x.Count == 0:
			raise IndexOutOfRangeException("No elements to sample.")
		else:
			return x[rint(0, x.Count)]
				
	[Extension]
	def Sample[of T](x as (T)) as T:
		if x.Length == 0:
			raise IndexOutOfRangeException("No elements to sample.")
		else:
			return x[rint(0, x.Length)]
	
