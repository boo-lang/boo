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

namespace Boo.Lang.Compiler.Infos
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Services;

	public class InternalMethodInfo : AbstractInternalInfo, IMethodInfo, INamespace
	{
		DefaultInfoService _bindingService;
		
		Boo.Lang.Compiler.Ast.Method _method;
		
		IMethodInfo _override;
		
		ITypeInfo _declaringType;
		
		public ExpressionCollection ReturnExpressions;
		
		public ExpressionCollection SuperExpressions;
		
		internal InternalMethodInfo(DefaultInfoService manager, Method method) : this(manager, method, false)
		{
		}
		
		internal InternalMethodInfo(DefaultInfoService manager, Boo.Lang.Compiler.Ast.Method method, bool visited) : base(visited)
		{			
			_bindingService = manager;
			_method = method;
			if (method.NodeType != NodeType.Constructor)
			{
				SuperExpressions = new ExpressionCollection();
				ReturnExpressions = new ExpressionCollection();
				if (null == _method.ReturnType)
				{
					if (_method.DeclaringType.NodeType == NodeType.ClassDefinition)
					{
						_method.ReturnType = new SimpleTypeReference("unknown");
						DefaultInfoService.Bind(_method.ReturnType, UnknownInfo.Default);
					}
					else
					{
						_method.ReturnType = new SimpleTypeReference("System.Void");
						DefaultInfoService.Bind(_method.ReturnType, _bindingService.VoidTypeInfo);
					}
				}
			}
		}
		
		public ITypeInfo DeclaringType
		{
			get
			{
				if (null == _declaringType)
				{
					_declaringType = (ITypeInfo)DefaultInfoService.GetInfo(_method.DeclaringType);
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
		
		public virtual InfoType InfoType
		{
			get
			{
				return InfoType.Method;
			}
		}
		
		public ITypeInfo BoundType
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
		
		override public Node Node
		{
			get
			{
				return _method;
			}
		}
		
		public IMethodInfo Override
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
		
		public ITypeInfo GetParameterType(int parameterIndex)
		{
			return _bindingService.GetBoundType(_method.Parameters[parameterIndex].Type);
		}
		
		public ITypeInfo ReturnType
		{
			get
			{					
				return _bindingService.GetBoundType(_method.ReturnType);
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return DeclaringType;
			}
		}
		
		public IInfo Resolve(string name)
		{
			foreach (Local local in _method.Locals)
			{
				if (local.PrivateScope)
				{
					continue;
				}
				
				if (name == local.Name)
				{
					return DefaultInfoService.GetInfo(local);
				}
			}
			
			foreach (ParameterDeclaration parameter in _method.Parameters)
			{
				if (name == parameter.Name)
				{
					return DefaultInfoService.GetInfo(parameter);
				}
			}
			return null;
		}
		
		override public string ToString()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append(_method.FullName);
			builder.Append("(");
			
			int i=0;
			foreach (ParameterDeclaration parameter in _method.Parameters)
			{
				if (i > 0)
				{
					builder.Append(", ");					
				}
				else
				{
					++i;
				}
				if (null == parameter.Type)
				{
					builder.Append("System.Object");
				}
				else
				{
					builder.Append(parameter.Type.ToString());
				}
			}
			
			builder.Append(")");
			return builder.ToString();
		}
	}
	
	public class InternalConstructorInfo : InternalMethodInfo, IConstructorInfo
	{
		bool _hasSuperCall = false;
		
		public InternalConstructorInfo(DefaultInfoService bindingManager,
		                                  Constructor constructor) : base(bindingManager, constructor)
		  {
		  }
		  
		public InternalConstructorInfo(DefaultInfoService bindingManager,
		                                  Constructor constructor,
										  bool visited) : base(bindingManager, constructor, visited)
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
	      
	    override public InfoType InfoType
	    {
	    	get
	    	{
	    		return InfoType.Constructor;
	    	}
	    }
	}
}
