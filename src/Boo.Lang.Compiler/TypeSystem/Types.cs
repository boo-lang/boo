#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
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
