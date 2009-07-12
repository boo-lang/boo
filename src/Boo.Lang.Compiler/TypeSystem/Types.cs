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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using System.Text.RegularExpressions;
	using Boo.Lang.Runtime;

	public class Types
	{
		public static readonly Type RuntimeServices = typeof(RuntimeServices);

		public static readonly Type Builtins = typeof(Builtins);

		public static readonly Type List = typeof(List);

		public static readonly Type Hash = typeof(Hash);

		public static readonly Type ICallable = typeof(ICallable);

		public static readonly Type ICollection = typeof(ICollection);

		public static readonly Type IList = typeof(IList);

		public static readonly Type IDictionary = typeof(IDictionary);

		public static readonly Type IEnumerable = typeof(IEnumerable);

		public static readonly Type IEnumerableGeneric = typeof(System.Collections.Generic.IEnumerable<>);

		public static readonly Type IEnumerator = typeof(IEnumerator);

		public static readonly Type IDisposable = typeof(IDisposable);

		public static readonly Type Object = typeof(object);

		public static readonly Type Regex = typeof(Regex);

		public static readonly Type ValueType = typeof(ValueType);

		public static readonly Type Array = typeof(Array);

		public static readonly Type ObjectArray = Type.GetType("System.Object[]");

		public static readonly Type Void = typeof(void);

		public static readonly Type String = typeof(string);

		public static readonly Type Byte = typeof(byte);

		public static readonly Type SByte = typeof(sbyte);

		public static readonly Type Char = typeof(char);

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

		public static readonly Type Decimal = typeof(decimal);

		public static readonly Type Date = typeof(DateTime);

		public static readonly Type Bool = typeof(bool);

		public static readonly Type IntPtr = typeof(IntPtr);

		public static readonly Type UIntPtr = typeof(UIntPtr);

		public static readonly Type Type = typeof(Type);

		public static readonly Type MulticastDelegate = typeof(MulticastDelegate);

		public static readonly Type Delegate = typeof(Delegate);

		public static readonly Type DuckTypedAttribute = typeof(Boo.Lang.DuckTypedAttribute);

		public static readonly Type BooExtensionAttribute = typeof(Boo.Lang.ExtensionAttribute);

		public static readonly Type ClrExtensionAttribute = Type.GetType("System.Runtime.CompilerServices.ExtensionAttribute, System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

		public static readonly Type DllImportAttribute = typeof(System.Runtime.InteropServices.DllImportAttribute);

		public static readonly Type ModuleAttribute = typeof(System.Runtime.CompilerServices.CompilerGlobalScopeAttribute);

		public static readonly Type ParamArrayAttribute = typeof(ParamArrayAttribute);

		public static readonly Type DefaultMemberAttribute = typeof(System.Reflection.DefaultMemberAttribute);

		public static readonly Type Nullable = typeof(Nullable<>);

		public static readonly Type CompilerGeneratedAttribute = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
	}
}

