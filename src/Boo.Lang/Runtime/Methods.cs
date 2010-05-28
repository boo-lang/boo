using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public static class Methods
	{
		public static MethodInfo Of<T>(Func<T> lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo Of<T1, TRet>(Func<T1, TRet> lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo Of<T1, T2, TRet>(Func<T1, T2, TRet> lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo Of<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo Of(Action lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo Of<T>(Action<T> lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo Of<T1, T2>(Action<T1, T2> lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo Of<T1, T2, T3, T4>(Action<T1, T2, T3, T4> lambda)
		{
			return lambda.Method;
		}

		public static MethodInfo InstanceActionOf<TInstance>(Expression<Func<TInstance, Action>> lambda)
		{
			return MethodInfoFromLambdaExpressionBody(lambda.Body);
		}

		public static MethodInfo InstanceActionOf<TInstance, TArg1, TArg2>(Expression<Func<TInstance, Action<TArg1, TArg2>>> lambda)
		{
			return MethodInfoFromLambdaExpressionBody(lambda.Body);
		}

		public static MethodInfo InstanceFunctionOf<TInstance, TArg1>(Expression<Func<TInstance, Func<TArg1>>> lambda)
		{
			return MethodInfoFromLambdaExpressionBody(lambda.Body);
		}

		public static MethodInfo InstanceFunctionOf<TInstance, TArg1, TReturn>(Expression<Func<TInstance, Func<TArg1, TReturn>>> lambda)
		{
			return MethodInfoFromLambdaExpressionBody(lambda.Body);
		}

		public static MethodInfo InstanceFunctionOf<TInstance, TArg1, TArg2, TReturn>(Expression<Func<TInstance, Func<TArg1, TArg2, TReturn>>> lambda)
		{
			return MethodInfoFromLambdaExpressionBody(lambda.Body);
		}

		public static MethodInfo GetterOf<TInstance, TProperty>(Expression<Func<TInstance, TProperty>> lambda)
		{
			return ((PropertyInfo)((MemberExpression)lambda.Body).Member).GetGetMethod();
		}

		private static MethodInfo MethodInfoFromLambdaExpressionBody(Expression expression)
		{
			// Convert(CreateDelegate(DelegateType, instance, member))
			var methodRef = ((MethodCallExpression) ((UnaryExpression) expression).Operand).Arguments[2];
			return (MethodInfo) ((ConstantExpression) methodRef).Value;
		}

		public static ConstructorInfo ConstructorOf<T>(Expression<Func<T>> lambda)
		{
			return ((NewExpression) lambda.Body).Constructor;
		}
	}
}
