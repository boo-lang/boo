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
using Boo.Lang.Ast;

namespace Boo.Lang.Compiler.Bindings
{
	public class BindingManager
	{		
		public ExternalTypeBinding ExceptionTypeBinding;
		
		public ExternalTypeBinding ApplicationExceptionBinding;
		
		public ExternalTypeBinding ObjectTypeBinding;
		
		public ExternalTypeBinding EnumTypeBinding;
		
		public ExternalTypeBinding ArrayTypeBinding;
		
		public ExternalTypeBinding TypeTypeBinding;
		
		public ITypeBinding ObjectTupleBinding;
	
		public ExternalTypeBinding VoidTypeBinding;
		
		public ExternalTypeBinding StringTypeBinding;
		
		public ExternalTypeBinding BoolTypeBinding;
		
		public ExternalTypeBinding ByteTypeBinding;
		
		public ExternalTypeBinding IntTypeBinding;
		
		public ExternalTypeBinding LongTypeBinding;
		
		public ExternalTypeBinding SingleTypeBinding;
		
		public ExternalTypeBinding RealTypeBinding;
		
		public ExternalTypeBinding TimeSpanTypeBinding;
		
		public ExternalTypeBinding RuntimeServicesBinding;
		
		public ExternalTypeBinding BuiltinsBinding;
		
		public ExternalTypeBinding ListTypeBinding;
		
		public ExternalTypeBinding HashTypeBinding;
		
		public ExternalTypeBinding IEnumerableTypeBinding;
		
		public ExternalTypeBinding ICollectionTypeBinding;
		
		public ExternalTypeBinding IListTypeBinding;
		
		public ExternalTypeBinding IDictionaryTypeBinding;
		
		System.Collections.Hashtable _bindingCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _tupleBindingCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _referenceCache = new System.Collections.Hashtable();
		
		static readonly IBinding _typeOfBinding = new SpecialFunctionBinding(SpecialFunction.Typeof);
		
		static readonly IBinding _lenBinding = new SpecialFunctionBinding(SpecialFunction.Len);
		
		public BindingManager()		
		{			
			Cache(VoidTypeBinding = new VoidTypeBindingImpl(this));
			Cache(ObjectTypeBinding = new ExternalTypeBinding(this, Types.Object));
			Cache(EnumTypeBinding = new ExternalTypeBinding(this, typeof(System.Enum)));
			Cache(ArrayTypeBinding = new ExternalTypeBinding(this, Types.Array));
			Cache(TypeTypeBinding = new ExternalTypeBinding(this, Types.Type));
			Cache(StringTypeBinding = new ExternalTypeBinding(this, Types.String));
			Cache(BoolTypeBinding = new ExternalTypeBinding(this, Types.Bool));
			Cache(ByteTypeBinding = new ExternalTypeBinding(this, Types.Byte));
			Cache(IntTypeBinding = new ExternalTypeBinding(this, Types.Int));
			Cache(LongTypeBinding = new ExternalTypeBinding(this, Types.Long));
			Cache(SingleTypeBinding = new ExternalTypeBinding(this, Types.Single));
			Cache(RealTypeBinding = new ExternalTypeBinding(this, Types.Real));
			Cache(TimeSpanTypeBinding = new ExternalTypeBinding(this, Types.TimeSpan));
			Cache(new ExternalTypeBinding(this, Types.Date));
			Cache(RuntimeServicesBinding = new ExternalTypeBinding(this, Types.RuntimeServices));
			Cache(BuiltinsBinding = new ExternalTypeBinding(this, Types.Builtins));
			Cache(ListTypeBinding = new ExternalTypeBinding(this, Types.List));
			Cache(HashTypeBinding = new ExternalTypeBinding(this, Types.Hash));
			Cache(IEnumerableTypeBinding = new ExternalTypeBinding(this, Types.IEnumerable));
			Cache(ICollectionTypeBinding = new ExternalTypeBinding(this, Types.ICollection));
			Cache(IListTypeBinding = new ExternalTypeBinding(this, Types.IList));
			Cache(IDictionaryTypeBinding = new ExternalTypeBinding(this, Types.IDictionary));
			Cache(ApplicationExceptionBinding = new ExternalTypeBinding(this, Types.ApplicationException));
			Cache(ExceptionTypeBinding = new ExternalTypeBinding(this, Types.Exception));
			
			ObjectTupleBinding = AsTupleBinding(ObjectTypeBinding);
		}
		
		public Boo.Lang.Ast.TypeReference CreateBoundTypeReference(ITypeBinding binding)
		{
			TypeReference typeReference = null;
			
			if (binding.IsArray)
			{
				typeReference = new TupleTypeReference(CreateBoundTypeReference(binding.GetElementType()));
			}
			else
			{				
				typeReference = new SimpleTypeReference(binding.FullName);				
			}
			Bind(typeReference, AsTypeReference(binding));
			return typeReference;
		}
		
		public ITypeBinding GetPromotedNumberType(ITypeBinding left, ITypeBinding right)
		{
			if (left == RealTypeBinding ||
				right == RealTypeBinding)
			{
				return RealTypeBinding;
			}
			if (left == SingleTypeBinding ||
				right == SingleTypeBinding)
			{
				return SingleTypeBinding;
			}
			if (left == LongTypeBinding ||
				right == LongTypeBinding)
			{
				return LongTypeBinding;
			}
			return left;
		}
		
		public static bool IsUnknown(Node node)
		{
			ITypedBinding binding = GetBinding(node) as ITypedBinding;
			if (null != binding)
			{
				return IsUnknown(binding.BoundType);
			}
			return false;
		}
		
		public static bool IsUnknown(ITypeBinding binding)
		{
			return BindingType.Unknown == binding.BindingType;
		}
		
		public static bool IsError(Node node)
		{			
			ITypedBinding binding = GetBinding(node) as ITypedBinding;
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
		
		public static bool IsError(IBinding binding)
		{
			return BindingType.Error == binding.BindingType;
		}		
		
		public static bool IsBound(Node node)
		{
			return null != node[BindingKey];
		}
		
		public static void Bind(Node node, IBinding binding)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			if (null == binding)
			{
				throw new ArgumentNullException("binding");
			}
			
			node[BindingKey] = binding;
		}
		
		public void Bind(TypeDefinition type)
		{
			Bind(type, AsTypeBinding(type));
		}
		
		public static void Error(Node node)
		{
			Bind(node, ErrorBinding.Default);
		}
		
		public static IBinding GetOptionalBinding(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			return (IBinding)node[BindingKey];
		}
		
		public static IBinding GetBinding(Node node)
		{
			IBinding binding = GetOptionalBinding(node);
			if (null == binding)
			{
				NodeNotBound(node);
			}
			return binding;
		}	
		
		public ITypeBinding GetBoundType(Node node)
		{
			return ((ITypedBinding)GetBinding(node)).BoundType;
		}
		
		public ITypeBinding AsTypeBinding(System.Type type)
		{
			if (type.IsArray)
			{
				return AsTupleBinding(AsTypeBinding(type.GetElementType()));
			}
			
			ExternalTypeBinding binding = (ExternalTypeBinding)_bindingCache[type];
			if (null == binding)
			{
				Cache(binding = new ExternalTypeBinding(this, type));
			}
			return binding;
		}
		
		public ITypeBinding AsTypeBinding(TypeDefinition typeDefinition)
		{
			ITypeBinding binding = (ITypeBinding)_bindingCache[typeDefinition];
			if (null == binding)
			{
				Cache(typeDefinition, binding = new InternalTypeBinding(this, typeDefinition));
			}
			return binding;
		}
		
		public ITypeBinding AsTupleBinding(ITypeBinding elementType)
		{
			ITypeBinding binding = (ITypeBinding)_tupleBindingCache[elementType];
			if (null == binding)
			{
				_tupleBindingCache.Add(elementType, binding = new TupleTypeBinding(this, elementType));
			}
			return binding;
		}
		
		public ITypedBinding AsTypeReference(ITypeBinding type)
		{
			ITypedBinding cached = (ITypedBinding)_referenceCache[type];
			if (null == cached)
			{
				cached = new TypeReferenceBinding(type);
				_referenceCache[type] = cached;
			}
			return cached;
		}
		
		public ITypedBinding AsTypeReference(System.Type type)
		{
			return AsTypeReference(AsTypeBinding(type));
		}
		
		public IBinding AsBinding(System.Reflection.MemberInfo[] info)
		{
			if (info.Length > 1)
			{
				IBinding[] bindings = new IBinding[info.Length];
				for (int i=0; i<bindings.Length; ++i)
				{
					bindings[i] = AsBinding(info[i]);
				}
				return new AmbiguousBinding(bindings);
			}
			return AsBinding(info[0]);
		}
		
		public IBinding AsBinding(System.Reflection.MemberInfo mi)
		{
			IBinding binding = (IBinding)_bindingCache[mi];
			if (null == binding)
			{			
				switch (mi.MemberType)
				{
					case MemberTypes.Method:
					{
						binding = new ExternalMethodBinding(this, (System.Reflection.MethodInfo)mi);
						break;
					}
					
					case MemberTypes.Constructor:
					{
						binding = new ExternalConstructorBinding(this, (System.Reflection.ConstructorInfo)mi);
						break;
					}
					
					case MemberTypes.Field:
					{
						binding = new ExternalFieldBinding(this, (System.Reflection.FieldInfo)mi);
						break;
					}
					
					case MemberTypes.Property:
					{
						binding = new ExternalPropertyBinding(this, (System.Reflection.PropertyInfo)mi);
						break;
					}
					
					case MemberTypes.Event:
					{
						binding = new ExternalEventBinding(this, (System.Reflection.EventInfo)mi);
						break;
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
		
		public IBinding ResolvePrimitive(string name)
		{
			IBinding binding = null;
			switch (name)
			{
				case "void":
				{
					binding = AsTypeReference(VoidTypeBinding);
					break;
				}
				
				case "bool":
				{
					binding = AsTypeReference(BoolTypeBinding);
					break;
				}
				
				case "date":
				{
					binding = AsTypeReference(Types.Date);
					break;
				}
				
				case "string":
				{
					binding = AsTypeReference(StringTypeBinding);
					break;
				}
				
				case "object":
				{
					binding = AsTypeReference(ObjectTypeBinding);
					break;
				}
				
				case "byte":
				{
					binding = AsTypeReference(ByteTypeBinding);
					break;
				}
				
				case "real":
				{
					binding = AsTypeReference(RealTypeBinding);
					break;
				}
				
				case "int":
				{
					binding = AsTypeReference(IntTypeBinding);
					break;
				}
				
				case "long":
				{
					binding = AsTypeReference(LongTypeBinding);
					break;
				}
				
				case "typeof":
				{
					binding = _typeOfBinding;
					break;
				}
				
				case "len":
				{
					binding = _lenBinding;
					break;
				}
			}
			return binding;
		}
		
		void Cache(ExternalTypeBinding binding)
		{
			_bindingCache[binding.Type] = binding;
		}
		
		void Cache(object key, ITypeBinding binding)
		{
			_bindingCache[key] = binding;
		}
		
		public static string GetSignature(IMethodBinding binding)
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
			
			ITypeBinding rt = binding.ReturnType;
			if (null != rt)
			{
				sb.Append(" as ");
				sb.Append(rt.FullName);
			}
			return sb.ToString();
		}
		
		private static void NodeNotBound(Node node)
		{
			throw CompilerErrorFactory.NodeNotBound(node);
		}
		
		static object BindingKey = new object();		
		
		#region VoidTypeBindingImpl
		class VoidTypeBindingImpl : ExternalTypeBinding
		{			
			internal VoidTypeBindingImpl(BindingManager manager) : base(manager, Types.Void)
			{				
			}		
			
			public override IBinding Resolve(string name)
			{	
				return null;
			}	
		}

		#endregion
	}
}
