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

namespace Boo.Lang.Compiler.Taxonomy
{
	using System;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Services;
	using System.Reflection;

	public class EnumTypeInfo : AbstractInternalType
	{
		internal EnumTypeInfo(DefaultInfoService bindingManager, EnumDefinition enumDefinition) :
			base(bindingManager, enumDefinition)
		{
		}
		
		override public ITypeInfo BaseType
		{
			get
			{
				return _bindingService.EnumTypeInfo;
			}
		}
		
		override public bool IsSubclassOf(ITypeInfo type)
		{
			return type == _bindingService.EnumTypeInfo ||
				_bindingService.EnumTypeInfo.IsSubclassOf(type);
		}
	}
	
	public class InternalType : AbstractInternalType
	{		
		IConstructorInfo[] _constructors;
		
		ITypeInfo _baseType;
		
		int _typeDepth = -1;
		
		internal InternalType(DefaultInfoService manager, TypeDefinition typeDefinition) :
			base(manager, typeDefinition)
		{
		}		
		
		override public ITypeInfo BaseType
		{
			get
			{
				if (null == _baseType)
				{
					if (IsClass)
					{
						foreach (TypeReference baseType in _typeDefinition.BaseTypes)
						{
							ITypeInfo binding = _bindingService.GetBoundType(baseType);
							if (binding.IsClass)
							{
								_baseType = binding;
								break;
							}
						}
					}
					else if (IsInterface)
					{
						_baseType = _bindingService.ObjectTypeInfo;
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
		
		override public bool IsSubclassOf(ITypeInfo type)
		{				
			foreach (TypeReference baseTypeReference in _typeDefinition.BaseTypes)
			{
				ITypeInfo baseType = _bindingService.GetBoundType(baseTypeReference);
				if (type == baseType || baseType.IsSubclassOf(type))
				{
					return true;
				}
			}
			return false;
		}
		
		override public IConstructorInfo[] GetConstructors()
		{
			if (null == _constructors)
			{
				List constructors = new List();
				foreach (TypeMember member in _typeDefinition.Members)
				{					
					if (member.NodeType == NodeType.Constructor && !member.IsStatic)
					{
						IInfo binding = member.Info;
						if (null == binding)
						{
							binding = new InternalConstructorInfo(_bindingService, (Constructor)member);
							DefaultInfoService.Bind(member, binding);
						}
						constructors.Add(binding);
					}
				}
				_constructors = (IConstructorInfo[])constructors.ToArray(typeof(IConstructorInfo));
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
				ITypeInfo binding = _bindingService.GetBoundType(baseType);
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
