#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Text;
	using Boo.Lang.Compiler;	
	using Boo.Lang.Compiler.Ast;

	public class TypeSystemServices
	{			
		public DuckTypeImpl DuckType;
		
		public ExternalType ExceptionType;
		
		public ExternalType ApplicationExceptionType;
		
		public ExternalType MulticastDelegateType;
		
		public ExternalType DelegateType;
		
		public ExternalType IntPtrType;
		
		public ExternalType ObjectType;
		
		public ExternalType ValueTypeType;
		
		public ExternalType EnumType;
		
		public ExternalType RegexType;
		
		public ExternalType ArrayType;
		
		public ExternalType TypeType;
		
		public IType ObjectArrayType;
	
		public ExternalType VoidType;
		
		public ExternalType StringType;
		
		public ExternalType BoolType;
		
		public ExternalType CharType;
		
		public ExternalType SByteType;
		
		public ExternalType ByteType;
		
		public ExternalType ShortType;
		
		public ExternalType UShortType;
		
		public ExternalType IntType;
		
		public ExternalType UIntType;
		
		public ExternalType LongType;
		
		public ExternalType ULongType;
		
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
		
		public ExternalType IEnumeratorType;
		
		public ExternalType ICollectionType;
		
		public ExternalType IListType;
		
		public ExternalType IDictionaryType;
		
		System.Collections.Hashtable _primitives = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _entityCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _arrayCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _anonymousCallableTypes = new System.Collections.Hashtable();
		
		public static readonly IType ErrorEntity = Boo.Lang.Compiler.TypeSystem.Error.Default;
		
		public readonly BooCodeBuilder CodeBuilder;
		
		StringBuilder _buffer = new StringBuilder();
		
		Boo.Lang.Compiler.Ast.Module _anonymousTypesModule;
		
		CompilerContext _context;
		
		public TypeSystemServices() : this(new CompilerContext())
		{
		}
		
		public TypeSystemServices(CompilerContext context)		
		{			
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			
			_context = context;

			CodeBuilder = new BooCodeBuilder(this);			
			
			Cache(typeof(Boo.Lang.Builtins.duck), DuckType = new DuckTypeImpl(this));
			Cache(VoidType = new VoidTypeImpl(this));
			Cache(ObjectType = new ExternalType(this, Types.Object));
			Cache(RegexType = new ExternalType(this, Types.Regex));
			Cache(ValueTypeType = new ExternalType(this, typeof(System.ValueType)));
			Cache(EnumType = new ExternalType(this, typeof(System.Enum)));
			Cache(ArrayType = new ExternalType(this, Types.Array));
			Cache(TypeType = new ExternalType(this, Types.Type));
			Cache(StringType = new ExternalType(this, Types.String));
			Cache(BoolType = new ExternalType(this, Types.Bool));
			Cache(SByteType = new ExternalType(this, Types.SByte));
			Cache(CharType = new ExternalType(this, Types.Char));
			Cache(ShortType = new ExternalType(this, Types.Short));
			Cache(IntType = new ExternalType(this, Types.Int));
			Cache(LongType = new ExternalType(this, Types.Long));
			Cache(ByteType = new ExternalType(this, Types.Byte));
			Cache(UShortType = new ExternalType(this, Types.UShort));
			Cache(UIntType = new ExternalType(this, Types.UInt));
			Cache(ULongType = new ExternalType(this, Types.ULong));
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
			Cache(IEnumeratorType = new ExternalType(this, typeof(System.Collections.IEnumerator)));
			Cache(ICollectionType = new ExternalType(this, Types.ICollection));
			Cache(IListType = new ExternalType(this, Types.IList));
			Cache(IDictionaryType = new ExternalType(this, Types.IDictionary));
			Cache(ApplicationExceptionType = new ExternalType(this, Types.ApplicationException));
			Cache(ExceptionType = new ExternalType(this, Types.Exception));
			Cache(IntPtrType = new ExternalType(this, Types.IntPtr));
			Cache(MulticastDelegateType = new ExternalType(this, Types.MulticastDelegate));
			Cache(DelegateType = new ExternalType(this, Types.Delegate));
			
			ObjectArrayType = GetArrayType(ObjectType);
			
			PreparePrimitives();
		}
		
		public CompilerContext Context
		{
			get
			{
				return _context;
			}
		}
		
		public IType GetMostGenericType(IType current, IType candidate)
		{
			if (current.IsAssignableFrom(candidate))
			{
				return current;
			}
			
			if (candidate.IsAssignableFrom(current))
			{
				return candidate;
			}
			
			if (IsNumberOrBool(current) && IsNumberOrBool(candidate))
			{
				return GetPromotedNumberType(current, candidate);
			}
			
			if (IsCallableType(current) && IsCallableType(candidate))
			{
				return ICallableType;
			}
			
			IType obj = ObjectType;			
			if (current.IsClass && candidate.IsClass)
			{
				if (current ==  obj || candidate == obj)
				{
					return obj;
				}
				if (current.GetTypeDepth() < candidate.GetTypeDepth())
				{
					return GetMostGenericType(current.BaseType, candidate);
				}			
				return GetMostGenericType(current, candidate.BaseType);
			}			
			return obj;
		}
		
		public IType GetPromotedNumberType(IType left, IType right)
		{
/* Don't know if we ever want to support decimals
 * If so, the arg exception will have to be handled properly
			if (left == DecimalType)
			{
				if (right == DoubleType ||
				    right == SingleType)
				{
					throw new ArgumentException("decimal <op> " + right);
				}
				return DecimalType;
			}
			if (right == DecimalType)
			{
				if (left == DoubleType ||
				    left == SingleType)
				{
					throw new ArgumentException(left + " <op> decimal");
				}
				return DecimalType;
			}
*/
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
			if (left == ULongType)
			{
				if (right == SByteType ||
				    right == ShortType ||
				    right == IntType ||
				    right == LongType)
				{
					// This is against the C# spec but allows expressions like:
					//    ulong x = 4
					//    y = x + 1
					// y will be long.
					// C# disallows mixing ulongs and signed numbers
					// but in the above case it promotes the constant to ulong
					// and the result is ulong.
					// Since its too late here to promote the constant,
					// maybe we should return LongType.  I didn't chose ULongType
					// because in other cases <unsigned> <op> <signed> returns <signed>.
					return LongType;
				}
				return ULongType;
			}
			if (right == ULongType)
			{
				if (left == SByteType ||
				    left == ShortType ||
				    left == IntType ||
				    left == LongType)
				{
					// This is against the C# spec but allows expressions like:
					//    ulong x = 4
					//    y = 1 + x
					// y will be long.
					// C# disallows mixing ulongs and signed numbers
					// but in the above case it promotes the constant to ulong
					// and the result is ulong.
					// Since its too late here to promote the constant,
					// maybe we should return LongType.  I didn't chose ULongType
					// because in other cases <signed> <op> <unsigned> returns <signed>.
					return LongType;
				}
				return ULongType;
			}
			if (left == LongType ||
				right == LongType)
			{
				return LongType;
			}
			if (left == UIntType)
			{
				if (right == SByteType ||
				    right == ShortType ||
				    right == IntType)
				{
					// This is allowed per C# spec and y is long:
					//    uint x = 4
					//    y = x + 1
					// C# promotes <uint> <op> <signed> to <long> also
					// but in the above case it promotes the constant to uint first
					// and the result of "x + 1" is uint.
					// Since its too late here to promote the constant,
					// "y = x + 1" will be long in boo.
					return LongType;
				}
				return UIntType;
			}
			if (right == UIntType)
			{
				if (left == SByteType ||
				    left == ShortType ||
				    left == IntType)
				{
					// This is allowed per C# spec and y is long:
					//    uint x = 4
					//    y = 1 + x
					// C# promotes <signed> <op> <uint> to <long> also
					// but in the above case it promotes the constant to uint first
					// and the result of "1 + x" is uint.
					// Since its too late here to promote the constant,
					// "y = x + 1" will be long in boo.
					return LongType;
				}
				return UIntType;
			}
			if (left == IntType ||
				right == IntType ||
				left == ShortType ||
				right == ShortType ||
				left == UShortType ||
				right == UShortType ||
				left == ByteType ||
				right == ByteType ||
				left == SByteType ||
				right == SByteType)
			{
				return IntType;
			}
			return left;
		}
		
		public bool IsCallable(IType type)
		{
			return (TypeType == type) || IsCallableType(type);
		}
		
		bool IsCallableType(IType type)
		{			
			return (ICallableType.IsAssignableFrom(type)) ||
				(type is Boo.Lang.Compiler.TypeSystem.ICallableType);
		}
		
		public AnonymousCallableType GetCallableType(IMethod method)
		{
			CallableSignature signature = new CallableSignature(method);
			return GetCallableType(signature);
		}
		
		public AnonymousCallableType GetCallableType(CallableSignature signature)
		{
			AnonymousCallableType type = (AnonymousCallableType)_anonymousCallableTypes[signature];
			if (null == type)
			{
				type = new AnonymousCallableType(this, signature);
				_anonymousCallableTypes.Add(signature, type);
			}
			return type;
		}
		
		public IType GetConcreteCallableType(Node sourceNode, CallableSignature signature)
		{
			AnonymousCallableType type = GetCallableType(signature);
			return GetConcreteCallableType(sourceNode, type);
		}
		
		public IType GetEnumeratorItemType(IType iteratorType)
		{
			if (iteratorType.IsArray)
			{
				return ((IArrayType)iteratorType).GetElementType();
			}
			else
			{
				if (iteratorType.IsClass)
				{
					IType enumeratorItemType = GetEnumeratorItemTypeFromAttribute(iteratorType);
					if (null != enumeratorItemType)
					{
						return enumeratorItemType;
					}
				}
			}
			return ObjectType;
		}
		
		public IType GetExpressionType(Expression node)
		{			
			IType type = node.ExpressionType;
			if (null == type)
			{
				throw CompilerErrorFactory.InvalidNode(node);
			}
			return type;
		}
		
		public IType GetConcreteExpressionType(Expression expression)
		{
			IType type = GetExpressionType(expression);
			AnonymousCallableType anonymousType = type as AnonymousCallableType;
			if (null != anonymousType)
			{
				IType concreteType = GetConcreteCallableType(expression, anonymousType);
				expression.ExpressionType = concreteType;
				return concreteType;
			}
			return type;
		}
		
		public void MapToConcreteExpressionTypes(ExpressionCollection items)
		{
			foreach (Expression item in items)
			{
				GetConcreteExpressionType(item);
			}
		}
		
		public Boo.Lang.Compiler.Ast.Module GetAnonymousTypesModule()
		{
			if (null == _anonymousTypesModule)
			{
				_anonymousTypesModule = new Boo.Lang.Compiler.Ast.Module();
				_anonymousTypesModule.Entity = new ModuleEntity(_context.NameResolutionService, 
																this,
																_anonymousTypesModule);
				_context.CompileUnit.Modules.Add(_anonymousTypesModule);
			}
			return _anonymousTypesModule;
		}
		
		public ClassDefinition CreateCallableDefinition(string name)
		{
			ClassDefinition cd = new ClassDefinition();
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(this.MulticastDelegateType));
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(this.ICallableType));
			cd.Name = name;
			cd.Modifiers = TypeMemberModifiers.Final;
			cd.Members.Add(CreateCallableConstructor());
			cd.Members.Add(CreateCallMethod());			
			cd.Entity = new InternalCallableType(this, cd);
			return cd;
		}
		
		Method CreateCallMethod()
		{
			Method method = new Method("Call");
			method.Modifiers = TypeMemberModifiers.Public|TypeMemberModifiers.Virtual;
			method.Parameters.Add(CodeBuilder.CreateParameterDeclaration(1, "args", ObjectArrayType));
			method.ReturnType = CodeBuilder.CreateTypeReference(ObjectType);
			method.Entity = new InternalMethod(this, method);
			return method;
		}
		
		Constructor CreateCallableConstructor()
		{
			Constructor constructor = new Constructor();
			constructor.Modifiers = TypeMemberModifiers.Public;
			constructor.ImplementationFlags = MethodImplementationFlags.Runtime;
			constructor.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(1, "instance", ObjectType));
			constructor.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(2, "method", IntPtrType));
			constructor.Entity = new InternalConstructor(this, constructor);						
			return constructor;
		}
		
		public bool AreTypesRelated(IType lhs, IType rhs)
		{
			return lhs.IsAssignableFrom(rhs) ||
				(lhs.IsInterface && rhs.IsInterface) ||
				CanBeReachedByDownCastOrPromotion(lhs, rhs);
		}
		
		public bool IsCallableTypeAssignableFrom(ICallableType lhs, IType rhs)
		{
			if (lhs == rhs || Null.Default == rhs)
			{
				return true;
			}
			
			ICallableType other = rhs as ICallableType;
			if (null != other)
			{			
				CallableSignature lvalue = lhs.GetSignature();
				CallableSignature rvalue = other.GetSignature(); 
				if (lvalue == rvalue)
				{
					return true;
				}
				
				IParameter[] lparams = lvalue.Parameters;
				IParameter[] rparams = rvalue.Parameters;
				if (lparams.Length >= rparams.Length)
				{
					for (int i=0; i<rparams.Length; ++i)
					{
						IType lparamType = lparams[i].Type;
						IType rparamType = rparams[i].Type;
						if (!AreTypesRelated(lparamType, rparamType))
						{
							return false;
						}
					}
					
					if (VoidType != lvalue.ReturnType)
					{				
						return AreTypesRelated(lvalue.ReturnType, rvalue.ReturnType);
					}
					
					return true;
				}
			}
			return false;
		}
		
		public static bool CheckOverrideSignature(IMethod impl, IMethod baseMethod)
		{
			IParameter[] implParameters = impl.GetParameters();
			IParameter[] baseParameters = baseMethod.GetParameters();
			
			if (implParameters.Length == baseParameters.Length)
			{
				for (int i=0; i<implParameters.Length; ++i)
				{
					if (implParameters[i].Type != baseParameters[i].Type)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		
		public bool CanBeReachedByDownCastOrPromotion(IType expectedType, IType actualType)
		{			
			if (actualType.IsAssignableFrom(expectedType))
			{
				return true;
			}
			if (expectedType.IsValueType)
			{
				return IsNumber(expectedType) && IsNumber(actualType);
			}
			return false;
		}
		
		public static bool IsPrimitiveTypeOrString(IType type)
		{
			ExternalType external = type as ExternalType;
			if (null != external)
			{
				Type actual = external.ActualType;
				return actual.IsPrimitive || Types.String == actual;
			}
			return false;
		}
		
		public bool IsIntegerNumber(IType type)
		{
			return
				type == this.ShortType ||
				type == this.IntType ||
				type == this.LongType ||
				type == this.SByteType ||
				type == this.UShortType ||
				type == this.UIntType ||				
				type == this.ULongType ||
				type == this.ByteType;
		}
		
		public bool IsIntegerOrBool(IType type)	
		{
			return BoolType == type || IsIntegerNumber(type);
		}
		
		public bool IsNumberOrBool(IType type)
		{
			return BoolType == type || IsNumber(type);
		}
		
		public bool IsNumber(IType type)
		{
			return
				IsIntegerNumber(type) ||
				//IsUnsignedNumber(type) ||
				type == this.DoubleType ||
				type == this.SingleType;
		}
		
		public static bool IsUnknown(Expression node)
		{
			IType type = node.ExpressionType;
			if (null != type)
			{
				return IsUnknown(type);
			}
			return false;
		}
		
		public static bool IsUnknown(IType tag)
		{
			return EntityType.Unknown == tag.EntityType;
		}
		
		public static bool IsError(Expression node)
		{				 
			IType type = node.ExpressionType;
			if (null != type)
			{
				return IsError(type);
			}
			return false;
		}
		
		public static bool IsErrorAny(ExpressionCollection collection)
		{
			foreach (Expression n in collection)
			{
				if (IsError(n))
				{
					return true;
				}
			}
			return false;
		}
		
		public bool IsBuiltin(IEntity tag)
		{
			if (EntityType.Method == tag.EntityType)
			{
				return BuiltinsType == ((IMethod)tag).DeclaringType;
			}
			return false;
		}		
		
		public static bool IsError(IEntity tag)
		{
			return EntityType.Error == tag.EntityType;
		}
		
		public static TypeMemberModifiers GetAccess(IAccessibleMember member)
		{
			if (member.IsPublic)
			{
				return TypeMemberModifiers.Public;
			}
			else if (member.IsProtected)
			{
				return TypeMemberModifiers.Protected;
			}
			return TypeMemberModifiers.Private;
		}
		
		public static IEntity GetEntity(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			IEntity tag = node.Entity;
			if (null == tag)
			{
				InvalidNode(node);
			}
			return tag;
		}	
		
		public static IType GetReferencedType(Expression typeref)
		{
			switch (typeref.NodeType)
			{
				case NodeType.TypeofExpression:
				{
					return GetType(((TypeofExpression)typeref).Type);
				}
				case NodeType.ReferenceExpression:
				case NodeType.MemberReferenceExpression:
				{
					return typeref.Entity as IType;					
				}
			}
			return null;
		}
		
		public static IType GetType(Node node)
		{
			return ((ITypedEntity)GetEntity(node)).Type;
		}
		
		public IType Map(System.Type type)
		{				
			ExternalType tag = (ExternalType)_entityCache[type];
			if (null == tag)
			{
				if (type.IsArray)
				{
					return GetArrayType(Map(type.GetElementType()));
				}				
				else
				{
					if (type.IsSubclassOf(Types.MulticastDelegate))
					{
						tag = new ExternalCallableType(this, type);
					}
					else
					{
						tag = new ExternalType(this, type);
					}
				}
				Cache(tag);
			}
			return tag;
		}
		
		public IArrayType GetArrayType(IType elementType)
		{
			IArrayType tag = (IArrayType)_arrayCache[elementType];
			if (null == tag)
			{
				tag = new ArrayType(this, elementType);
				_arrayCache.Add(elementType, tag);
			}
			return tag;
		}
		
		public IParameter[] Map(Boo.Lang.Compiler.Ast.ParameterDeclarationCollection parameters)
		{
			IParameter[] mapped = new IParameter[parameters.Count];
			for (int i=0; i<mapped.Length; ++i)
			{
				mapped[i] = (IParameter)GetEntity(parameters[i]);
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
		
		public IConstructor Map(ConstructorInfo constructor)
		{
			object key = GetCacheKey(constructor);
			IConstructor entity = (IConstructor)_entityCache[key];
			if (null == entity)
			{
				entity = new ExternalConstructor(this, constructor);
				_entityCache[key] = entity;
			}
			return entity;
		}
		
		public IMethod Map(MethodInfo method)
		{
			object key = GetCacheKey(method);
			IMethod entity = (IMethod)_entityCache[key];
			if (null == entity)
			{
				entity = new ExternalMethod(this, method);
				_entityCache[key] = entity;
			}
			return entity;
		}
		
		public IEntity Map(System.Reflection.MemberInfo[] info)
		{
			if (info.Length > 1)
			{
				IEntity[] tags = new IEntity[info.Length];
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
		
		public IEntity Map(System.Reflection.MemberInfo mi)
		{
			IEntity tag = (IEntity)_entityCache[GetCacheKey(mi)];
			if (null == tag)
			{			
				switch (mi.MemberType)
				{
					case MemberTypes.Method:
					{
						return Map((System.Reflection.MethodInfo)mi);
					}
					
					case MemberTypes.Constructor:
					{
						return Map((System.Reflection.ConstructorInfo)mi);
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
					
					case MemberTypes.NestedType:
					{
						return Map((System.Type)mi);
					}
					
					default:
					{
						throw new NotImplementedException(mi.ToString());
					}
				}
				_entityCache.Add(GetCacheKey(mi), tag);
			}
			return tag;
		}
		
		public string GetSignature(IMethod method)
		{
			return GetSignature(method, true);
		}
		
		public string GetSignature(IMethod method, bool includeFullName)
		{
			_buffer.Length = 0;
			if (includeFullName)
			{
				_buffer.Append(method.FullName);
			}
			else
			{
				_buffer.Append(method.Name);
			}
			_buffer.Append("(");
			
			IParameter[] parameters = method.GetParameters();
			for (int i=0; i<parameters.Length; ++i)
			{
				if (i > 0) { _buffer.Append(", "); }
				_buffer.Append(parameters[i].Type.FullName);
			}
			_buffer.Append(")");
			return _buffer.ToString();
		}
		
		public object GetCacheKey(System.Reflection.MemberInfo mi)
		{
			return mi;
		}
		
		public IEntity ResolvePrimitive(string name)
		{
			return (IEntity)_primitives[name];
		}
		
		public bool IsPrimitive(string name)
		{
			return _primitives.ContainsKey(name);
		}
		
		/// <summary>
		/// checks if the passed type will be equivalente to
		/// System.Object in runtime (accounting for the presence
		/// of duck typing).
		/// </summary>
		public bool IsSystemObject(IType type)
		{
			return type == ObjectType || type == DuckType;
		}
		
		void PreparePrimitives()
		{
			AddPrimitiveType("duck", DuckType);
			AddPrimitiveType("void", VoidType);
			AddPrimitiveType("bool", BoolType);
			AddPrimitiveType("date", DateTimeType);
			AddPrimitiveType("timespan", TimeSpanType);
			AddPrimitiveType("string", StringType);
			AddPrimitiveType("object", ObjectType);
			AddPrimitiveType("regex", RegexType);
			AddPrimitiveType("sbyte", SByteType);
			AddPrimitiveType("byte", ByteType);
			AddPrimitiveType("short", ShortType);
			AddPrimitiveType("ushort", UShortType);
			AddPrimitiveType("int", IntType);
			AddPrimitiveType("uint", UIntType);
			AddPrimitiveType("long", LongType);
			AddPrimitiveType("ulong", ULongType);
			AddPrimitiveType("single", SingleType);
			AddPrimitiveType("double", DoubleType);
			AddPrimitiveType("callable", ICallableType);
			AddBuiltin(BuiltinFunction.Len);
			AddBuiltin(BuiltinFunction.AddressOf);
			AddBuiltin(BuiltinFunction.Eval);
			AddBuiltin(BuiltinFunction.Switch);
		}
		
		void AddPrimitiveType(string name, ExternalType type)
		{
			_primitives[name] = type;
		}
		
		void AddBuiltin(BuiltinFunction function)
		{
			_primitives[function.Name] = function;
		}
		
		void AddPrimitive(string name, IEntity tag)
		{
			_primitives[name] = tag;
		}
		
		void Cache(ExternalType tag)
		{
			_entityCache[tag.ActualType] = tag;
		}
		
		void Cache(object key, IType tag)
		{
			_entityCache[key] = tag;
		}
		
		Method CreateBeginInvokeMethod(ICallableType anonymousType)
		{
			Method method = CodeBuilder.CreateRuntimeMethod("BeginInvoke", Map(typeof(IAsyncResult)),
												anonymousType.GetSignature().Parameters);
												
			int delta=method.Parameters.Count;
			method.Parameters.Add(
					CodeBuilder.CreateParameterDeclaration(delta+1, "callback", Map(typeof(AsyncCallback))));
			method.Parameters.Add(
					CodeBuilder.CreateParameterDeclaration(delta+1, "asyncState", ObjectType));
			return method;
		}
		
		Method CreateBeginInvokeOverload(ICallableType anonymousType, Method beginInvoke, out MethodInvocationExpression mie)
		{
			InternalMethod beginInvokeEntity = (InternalMethod)beginInvoke.Entity;
			
			Method overload = CodeBuilder.CreateMethod("BeginInvoke", Map(typeof(IAsyncResult)),
										TypeMemberModifiers.Public);
			CodeBuilder.DeclareParameters(overload, 1, anonymousType.GetSignature().Parameters);
			
			mie = CodeBuilder.CreateMethodInvocation(
						CodeBuilder.CreateSelfReference(beginInvokeEntity.DeclaringType),
						beginInvokeEntity);
						
			foreach (ParameterDeclaration parameter in overload.Parameters)
			{
				mie.Arguments.Add(CodeBuilder.CreateReference(parameter));
			}
			
			overload.Body.Add(new ReturnStatement(mie));
			return overload;
		}
		
		Method CreateBeginInvokeSimplerOverload(ICallableType anonymousType, Method beginInvoke)
		{
			MethodInvocationExpression mie;
			Method overload = CreateBeginInvokeOverload(anonymousType, beginInvoke, out mie);
			
			mie.Arguments.Add(CodeBuilder.CreateNullLiteral());
			mie.Arguments.Add(CodeBuilder.CreateNullLiteral());
			
			return overload;
		}
		
		Method CreateBeginInvokeCallbackOnlyOverload(ICallableType anonymousType, Method beginInvoke)
		{
			MethodInvocationExpression mie;
			
			Method overload = CreateBeginInvokeOverload(anonymousType, beginInvoke, out mie);
			ParameterDeclaration callback = CodeBuilder.CreateParameterDeclaration(overload.Parameters.Count+1,
										"callback", Map(typeof(AsyncCallback)));
			overload.Parameters.Add(callback);

			mie.Arguments.Add(CodeBuilder.CreateReference(callback));			
			mie.Arguments.Add(CodeBuilder.CreateNullLiteral());
			
			return overload;
		}
		
		public Method CreateEndInvokeMethod(ICallableType anonymousType)
		{
			CallableSignature signature = anonymousType.GetSignature();
			Method method = CodeBuilder.CreateRuntimeMethod("EndInvoke", signature.ReturnType);
			int delta=method.Parameters.Count;
			method.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(delta+1, "result", Map(typeof(IAsyncResult))));
			return method;
		}
		
		Method CreateInvokeMethod(AnonymousCallableType anonymousType)
		{
			CallableSignature signature = anonymousType.GetSignature();
			return CodeBuilder.CreateRuntimeMethod("Invoke", signature.ReturnType, signature.Parameters);
		}
		
		public IConstructor GetDefaultConstructor(IType type)
		{
			IConstructor[] constructors = type.GetConstructors();
			for (int i=0; i<constructors.Length; ++i)
			{
				IConstructor constructor = constructors[i];
				if (0 == constructor.GetParameters().Length)
				{
					return constructor;
				}
			}
			return null;
		}
		
		IType GetExternalEnumeratorItemType(IType iteratorType)
		{
			Type type = ((ExternalType)iteratorType).ActualType;
			EnumeratorItemTypeAttribute attribute = (EnumeratorItemTypeAttribute)System.Attribute.GetCustomAttribute(type, typeof(EnumeratorItemTypeAttribute));
			if (null != attribute)
			{
				return Map(attribute.ItemType);
			}
			return null;
		}
		
		IType GetEnumeratorItemTypeFromAttribute(IType iteratorType)
		{
			AbstractInternalType internalType = iteratorType as AbstractInternalType;
			if (null == internalType)
			{
				return GetExternalEnumeratorItemType(iteratorType);
			}
			
			IType enumeratorItemTypeAttribute = Map(typeof(EnumeratorItemTypeAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in internalType.TypeDefinition.Attributes)
			{				
				IConstructor constructor = GetEntity(attribute) as IConstructor;
				if (null != constructor)
				{
					if (constructor.DeclaringType == enumeratorItemTypeAttribute)
					{
						return GetType(attribute.Arguments[0]);
					}
				}
			}
			return null;
		}
		
		public virtual IType GetConcreteCallableType(Node sourceNode, AnonymousCallableType anonymousType)
		{
			if (null == anonymousType.ConcreteType)
			{
				anonymousType.ConcreteType = CreateConcreteCallableType(sourceNode, anonymousType);
			}
			return anonymousType.ConcreteType;
		}
		
		protected virtual IType CreateConcreteCallableType(Node sourceNode, AnonymousCallableType anonymousType)
		{
			Boo.Lang.Compiler.Ast.Module module = GetAnonymousTypesModule();
			
			string name = string.Format("___callable{0}", module.Members.Count);
			ClassDefinition cd = CreateCallableDefinition(name);
			cd.Modifiers |= TypeMemberModifiers.Public;
			cd.LexicalInfo = sourceNode.LexicalInfo;
			
			cd.Members.Add(CreateInvokeMethod(anonymousType));
			
			Method beginInvoke = CreateBeginInvokeMethod(anonymousType);
			cd.Members.Add(beginInvoke);
			cd.Members.Add(CreateBeginInvokeCallbackOnlyOverload(anonymousType, beginInvoke));
			cd.Members.Add(CreateBeginInvokeSimplerOverload(anonymousType, beginInvoke));
			
			cd.Members.Add(CreateEndInvokeMethod(anonymousType));
			_anonymousTypesModule.Members.Add(cd);
			
			return (IType)cd.Entity;
		}
		
		private static void InvalidNode(Node node)
		{
			throw CompilerErrorFactory.InvalidNode(node);
		}

		public class DuckTypeImpl : ExternalType
		{	
			public DuckTypeImpl(TypeSystemServices typeSystemServices) : 
				base(typeSystemServices, Types.Object)
			{
			}
		}	
		
		#region VoidTypeImpl
		class VoidTypeImpl : ExternalType
		{			
			internal VoidTypeImpl(TypeSystemServices manager) : base(manager, Types.Void)
			{				
			}		
			
			override public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
			{	
				return false;
			}	
			
			override public bool IsSubclassOf(IType other)
			{
				return false;
			}
			
			override public bool IsAssignableFrom(IType other)
			{
				return false;
			}
		}

		#endregion
	}
}
