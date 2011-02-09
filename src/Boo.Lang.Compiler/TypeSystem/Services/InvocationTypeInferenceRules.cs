using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public delegate IType InvocationTypeInferenceRule(MethodInvocationExpression invocation, IMethod method);

	public class InvocationTypeInferenceRules : AbstractCompilerComponent
	{
		public void RegisterTypeInferenceRuleFor(IMethod method, InvocationTypeInferenceRule rule)
		{
			_invocationTypeInferenceRules.Add(method, rule);
		}

		public void RegisterTypeInferenceRuleFor(IMethod[] methods, InvocationTypeInferenceRule rule)
		{
			foreach (var method in methods)
				RegisterTypeInferenceRuleFor(method, rule);
		}

		public IType ApplyTypeInferenceRuleTo(MethodInvocationExpression invocation, IMethod method)
		{
			InvocationTypeInferenceRule rule;
			if (_invocationTypeInferenceRules.TryGetValue(method, out rule))
				return rule(invocation, method);
			return null;
		}

		Dictionary<IMethod, InvocationTypeInferenceRule> _invocationTypeInferenceRules = new Dictionary<IMethod, InvocationTypeInferenceRule>();

		public InvocationTypeInferenceRules() : base(CompilerContext.Current)
		{
			var Array_EnumerableConstructor = Map(Methods.Of<IEnumerable, Array>(Builtins.array));
			var Array_TypedEnumerableConstructor = Map(Methods.Of<Type, IEnumerable, Array>(Builtins.array));
			var Array_TypedIntConstructor = Map(Methods.Of<Type, int, Array>(Builtins.array));
			var MultiDimensionalArray_TypedConstructor = Map(Methods.Of<Type, int[], Array>(Builtins.matrix));

			RegisterTypeInferenceRuleFor(
				new[] { Array_TypedEnumerableConstructor, Array_TypedIntConstructor },
				(invocation, method) =>
				{
					IType type = TypeSystemServices.GetReferencedType(invocation.Arguments[0]);
					if (type == null) return null;
					return type.MakeArrayType(1);
				});

			RegisterTypeInferenceRuleFor(
				MultiDimensionalArray_TypedConstructor,
				(invocation, method) =>
				{
					IType type = TypeSystemServices.GetReferencedType(invocation.Arguments[0]);
					if (type == null) return null;
					return type.MakeArrayType(invocation.Arguments.Count - 1);
				});

			RegisterTypeInferenceRuleFor(
				Array_EnumerableConstructor,
				(invocation, method) =>
				{
					IType enumeratorItemType = TypeSystemServices.GetEnumeratorItemType(TypeSystemServices.GetExpressionType(invocation.Arguments[0]));
					if (TypeSystemServices.ObjectType == enumeratorItemType)
						return null;

					invocation.Target.Entity = Array_TypedEnumerableConstructor;
					invocation.ExpressionType = Array_TypedEnumerableConstructor.ReturnType;
					invocation.Arguments.Insert(0, CodeBuilder.CreateReference(enumeratorItemType));
					return enumeratorItemType.MakeArrayType(1);
				});
		}

		IMethod Map(MethodInfo method)
		{
			return TypeSystemServices.Map(method);
		}

	}
}
