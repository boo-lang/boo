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

namespace Boo.Lang.Compiler.Util
{
	using System;
	using System.Collections;
	
	public class MarshalByRefCollectionBase : System.MarshalByRefObject, ICollection
	{
		protected ArrayList _items = new ArrayList();
		
		public bool IsSynchronized
		{
			get
			{
				return _items.IsSynchronized;
			}
		}
		
		public object SyncRoot
		{
			get
			{
				return _items.SyncRoot;
			}
		}
		
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}
		
		protected ArrayList InnerList
		{
			get
			{
				return _items;
			}
		}
		
		public void Clear()
		{
			_items.Clear();
		}
		
		public void CopyTo(System.Array array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}
		
		public IEnumerator GetEnumerator()
		{
			return new MarshalByRefEnumerator(_items.GetEnumerator());
		}
	}
}
