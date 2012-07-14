#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Linq.Expressions;
using System.Reflection;

namespace Boo.Lang.Compiler.Util
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
		
		public static MethodInfo Of<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> value)
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
