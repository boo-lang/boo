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

namespace Boo.Ast.Compilation.Binding
{
	public class InternalMethodBinding : IMethodBinding, INamespace
	{
		BindingManager _bindingManager;
		
		Boo.Ast.Method _method;
		
		bool _resolved;
		
		ITypeBinding _returnType;
		
		IMethodBinding _override;
		
		internal InternalMethodBinding(BindingManager manager, Boo.Ast.Method method)
		{
			_bindingManager = manager;
			_method = method;
			if (null == _method.ReturnType)
			{
				_returnType = new UnresolvedBinding();
			}
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return _bindingManager.ToTypeBinding((TypeDefinition)_method.ParentNode);
			}
		}
		
		public bool IsResolved
		{
			get
			{
				return _resolved;
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _method.IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _method.IsPublic;
			}
		}
		
		public string Name
		{
			get
			{
				return _method.Name;
			}
		}
		
		public virtual BindingType BindingType
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
		
		public Method Method
		{
			get
			{
				return _method;
			}
		}
		
		public IMethodBinding Override
		{
			get
			{
				return _override;
			}
			
			set
			{
				_override = null;
			}
		}
		
		public ITypeBinding GetParameterType(int parameterIndex)
		{
			return _bindingManager.GetBoundType(_method.Parameters[parameterIndex].Type);
		}
		
		public ITypeBinding ReturnType
		{
			get
			{				
				if (null == _returnType)
				{
					_returnType = _bindingManager.GetBoundType(_method.ReturnType);
				}
				return _returnType;
			}
		}
		
		public void Resolved()
		{
			ITypeBinding resolvedReturnType = _bindingManager.GetBoundType(_method.ReturnType);
			_resolved = true;
			
			if (null != _returnType)
			{
				if (BindingType.Unresolved == _returnType.BindingType)
				{
					((UnresolvedBinding)_returnType).Resolved = resolvedReturnType;
				}
			}
			_returnType = resolvedReturnType;
		}
		
		public IBinding Resolve(string name)
		{
			foreach (Local local in _method.Locals)
			{
				if (local.PrivateScope)
				{
					continue;
				}
				
				if (name == local.Name)
				{
					return _bindingManager.GetBinding(local);
				}
			}
			
			foreach (ParameterDeclaration parameter in _method.Parameters)
			{
				if (name == parameter.Name)
				{
					return _bindingManager.GetBinding(parameter);
				}
			}
			return null;
		}
	}
	
	public class InternalConstructorBinding : InternalMethodBinding, IConstructorBinding
	{
		public InternalConstructorBinding(BindingManager bindingManager,
		                                  Constructor constructor) : base(bindingManager, constructor)
	      {
	      }
	      
	    public override BindingType BindingType
	    {
	    	get
	    	{
	    		return BindingType.Constructor;
	    	}
	    }
	}
}
