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
using System.Collections;

namespace Boo.Lang.Compiler.Taxonomy
{
	public delegate bool InfoFilter(IInfo binding);
	
	public class Ambiguous : IInfo
	{
		IInfo[] _bindings;
		
		public Ambiguous(IInfo[] bindings)
		{
			if (null == bindings)
			{
				throw new ArgumentNullException("bindings");
			}
			if (0 == bindings.Length)
			{
				throw new ArgumentException("bindings");
			}
			_bindings = bindings;
		}
		
		public string Name
		{
			get
			{
				return _bindings[0].Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _bindings[0].FullName;
			}
		}
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Ambiguous;
			}
		}
		
		public IInfo[] Taxonomy
		{
			get
			{
				return _bindings;
			}
		}
		
		public Boo.Lang.List Filter(InfoFilter condition)
		{
			Boo.Lang.List found = new Boo.Lang.List();
			foreach (IInfo binding in _bindings)
			{
				if (condition(binding))
				{
					found.Add(binding);
				}
			}
			return found;
		}
		
		override public string ToString()
		{
			return "";
		}
	}
}
