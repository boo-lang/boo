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

namespace Boo.Lang.Compiler.TypeSystem
{
	public class Types
	{
		public static readonly Type RuntimeServices = typeof(Boo.Lang.RuntimeServices);
		
		public static readonly Type Builtins = typeof(Boo.Lang.Builtins);
		
		public static readonly Type Exception = typeof(System.Exception);
		
		public static readonly Type ApplicationException = typeof(System.ApplicationException);
		
		public static readonly Type List = typeof(Boo.Lang.List);
		
		public static readonly Type Hash = typeof(Boo.Lang.Hash);
		
		public static readonly Type ICallable = typeof(Boo.Lang.ICallable);
		
		public static readonly Type ICollection = typeof(System.Collections.ICollection);
		
		public static readonly Type IList = typeof(System.Collections.IList);
		
		public static readonly Type IDictionary = typeof(System.Collections.IDictionary);
		
		public static readonly Type IEnumerable = typeof(System.Collections.IEnumerable);
		
		public static readonly Type IEnumerator = typeof(System.Collections.IEnumerator);
		
		public static readonly Type Object = typeof(object);
		
		public static readonly Type Array = typeof(Array);
		
		public static readonly Type ObjectArray = Type.GetType("System.Object[]");
		
		public static readonly Type Void = typeof(void);
		
		public static readonly Type String = typeof(string);
		
		public static readonly Type Byte = typeof(byte);
		
		public static readonly Type Short = typeof(short);
		
		public static readonly Type Int = typeof(int);
		
		public static readonly Type Long = typeof(long);
		
		public static readonly Type UShort = typeof(ushort);
		
		public static readonly Type UInt = typeof(uint);
		
		public static readonly Type ULong = typeof(ulong);
		
		public static readonly Type TimeSpan = typeof(TimeSpan);
		
		public static readonly Type DateTime = typeof(DateTime);
		
		public static readonly Type Single = typeof(float);
		
		public static readonly Type Double = typeof(double);
		
		public static readonly Type Date = typeof(System.DateTime);
		
		public static readonly Type Bool = typeof(bool);
		
		public static readonly Type IntPtr = typeof(IntPtr);
		
		public static readonly Type Type = typeof(System.Type);
		
		public static readonly Type MulticastDelegate = typeof(System.MulticastDelegate);

	}
}
