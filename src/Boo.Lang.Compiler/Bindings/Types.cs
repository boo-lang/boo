#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;

namespace Boo.Lang.Compiler.Bindings
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

	}
}
