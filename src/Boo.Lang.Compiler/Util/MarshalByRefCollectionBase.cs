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
