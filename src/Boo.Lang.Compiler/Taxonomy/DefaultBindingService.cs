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
		public ExternalTypeInfo ExceptionTypeInfo;
		
		public ExternalTypeInfo ApplicationExceptionInfo;
		
		public ExternalTypeInfo ObjectTypeInfo;
		
		public ExternalTypeInfo EnumTypeInfo;
		
		public ExternalTypeInfo ArrayTypeInfo;
		
		public ExternalTypeInfo TypeTypeInfo;
		
		public ITypeInfo ObjectArrayInfo;
	
		public ExternalTypeInfo VoidTypeInfo;
		
		public ExternalTypeInfo StringTypeInfo;
		
		public ExternalTypeInfo BoolTypeInfo;
		
		public ExternalTypeInfo ByteTypeInfo;
		
		public ExternalTypeInfo ShortTypeInfo;
		
		public ExternalTypeInfo IntTypeInfo;
		
		public ExternalTypeInfo LongTypeInfo;
		
		public ExternalTypeInfo SingleTypeInfo;
		
		public ExternalTypeInfo DoubleTypeInfo;
		
		public ExternalTypeInfo TimeSpanTypeInfo;
		
		public ExternalTypeInfo DateTimeTypeInfo;
		
		public ExternalTypeInfo RuntimeServicesInfo;
		
		public ExternalTypeInfo BuiltinsInfo;
		
		public ExternalTypeInfo ListTypeInfo;
		
		public ExternalTypeInfo HashTypeInfo;
		
		public ExternalTypeInfo ICallableTypeInfo;
		
		public ExternalTypeInfo IEnumerableTypeInfo;
		
		public ExternalTypeInfo ICollectionTypeInfo;
		
		public ExternalTypeInfo IListTypeInfo;
		
		public ExternalTypeInfo IDictionaryTypeInfo;
		
		System.Collections.Hashtable _primitives = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _bindingCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _arrayInfoCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _referenceCache = new System.Collections.Hashtable();
		
		static readonly IInfo _lenInfo = new SpecialFunctionInfo(SpecialFunction.Len);
		
		public DefaultInfoService()		
		{			
			Cache(VoidTypeInfo = new VoidTypeInfoImpl(this));
			Cache(ObjectTypeInfo = new ExternalTypeInfo(this, Types.Object));
			Cache(EnumTypeInfo = new ExternalTypeInfo(this, typeof(System.Enum)));
			Cache(ArrayTypeInfo = new ExternalTypeInfo(this, Types.Array));
			Cache(TypeTypeInfo = new ExternalTypeInfo(this, Types.Type));
			Cache(StringTypeInfo = new ExternalTypeInfo(this, Types.String));
			Cache(BoolTypeInfo = new ExternalTypeInfo(this, Types.Bool));
			Cache(ByteTypeInfo = new ExternalTypeInfo(this, Types.Byte));
			Cache(ShortTypeInfo = new ExternalTypeInfo(this, Types.Short));
			Cache(IntTypeInfo = new ExternalTypeInfo(this, Types.Int));
			Cache(LongTypeInfo = new ExternalTypeInfo(this, Types.Long));
			Cache(SingleTypeInfo = new ExternalTypeInfo(this, Types.Single));
			Cache(DoubleTypeInfo = new ExternalTypeInfo(this, Types.Double));
			Cache(TimeSpanTypeInfo = new ExternalTypeInfo(this, Types.TimeSpan));
			Cache(DateTimeTypeInfo = new ExternalTypeInfo(this, Types.DateTime));
			Cache(RuntimeServicesInfo = new ExternalTypeInfo(this, Types.RuntimeServices));
			Cache(BuiltinsInfo = new ExternalTypeInfo(this, Types.Builtins));
			Cache(ListTypeInfo = new ExternalTypeInfo(this, Types.List));
			Cache(HashTypeInfo = new ExternalTypeInfo(this, Types.Hash));
			Cache(ICallableTypeInfo = new ExternalTypeInfo(this, Types.ICallable));
			Cache(IEnumerableTypeInfo = new ExternalTypeInfo(this, Types.IEnumerable));
			Cache(ICollectionTypeInfo = new ExternalTypeInfo(this, Types.ICollection));
			Cache(IListTypeInfo = new ExternalTypeInfo(this, Types.IList));
			Cache(IDictionaryTypeInfo = new ExternalTypeInfo(this, Types.IDictionary));
			Cache(ApplicationExceptionInfo = new ExternalTypeInfo(this, Types.ApplicationException));
			Cache(ExceptionTypeInfo = new ExternalTypeInfo(this, Types.Exception));
			
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
			
			ExternalTypeInfo binding = (ExternalTypeInfo)_bindingCache[type];
			if (null == binding)
			{
				Cache(binding = new ExternalTypeInfo(this, type));
			}
			return binding;
		}
		
		public ITypeInfo AsTypeInfo(TypeDefinition typeDefinition)
		{
			ITypeInfo binding = (ITypeInfo)_bindingCache[typeDefinition];
			if (null == binding)
			{
				Cache(typeDefinition, binding = new InternalTypeInfo(this, typeDefinition));
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
				return new AmbiguousInfo(bindings);
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
						binding = new ExternalMethodInfo(this, (System.Reflection.MethodInfo)mi);
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
						binding = new ExternalPropertyInfo(this, (System.Reflection.PropertyInfo)mi);
						break;
					}
					
					case MemberTypes.Event:
					{
						binding = new ExternalEventInfo(this, (System.Reflection.EventInfo)mi);
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
		
		void AddPrimitiveType(string name, ExternalTypeInfo type)
		{
			_primitives[name] = AsTypeReference(type);
		}
		
		void AddPrimitive(string name, IInfo binding)
		{
			_primitives[name] = binding;
		}
		
		void Cache(ExternalTypeInfo binding)
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
		class VoidTypeInfoImpl : ExternalTypeInfo
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
