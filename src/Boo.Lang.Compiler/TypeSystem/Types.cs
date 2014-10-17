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
using System.Collections;
using System.Text.RegularExpressions;
using Boo.Lang.Runtime;
	
namespace Boo.Lang.Compiler.TypeSystem
{
	public static class Types
	{
		public static readonly Type RuntimeServices = typeof(RuntimeServices);

		public static readonly Type Builtins = typeof(Builtins);

		public static readonly Type List = typeof(List);

		public static readonly Type Hash = typeof(Hash);

		public static readonly Type ICallable = typeof(ICallable);

		public static readonly Type ICollection = typeof(ICollection);

		public static readonly Type IEnumerable = typeof(IEnumerable);

		public static readonly Type IEnumerableGeneric = typeof(System.Collections.Generic.IEnumerable<>);

		public static readonly Type Object = typeof(object);

		public static readonly Type Regex = typeof(Regex);

		public static readonly Type ValueType = typeof(ValueType);

		public static readonly Type Array = typeof(Array);

		public static readonly Type ObjectArray = typeof(object[]);

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

		public static readonly Type Bool = typeof(bool);

		public static readonly Type IntPtr = typeof(IntPtr);

		public static readonly Type UIntPtr = typeof(UIntPtr);

		public static readonly Type Type = typeof(Type);

		public static readonly Type MulticastDelegate = typeof(MulticastDelegate);

		public static readonly Type Delegate = typeof(Delegate);

		public static readonly Type DuckTypedAttribute = typeof(DuckTypedAttribute);

		public static readonly Type ClrExtensionAttribute;

		public static readonly Type DllImportAttribute = typeof(System.Runtime.InteropServices.DllImportAttribute);

		public static readonly Type ModuleAttribute = typeof(System.Runtime.CompilerServices.CompilerGlobalScopeAttribute);

		public static readonly Type ParamArrayAttribute = typeof(ParamArrayAttribute);

		public static readonly Type DefaultMemberAttribute = typeof(System.Reflection.DefaultMemberAttribute);

		public static readonly Type Nullable = typeof(Nullable<>);

		public static readonly Type CompilerGeneratedAttribute = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);

		static Type FindType(string typename, string typenamespace)
		{
			foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
			    {
			    	if(type.Name == typename && type.Namespace == typenamespace)
			    		return type;
			    }
			}

			return null;		
		}

		static Types()
		{
			// ExtensionAttribute is in System.Core for .NET 4.0 and in mscorlib for .NET 4.5.
			// We use reflection to get the type to avoid a hardcoded reference to mscorlib that 
			// will crash the boo compiler if it was built with .NET 4.5 and then run on .NET 4.0.
			// Windows XP only supports .NET 4.0.
			ClrExtensionAttribute = FindType("ExtensionAttribute", "System.Runtime.CompilerServices");	
		}
	}
}

