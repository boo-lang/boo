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
	
	public class OverrideEqualityOperators
	{
		public static bool operator==(OverrideEqualityOperators lhs, OverrideEqualityOperators rhs)
		{
			if (null == lhs)
			{
				Console.WriteLine("lhs is null");
			}
			
			if (null == rhs)
			{
				Console.WriteLine("rhs is null");
			}
			return true;
		}
		
		public static bool operator!=(OverrideEqualityOperators lhs, OverrideEqualityOperators rhs)
		{
			 if (null == lhs)
			{
				Console.WriteLine("lhs is null");
			}
			
			if (null == rhs)
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
	}
	
	public class DerivedClass : BaseClass
	{
		protected DerivedClass()
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
