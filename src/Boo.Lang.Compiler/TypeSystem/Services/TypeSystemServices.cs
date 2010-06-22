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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;
using Attribute = System.Attribute;
using Module = Boo.Lang.Compiler.Ast.Module;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class TypeSystemServices
	{
		public static readonly IType ErrorEntity = Error.Default;
		
		private readonly AnonymousCallablesManager _anonymousCallablesManager;

		protected readonly CompilerContext _context;

		public IType ArrayType;
		public IType AstNodeType;

		public IType BoolType;
		public IType BuiltinsType;

		public IType ByteType;
		public IType CharType;
		public IType ConditionalAttribute;

		public IType DateTimeType;
		public IType DecimalType;
		public IType DelegateType;
		public IType DoubleType;
		public IType DuckType;
		public IType EnumType;

		public IType HashType;
		public IType IAstGeneratorMacroType;
		public IType IAstMacroType;

		public IType ICallableType;
		public IType ICollectionGenericType;
		public IType ICollectionType;
		public IType IDisposableType;

		public IType IEnumerableGenericType;
		public IType IEnumerableType;

		public IType IEnumeratorGenericType;
		public IType IEnumeratorType;
		public IType IQuackFuType;
		public IType IntPtrType;
		public IType IntType;
		public IType ListType;
		public IType LongType;
		public IType MulticastDelegateType;
		public IArrayType ObjectArrayType;
		public IType ObjectType;
		public IType RegexType;
		public IType RuntimeServicesType;
		public IType SByteType;
		public IType ShortType;
		public IType SingleType;
		public IType StringType;

		public IType SystemAttribute;
		public IType TimeSpanType;
		public IType TypeType;
		public IType UIntPtrType;
		public IType UIntType;
		public IType ULongType;
		public IType UShortType;
		public IType ValueTypeType;
		public IType VoidType;

		private ClassDefinition _compilerGeneratedExtensionsClass;
		private Module _compilerGeneratedExtensionsModule;
		private Module _compilerGeneratedTypesModule;
		protected Set<string> _literalPrimitives = new Set<string>();
		protected Hashtable _primitives = new Hashtable();
		private DowncastPermissions _downcastPermissions;
		private MemoizedFunction<IType, IType, IMethod> _findImplicitConversionOperator;
		private MemoizedFunction<IType, IType, IMethod> _findExplicitConversionOperator;
		private MemoizedFunction<IType, IType, bool> _canBeReachedByPromotion;

		public TypeSystemServices() : this(new CompilerContext())
		{
		}

		public TypeSystemServices(CompilerContext context)
		{
			if (null == context) throw new ArgumentNullException("context");

			_context = context;
			_anonymousCallablesManager = new AnonymousCallablesManager(this);

			_findImplicitConversionOperator =
				new MemoizedFunction<IType, IType, IMethod>((fromType, toType) => FindConversionOperator("op_Implicit", fromType, toType));
			_findExplicitConversionOperator =
				new MemoizedFunction<IType, IType, IMethod>((fromType, toType) => FindConversionOperator("op_Explicit", fromType, toType));

			context.Provide<CurrentScope>().Changed += (sender, args) => ClearScopeDependentMemoizedFunctions();

			_canBeReachedByPromotion = new MemoizedFunction<IType, IType, bool>(CanBeReachedByPromotionImpl);

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
			IDisposableType = Map(typeof(IDisposable));
			IntPtrType = Map(Types.IntPtr);
			UIntPtrType = Map(Types.UIntPtr);
			MulticastDelegateType = Map(Types.MulticastDelegate);
			DelegateType = Map(Types.Delegate);
			SystemAttribute = Map(typeof(Attribute));
			ConditionalAttribute = Map(typeof(ConditionalAttribute));
			IEnumerableGenericType = Map(Types.IEnumerableGeneric);
			IEnumeratorGenericType = Map(typeof(IEnumerator<>));
			ICollectionGenericType = Map(typeof(ICollection<>));
			IAstMacroType = Map(typeof(IAstMacro));
			IAstGeneratorMacroType = Map(typeof(IAstGeneratorMacro));
			AstNodeType = Map(typeof(Node));

			ObjectArrayType = ObjectType.MakeArrayType(1);

			PreparePrimitives();
			PrepareBuiltinFunctions();
		}

		private void ClearScopeDependentMemoizedFunctions()
		{
			_findImplicitConversionOperator.Clear();
			_findExplicitConversionOperator.Clear();
		}

		public CompilerContext Context
		{
			get { return _context; }
		}

		public BooCodeBuilder CodeBuilder
		{
			get { return _context.CodeBuilder; }
		}

		public virtual IType ExceptionType
		{
			get { return Map(typeof(Exception)); }
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

			if (IsAssignableFrom(current, candidate))
				return current;

			if (IsAssignableFrom(candidate, current))
				return candidate;

			if (IsNumberOrBool(current) && IsNumberOrBool(candidate))
				return GetPromotedNumberType(current, candidate);

			if (IsCallableType(current) && IsCallableType(candidate))
				return ICallableType;

			if (current.IsClass && candidate.IsClass)
			{
				if (current == ObjectType || candidate == ObjectType)
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
			return null != type && IsDuckType(type);
		}

		public bool IsQuackBuiltin(Expression node)
		{
			return IsQuackBuiltin(node.Entity);
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

		private bool IsCallableType(IType type)
		{
			return (IsAssignableFrom(ICallableType, type)) || (type is ICallableType);
		}

		public AnonymousCallableType GetCallableType(IMethodBase method)
		{
			var signature = new CallableSignature(method);
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
				throw CompilerErrorFactory.InvalidNode(node);
			return type;
		}

		public IType GetConcreteExpressionType(Expression expression)
		{
			IType type = GetExpressionType(expression);
			var anonymousType = type as AnonymousCallableType;
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
				GetConcreteExpressionType(item);
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
			var buffer = new Set<IEntity>();
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
				builder.AddAttribute(CodeBuilder.CreateAttribute(typeof(CompilerGeneratedAttribute)));

				ClassDefinition cd = builder.ClassDefinition;
				Module module = GetCompilerGeneratedExtensionsModule();
				module.Members.Add(cd);
				((InternalModule) module.Entity).InitializeModuleClass(cd);

				_compilerGeneratedExtensionsClass = cd;
			}
			return _compilerGeneratedExtensionsClass;
		}

		public Module GetCompilerGeneratedExtensionsModule()
		{
			if (null != _compilerGeneratedExtensionsModule) return _compilerGeneratedExtensionsModule;
			_compilerGeneratedExtensionsModule = NewModule(null, "CompilerGeneratedExtensions");
			return _compilerGeneratedExtensionsModule;
		}

		public void AddCompilerGeneratedType(TypeDefinition type)
		{
			GetCompilerGeneratedTypesModule().Members.Add(type);
		}

		public Module GetCompilerGeneratedTypesModule()
		{
			return _compilerGeneratedTypesModule ?? (_compilerGeneratedTypesModule = NewModule("CompilerGenerated"));
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
			var cd = new ClassDefinition();
			cd.IsSynthetic = true;
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(MulticastDelegateType));
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(ICallableType));
			cd.Name = name;
			cd.Modifiers = TypeMemberModifiers.Final;
			cd.Members.Add(CreateCallableConstructor());
			cd.Members.Add(CreateCallMethod());
			cd.Entity = new InternalCallableType(My<InternalTypeSystemProvider>.Instance, cd);
			cd.Attributes.Add(CodeBuilder.CreateAttribute(typeof(CompilerGeneratedAttribute)));
			return cd;
		}

		private Method CreateCallMethod()
		{
			Method method = CodeBuilder.CreateMethod("Call", ObjectType, TypeMemberModifiers.Public | TypeMemberModifiers.Virtual);
			method.Parameters.Add(CodeBuilder.CreateParameterDeclaration(1, "args", ObjectArrayType));
			return method;
		}

		private Constructor CreateCallableConstructor()
		{
			Constructor constructor = CodeBuilder.CreateConstructor(TypeMemberModifiers.Public);
			constructor.ImplementationFlags = MethodImplementationFlags.Runtime;
			constructor.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(1, "instance", ObjectType));
			constructor.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(2, "method", IntPtrType));
			return constructor;
		}

		public bool CanBeReachedFrom(IType expectedType, IType actualType)
		{
			bool byDowncast;
			return CanBeReachedFrom(expectedType, actualType, out byDowncast);
		}

		public bool CanBeReachedFrom(IType expectedType, IType actualType, out bool byDowncast)
		{
			bool considerExplicitConversionOperators = !InStrictMode();
			return CanBeReachedFrom(expectedType, actualType, considerExplicitConversionOperators, out byDowncast);
		}

		public bool CanBeReachedFrom(IType expectedType, IType actualType, bool considerExplicitConversionOperators, out bool byDowncast)
		{
			byDowncast = false;

			var ctype = expectedType as ICallableType;
			if (null != ctype)
				return IsAssignableFrom(ctype, actualType) || ctype.IsSubclassOf(actualType);

			return IsAssignableFrom(expectedType, actualType)
			       || (byDowncast = DowncastPermissions().CanBeReachedByDowncast(expectedType, actualType))
			       || CanBeReachedByPromotion(expectedType, actualType)
			       || FindImplicitConversionOperator(actualType, expectedType) != null
			       || (considerExplicitConversionOperators && FindExplicitConversionOperator(actualType, expectedType) != null);
		}

		private DowncastPermissions DowncastPermissions()
		{
			return _downcastPermissions ?? (_downcastPermissions = My<DowncastPermissions>.Instance);
		}

		private bool InStrictMode()
		{
			return _context.Parameters.Strict;
		}

		public IMethod FindExplicitConversionOperator(IType fromType, IType toType)
		{
			return _findExplicitConversionOperator.Invoke(fromType, toType);
		}

		public IMethod FindImplicitConversionOperator(IType fromType, IType toType)
		{
			return _findImplicitConversionOperator.Invoke(fromType, toType);
		}

		private IMethod FindConversionOperator(string name, IType fromType, IType toType)
		{
			while (fromType != ObjectType)
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

			var a = extension as Ambiguous;
			if (null != a) return a.Entities;
			return new[] {extension};
		}

		private IMethod FindConversionOperator(string name, IType fromType, IType toType, IEnumerable<IEntity> candidates)
		{
			foreach (IMethod method in _context.NameResolutionService.Select<IMethod>(candidates, name, EntityType.Method))
				if (IsConversionOperator(method, fromType, toType)) return method;
			return null;
		}

		private static bool IsConversionOperator(IMethod method, IType fromType, IType toType)
		{
			if (!method.IsStatic) return false;
			if (method.ReturnType != toType) return false;
			IParameter[] parameters = method.GetParameters();
			return (1 == parameters.Length && fromType == parameters[0].Type);
		}

		public bool IsCallableTypeAssignableFrom(ICallableType lhs, IType rhs)
		{
			if (lhs == rhs) return true;
			if (Null.Default == rhs) return true;

			var other = rhs as ICallableType;
			if (null == other) return false;

			CallableSignature lvalue = lhs.GetSignature();
			CallableSignature rvalue = other.GetSignature();
			if (lvalue == rvalue) return true;

			IParameter[] lparams = lvalue.Parameters;
			IParameter[] rparams = rvalue.Parameters;
			if (lparams.Length < rparams.Length) return false;

			for (int i = 0; i < rparams.Length; ++i)
				if (!CanBeReachedFrom(lparams[i].Type, rparams[i].Type))
					return false;

			return CompatibleReturnTypes(lvalue, rvalue);
		}

		private bool CompatibleReturnTypes(CallableSignature lvalue, CallableSignature rvalue)
		{
			if (VoidType != lvalue.ReturnType && VoidType != rvalue.ReturnType)
				return CanBeReachedFrom(lvalue.ReturnType, rvalue.ReturnType);
			return true;
		}

		public static bool CheckOverrideSignature(IMethod impl, IMethod baseMethod)
		{
			if (!GenericsServices.AreOfSameGenerity(impl, baseMethod))
				return false;

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
				return baseMethod.GenericInfo.ConstructMethod(impl.GenericInfo.GenericParameters).CallableType.GetSignature();
			return baseMethod.CallableType.GetSignature();
		}

		public virtual bool CanBeReachedByDownCastOrPromotion(IType expectedType, IType actualType)
		{
			return My<DowncastPermissions>.Instance.CanBeReachedByDowncast(expectedType, actualType)
			       || CanBeReachedByPromotion(expectedType, actualType);
		}

	    public virtual bool CanBeReachedByPromotion(IType expectedType, IType actualType)
	    {
	    	return _canBeReachedByPromotion.Invoke(expectedType, actualType);
	    }

		private bool CanBeReachedByPromotionImpl(IType expectedType, IType actualType)
		{
			if (IsNullable(expectedType) && Null.Default == actualType)
				return true;
			if (IsIntegerNumber(actualType) && CanBeExplicitlyCastToInteger(expectedType))
				return true;
			if (IsIntegerNumber(expectedType) && CanBeExplicitlyCastToInteger(actualType))
				return true;
			return (expectedType.IsValueType && IsNumber(expectedType) && IsNumber(actualType));
		}

		public bool CanBeExplicitlyCastToInteger(IType type)
		{
			return type.IsEnum || type == CharType;
		}

		public static bool ContainsMethodsOnly(ICollection<IEntity> members)
		{
			return members.All(member => EntityType.Method == member.EntityType);
		}

		public bool IsIntegerNumber(IType type)
		{
			return
				type == ShortType ||
				type == IntType ||
				type == LongType ||
				type == SByteType ||
				type == UShortType ||
				type == UIntType ||
				type == ULongType ||
				type == ByteType;
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
				type == DecimalType;
		}

		public bool IsPrimitiveNumber(IType type)
		{
			return
				IsIntegerNumber(type) ||
				type == DoubleType ||
				type == SingleType;
		}

		public bool IsSignedNumber(IType type)
		{
			return IsNumber(type)
			       && type != UShortType
			       && type != UIntType
			       && type != ULongType
			       && type != ByteType;
		}

		/// <summary>
		/// Returns true if the type is a reference type or a generic parameter
		/// type that is constrained to represent a reference type.
		/// </summary>
		public static bool IsReferenceType(IType type)
		{
			var gp = type as IGenericParameter;
			if (null == gp)
				return !type.IsValueType;

			if (gp.IsClass)
				return true;

			foreach (IType tc in gp.GetTypeConstraints())
				if (!tc.IsValueType && !tc.IsInterface)
					return true;
			return false;
		}

		/// <summary>
		/// Returns true if the type can be either a reference type or a value type.
		/// Currently it returns true only for an unconstrained generic parameter type.
		/// </summary>
		public static bool IsAnyType(IType type)
		{
			var gp = type as IGenericParameter;
			return (null != gp && !gp.IsClass && !gp.IsValueType && 0 == gp.GetTypeConstraints().Length);
		}

		public static bool IsNullable(IType type)
		{
			var et = type as ExternalType;
			return (null != et && et.ActualType.IsGenericType && et.ActualType.GetGenericTypeDefinition() == Types.Nullable);
		}

		public IType GetNullableUnderlyingType(IType type)
		{
			var et = type as ExternalType;
			return Map(Nullable.GetUnderlyingType(et.ActualType));
		}

		public static bool IsUnknown(Expression node)
		{
			var type = node.ExpressionType;
			return null != type && IsUnknown(type);
		}

		public static bool IsUnknown(IType type)
		{
			return EntityType.Unknown == type.EntityType;
		}

		public static bool IsError(Expression node)
		{
			var type = node.ExpressionType;
			return null != type && IsError(type);
		}

		public static bool IsErrorAny(ExpressionCollection collection)
		{
			return collection.Any(IsError);
		}

		public bool IsBuiltin(IEntity entity)
		{
			if (EntityType.Method == entity.EntityType)
				return BuiltinsType == ((IMethod) entity).DeclaringType;
			return false;
		}

		public static bool IsError(IEntity entity)
		{
			return EntityType.Error == entity.EntityType;
		}

		public static TypeMemberModifiers GetAccess(IAccessibleMember member)
		{
			if (member.IsPublic)
				return TypeMemberModifiers.Public;
			if (member.IsProtected)
			{
				if (member.IsInternal)
					return TypeMemberModifiers.Protected | TypeMemberModifiers.Internal;
				return TypeMemberModifiers.Protected;
			}
			if (member.IsInternal)
				return TypeMemberModifiers.Internal;
			return TypeMemberModifiers.Private;
		}

		[Obsolete("Use Node.Entity instead.")]
		public static IEntity GetOptionalEntity(Node node)
		{
			return node.Entity;
		}

		public static IEntity GetEntity(Node node)
		{
			IEntity entity = node.Entity;
			if (null == entity) InvalidNode(node);
			return entity;
		}

		public static IType GetReferencedType(Expression typeref)
		{
			switch (typeref.NodeType)
			{
				case NodeType.TypeofExpression:
						return GetType(((TypeofExpression) typeref).Type);
				case NodeType.ReferenceExpression:
				case NodeType.MemberReferenceExpression:
				case NodeType.GenericReferenceExpression:
						return typeref.Entity as IType;
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
			return ((ITypedEntity) GetEntity(node)).Type;
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
			var buffer = new StringBuilder(includeFullName ? method.FullName : method.Name);
			buffer.Append("(");

			IParameter[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; ++i)
			{
				if (i > 0)
					buffer.Append(", ");

				if (method.AcceptVarArgs && i == parameters.Length - 1)
					buffer.Append('*');

				buffer.Append(parameters[i].Type);
			}

			buffer.Append(")");
			return buffer.ToString();
		}

		public object GetCacheKey(MemberInfo mi)
		{
			return mi;
		}

		public IEntity ResolvePrimitive(string name)
		{
			return (IEntity) _primitives[name];
		}

		public bool IsPrimitive(string name)
		{
			return _primitives.ContainsKey(name);
		}

		public bool IsLiteralPrimitive(IType type)
		{
			var typ = type as ExternalType;
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
			((ExternalType) type).PrimitiveName = name;
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
			return type.GetConstructors().FirstOrDefault(constructor => 0 == constructor.GetParameters().Length);
		}

		private IType GetExternalEnumeratorItemType(IType iteratorType)
		{
			Type type = ((ExternalType) iteratorType).ActualType;
			var attribute = (EnumeratorItemTypeAttribute) Attribute.GetCustomAttribute(type, typeof(EnumeratorItemTypeAttribute));
			return null != attribute ? Map(attribute.ItemType) : null;
		}

		private IType GetEnumeratorItemTypeFromAttribute(IType iteratorType)
		{
			// If iterator type is external get its attributes via reflection
			if (iteratorType is ExternalType)
				return GetExternalEnumeratorItemType(iteratorType);

			// If iterator type is a generic constructed type, get its attribute from its definition
			// and map generic parameters 
			if (iteratorType.ConstructedInfo != null)
			{
				IType definitionItemType = GetEnumeratorItemType(iteratorType.ConstructedInfo.GenericDefinition);
				return iteratorType.ConstructedInfo.Map(definitionItemType);
			}

			// If iterator type is internal get its attributes from its type definition
			var internalType = (AbstractInternalType) iteratorType;
			IType enumeratorItemTypeAttribute = Map(typeof(EnumeratorItemTypeAttribute));
			foreach (Ast.Attribute attribute in internalType.TypeDefinition.Attributes)
			{
				var constructor = GetEntity(attribute) as IConstructor;
				if (null != constructor && constructor.DeclaringType == enumeratorItemTypeAttribute)
					return GetType(attribute.Arguments[0]);
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
					itemType = GetMostGenericType(itemType, candidateItemType);
				else
					itemType = candidateItemType;
			}

			return itemType;
		}

		private static void InvalidNode(Node node)
		{
			throw CompilerErrorFactory.InvalidNode(node);
		}

		public virtual bool IsValidException(IType type)
		{
			return IsAssignableFrom(ExceptionType, type);
		}

	    private bool IsAssignableFrom(IType expectedType, IType actualType)
	    {
	        return TypeCompatibilityRules.IsAssignableFrom(expectedType, actualType);
	    }

	    public virtual IConstructor GetStringExceptionConstructor()
		{
			return Map(typeof(Exception).GetConstructor(new[] {typeof(string)}));
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

			var et = type as ExternalType;
			if (null != et)
				return Marshal.SizeOf(et.ActualType);

			int size = 0;
			var it = type as InternalClass;
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