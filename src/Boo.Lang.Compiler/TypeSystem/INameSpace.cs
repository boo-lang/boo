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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	
	/// <summary>
	/// A namespace.
	/// </summary>
	public interface INamespace
	{					
		/// <summary>
		/// The parent namespace.
		/// </summary>
		INamespace ParentNamespace
		{
			get;
		}
		
		/// <summary>
		/// Resolves the name passed as argument to the appropriate elements
		/// in the namespace, all elements with the specified name must be
		/// added to the targetList.
		/// </summary>
		/// <param name="targetList">list where to put the found elements</param>
		/// <param name="name">name of the desired elements</param>
		/// <param name="filter">element filter</param>
		/// <returns>
		/// true if at least one element was added to the targetList, false
		/// otherwise.
		/// </returns>
		bool Resolve(Boo.Lang.List targetList, string name, EntityType filter);
	}
	
	public class NullNamespace : INamespace
	{
		public static readonly INamespace Default = new NullNamespace();
		
		private NullNamespace()
		{
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return null;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			return false;
		}
	}
}
