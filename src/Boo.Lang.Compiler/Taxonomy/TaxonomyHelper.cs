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

namespace Boo.Lang.Compiler.Services
{
	using System;
	using System.Reflection;
	using Boo.Lang.Compiler;	
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Taxonomy;

	public class DefaultInfoService
	{			
		public ExternalType ExceptionTypeInfo;
		
		public ExternalType ApplicationExceptionInfo;
		
		public ExternalType ObjectTypeInfo;
		
		public ExternalType EnumTypeInfo;
		
		public ExternalType ArrayTypeInfo;
		
		public ExternalType TypeTypeInfo;
		
		public ITypeInfo ObjectArrayInfo;
	
		public ExternalType VoidTypeInfo;
		
		public ExternalType StringTypeInfo;
		
		public ExternalType BoolTypeInfo;
		
		public ExternalType ByteTypeInfo;
		
		public ExternalType ShortTypeInfo;
		
		public ExternalType IntTypeInfo;
		
		public ExternalType LongTypeInfo;
		
		public ExternalType SingleTypeInfo;
		
		public ExternalType DoubleTypeInfo;
		
		public ExternalType TimeSpanTypeInfo;
		
		public ExternalType DateTimeTypeInfo;
		
		public ExternalType RuntimeServicesInfo;
		
		public ExternalType BuiltinsInfo;
		
		public ExternalType ListTypeInfo;
		
		public ExternalType HashTypeInfo;
		
		public ExternalType ICallableTypeInfo;
		
		public ExternalType IEnumerableTypeInfo;
		
		public ExternalType ICollectionTypeInfo;
		
		public ExternalType IListTypeInfo;
		
		public ExternalType IDictionaryTypeInfo;
		
		System.Collections.Hashtable _primitives = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _bindingCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _arrayInfoCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _referenceCache = new System.Collections.Hashtable();
		
		static readonly IInfo _lenInfo = new SpecialFunctionInfo(SpecialFunction.Len);
		
		public DefaultInfoService()		
		{			
			Cache(VoidTypeInfo = new VoidTypeInfoImpl(this));
			Cache(ObjectTypeInfo = new ExternalType(this, Types.Object));
			Cache(EnumTypeInfo = new ExternalType(this, typeof(System.Enum)));
			Cache(ArrayTypeInfo = new ExternalType(this, Types.Array));
			Cache(TypeTypeInfo = new ExternalType(this, Types.Type));
			Cache(StringTypeInfo = new ExternalType(this, Types.String));
			Cache(BoolTypeInfo = new ExternalType(this, Types.Bool));
			Cache(ByteTypeInfo = new ExternalType(this, Types.Byte));
			Cache(ShortTypeInfo = new ExternalType(this, Types.Short));
			Cache(IntTypeInfo = new ExternalType(this, Types.Int));
			Cache(LongTypeInfo = new ExternalType(this, Types.Long));
			Cache(SingleTypeInfo = new ExternalType(this, Types.Single));
			Cache(DoubleTypeInfo = new ExternalType(this, Types.Double));
			Cache(TimeSpanTypeInfo = new ExternalType(this, Types.TimeSpan));
			Cache(DateTimeTypeInfo = new ExternalType(this, Types.DateTime));
			Cache(RuntimeServicesInfo = new ExternalType(this, Types.RuntimeServices));
			Cache(BuiltinsInfo = new ExternalType(this, Types.Builtins));
			Cache(ListTypeInfo = new ExternalType(this, Types.List));
			Cache(HashTypeInfo = new ExternalType(this, Types.Hash));
			Cache(ICallableTypeInfo = new ExternalType(this, Types.ICallable));
			Cache(IEnumerableTypeInfo = new ExternalType(this, Types.IEnumerable));
			Cache(ICollectionTypeInfo = new ExternalType(this, Types.ICollection));
			Cache(IListTypeInfo = new ExternalType(this, Types.IList));
			Cache(IDictionaryTypeInfo = new ExternalType(this, Types.IDictionary));
			Cache(ApplicationExceptionInfo = new ExternalType(this, Types.ApplicationException));
			Cache(ExceptionTypeInfo = new ExternalType(this, Types.Exception));
			
			ObjectArrayInfo = AsArrayInfo(ObjectTypeInfo);
			
			PreparePrimitives();
		}
		
		public Boo.Lang.Compiler.Ast.TypeReference CreateBoundTypeReference(ITypeInfo binding)
		{
			TypeReference typeReference = null;
			
			if (binding.IsArray)
			{
				typeReference = new ArrayTypeReference(CreateBoundTypeReference(binding.GetElementType()));
			}
			else
			{				
				typeReference = new SimpleTypeReference(binding.FullName);				
			}
			Bind(typeReference, AsTypeReference(binding));
			return typeReference;
		}
		
		public ITypeInfo GetPromotedNumberType(ITypeInfo left, ITypeInfo right)
		{
			if (left == DoubleTypeInfo ||
				right == DoubleTypeInfo)
			{
				return DoubleTypeInfo;
			}
			if (left == SingleTypeInfo ||
				right == SingleTypeInfo)
			{
				return SingleTypeInfo;
			}
			if (left == LongTypeInfo ||
				right == LongTypeInfo)
			{
				return LongTypeInfo;
			}
			if (left == ShortTypeInfo ||
				right == ShortTypeInfo)
			{
				return ShortTypeInfo;
			}
			return left;
		}
		
		public static bool IsUnknown(Node node)
		{
			ITypedInfo binding = GetInfo(node) as ITypedInfo;
			if (null != binding)
			{
				return IsUnknown(binding.BoundType);
			}
			return false;
		}
		
		public static bool IsUnknown(ITypeInfo binding)
		{
			return InfoType.Unknown == binding.InfoType;
		}
		
		public static bool IsError(Node node)
		{			
			ITypedInfo binding = GetInfo(node) as ITypedInfo;
			if (null != binding)
			{
				return IsError(binding.BoundType);
			}
			return false;
		}
		
		public static bool IsErrorAny(NodeCollection collection)
		{
			foreach (Node n in collection)
			{
				if (IsError(n))
				{
					return true;
				}
			}
			return false;
		}
		
		public static bool IsError(IInfo binding)
		{
			return InfoType.Error == binding.InfoType;
		}		
		
		public static bool IsBound(Node node)
		{
			return null != node.Info;
		}
		
		public static void Unbind(Node node)
		{
			node.Info = null;
		}
		
		public static void Bind(Node node, IInfo binding)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			if (null == binding)
			{
				throw new ArgumentNullException("binding");
			}
			
			node.Info = binding;
		}
		
		public void Bind(TypeDefinition type)
		{
			Bind(type, AsTypeInfo(type));
		}
		
		public static void Error(Node node)
		{
			Bind(node, ErrorInfo.Default);
		}
		
		public static IInfo GetInfo(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			IInfo binding = node.Info;
			if (null == binding)
			{
				NodeNotBound(node);
			}
			return binding;
		}	
		
		public ITypeInfo GetBoundType(Node node)
		{
			return ((ITypedInfo)GetInfo(node)).BoundType;
		}
		
		public ITypeInfo AsTypeInfo(System.Type type)
		{
			if (type.IsArray)
			{
				return AsArrayInfo(AsTypeInfo(type.GetElementType()));
			}
			
			ExternalType binding = (ExternalType)_bindingCache[type];
			if (null == binding)
			{
				Cache(binding = new ExternalType(this, type));
			}
			return binding;
		}
		
		public ITypeInfo AsTypeInfo(TypeDefinition typeDefinition)
		{
			ITypeInfo binding = (ITypeInfo)_bindingCache[typeDefinition];
			if (null == binding)
			{
				Cache(typeDefinition, binding = new InternalType(this, typeDefinition));
			}
			return binding;
		}
		
		public ITypeInfo AsArrayInfo(ITypeInfo elementType)
		{
			ITypeInfo binding = (ITypeInfo)_arrayInfoCache[elementType];
			if (null == binding)
			{
				_arrayInfoCache.Add(elementType, binding = new ArrayTypeInfo(this, elementType));
			}
			return binding;
		}
		
		public ITypedInfo AsTypeReference(ITypeInfo type)
		{
			ITypedInfo cached = (ITypedInfo)_referenceCache[type];
			if (null == cached)
			{
				cached = new TypeReferenceInfo(type);
				_referenceCache[type] = cached;
			}
			return cached;
		}
		
		public ITypedInfo AsTypeReference(System.Type type)
		{
			return AsTypeReference(AsTypeInfo(type));
		}
		
		public IInfo AsInfo(System.Reflection.MemberInfo[] info)
		{
			if (info.Length > 1)
			{
				IInfo[] bindings = new IInfo[info.Length];
				for (int i=0; i<bindings.Length; ++i)
				{
					bindings[i] = AsInfo(info[i]);
				}
				return new Ambiguous(bindings);
			}
			if (info.Length > 0)
			{
				return AsInfo(info[0]);
			}
			return null;
		}
		
		public IInfo AsInfo(System.Reflection.MemberInfo mi)
		{
			IInfo binding = (IInfo)_bindingCache[mi];
			if (null == binding)
			{			
				switch (mi.MemberType)
				{
					case MemberTypes.Method:
					{
						binding = new ExternalMethod(this, (System.Reflection.MethodInfo)mi);
						break;
					}
					
					case MemberTypes.Constructor:
					{
						binding = new ExternalConstructorInfo(this, (System.Reflection.ConstructorInfo)mi);
						break;
					}
					
					case MemberTypes.Field:
					{
						binding = new ExternalFieldInfo(this, (System.Reflection.FieldInfo)mi);
						break;
					}
					
					case MemberTypes.Property:
					{
						binding = new ExternalProperty(this, (System.Reflection.PropertyInfo)mi);
						break;
					}
					
					case MemberTypes.Event:
					{
						binding = new ExternalEvent(this, (System.Reflection.EventInfo)mi);
						break;
					}
					
					case MemberTypes.TypeInfo:
					{
						return AsTypeInfo((Type)mi);
					}
					
					default:
					{
						throw new NotImplementedException(mi.ToString());
					}
				}
				_bindingCache.Add(mi, binding);
			}
			return binding;
		}
		
		public IInfo ResolvePrimitive(string name)
		{
			return (IInfo)_primitives[name];
		}
		
		public bool IsPrimitive(string name)
		{
			return _primitives.ContainsKey(name);
		}
		
		void PreparePrimitives()
		{
			AddPrimitiveType("void", VoidTypeInfo);
			AddPrimitiveType("bool", BoolTypeInfo);
			AddPrimitiveType("date", DateTimeTypeInfo);
			AddPrimitiveType("string", StringTypeInfo);
			AddPrimitiveType("object", ObjectTypeInfo);
			AddPrimitiveType("byte", ByteTypeInfo);
			AddPrimitiveType("int", IntTypeInfo);
			AddPrimitiveType("long", LongTypeInfo);
			AddPrimitiveType("float", SingleTypeInfo);
			AddPrimitiveType("double", DoubleTypeInfo);
			AddPrimitive("len", _lenInfo);
		}
		
		void AddPrimitiveType(string name, ExternalType type)
		{
			_primitives[name] = AsTypeReference(type);
		}
		
		void AddPrimitive(string name, IInfo binding)
		{
			_primitives[name] = binding;
		}
		
		void Cache(ExternalType binding)
		{
			_bindingCache[binding.Type] = binding;
		}
		
		void Cache(object key, ITypeInfo binding)
		{
			_bindingCache[key] = binding;
		}
		
		public static string GetSignature(IMethodInfo binding)
		{			
			System.Text.StringBuilder sb = new System.Text.StringBuilder(binding.DeclaringType.FullName);
			sb.Append(".");
			sb.Append(binding.Name);
			sb.Append("(");
			for (int i=0; i<binding.ParameterCount; ++i)
			{				
				if (i>0) 
				{
					sb.Append(", ");
				}
				sb.Append(binding.GetParameterType(i).FullName);
			}
			sb.Append(")");
			
			/*
			ITypeInfo rt = binding.ReturnType;
			if (null != rt)
			{
				sb.Append(" as ");
				sb.Append(rt.FullName);
			}
			*/
			return sb.ToString();
		}
		
		private static void NodeNotBound(Node node)
		{
			throw CompilerErrorFactory.NodeNotBound(node);
		}		
		
		#region VoidTypeInfoImpl
		class VoidTypeInfoImpl : ExternalType
		{			
			internal VoidTypeInfoImpl(DefaultInfoService manager) : base(manager, Types.Void)
			{				
			}		
			
			override public IInfo Resolve(string name)
			{	
				return null;
			}	
		}

		#endregion
	}
}
