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
	
	public class MarshalByRefEnumerator : System.MarshalByRefObject, IEnumerator
	{
		IEnumerator _enumerator;
		
		public MarshalByRefEnumerator(IEnumerator enumerator)
		{
			_enumerator = enumerator;
		}
		
		public bool MoveNext()
		{
			return _enumerator.MoveNext();
		}
		
		public object Current
		{
			get
			{
				return _enumerator.Current;
			}
		}
		
		public void Reset()
		{
			_enumerator.Reset();
		}
	}
}
