#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

	public class InternalMethod : IInternalElement, IMethod, INamespace
	{
		TypeSystemServices _tagService;
		
		Boo.Lang.Compiler.Ast.Method _method;
		
		IMethod _override;
		
		IType _declaringType;
		
		IParameter[] _parameters;
		
		public ExpressionCollection ReturnExpressions;
		
		public ExpressionCollection SuperExpressions;
		
		internal InternalMethod(TypeSystemServices manager, Boo.Lang.Compiler.Ast.Method method)
		{			
			_tagService = manager;
			_method = method;
			if (method.NodeType != NodeType.Constructor)
			{
				SuperExpressions = new ExpressionCollection();
				ReturnExpressions = new ExpressionCollection();
				if (null == _method.ReturnType)
				{
					if (_method.DeclaringType.NodeType == NodeType.ClassDefinition)
					{
						_method.ReturnType = _tagService.CreateTypeReference(Unknown.Default);
					}
					else
					{
						_method.ReturnType = _tagService.CreateTypeReference(_tagService.VoidType);
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
					_declaringType = (IType)TypeSystemServices.GetTag(_method.DeclaringType);
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
		
		public virtual ElementType ElementType
		{
			get
			{
				return ElementType.Method;
			}
		}
		
		public IType Type
		{
			get
			{
				return ReturnType;
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
				_parameters = _tagService.Map(_method.Parameters);				
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
		
		public bool Resolve(Boo.Lang.List targetList, string name, ElementType flags)
		{
			if (NameResolutionService.IsFlagSet(flags, ElementType.Local))
			{
				Boo.Lang.Compiler.Ast.Local local = ResolveLocal(name);
				if (null != local)
				{
					targetList.Add(TypeSystemServices.GetTag(local));
					return true;
				}
			}
			
			if (NameResolutionService.IsFlagSet(flags, ElementType.Parameter))
			{
				ParameterDeclaration parameter = ResolveParameter(name);
				if (null != parameter)
				{
					targetList.Add(TypeSystemServices.GetTag(parameter));
					return true;
				}
			}
			return false;
		}
		
		override public string ToString()
		{
			return TypeSystemServices.GetSignature(this);
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
	      
	    override public ElementType ElementType
	    {
	    	get
	    	{
	    		return ElementType.Constructor;
	    	}
	    }
	}
}
