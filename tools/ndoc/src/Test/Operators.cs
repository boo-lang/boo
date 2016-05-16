using System;

namespace NDoc.Test.Operator
{
	/// <summary>
	/// Various permutations of operators
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>This class contains some operators; some overloaded and some not.</summary>
	public class OpOverloads 
	{
		///<overloads>Addition Operator.</overloads>
		/// <summary>Add OperatorOverloads to OperatorOverloads.</summary>
		public static int operator +(OpOverloads x, OpOverloads y) { return 0; }
		/// <summary>Add int to OperatorOverloads.</summary>
		public static int operator +(OpOverloads x, int y) { return 0; }
		/// <summary>double to OperatorOverloads.</summary>
		public static int operator +(OpOverloads x, double y) { return 0; }

		/// <summary>Subtraction operator.</summary>
		public static int operator -(OpOverloads x, OpOverloads y) { return 0; }
		/// <summary>Subtraction operator.</summary>
		public static int operator -(OpOverloads x, int y) { return 0; }
		/// <summary>Subtraction operator.</summary>
		public static int operator -(OpOverloads x, double y) { return 0; }

		/// <summary>divide OperatorOverloads by OperatorOverloads.</summary>
		public static int operator *(OpOverloads x, OpOverloads y) { return 0; }
		/// <summary>multiply OperatorOverloads with OperatorOverloads.</summary>
		public static int operator /(OpOverloads x, OpOverloads y) { return 0; }
	}

	/// <summary>This class contains various overloaded type conversions.</summary>
	public class ConvExplicit 
	{
	
		/// <summary>Explicit conversion to an int.</summary>
		public static explicit operator int (ConvExplicit t) {return	0;}

		/// <summary>Explicit conversion to an float.</summary>
		public static explicit operator float (ConvExplicit t) {return	0;}
		
		/// <summary>Explicit conversion to an double.</summary>
		public static explicit operator double (ConvExplicit t) {return	0;}

		/// <summary>Explicit conversion from a double.</summary>
		public static explicit operator ConvExplicit (double d) {return	new ConvExplicit();}
	}

	/// <summary>This class contains various overloaded type conversions.</summary>
	public class ConvImplicit 
	{
	
		/// <summary>Implicit conversion to an int.</summary>
		public static implicit operator int (ConvImplicit t) {return	0;}

		/// <summary>Implicit conversion to an float.</summary>
		public static implicit operator float (ConvImplicit t) {return	0;}
		
		/// <summary>Implicit conversion to an double.</summary>
		public static implicit operator double (ConvImplicit t) {return	0;}

		/// <summary>Implicit conversion from a double.</summary>
		public static implicit operator ConvImplicit (double d) {return	new ConvImplicit();}
	}

	/// <summary>This class contains various overloaded operators and type conversions.</summary>
	public class MixedOps 
	{
	
		/// <summary>Implicit conversion to a string.</summary>
		public static implicit operator string (MixedOps t) {return	"abcd";}

		/// <summary>Implicit conversion to an int.</summary>
		public static implicit operator int (MixedOps t) {return	0;}

		/// <summary>Implicit conversion to an float.</summary>
		public static implicit operator float (MixedOps t) {return	0;}
		
		/// <summary>Implicit conversion to an double.</summary>
		public static implicit operator double (MixedOps t) {return	0;}

		/// <summary>Implicit conversion from a double.</summary>
		public static implicit operator MixedOps (double d) {return	new MixedOps();}

		/// <summary>Implicit conversion to a boolean.</summary>
		public static implicit operator bool (MixedOps t) {return true;}

		///<overloads>Addition Operator.</overloads>
		/// <summary>Add OperatorOverloads to OperatorOverloads.</summary>
		public static int operator +(MixedOps x, MixedOps y) { return 0; }
		/// <summary>Add int to OperatorOverloads.</summary>
		public static int operator +(MixedOps x, int y) { return 0; }
		/// <summary>double to OperatorOverloads.</summary>
		public static int operator +(MixedOps x, double y) { return 0; }

		/// <summary>Subtraction operator.</summary>
		public static int operator -(MixedOps x, MixedOps y) { return 0; }
		/// <summary>Subtraction operator.</summary>
		public static int operator -(MixedOps x, int y) { return 0; }
		/// <summary>Subtraction operator.</summary>
		public static int operator -(MixedOps x, double y) { return 0; }

		/// <summary>Greater-than operator.</summary>
		public static bool operator >(MixedOps x, MixedOps y) { return true; }
		/// <summary>Greater-than operator.</summary>
		public static bool operator >(MixedOps x, double y) { return true; }
		/// <summary>Less-than operator.</summary>
		public static bool operator <(MixedOps x, MixedOps y) { return true; }
		/// <summary>Less-than operator.</summary>
		public static bool operator <(MixedOps x, double y) { return true; }
	}

	/// <summary>This class contains all the overloadable operators.</summary>
	public class Operators
	{
		/// <summary>Unary plus operator.</summary>
		public static int operator +(Operators o) { return 0; }

		/// <summary>Unary minus operator.</summary>
		public static int operator -(Operators o) { return 0; }

		/// <summary>Logical negation operator.</summary>
		public static int operator !(Operators o) { return 0; }

		/// <summary>Bitwise complement operator.</summary>
		public static int operator ~(Operators o) { return 0; }

		/// <summary>Increment operator.</summary>
		public static Operators operator ++(Operators o) { return null; }

		/// <summary>Decrement operator.</summary>
		public static Operators operator --(Operators o) { return null; }

		/// <summary>Definitely true operator.</summary>
		public static bool operator true(Operators o) { return true; }

		/// <summary>Definitely false operator.</summary>
		public static bool operator false(Operators o) { return false; }

		/// <summary>Addition operator.</summary>
		public static int operator +(Operators x, Operators y) { return 0; }

		/// <summary>Subtraction operator.</summary>
		public static int operator -(Operators x, Operators y) { return 0; }

		/// <summary>Multiplication operator.</summary>
		public static int operator *(Operators x, Operators y) { return 0; }

		/// <summary>Division operator.</summary>
		public static int operator /(Operators x, Operators y) { return 0; }

		/// <summary>Remainder operator.</summary>
		public static int operator %(Operators x, Operators y) { return 0; }

		/// <summary>And operator.</summary>
		public static int operator &(Operators x, Operators y) { return 0; }

		/// <summary>Or operator.</summary>
		public static int operator |(Operators x, Operators y) { return 0; }

		/// <summary>Exclusive-or operator.</summary>
		public static int operator ^(Operators x, Operators y) { return 0; }

		/// <summary>Left-shift operator.</summary>
		public static int operator <<(Operators x, int y) { return 0; }

		/// <summary>Right-shift operator.</summary>
		public static int operator >>(Operators x, int y) { return 0; }

		/// <summary>Equality operator.</summary>
		public static bool operator ==(Operators x, Operators y) { return false; }

		/// <summary>Inequality operator.</summary>
		public static bool operator !=(Operators x, Operators y) { return false; }

		/// <summary>Equals method.</summary>
		public override bool Equals(Object o) { return false; }

		/// <summary>GetHashCode method.</summary>
		public override int GetHashCode() { return 0; }

		/// <summary>Less-than operator.</summary>
		public static bool operator <(Operators x, Operators y) { return false; }

		/// <summary>Greater-than operator.</summary>
		public static bool operator >(Operators x, Operators y) { return false; }

		/// <summary>Less-than-or-equal operator.</summary>
		public static bool operator <=(Operators x, Operators y) { return false; }

		/// <summary>Greater-than-or-equal operator.</summary>
		public static bool operator >=(Operators x, Operators y) { return false; }

		/// <summary>A multiplication method.</summary>
		public static int Multiply(Operators x, Operators y) { return 0; }
	}

}
