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

using System;

namespace BooCompiler.Tests.SupportingClasses
{
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
}