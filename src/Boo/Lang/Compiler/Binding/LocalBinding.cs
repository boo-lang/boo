#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Ast.Compilation.Binding
{
	public class LocalBinding : ITypedBinding
	{		
		Local _local;
		
		ITypeBinding _typeInfo;
		
		System.Reflection.Emit.LocalBuilder _builder;
		
		public LocalBinding(Local local, ITypeBinding typeInfo)
		{			
			_local = local;
			_typeInfo = typeInfo;
		}
		
		public string Name
		{
			get
			{
				return _local.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Local;
			}
		}
		
		public Local Local
		{
			get
			{
				return _local;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _typeInfo;
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
		
		public override string ToString()
		{
			return string.Format("Local<Name={0}, Type={1}>", Name, BoundType);
		}
	}
}
