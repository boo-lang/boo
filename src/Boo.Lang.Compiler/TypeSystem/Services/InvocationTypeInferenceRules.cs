using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public delegate IType InvocationTypeInferenceRule(MethodInvocationExpression invocation, IMethod method);

	public class TypeInferenceRuleProvider
	{
		public virtual string TypeInferenceRuleFor(MethodBase method)
		{
			return TypeInferenceRuleFor(method, typeof(TypeInferenceRuleAttribute));
		}

		protected string TypeInferenceRuleFor(MethodBase method, Type attributeType)
		{
			var rule = System.Attribute.GetCustomAttribute(method, attributeType);
			if (rule != null)
				return rule.ToString();
			return null;
		}
	}

	public class CustomTypeInferenceRuleProvider : TypeInferenceRuleProvider
	{
		private readonly Type _attribute;

		public CustomTypeInferenceRuleProvider(string attribute)
		{
			_attribute = ResolveAttribute(attribute);
		}

		public override string TypeInferenceRuleFor(MethodBase method)
		{
			return base.TypeInferenceRuleFor(method) ?? TypeInferenceRuleFor(method, _attribute);
		}

		private static Type ResolveAttribute(string attribute)
		{
			var externalType = (My<NameResolutionService>.Instance.ResolveQualifiedName(attribute) as ExternalType);
			if (externalType == null)
				throw new InvalidOperationException(string.Format("Type '{0}' could not be found!", attribute));
			return externalType.ActualType;
		}
	}

	public class InvocationTypeInferenceRules : AbstractCompilerComponent
	{
		public static class BuiltinRules
		{
			public static IType ArrayOfTypeReferencedByFirstArgument(MethodInvocationExpression invocation, IMethod method)
			{
				var type = TypeSystemServices.GetReferencedType(invocation.Arguments[0]);
				if (type == null) return null;
				return type.MakeArrayType(1);
			}

			public static IType TypeReferencedByFirstArgument(MethodInvocationExpression invocation, IMethod method)
			{
				return TypeSystemServices.GetReferencedType(invocation.Arguments[0]);
			}

			public static IType TypeReferencedBySecondArgument(MethodInvocationExpression invocation, IMethod method)
			{
				return TypeSystemServices.GetReferencedType(invocation.Arguments[1]);
			}

			public static IType TypeOfFirstArgument(MethodInvocationExpression invocation, IMethod method)
			{
				return TypeSystemServices.GetExpressionType(invocation.Arguments[0]);
			}

			public static IType NoTypeInference(MethodInvocationExpression invocation, IMethod method)
			{
				return null;
			}
		}

		public void RegisterTypeInferenceRuleFor(IMethod method, InvocationTypeInferenceRule rule)
		{
			_invocationTypeInferenceRules.Add(method, rule);
		}

		public void RegisterTypeInferenceRuleFor(IMethod[] methods, InvocationTypeInferenceRule rule)
		{
			foreach (var method in methods)
				RegisterTypeInferenceRuleFor(method, rule);
		}

		public IType ApplyTo(MethodInvocationExpression invocation, IMethod method)
		{
			InvocationTypeInferenceRule rule;
			if (!_invocationTypeInferenceRules.TryGetValue(method, out rule))
			{
				rule = ResolveRuleFor(invocation, method);
				_invocationTypeInferenceRules.Add(method, rule);
			}
			return rule(invocation, method);
		}

		Dictionary<IMethod, InvocationTypeInferenceRule> _invocationTypeInferenceRules = new Dictionary<IMethod, InvocationTypeInferenceRule>();

		TypeInferenceRuleProvider _provider = My<TypeInferenceRuleProvider>.Instance;
 
		public InvocationTypeInferenceRules() : base(CompilerContext.Current)
		{
			var Array_EnumerableConstructor = Map(Methods.Of<IEnumerable, Array>(Builtins.array));
			var Array_TypedEnumerableConstructor = Map(Methods.Of<Type, IEnumerable, Array>(Builtins.array));
			var MultiDimensionalArray_TypedConstructor = Map(Methods.Of<Type, int[], Array>(Builtins.matrix));

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

		InvocationTypeInferenceRule ResolveRuleFor(MethodInvocationExpression invocation, IMethod method)
		{
			var external = method as ExternalMethod;
			if (external != null)
			{
				var ruleName = TypeInferenceRuleFor(external.MethodInfo);
				if (ruleName != null)
					return ResolveRule(invocation, method, ruleName);
			}
			return BuiltinRules.NoTypeInference;
		}

		private string TypeInferenceRuleFor(MethodBase method)
		{
			return _provider.TypeInferenceRuleFor(method);
		}

		private InvocationTypeInferenceRule ResolveRule(MethodInvocationExpression invocation, IMethod method, string rule)
		{
			var ruleImpl = typeof(BuiltinRules).GetMethod(rule);
			if (ruleImpl != null)
				return (InvocationTypeInferenceRule)Delegate.CreateDelegate(typeof(InvocationTypeInferenceRule), ruleImpl);

			Warnings.Add(CompilerWarningFactory.CustomWarning(invocation, string.Format("Unknown type inference rule '{0}' on method '{1}'.", rule, method)));
			return BuiltinRules.NoTypeInference;
		}
	}
}
