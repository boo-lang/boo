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
	public class LocalVariable : ITypedEntity
	{		
		Boo.Lang.Compiler.Ast.Local _local;
		
		IType _type;
		
		System.Reflection.Emit.LocalBuilder _builder;
		
		public LocalVariable(Boo.Lang.Compiler.Ast.Local local, IType type)
		{			
			_local = local;
			_type = type;
		}
		
		public string Name
		{
			get
			{
				return _local.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _local.Name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Local;
			}
		}
		
		public bool IsPrivateScope
		{
			get
			{
				return _local.PrivateScope;
			}
		}
		
		public Boo.Lang.Compiler.Ast.Local Local
		{
			get
			{
				return _local;
			}
		}
		
		public IType Type
		{
			get
			{
				return _type;
			}
		}
		
		public System.Reflection.Emit.LocalBuilder LocalBuilder
		{
			get
			{
				return _builder;
			}
			
			set
			{
				_builder = value;
			}
		}
		
		override public string ToString()
		{
			return string.Format("Local<Name={0}, Type={1}>", Name, Type);
		}
	}
}
