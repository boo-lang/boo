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

	public class InternalMethod : IInternalEntity, IMethod, INamespace
	{
		TypeSystemServices _typeSystemServices;
		
		Boo.Lang.Compiler.Ast.Method _method;
		
		IMethod _override;
		
		ICallableType _type;
		
		IType _declaringType;
		
		IParameter[] _parameters;
		
		public ExpressionCollection ReturnExpressions;
		
		public ExpressionCollection SuperExpressions;
		
		public ExpressionCollection References;
		
		internal InternalMethod(TypeSystemServices manager, Boo.Lang.Compiler.Ast.Method method)
		{			
			_typeSystemServices = manager;
			_method = method;
			if (method.NodeType != NodeType.Constructor)
			{
				SuperExpressions = new ExpressionCollection();
				ReturnExpressions = new ExpressionCollection();
				if (null == _method.ReturnType)
				{
					if (_method.DeclaringType.NodeType == NodeType.ClassDefinition)
					{
						_method.ReturnType = _typeSystemServices.CreateTypeReference(Unknown.Default);
					}
					else
					{
						_method.ReturnType = _typeSystemServices.CreateTypeReference(_typeSystemServices.VoidType);
					}
				}
			}
		}
		
		public IType DeclaringType
		{
			get
			{
				if (null == _declaringType)
				{
					_declaringType = (IType)TypeSystemServices.GetEntity(_method.DeclaringType);
				}
				return _declaringType;
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
		
		public bool IsVirtual
		{
			get
			{
				return !_method.IsFinal;
			}
		}
		
		public bool IsSpecialName
		{
			get
			{
				return false;
			}
		}
		
		public string Name
		{
			get
			{
				return _method.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _method.DeclaringType.FullName + "." + _method.Name;
			}
		}
		
		public virtual EntityType EntityType
		{
			get
			{
				return EntityType.Method;
			}
		}
		
		public ICallableType CallableType
		{
			get
			{
				if (null == _type)
				{
					_type = _typeSystemServices.GetCallableType(this);
				}
				return _type;
			}
		}
		
		public IType Type
		{
			get
			{
				return CallableType;
			}
		}
		
		public Method Method
		{
			get
			{
				return _method;
			}
		}
		
		public Node Node
		{
			get
			{
				return _method;
			}
		}
		
		public IMethod Override
		{
			get
			{
				return _override;
			}
			
			set
			{
				_override = value;
			}
		}
		
		public IParameter[] GetParameters()
		{
			if (null == _parameters)
			{
				_parameters = _typeSystemServices.Map(_method.Parameters);				
			}
			return _parameters;
		}
		
		public IType ReturnType
		{
			get
			{					
				return TypeSystemServices.GetType(_method.ReturnType);
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return DeclaringType;
			}
		}
		
		public Boo.Lang.Compiler.Ast.Local ResolveLocal(string name)
		{
			foreach (Boo.Lang.Compiler.Ast.Local local in _method.Locals)
			{
				if (local.PrivateScope)
				{
					continue;
				}
				
				if (name == local.Name)
				{
					return local;
				}
			}
			return null;
		}
		
		public ParameterDeclaration ResolveParameter(string name)
		{
			foreach (ParameterDeclaration parameter in _method.Parameters)
			{
				if (name == parameter.Name)
				{
					return parameter;
				}
			}
			return null;
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (NameResolutionService.IsFlagSet(flags, EntityType.Local))
			{
				Boo.Lang.Compiler.Ast.Local local = ResolveLocal(name);
				if (null != local)
				{
					targetList.Add(TypeSystemServices.GetEntity(local));
					return true;
				}
			}
			
			if (NameResolutionService.IsFlagSet(flags, EntityType.Parameter))
			{
				ParameterDeclaration parameter = ResolveParameter(name);
				if (null != parameter)
				{
					targetList.Add(TypeSystemServices.GetEntity(parameter));
					return true;
				}
			}
			return false;
		}
		
		override public string ToString()
		{
			return _typeSystemServices.GetSignature(this);
		}
	}
	
	public class InternalConstructor : InternalMethod, IConstructor
	{
		bool _hasSuperCall = false;
		
		public InternalConstructor(TypeSystemServices tagManager,
		                                  Constructor constructor) : base(tagManager, constructor)
		{
		}
		  
		public bool HasSuperCall
		{
			get
			{
				return _hasSuperCall;
			}
			
			set
			{
				_hasSuperCall = value;
			}
		}
	      
	    override public EntityType EntityType
	    {
	    	get
	    	{
	    		return EntityType.Constructor;
	    	}
	    }
	}
}
