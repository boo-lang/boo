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

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.Binding
{
	public class InternalMethodBinding : IMethodBinding, INamespace
	{
		BindingManager _manager;
		
		Boo.Ast.Method _method;
		
		MethodBuilder _builder;
		
		internal InternalMethodBinding(BindingManager manager, Boo.Ast.Method method, MethodBuilder builder)
		{
			_manager = manager;
			_method = method;
			_builder = builder;
		}
		
		public bool IsStatic
		{
			get
			{
				return _method.IsStatic;
			}
		}
		
		public string Name
		{
			get
			{
				return _method.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Method;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return ReturnType;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _method.Parameters.Count;
			}
		}
		
		public Type GetParameterType(int parameterIndex)
		{
			return _manager.GetBoundType(_method.Parameters[parameterIndex].Type);
		}
		
		public ITypeBinding ReturnType
		{
			get
			{
				return _manager.GetTypeBinding(_method.ReturnType);
			}
		}
		
		public MethodBase MethodInfo
		{
			get
			{
				return _builder;
			}
		}
		
		public MethodBuilder MethodBuilder
		{
			get
			{
				return _builder;
			}
		}
		
		public IBinding Resolve(string name)
		{
			foreach (Local local in _method.Locals)
			{
				if (name == local.Name)
				{
					return _manager.GetBinding(local);
				}
			}
			
			foreach (ParameterDeclaration parameter in _method.Parameters)
			{
				if (name == parameter.Name)
				{
					return _manager.GetBinding(parameter);
				}
			}
			return null;
		}

	}
}
