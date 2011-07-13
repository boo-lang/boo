namespace Statsia
import System
import System.Math
import MathNet.Numerics

// Static class that will be imported.
public static class Math:
	
	def abs(x as decimal):
		return Abs(x)

	def abs(x as double):
		return Abs(x)
	
	def abs(x as Int16):
    	return Abs(x)
    
    def abs(x as Int32):
    	return Abs(x)
    
  	def abs(x as Int64):
  		return Abs(x)
  	
  	def abs(x as SByte):
  		return Abs(x)
  
  	def abs(x as Single):
  		return Abs(x)
  		  	
  	def acos(x as double):
  		return Acos(x)
  	
  	def asin(x as double):
  		return Asin(x)
  	
  	def atan(x as double):
  		return Atan(x)
  	
  	def atan2(y as double, x):
  		return Atan2(y, x)

	def bigMul(a as Int32, b as Int32):
		return BigMul(a, b)
	
	def ceiling(x as decimal):
		return Ceiling(x)
	
	def ceiling(x as double):
		return Ceiling(x)
	
	def cos(x as double):
		return Cos(x)
	
	def cosh(x as double):
		return Cosh(x)
	
	def divRem(a as Int32, b as Int32, result as Int32):
		return DivRem(a, b, result)
	
	def divRem(a as Int64, b as Int64, result as Int64):
		return DivRem(a, b, result)
	
	def exp(x as double):
		return Exp(x)
	
	def floor(x as decimal):
		return Floor(x)
	
	def floor(x as double):
		return Floor(x)
	
	def IEEERemainer(a as double, b as double):
		return System.Math.IEEERemainder(a, b)
	
	def log(x as double):
		return Log(x)
	
	def log(x as double, base as double):
		return Log(x, base)
	
	def log10(x as double):
		return Log10(x)
	
	def max(a as byte, b as byte):
		return Max(a, b)
	
	def max(a as decimal, b as decimal):
		return Max(a, b)
	
	def max(a as Int16, b as Int16):
		return Max(a, b)
	
	def max(a as Int32, b as Int32):
		return Max(a, b)
		
	def max(a as Int64, b as Int64):
		return Max(a, b)
		
	def max(a as SByte, b as SByte):
		return Max(a, b)
	
	def max(a as Single, b as Single):
		return Max(a, b)
		
	def max(a as UInt16, b as UInt16):
		return Max(a, b)

	def max(a as UInt32, b as UInt32):
		return Max(a, b)

	def max(a as UInt64, b as UInt64):
		return Max(a, b)

	def min(a as byte, b as byte):
		return Min(a, b)
	
	def min(a as decimal, b as decimal):
		return Min(a, b)
	
	def min(a as Int16, b as Int16):
		return Min(a, b)
	
	def min(a as Int32, b as Int32):
		return Min(a, b)
		
	def min(a as Int64, b as Int64):
		return Min(a, b)
		
	def min(a as SByte, b as SByte):
		return Min(a, b)
	
	def min(a as Single, b as Single):
		return Min(a, b)
		
	def min(a as UInt16, b as UInt16):
		return Min(a, b)

	def min(a as UInt32, b as UInt32):
		return Min(a, b)

	def min(a as UInt64, b as UInt64):
		return Min(a, b)
	
	def pow(x as double, y as double):
		return Pow(x, y)
	
	def round(x as decimal):
		return Round(x)
	
	def round(x as double):
		return Round(x)
	
	def round(x as decimal, decimals as int):
		return Round(x, decimals)
	
	def round(x as double, decimals as int):
		return Round(x, decimals)

	def sign(x as decimal):
		return Sign(x)
	
	def sign(a as double):
		return Sign(a)
		
	def sign(x as Int16):
		return Sign(x)
		
	def sign(x as Int32):
		return Sign(x)
		
	def sign(a as Int64):
		return Sign(a)
		
	def sign(a as SByte):
		return Sign(a)
	
	def sign(a as Single):
		return Sign(a)
	
	def sin(x as double):
		return Sin(x)
	
	def sinh(x as double):
		return Sinh(x)
	
	def sqrt(x as double):
		return Sqrt(x)

	def tan(x as double):
		return Tan(x)
	
	def tanh(x as double):
		return Tanh(x)
	
	def truncate(d as decimal):
		return Truncate(d)
	
	def truncate(x as double):
		return Truncate(x)
	
	public final pi as double = PI	

	// def special functions
	