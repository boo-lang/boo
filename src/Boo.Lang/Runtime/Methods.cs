using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public static class Methods
	{
		
		public static MethodInfo Of<T>(Func<T> value)
		{
			return value.Method;
		}

		public static MethodInfo Of<T1, TRet>(Func<T1, TRet> value)
		{
			return value.Method;
		}

		public static MethodInfo Of<T1, T2, TRet>(Func<T1, T2, TRet> value)
		{
			return value.Method;
		}

		public static MethodInfo Of<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> value)
		{
			return value.Method;
		}

		public static MethodInfo Of(Action value)
		{
			return value.Method;
		}

		public static MethodInfo Of<T>(Action<T> value)
		{
			return value.Method;
		}

		public static MethodInfo Of<T1, T2>(Action<T1, T2> value)
		{
			return value.Method;
		}

		public static MethodInfo Of<T1, T2, T3, T4>(Action<T1, T2, T3, T4> value)
		{
			return value.Method;
		}

		public static MethodInfo InstanceActionOf<TInstance>(Expression<Func<TInstance, Action>> func)
		{
			return MethodInfoFromLambdaExpressionBody(func.Body);
		}

		public static MethodInfo InstanceActionOf<TInstance, T1, T2>(Expression<Func<TInstance, Action<T1, T2>>> func)
		{
			return MethodInfoFromLambdaExpressionBody(func.Body);
		}

		public static MethodInfo InstanceFunctionOf<TInstance, TArg1>(Expression<Func<TInstance, Func<TArg1>>> func)
		{
			return MethodInfoFromLambdaExpressionBody(func.Body);
		}

		public static MethodInfo InstanceFunctionOf<TInstance, TArg1, TReturn>(Expression<Func<TInstance, Func<TArg1, TReturn>>> func)
		{
			return MethodInfoFromLambdaExpressionBody(func.Body);
		}

		public static MethodInfo InstanceFunctionOf<TInstance, TArg1, TArg2, TReturn>(Expression<Func<TInstance, Func<TArg1, TArg2, TReturn>>> func)
		{
			return MethodInfoFromLambdaExpressionBody(func.Body);
		}

		public static MethodInfo GetterOf<TInstance, TProperty>(Expression<Func<TInstance, TProperty>> func)
		{
			return ((PropertyInfo)((MemberExpression)func.Body).Member).GetGetMethod();
		}

		private static MethodInfo MethodInfoFromLambdaExpressionBody(Expression expression)
		{
			// Convert(CreateDelegate(DelegateType, instance, member))
			var methodRef = ((MethodCallExpression) ((UnaryExpression) expression).Operand).Arguments[2];
			return (MethodInfo) ((ConstantExpression) methodRef).Value;
		}

		public static ConstructorInfo ConstructorOf<T>(Expression<Func<T>> func)
		{
			return ((NewExpression) func.Body).Constructor;
		}
	}
}
