#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace BooCompiler.Tests
{
	using System;
	
	public class ObsoleteClass
	{
		[Obsolete("It is." )]
		public static int Bar = 42;
		
		[Obsolete("Indeed it is." )]
		public static void Foo()
		{
		}

		[Obsolete("We said so.")]
		public static int Baz
		{
			get { return 42; }
		}
	}

	public class ConditionalClass
	{
		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
		public static void PrintNothing(int i)
		{
			Console.WriteLine(i);
		}

		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
		public static void PrintSomething(string s)
		{
			Console.WriteLine(s);
		}

		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
		public static void PrintNoT<T>(T s)
		{
			Console.WriteLine(s);
		}

		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
		public static void PrintSomeT<T>(T s)
		{
			Console.WriteLine(s);
		}
	}
	
	public class ReturnDucks
	{
		public class DuckBase {}
		
		public class DuckFoo : DuckBase
		{
			public string Foo() { return "foo"; }
		}
		
		public class DuckBar : DuckBase
		{
			public string Bar() { return "bar"; }
		}
		
		[Boo.Lang.DuckTypedAttribute]
		public DuckBase GetDuck(bool foo)
		{
			if (foo) return new DuckFoo();
			return new DuckBar();
		}
	}
	
	public struct Point
	{
		public int x;
		public int y;
	}

	public struct Rectangle
	{
		Point _top;
		public Point topLeft
		{
			get { return _top; }
			set { _top = value; }
		}
	}
	
	public struct Vector3
	{
		public float x, y, z;
	}

	public class Transform
	{	
		Vector3 _position;
	
		public Vector3 position
		{
			get { return _position; }
			set { _position = value; }
		}
	}
	
	public class BOO313BaseClass
	{
		Transform _t = new Transform();
		public Transform transform
		{
			get { return _t; }
		}
	}
	
	public class OutterClass
	{
		public class InnerClass
		{
			public static int X = 3;
		}
	}
	
	public class OverrideBoolOperator
	{
		public static implicit operator bool(OverrideBoolOperator instance)
		{
			Console.WriteLine("OverrideBoolOperator.operator bool");
			return false;
		}
	}
	
	public struct ValueTypeOverrideBoolOperator
	{
		public static implicit operator bool(ValueTypeOverrideBoolOperator instance)
		{
			Console.WriteLine("ValueTypeOverrideBoolOperator.operator bool");
			return false;
		}
	}
	
	public class ExtendsOverridenBoolOperator : OverrideBoolOperator
	{
		[Boo.Lang.DuckTypedAttribute]
		public ExtendsOverridenBoolOperator GetFoo()
		{
			return new ExtendsOverridenBoolOperator();
		}
	}
	
	public class OverrideEqualityOperators
	{
		public static bool operator==(OverrideEqualityOperators lhs, OverrideEqualityOperators rhs)
		{
			if (Object.Equals(null, lhs))
			{
				Console.WriteLine("lhs is null");
			}
			
			if (Object.Equals(null, rhs))
			{
				Console.WriteLine("rhs is null");
			}
			return true;
		}
		
		public static bool operator!=(OverrideEqualityOperators lhs, OverrideEqualityOperators rhs)
		{
			if (Object.Equals(null, lhs))
			{
				Console.WriteLine("lhs is null");
			}
			
			if (Object.Equals(null, rhs))
			{
				Console.WriteLine("rhs is null");
			}
			return false;
		}
	}
	
	public class AmbiguousBase
	{
		public string Path(string empty)
		{
			return "Base";
		}
	}

	public class AmbiguousSub1 : AmbiguousBase
	{
        public new string Path
        {
        	get
        	{
        		return "Sub1";
        	}
        }
	}
	
	public class AmbiguousSub2 : AmbiguousSub1
	{
	}
	
	[Flags]
	public enum TestEnum
	{
		Foo = 1,
		Bar = 2,
		Baz = 4
	}
	
	public enum ByteEnum : byte
	{
		Foo = 1,
		Bar = 2,
		Baz = 4
	}
	
	public enum SByteEnum : sbyte
	{
		Foo = -1,
		Bar = -2,
		Baz = -4
	}
	
	public class Person
	{
		string _fname;
		string _lname;
		uint _age;
		
		public Person()
		{			
		}
		
		public uint Age
		{
			get
			{
				return _age;
			}
			
			set
			{
				_age = value;
			}
		}
		
		public string FirstName
		{
			get
			{
				return _fname;
			}
			
			set
			{
				_fname = value;
			}
		}
		
		public string LastName
		{
			get
			{
				return _lname;
			}
			
			set
			{
				_lname = value;
			}
		}
	}
	
	public class PersonCollection : System.Collections.CollectionBase
	{
		public PersonCollection()
		{
		}
		
		public Person this[int index]
		{
			get
			{
				return (Person)InnerList[index];
			}
			
			set
			{
				InnerList[index] = value;
			}
		}
		
		public Person this[string fname]
		{
			get
			{
				foreach (Person p in InnerList)
				{
					if (p.FirstName == fname)
					{
						return p;
					}
				}
				return null;
			}
			
			set
			{
				int index = 0;
				foreach (Person p in InnerList)
				{
					if (p.FirstName == fname)
					{
						InnerList[index] = value;
						break;						
					}
					++index;
				}
			}
		}
		
		public void Add(Person person)
		{
			InnerList.Add(person);
		}
	}
	
	public class Clickable
	{
		public Clickable()
		{			
		}
		
		public event EventHandler Click;
		
		public static event EventHandler Idle; 
		
		public void RaiseClick()
		{
			if (null != Click)
			{
				Click(this, EventArgs.Empty);
			}
		}
		
		public static void RaiseIdle()
		{
			if (null != Idle)
			{
				Idle(null, EventArgs.Empty);
			}
		}
	}
	
	public class BaseClass
	{
		protected BaseClass()
		{			
		}
		
		protected BaseClass(string message)
		{
			Console.WriteLine("BaseClass.constructor('{0}')", message);
		}
		
		public virtual void Method0()
		{
			Console.WriteLine("BaseClass.Method0");
		}
		
		public virtual void Method0(string text)
		{
			Console.WriteLine("BaseClass.Method0('{0}')", text);
		}
		
		public virtual void Method1()
		{
			Console.WriteLine("BaseClass.Method1");
		}
		
		//for BOO-632 regression test
		protected int _protectedfield = 0;
		protected int ProtectedProperty
		{
			get
			{
				return _protectedfield;
			}
			
			set
			{
				_protectedfield = value;
			}
		}
	}
	
	public class DerivedClass : BaseClass
	{
		public DerivedClass()
		{
		}
		
		public void Method2()
		{
			Method0();
			Method1();
		}
	}
	
	public class ClassWithNewMethod : DerivedClass
	{
		new public void Method2()
		{
			Console.WriteLine("ClassWithNewMethod.Method2");
		}
	}	
	
	public class VarArgs
	{
		public void Method()
		{
			Console.WriteLine("VarArgs.Method");
		}
		
		public void Method(params object[] args)
		{
			Console.WriteLine("VarArgs.Method({0})", Boo.Lang.Builtins.join(args, ", "));
		}
	}
	
	public class Disposable : System.IDisposable
	{
		public Disposable()
		{
			Console.WriteLine("Disposable.constructor");
		}
		
		public void foo()
		{
			Console.WriteLine("Disposable.foo");
		}
		
		void System.IDisposable.Dispose()
		{
			Console.WriteLine("Disposable.Dispose");
		}
	}
	
	public class Constants
	{
		public const string StringConstant = "Foo";
		
		public const int IntegerConstant = 14;
		
		public const uint UnsignedInt = 255;
		
		public const ulong UnsignedLong = 297;
	}
	
	public class ByRef
	{
		public static void SetValue(int value, ref int output)
		{
			output = value;
		}
		
		public static void SetRef(object value, ref object output)
		{
			output = value;
		}
		
		public static void ReturnValue(int value, out int output)
		{
			output = value;
		}
		
		public static void ReturnRef(object value, out object output)
		{
			output = value;
		}
	}

	public class PointersBase
	{
		public void Foo(ref int bar)
		{
			System.Console.WriteLine("PointersBase.Foo(int&)");
		}
		
		public unsafe void Foo(int* bar)
		{
			System.Console.WriteLine("Pointers.Foo(int*)");
		}
	}
	
	public class Pointers : PointersBase
	{
		public new void Foo(ref int bar)
		{
			System.Console.WriteLine("Pointers.Foo(int&)");
		}
	}
	
	public class NoParameterlessConstructor
	{
		public NoParameterlessConstructor(object param)
		{
		}
	}
	
	public abstract class AbstractClass
	{
	}
	
	public abstract class AnotherAbstractClass
	{
		protected abstract string Foo();
		
		public virtual string Bar()
		{
			return "Bar";
		}
	}
}
