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

using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class InternalParameter : IParameter
	{
		ParameterDeclaration _parameter;
		
		int _index;
		
		public InternalParameter(ParameterDeclaration parameter, int index)
		{
			_parameter = parameter;
			_index = index;
		}
		
		public string Name
		{
			get
			{
				return _parameter.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _parameter.Name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Parameter;
			}
		}
		
		public ParameterDeclaration Parameter
		{
			get
			{
				return _parameter;
			}
		}
		
		public IType Type
		{
			get
			{
				return (IType)TypeSystemServices.GetEntity(_parameter.Type);
			}
		}
		
		public int Index
		{
			get
			{
				return _index;
			}
		}
	}
}
