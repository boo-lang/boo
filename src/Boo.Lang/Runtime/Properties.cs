using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public static class Properties
	{
		public static PropertyInfo Of<TInstance, TProperty>(Expression<Func<TInstance, TProperty>> lambda)
		{
			return (PropertyInfo) ((MemberExpression) lambda.Body).Member;
		}
	}
}
