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

namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Services;
	using System.Reflection;

	public class EnumTypeBinding : AbstractInternalTypeBinding
	{
		internal EnumTypeBinding(DefaultBindingService bindingManager, EnumDefinition enumDefinition) :
			base(bindingManager, enumDefinition)
		{
		}
		
		override public ITypeBinding BaseType
		{
			get
			{
				return _bindingService.EnumTypeBinding;
			}
		}
		
		override public bool IsSubclassOf(ITypeBinding type)
		{
			return type == _bindingService.EnumTypeBinding ||
				_bindingService.EnumTypeBinding.IsSubclassOf(type);
		}
	}
	
	public class InternalTypeBinding : AbstractInternalTypeBinding
	{		
		IConstructorBinding[] _constructors;
		
		ITypeBinding _baseType;
		
		int _typeDepth = -1;
		
		internal InternalTypeBinding(DefaultBindingService manager, TypeDefinition typeDefinition) :
			base(manager, typeDefinition)
		{
		}		
		
		override public ITypeBinding BaseType
		{
			get
			{
				if (null == _baseType)
				{
					if (IsClass)
					{
						foreach (TypeReference baseType in _typeDefinition.BaseTypes)
						{
							ITypeBinding binding = _bindingService.GetBoundType(baseType);
							if (binding.IsClass)
							{
								_baseType = binding;
								break;
							}
						}
					}
					else if (IsInterface)
					{
						_baseType = _bindingService.ObjectTypeBinding;
					}
				}
				return _baseType;
			}
		}
		
		override public int GetTypeDepth()
		{
			if (-1 == _typeDepth)
			{
				_typeDepth = CalcTypeDepth();
			}
			return _typeDepth;
		}
		
		override public bool IsSubclassOf(ITypeBinding type)
		{				
			foreach (TypeReference baseTypeReference in _typeDefinition.BaseTypes)
			{
				ITypeBinding baseType = _bindingService.GetBoundType(baseTypeReference);
				if (type == baseType || baseType.IsSubclassOf(type))
				{
					return true;
				}
			}
			return false;
		}
		
		override public IConstructorBinding[] GetConstructors()
		{
			if (null == _constructors)
			{
				List constructors = new List();
				foreach (TypeMember member in _typeDefinition.Members)
				{					
					if (member.NodeType == NodeType.Constructor && !member.IsStatic)
					{
						IBinding binding = member.Binding;
						if (null == binding)
						{
							binding = new InternalConstructorBinding(_bindingService, (Constructor)member);
							DefaultBindingService.Bind(member, binding);
						}
						constructors.Add(binding);
					}
				}
				_constructors = (IConstructorBinding[])constructors.ToArray(typeof(IConstructorBinding));
			}
			return _constructors;
		}
		
		int CalcTypeDepth()
		{
			if (IsInterface)
			{
				return 1+GetMaxBaseInterfaceDepth();
			}
			return 1+BaseType.GetTypeDepth();
		}
		
		int GetMaxBaseInterfaceDepth()
		{
			int current = 0;
			foreach (TypeReference baseType in _typeDefinition.BaseTypes)
			{
				ITypeBinding binding = _bindingService.GetBoundType(baseType);
				int depth = binding.GetTypeDepth();
				if (depth > current)
				{
					current = depth;
				}
			}
			return current;
		}
	}
}
