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
	using System.Reflection;
	using Boo.Lang.Compiler;	
	using Boo.Lang.Compiler.Ast;

	public class TagService
	{			
		public ExternalType ExceptionType;
		
		public ExternalType ApplicationExceptionType;
		
		public ExternalType ObjectType;
		
		public ExternalType EnumType;
		
		public ExternalType ArrayType;
		
		public ExternalType TypeType;
		
		public IType ObjectArrayType;
	
		public ExternalType VoidType;
		
		public ExternalType StringType;
		
		public ExternalType BoolType;
		
		public ExternalType ByteType;
		
		public ExternalType ShortType;
		
		public ExternalType IntType;
		
		public ExternalType LongType;
		
		public ExternalType SingleType;
		
		public ExternalType DoubleType;
		
		public ExternalType TimeSpanType;
		
		public ExternalType DateTimeType;
		
		public ExternalType RuntimeServicesType;
		
		public ExternalType BuiltinsType;
		
		public ExternalType ListType;
		
		public ExternalType HashType;
		
		public ExternalType ICallableType;
		
		public ExternalType IEnumerableType;
		
		public ExternalType ICollectionType;
		
		public ExternalType IListType;
		
		public ExternalType IDictionaryType;
		
		System.Collections.Hashtable _primitives = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _tagCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _arrayCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _referenceCache = new System.Collections.Hashtable();
		
		static readonly IElement _lenInfo = new BuiltinFunction(BuiltinFunctionType.Len);
		
		public static readonly IElement ErrorTag = Boo.Lang.Compiler.Taxonomy.Error.Default;
		
		public TagService()		
		{			
			Cache(VoidType = new VoidTypeImpl(this));
			Cache(ObjectType = new ExternalType(this, Types.Object));
			Cache(EnumType = new ExternalType(this, typeof(System.Enum)));
			Cache(ArrayType = new ExternalType(this, Types.Array));
			Cache(TypeType = new ExternalType(this, Types.Type));
			Cache(StringType = new ExternalType(this, Types.String));
			Cache(BoolType = new ExternalType(this, Types.Bool));
			Cache(ByteType = new ExternalType(this, Types.Byte));
			Cache(ShortType = new ExternalType(this, Types.Short));
			Cache(IntType = new ExternalType(this, Types.Int));
			Cache(LongType = new ExternalType(this, Types.Long));
			Cache(SingleType = new ExternalType(this, Types.Single));
			Cache(DoubleType = new ExternalType(this, Types.Double));
			Cache(TimeSpanType = new ExternalType(this, Types.TimeSpan));
			Cache(DateTimeType = new ExternalType(this, Types.DateTime));
			Cache(RuntimeServicesType = new ExternalType(this, Types.RuntimeServices));
			Cache(BuiltinsType = new ExternalType(this, Types.Builtins));
			Cache(ListType = new ExternalType(this, Types.List));
			Cache(HashType = new ExternalType(this, Types.Hash));
			Cache(ICallableType = new ExternalType(this, Types.ICallable));
			Cache(IEnumerableType = new ExternalType(this, Types.IEnumerable));
			Cache(ICollectionType = new ExternalType(this, Types.ICollection));
			Cache(IListType = new ExternalType(this, Types.IList));
			Cache(IDictionaryType = new ExternalType(this, Types.IDictionary));
			Cache(ApplicationExceptionType = new ExternalType(this, Types.ApplicationException));
			Cache(ExceptionType = new ExternalType(this, Types.Exception));
			
			ObjectArrayType = GetArrayType(ObjectType);
			
			PreparePrimitives();
		}
		
		public Boo.Lang.Compiler.Ast.TypeReference CreateTypeReference(IType tag)
		{
			TypeReference typeReference = null;
			
			if (tag.IsArray)
			{
				typeReference = new ArrayTypeReference(CreateTypeReference(tag.GetElementType()));
			}
			else
			{				
				typeReference = new SimpleTypeReference(tag.FullName);				
			}
			
			typeReference.Tag = GetTypeReference(tag);
			return typeReference;
		}
		
		public IType GetPromotedNumberType(IType left, IType right)
		{
			if (left == DoubleType ||
				right == DoubleType)
			{
				return DoubleType;
			}
			if (left == SingleType ||
				right == SingleType)
			{
				return SingleType;
			}
			if (left == LongType ||
				right == LongType)
			{
				return LongType;
			}
			if (left == ShortType ||
				right == ShortType)
			{
				return ShortType;
			}
			return left;
		}
		
		public static bool IsUnknown(Node node)
		{
			ITypedElement tag = GetTag(node) as ITypedElement;
			if (null != tag)
			{
				return IsUnknown(tag.Type);
			}
			return false;
		}
		
		public static bool IsUnknown(IType tag)
		{
			return ElementType.Unknown == tag.ElementType;
		}
		
		public static bool IsError(Node node)
		{			
			ITypedElement tag = GetTag(node) as ITypedElement;
			if (null != tag)
			{
				return IsError(tag.Type);
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
		
		public static bool IsError(IElement tag)
		{
			return ElementType.Error == tag.ElementType;
		}
		
		public static IElement GetTag(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			IElement tag = node.Tag;
			if (null == tag)
			{
				NodeNotTagged(node);
			}
			return tag;
		}	
		
		public static IType GetType(Node node)
		{
			return ((ITypedElement)GetTag(node)).Type;
		}
		
		public IType Map(System.Type type)
		{				
			ExternalType tag = (ExternalType)_tagCache[type];
			if (null == tag)
			{
				if (type.IsArray)
				{
					return GetArrayType(Map(type.GetElementType()));
				}				
				else
				{
					tag = new ExternalType(this, type);
				}
				Cache(tag);
			}
			return tag;
		}
		
		public IType GetArrayType(IType elementType)
		{
			IType tag = (IType)_arrayCache[elementType];
			if (null == tag)
			{
				tag = new ArrayType(this, elementType);
				_arrayCache.Add(elementType, tag);
			}
			return tag;
		}
		
		public ITypedElement GetTypeReference(IType type)
		{
			ITypedElement tag = (ITypedElement)_referenceCache[type];
			if (null == tag)
			{
				tag = new TypeReferenceTag(type);
				_referenceCache[type] = tag;
			}
			return tag;
		}
		
		public ITypedElement GetTypeReference(System.Type type)
		{
			return GetTypeReference(Map(type));
		}
		
		public IParameter[] Map(Boo.Lang.Compiler.Ast.ParameterDeclarationCollection parameters)
		{
			IParameter[] mapped = new IParameter[parameters.Count];
			for (int i=0; i<mapped.Length; ++i)
			{
				mapped[i] = (IParameter)GetTag(parameters[i]);
			}
			return mapped;
		}
		
		public IParameter[] Map(System.Reflection.ParameterInfo[] parameters)
		{			
			IParameter[] mapped = new IParameter[parameters.Length];
			for (int i=0; i<parameters.Length; ++i)
			{
				mapped[i] = new ExternalParameter(this, parameters[i]);
			}
			return mapped;
		}
		
		public IElement Map(System.Reflection.MemberInfo[] info)
		{
			if (info.Length > 1)
			{
				IElement[] tags = new IElement[info.Length];
				for (int i=0; i<tags.Length; ++i)
				{
					tags[i] = Map(info[i]);
				}
				return new Ambiguous(tags);
			}
			if (info.Length > 0)
			{
				return Map(info[0]);
			}
			return null;
		}
		
		public IElement Map(System.Reflection.MemberInfo mi)
		{
			IElement tag = (IElement)_tagCache[mi];
			if (null == tag)
			{			
				switch (mi.MemberType)
				{
					case MemberTypes.Method:
					{
						tag = new ExternalMethod(this, (System.Reflection.MethodInfo)mi);
						break;
					}
					
					case MemberTypes.Constructor:
					{
						tag = new ExternalConstructor(this, (System.Reflection.ConstructorInfo)mi);
						break;
					}
					
					case MemberTypes.Field:
					{
						tag = new ExternalField(this, (System.Reflection.FieldInfo)mi);
						break;
					}
					
					case MemberTypes.Property:
					{
						tag = new ExternalProperty(this, (System.Reflection.PropertyInfo)mi);
						break;
					}
					
					case MemberTypes.Event:
					{
						tag = new ExternalEvent(this, (System.Reflection.EventInfo)mi);
						break;
					}
					
					case MemberTypes.TypeInfo:
					{
						return Map((System.Type)mi);
					}
					
					default:
					{
						throw new NotImplementedException(mi.ToString());
					}
				}
				_tagCache.Add(mi, tag);
			}
			return tag;
		}
		
		public IElement ResolvePrimitive(string name)
		{
			return (IElement)_primitives[name];
		}
		
		public bool IsPrimitive(string name)
		{
			return _primitives.ContainsKey(name);
		}
		
		void PreparePrimitives()
		{
			AddPrimitiveType("void", VoidType);
			AddPrimitiveType("bool", BoolType);
			AddPrimitiveType("date", DateTimeType);
			AddPrimitiveType("string", StringType);
			AddPrimitiveType("object", ObjectType);
			AddPrimitiveType("byte", ByteType);
			AddPrimitiveType("int", IntType);
			AddPrimitiveType("long", LongType);
			AddPrimitiveType("single", SingleType);
			AddPrimitiveType("double", DoubleType);
			AddPrimitive("len", _lenInfo);
		}
		
		void AddPrimitiveType(string name, ExternalType type)
		{
			_primitives[name] = GetTypeReference(type);
		}
		
		void AddPrimitive(string name, IElement tag)
		{
			_primitives[name] = tag;
		}
		
		void Cache(ExternalType tag)
		{
			_tagCache[tag.ActualType] = tag;
		}
		
		void Cache(object key, IType tag)
		{
			_tagCache[key] = tag;
		}
		
		private static void NodeNotTagged(Node node)
		{
			throw CompilerErrorFactory.NodeNotTagged(node);
		}		
		
		#region VoidTypeImpl
		class VoidTypeImpl : ExternalType
		{			
			internal VoidTypeImpl(TagService manager) : base(manager, Types.Void)
			{				
			}		
			
			override public IElement Resolve(string name)
			{	
				return null;
			}	
		}

		#endregion
	}
}
