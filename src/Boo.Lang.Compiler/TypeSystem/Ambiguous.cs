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
using System.Collections;

namespace Boo.Lang.Compiler.TypeSystem
{
	public delegate bool InfoFilter(IEntity tag);
	
	public class Ambiguous : IEntity
	{
		IEntity[] _entitys;
		
		public Ambiguous(IEntity[] tags)
		{
			if (null == tags)
			{
				throw new ArgumentNullException("tags");
			}
			if (0 == tags.Length)
			{
				throw new ArgumentException("tags");
			}
			_entitys = tags;
		}
		
		public string Name
		{
			get
			{
				return _entitys[0].Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _entitys[0].FullName;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Ambiguous;
			}
		}
		
		public IEntity[] Entities
		{
			get
			{
				return _entitys;
			}
		}
		
		public Boo.Lang.List Filter(InfoFilter condition)
		{
			Boo.Lang.List found = new Boo.Lang.List();
			foreach (IEntity tag in _entitys)
			{
				if (condition(tag))
				{
					found.Add(tag);
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
