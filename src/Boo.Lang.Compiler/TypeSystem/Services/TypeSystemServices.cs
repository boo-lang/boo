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

using System.Runtime.InteropServices;
using System.Collections.Generic;

using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Text;
	using Boo.Lang.Compiler.Ast;
	using Attribute = Boo.Lang.Compiler.Ast.Attribute;
	using Module = Boo.Lang.Compiler.Ast.Module;

	public class TypeSystemServices
	{
		public IType DuckType;

		public IType IQuackFuType;

		public IType MulticastDelegateType;

		public IType DelegateType;

		public IType IntPtrType;

		public IType UIntPtrType;

		public IType ObjectType;

		public IType ValueTypeType;

		public IType EnumType;

		public IType RegexType;

		public IType ArrayType;

		public IType TypeType;

		public IArrayType ObjectArrayType;

		public IType VoidType;

		public IType StringType;

		public IType BoolType;

		public IType CharType;

		public IType SByteType;

		public IType ByteType;

		public IType ShortType;

		public IType UShortType;

		public IType IntType;

		public IType UIntType;

		public IType LongType;

		public IType ULongType;

		public IType SingleType;

		public IType DoubleType;

		public IType DecimalType;

		public IType TimeSpanType;

		public IType DateTimeType;

		public IType RuntimeServicesType;

		public IType BuiltinsType;

		public IType ListType;

		public IType HashType;

		public IType ICallableType;

		public IType IEnumerableType;

		public IType IEnumeratorType;

		public IType IEnumerableGenericType;

		public IType IEnumeratorGenericType;

		public IType IDisposableType;

		public IType ICollectionType;

		public IType ICollectionGenericType;

		public IType IListType;

		public IType IDictionaryType;

		public IType SystemAttribute;

		public IType ConditionalAttribute;

		public IType IAstMacroType;

		public IType IAstGeneratorMacroType;

		public IType AstNodeType;

		protected Hashtable _primitives = new Hashtable();

		protected Boo.Lang.Compiler.Util.Set<string> _literalPrimitives = new Boo.Lang.Compiler.Util.Set<string>();

		public static readonly IType ErrorEntity = Error.Default;

		private StringBuilder _buffer = new StringBuilder();

		private Module _compilerGeneratedTypesModule;

		private Module _compilerGeneratedExtensionsModule;

		private ClassDefinition _compilerGeneratedExtensionsClass;

		protected readonly CompilerContext _context;

		private readonly AnonymousCallablesManager _anonymousCallablesManager;

		public TypeSystemServices() : this(new CompilerContext())
		{
		}

		public TypeSystemServices(CompilerContext context)
		{
			if (null == context) throw new ArgumentNullException("context");

			_context = context;
			_anonymousCallablesManager = new AnonymousCallablesManager(this);

			DuckType = Map(typeof(Builtins.duck));
			IQuackFuType = Map(typeof(IQuackFu));
			VoidType = Map(Types.Void);
			ObjectType = Map(Types.Object);
			RegexType = Map(Types.Regex);
			ValueTypeType = Map(typeof(ValueType));
			EnumType = Map(typeof(Enum));
			ArrayType = Map(Types.Array);
			TypeType = Map(Types.Type);
			StringType = Map(Types.String);
			BoolType = Map(Types.Bool);
			SByteType = Map(Types.SByte);
			CharType = Map(Types.Char);
			ShortType = Map(Types.Short);
			IntType = Map(Types.Int);
			LongType = Map(Types.Long);
			ByteType = Map(Types.Byte);
			UShortType = Map(Types.UShort);
			UIntType = Map(Types.UInt);
			ULongType = Map(Types.ULong);
			SingleType = Map(Types.Single);
			DoubleType = Map(Types.Double);
			DecimalType = Map(Types.Decimal);
			TimeSpanType = Map(Types.TimeSpan);
			DateTimeType = Map(Types.DateTime);
			RuntimeServicesType = Map(Types.RuntimeServices);
			BuiltinsType = Map(Types.Builtins);
			ListType = Map(Types.List);
			HashType = Map(Types.Hash);
			ICallableType = Map(Types.ICallable);
			IEnumerableType = Map(Types.IEnumerable);
			IEnumeratorType = Map(typeof(IEnumerator));
			ICollectionType = Map(Types.ICollection);
			IListType = Map(Types.IList);
			IDictionaryType = Map(Types.IDictionary);
			IDisposableType = Map(typeof(IDisposable));
			IntPtrType = Map(Types.IntPtr);
			UIntPtrType = Map(Types.UIntPtr);
			MulticastDelegateType = Map(Types.MulticastDelegate);
			DelegateType = Map(Types.Delegate);
			SystemAttribute = Map(typeof(System.Attribute));
			ConditionalAttribute = Map(typeof(System.Diagnostics.ConditionalAttribute));
			IEnumerableGenericType = Map(Types.IEnumerableGeneric);
			IEnumeratorGenericType = Map(typeof(System.Collections.Generic.IEnumerator<>));
			ICollectionGenericType = Map(typeof(System.Collections.Generic.ICollection<>));
			IAstMacroType = Map(typeof(IAstMacro));
			IAstGeneratorMacroType = Map(typeof(IAstGeneratorMacro));
			AstNodeType = Map(typeof(Node));

			ObjectArrayType = ObjectType.MakeArrayType(1);

			PreparePrimitives();
			PrepareBuiltinFunctions();
		}

		public CompilerContext Context
		{
			get { return _context; }
		}

		public BooCodeBuilder CodeBuilder
		{
			get { return _context.CodeBuilder;  }
		}

		public bool IsGenericGeneratorReturnType(IType returnType)
		{
			return returnType.ConstructedInfo != null &&
				(returnType.ConstructedInfo.GenericDefinition == IEnumerableGenericType ||
				 returnType.ConstructedInfo.GenericDefinition == IEnumeratorGenericType);
		}

		public IType GetMostGenericType(ExpressionCollection args)
		{
			IType type = GetConcreteExpressionType(args[0]);
			for (int i = 1; i < args.Count; ++i)
			{
				IType newType = GetConcreteExpressionType(args[i]);
				if (type == newType)
					continue;

				type = GetMostGenericType(type, newType);
				if (IsSystemObject(type))
					break;
			}
			return type;
		}

		public IType GetMostGenericType(IType current, IType candidate)
		{
			if (null == current && null == candidate)
				throw new ArgumentNullException("current", "Both 'current' and 'candidate' are null");

			if (null == current)
				return candidate;

			if (null == candidate)
				return current;

			if (current.IsAssignableFrom(candidate))
				return current;

			if (candidate.IsAssignableFrom(current))
				return candidate;

			if (IsNumberOrBool(current) && IsNumberOrBool(candidate))
				return GetPromotedNumberType(current, candidate);

			if (IsCallableType(current) && IsCallableType(candidate))
				return ICallableType;

			if (current.IsClass && candidate.IsClass)
			{
				if (current ==  ObjectType || candidate == ObjectType)
					return ObjectType;

				if (current.GetTypeDepth() < candidate.GetTypeDepth())
					return GetMostGenericType(current.BaseType, candidate);

				return GetMostGenericType(current, candidate.BaseType);
			}
			return ObjectType;
		}

		public IType GetPromotedNumberType(IType left, IType right)
		{
			if (left == DecimalType ||
				right == DecimalType)
			{
				return DecimalType;
			}
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

		public static bool IsReadOnlyField(IField field)
		{
			return field.IsInitOnly || field.IsLiteral;
		}

		public bool IsCallable(IType type)
		{
			return (TypeType == type) || IsCallableType(type) || IsDuckType(type);
		}

		public virtual bool IsDuckTyped(Expression expression)
		{
			IType type = expression.ExpressionType;
			return null != type && this.IsDuckType(type);
		}

		public bool IsQuackBuiltin(Expression node)
		{
			return IsQuackBuiltin(GetOptionalEntity(node));
		}

		public bool IsQuackBuiltin(IEntity entity)
		{
			return BuiltinFunction.Quack == entity;
		}

		public bool IsDuckType(IType type)
		{
			if (null == type)
			{
				throw new ArgumentNullException("type");
			}
			return (
				(type == DuckType)
				|| KnowsQuackFu(type)
				|| (_context.Parameters.Ducky
					&& (type == ObjectType)));
		}

		public bool KnowsQuackFu(IType type)
		{
			return type.IsSubclassOf(IQuackFuType);
		}

		bool IsCallableType(IType type)
		{
			return (ICallableType.IsAssignableFrom(type)) || (type is ICallableType);
		}

		public AnonymousCallableType GetCallableType(IMethodBase method)
		{
			CallableSignature signature = new CallableSignature(method);
			return GetCallableType(signature);
		}

		public AnonymousCallableType GetCallableType(CallableSignature signature)
		{
			return _anonymousCallablesManager.GetCallableType(signature);
		}

		public virtual IType GetConcreteCallableType(Node sourceNode, CallableSignature signature)
		{
			return _anonymousCallablesManager.GetConcreteCallableType(sourceNode, signature);
		}

		public virtual IType GetConcreteCallableType(Node sourceNode, AnonymousCallableType anonymousType)
		{
			return _anonymousCallablesManager.GetConcreteCallableType(sourceNode, anonymousType);
		}

		public IType GetEnumeratorItemType(IType iteratorType)
		{
			// Arrays are enumerators of their element type
			if (iteratorType.IsArray) return iteratorType.GetElementType();

			// String are enumerators of char
			if (StringType == iteratorType) return CharType;

			// Try to use an EnumerableItemType attribute
			if (iteratorType.IsClass)
			{
				IType enumeratorItemType = GetEnumeratorItemTypeFromAttribute(iteratorType);
				if (null != enumeratorItemType) return enumeratorItemType;
			}

			// Try to use a generic IEnumerable interface
			IType genericItemType = GetGenericEnumerableItemType(iteratorType);
			if (null != genericItemType) return genericItemType;

			// If none of these work, the type is an enumerator of object
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

		public IEntity GetDefaultMember(IType type)
		{
			// Search for a default member on this type or any of its non-interface basetypes
			for (IType t = type; t != null; t = t.BaseType)
			{
				IEntity member = t.GetDefaultMember();
				if (member != null) return member;
			}

			// Search for default members on the type's interfaces
			Set<IEntity> buffer = new Set<IEntity>();
			foreach (IType interfaceType in type.GetInterfaces())
			{
				IEntity member = GetDefaultMember(interfaceType);
				if (member != null) buffer.Add(member);
			}
			return Entities.EntityFromList(buffer);
		}

		public ClassDefinition GetCompilerGeneratedExtensionsClass()
		{
			if (null == _compilerGeneratedExtensionsClass)
			{
				BooClassBuilder builder = CodeBuilder.CreateClass("CompilerGeneratedExtensions");
				builder.Modifiers = TypeMemberModifiers.Private | TypeMemberModifiers.Static | TypeMemberModifiers.Transient;
				builder.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));

				ClassDefinition cd = builder.ClassDefinition;
				Module module = GetCompilerGeneratedExtensionsModule();
				module.Members.Add(cd);
				((InternalModule)module.Entity).InitializeModuleClass(cd);

				_compilerGeneratedExtensionsClass = cd;
			}
			return _compilerGeneratedExtensionsClass;
		}

		public Module GetCompilerGeneratedExtensionsModule()
		{
			if (null == _compilerGeneratedExtensionsModule)
			{
				_compilerGeneratedExtensionsModule = NewModule(null, "CompilerGeneratedExtensions");
			}
			return _compilerGeneratedExtensionsModule;
		}

		public void AddCompilerGeneratedType(TypeDefinition type)
		{
			GetCompilerGeneratedTypesModule().Members.Add(type);
		}

		public Module GetCompilerGeneratedTypesModule()
		{
			if (null == _compilerGeneratedTypesModule)
			{
				_compilerGeneratedTypesModule = NewModule("CompilerGenerated");
			}
			return _compilerGeneratedTypesModule;
		}

        private Module NewModule(string nameSpace)
        {
            return NewModule(nameSpace, nameSpace);
        }

		private Module NewModule(string nameSpace, string moduleName)
		{
            Module module = CodeBuilder.CreateModule(moduleName, nameSpace);
			_context.CompileUnit.Modules.Add(module);
			return module;
		}
		
		public ClassDefinition CreateCallableDefinition(string name)
		{
			ClassDefinition cd = new ClassDefinition();
			cd.IsSynthetic = true;
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(this.MulticastDelegateType));
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(this.ICallableType));
			cd.Name = name;
			cd.Modifiers = TypeMemberModifiers.Final;
			cd.Members.Add(CreateCallableConstructor());
			cd.Members.Add(CreateCallMethod());
			cd.Entity = new InternalCallableType(My<InternalTypeSystemProvider>.Instance, cd);
			cd.Attributes.Add(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));
			return cd;
		}

		Method CreateCallMethod()
		{
			Method method = CodeBuilder.CreateMethod("Call", ObjectType, TypeMemberModifiers.Public|TypeMemberModifiers.Virtual);
			method.Parameters.Add(CodeBuilder.CreateParameterDeclaration(1, "args", ObjectArrayType));
			return method;
		}

		Constructor CreateCallableConstructor()
		{
			Constructor constructor = CodeBuilder.CreateConstructor(TypeMemberModifiers.Public);
			constructor.ImplementationFlags = MethodImplementationFlags.Runtime;
			constructor.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(1, "instance", ObjectType));
			constructor.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(2, "method", IntPtrType));
			return constructor;
		}

		public bool AreTypesRelated(IType lhs, IType rhs)
		{
			bool byDowncast;
			return AreTypesRelated(lhs, rhs, out byDowncast);
		}

		public bool AreTypesRelated(IType lhs, IType rhs, out bool byDowncast)
		{
			byDowncast = false;

			ICallableType ctype = lhs as ICallableType;
			if (null != ctype)
			{
				return ctype.IsAssignableFrom(rhs)
					|| ctype.IsSubclassOf(rhs);
			}

			return lhs.IsAssignableFrom(rhs)
				|| (byDowncast = CanBeReachedByDowncast(lhs, rhs))
				|| CanBeReachedByPromotion(lhs, rhs)
				|| FindImplicitConversionOperator(rhs,lhs) != null;
		}

		public IMethod FindExplicitConversionOperator(IType fromType, IType toType)
		{
			return FindConversionOperator("op_Explicit", fromType, toType);
		}

		public IMethod FindImplicitConversionOperator(IType fromType, IType toType)
		{
			return FindConversionOperator("op_Implicit", fromType, toType);
		}

		public IMethod FindConversionOperator(string name, IType fromType, IType toType)
		{
			while (fromType != this.ObjectType)
			{
				IMethod method = FindConversionOperator(name, fromType, toType, fromType.GetMembers());
				if (null != method) return method;
				method = FindConversionOperator(name, fromType, toType, toType.GetMembers());
				if (null != method) return method;
				method = FindConversionOperator(name, fromType, toType, FindExtension(fromType, name));
				if (null != method) return method;

				fromType = fromType.BaseType;
				if (null == fromType) break;
			}
			return null;
		}

		private IEntity[] FindExtension(IType fromType, string name)
		{
			IEntity extension = Context.NameResolutionService.ResolveExtension(fromType, name);
			if (null == extension) return Ambiguous.NoEntities;

			Ambiguous a = extension as Ambiguous;
			if (null != a) return a.Entities;
			return new IEntity[] { extension };
		}

		IMethod FindConversionOperator(string name, IType fromType, IType toType, IEnumerable<IEntity> candidates)
		{
			foreach (IMethod method in _context.NameResolutionService.Select<IMethod>(candidates, name, EntityType.Method))
				if (IsConversionOperator(method, fromType, toType)) return method;
			return null;
		}

		static bool IsConversionOperator(IMethod method, IType fromType, IType toType)
		{
			if (!method.IsStatic) return false;
			if (method.ReturnType != toType) return false;
			IParameter[] parameters = method.GetParameters();
			return  (1 == parameters.Length &&	fromType == parameters[0].Type);
		}

		public bool IsCallableTypeAssignableFrom(ICallableType lhs, IType rhs)
		{
			if (lhs == rhs) return true;
			if (Null.Default == rhs) return true;

			ICallableType other = rhs as ICallableType;
			if (null == other) return false;

				CallableSignature lvalue = lhs.GetSignature();
				CallableSignature rvalue = other.GetSignature();
			if (lvalue == rvalue) return true;

				IParameter[] lparams = lvalue.Parameters;
				IParameter[] rparams = rvalue.Parameters;
			if (lparams.Length < rparams.Length) return false;

					for (int i=0; i<rparams.Length; ++i)
					{
				if (!AreTypesRelated(lparams[i].Type, rparams[i].Type)) return false;
						}

			return CompatibleReturnTypes(lvalue, rvalue);
					}

		private bool CompatibleReturnTypes(CallableSignature lvalue, CallableSignature rvalue)
		{
			if (VoidType != lvalue.ReturnType && VoidType != rvalue.ReturnType)
			{
				return AreTypesRelated(lvalue.ReturnType, rvalue.ReturnType);
			}

			return true;
		}

		public static bool CheckOverrideSignature(IMethod impl, IMethod baseMethod)
		{
			if (!GenericsServices.AreOfSameGenerity(impl, baseMethod))
			{
				return false;
			}

			CallableSignature baseSignature = GetOverriddenSignature(baseMethod, impl);
			return CheckOverrideSignature(impl.GetParameters(), baseSignature.Parameters);
		}

		public static bool CheckOverrideSignature(IParameter[] implParameters, IParameter[] baseParameters)
		{
			return CallableSignature.AreSameParameters(implParameters, baseParameters);
		}

		public static CallableSignature GetOverriddenSignature(IMethod baseMethod, IMethod impl)
		{
			if (baseMethod.GenericInfo != null && GenericsServices.AreOfSameGenerity(baseMethod, impl))
			{
				return baseMethod.GenericInfo.ConstructMethod(impl.GenericInfo.GenericParameters).CallableType.GetSignature();
			}
			return baseMethod.CallableType.GetSignature();
		}

		public virtual bool CanBeReachedByDownCastOrPromotion(IType expectedType, IType actualType)
		{
			return CanBeReachedByDowncast(expectedType, actualType)
				|| CanBeReachedByPromotion(expectedType, actualType);
		}

		public virtual bool CanBeReachedByDowncast(IType expectedType, IType actualType)
		{
			if ((expectedType.IsInterface && !actualType.IsFinal)
			    || (actualType.IsInterface && !expectedType.IsFinal))
			{
				if (CanBeReachedByInterfaceDowncast(expectedType, actualType))
					return true;
			}
			return actualType.IsAssignableFrom(expectedType);
		}

		public virtual bool CanBeReachedByInterfaceDowncast(IType expectedType, IType actualType)
		{
			return true; //FIXME: currently interface downcast implements no type safety check at all (see BOO-1211)
		}

		public virtual bool CanBeReachedByPromotion(IType expectedType, IType actualType)
		{
			if (IsNullable(expectedType) && Null.Default == actualType)
				return true;
			return (expectedType.IsValueType
			        && IsNumber(expectedType)
			        && IsNumber(actualType));
		}

		public bool CanBeExplicitlyCastToInteger(IType type)
		{
			return type.IsEnum || type == this.CharType;
		}

		public static bool ContainsMethodsOnly(ICollection<IEntity> members)
		{
			foreach (IEntity member in members)
			{
				if (EntityType.Method != member.EntityType) return false;
			}
			return true;
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
				IsPrimitiveNumber(type) ||
				type == this.DecimalType;
		}

		public bool IsPrimitiveNumber(IType type)
		{
			return
				IsIntegerNumber(type) ||
				type == this.DoubleType ||
				type == this.SingleType;
		}

		public bool IsSignedNumber(IType type)
		{
			return IsNumber(type)
				&& type != this.UShortType
				&& type != this.UIntType
				&& type != this.ULongType
				&& type != this.ByteType;
		}

		/// <summary>
		/// Returns true if the type is a reference type or a generic parameter
		/// type that is constrained to represent a reference type.
		/// </summary>
		public static bool IsReferenceType(IType type)
		{
			IGenericParameter gp = type as IGenericParameter;
			if (null == gp)
				return !type.IsValueType;

			if (gp.IsClass)
				return true;

			foreach (IType tc in gp.GetTypeConstraints())
			{
				if (!tc.IsValueType && !tc.IsInterface)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns true if the type can be either a reference type or a value type.
		/// Currently it returns true only for an unconstrained generic parameter type.
		/// </summary>
		public static bool IsAnyType(IType type)
		{
			IGenericParameter gp = type as IGenericParameter;
			return (null != gp && !gp.IsClass && !gp.IsValueType && 0 == gp.GetTypeConstraints().Length);
		}

		public static bool IsNullable(IType type)
		{
			ExternalType et = type as ExternalType;
			return (null != et && et.ActualType.IsGenericType && et.ActualType.GetGenericTypeDefinition() == Types.Nullable);
		}

		public IType GetNullableUnderlyingType(IType type)
		{
			ExternalType et = type as ExternalType;
			return Map(Nullable.GetUnderlyingType(et.ActualType));
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

		public static bool IsUnknown(IType type)
		{
			return EntityType.Unknown == type.EntityType;
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
				if (member.IsInternal)
					return TypeMemberModifiers.Protected | TypeMemberModifiers.Internal;
				return TypeMemberModifiers.Protected;
			}
			else if (member.IsInternal)
			{
				return TypeMemberModifiers.Internal;
			}
			return TypeMemberModifiers.Private;
		}

		static object EntityAnnotationKey = new object();

		public static void Bind(Node node, IEntity entity)
		{
			if (null == node) throw new ArgumentNullException("node");
			node[EntityAnnotationKey] = entity;
		}

		public static IEntity GetOptionalEntity(Node node)
		{
			if (null == node) throw new ArgumentNullException("node");
			return (IEntity)node[EntityAnnotationKey];
		}

		public static IEntity GetEntity(Node node)
		{
			IEntity tag = GetOptionalEntity(node);
			if (null == tag) InvalidNode(node);

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
				case NodeType.GenericReferenceExpression:
				{
					return typeref.Entity as IType;
				}
			}
			return null;
		}

		public virtual bool IsModule(Type type)
		{
			return type.IsClass
				&& type.IsSealed
				&& !type.IsNestedPublic
				&& MetadataUtil.IsAttributeDefined(type, Types.ModuleAttribute);
		}

		public bool IsAttribute(IType type)
		{
			return type.IsSubclassOf(SystemAttribute);
		}

		public static IType GetType(Node node)
		{
			return ((ITypedEntity)GetEntity(node)).Type;
		}

		public IType Map(Type type)
		{
			return TypeSystemProvider().Map(type);
		}

		private IReflectionTypeSystemProvider TypeSystemProvider()
		{
			return My<IReflectionTypeSystemProvider>.Instance;
		}

		public IParameter[] Map(ParameterInfo[] parameters)
		{
			return TypeSystemProvider().Map(parameters);
		}

		public IConstructor Map(ConstructorInfo constructor)
		{
			return (IConstructor) Map((MemberInfo) constructor);
		}

		public IMethod Map(MethodInfo method)
		{
			return (IMethod) Map((MemberInfo) method);
		}

		public IEntity Map(MemberInfo[] members)
		{
			return TypeSystemProvider().Map(members);
		}

		public IEntity Map(MemberInfo mi)
		{
			return TypeSystemProvider().Map(mi);
		}

		public static string GetSignature(IEntityWithParameters method)
		{
			return GetSignature(method, true);
		}

		public static string GetSignature(IEntityWithParameters method, bool includeFullName)
		{
			StringBuilder _buffer = new StringBuilder();
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
				if (method.AcceptVarArgs && i == parameters.Length-1) { _buffer.Append('*'); }
				_buffer.Append(parameters[i].Type);
			}
			_buffer.Append(")");
			return _buffer.ToString();
		}

		public object GetCacheKey(MemberInfo mi)
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

		public bool IsLiteralPrimitive(IType type)
		{
			ExternalType typ = type as ExternalType;
			return typ != null
					&& typ.PrimitiveName != null
					&& _literalPrimitives.Contains(typ.PrimitiveName);
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

		public bool IsPointerCompatible(IType type)
		{
			return IsPrimitiveNumber(type) || (type.IsValueType && 0 != SizeOf(type));
		}

		public bool RequiresBoxing(IType expectedType, IType actualType)
		{
			if (!actualType.IsValueType) return false;
			return IsSystemObject(expectedType);
		}

		protected virtual void PreparePrimitives()
		{
			AddPrimitiveType("duck", DuckType);
			AddPrimitiveType("void", VoidType);
			AddPrimitiveType("object", ObjectType);
			AddPrimitiveType("callable", ICallableType);
			AddPrimitiveType("decimal", DecimalType);
			AddPrimitiveType("date", DateTimeType);

			AddLiteralPrimitiveType("bool", BoolType);
			AddLiteralPrimitiveType("sbyte", SByteType);
			AddLiteralPrimitiveType("byte", ByteType);
			AddLiteralPrimitiveType("short", ShortType);
			AddLiteralPrimitiveType("ushort", UShortType);
			AddLiteralPrimitiveType("int", IntType);
			AddLiteralPrimitiveType("uint", UIntType);
			AddLiteralPrimitiveType("long", LongType);
			AddLiteralPrimitiveType("ulong", ULongType);
			AddLiteralPrimitiveType("single", SingleType);
			AddLiteralPrimitiveType("double", DoubleType);
			AddLiteralPrimitiveType("char", CharType);
			AddLiteralPrimitiveType("string", StringType);
			AddLiteralPrimitiveType("regex", RegexType);
			AddLiteralPrimitiveType("timespan", TimeSpanType);
		}

		protected virtual void PrepareBuiltinFunctions()
		{
			AddBuiltin(BuiltinFunction.Len);
			AddBuiltin(BuiltinFunction.AddressOf);
			AddBuiltin(BuiltinFunction.Eval);
			AddBuiltin(BuiltinFunction.Switch);
		}

		protected void AddPrimitiveType(string name, IType type)
		{
			_primitives[name] = type;
			((ExternalType)type).PrimitiveName = name;
		}

		protected void AddLiteralPrimitiveType(string name, IType type)
		{
			AddPrimitiveType(name, type);
			_literalPrimitives.Add(name);
		}

		protected void AddBuiltin(BuiltinFunction function)
		{
			_primitives[function.Name] = function;
		}

		public IConstructor GetDefaultConstructor(IType type)
		{
			foreach (IConstructor constructor in type.GetConstructors())
				if (0 == constructor.GetParameters().Length)
					return constructor;
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
            // If iterator type is external get its attributes via reflection
            if (iteratorType is ExternalType)
            {
                return GetExternalEnumeratorItemType(iteratorType);
            }

            // If iterator type is a generic constructed type, get its attribute from its definition
			// and map generic parameters 
			if (iteratorType.ConstructedInfo != null)
			{
				IType definitionItemType = GetEnumeratorItemType(iteratorType.ConstructedInfo.GenericDefinition);
				return iteratorType.ConstructedInfo.Map(definitionItemType);
			}

            // If iterator type is internal get its attributes from its type definition
			AbstractInternalType internalType = (AbstractInternalType)iteratorType;
            IType enumeratorItemTypeAttribute = Map(typeof(EnumeratorItemTypeAttribute));
			foreach (Attribute attribute in internalType.TypeDefinition.Attributes)
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

		public IType GetGenericEnumerableItemType(IType iteratorType)
		{
			// Arrays implicitly implement IEnumerable[of element type]
			if (iteratorType is ArrayType) return iteratorType.GetElementType();

			// If type is not an array, try to find IEnumerable[of some type] in its interfaces
			IType itemType = null;
			foreach (IType type in GenericsServices.FindConstructedTypes(iteratorType, IEnumerableGenericType))
			{
				IType candidateItemType = type.ConstructedInfo.GenericArguments[0];

				if (itemType != null)
				{
					itemType = GetMostGenericType(itemType, candidateItemType);
				}
				else
				{
					itemType = candidateItemType;
				}
			}

			return itemType;
		}

		private static void InvalidNode(Node node)
		{
			throw CompilerErrorFactory.InvalidNode(node);
		}

		public virtual IType ExceptionType
		{
			get { return Map(typeof(Exception)); }
		}

		public virtual bool IsValidException(IType type)
		{
			return ExceptionType.IsAssignableFrom(type);
		}

		public virtual IConstructor GetStringExceptionConstructor()
		{
			return Map(typeof(Exception).GetConstructor(new Type[] { typeof(string) }));
		}

		public virtual bool IsMacro(IType type)
		{
			return type.IsSubclassOf(IAstMacroType) || type.IsSubclassOf(IAstGeneratorMacroType);
		}

		public virtual bool IsAstNode(IType type)
		{
			return type == AstNodeType || type.IsSubclassOf(AstNodeType);
		}

		public virtual int SizeOf(IType type)
		{
			if (type.IsPointer)
				type = type.GetElementType();
			if (null == type || !type.IsValueType)
				return 0;

			ExternalType et = type as ExternalType;
			if (null != et)
				return Marshal.SizeOf(et.ActualType);

			int size = 0;
			InternalClass it = type as InternalClass;
			if (null == it)
				return 0;

			//FIXME: TODO: warning if no predefined size => StructLayoutAttribute
			foreach (Field f in it.TypeDefinition.Members.OfType<Field>())
			{
				int fsize = SizeOf(f.Type.Entity as IType);
				if (0 == fsize)
					return 0; //cannot be unmanaged
				size += fsize;
			}
			return size;
		}
	}
}
