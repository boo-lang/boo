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

using System.Linq;
using System.Runtime.Remoting.Messaging;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
	
namespace Boo.Lang.Compiler.Steps
{
	public class InjectCallableConversions : AbstractVisitorCompilerStep
	{
		IMethod _current;
		
		IType _asyncResultType;
		
		IMethod _asyncResultTypeAsyncDelegateGetter;

		readonly List<AdaptorRecord> _adaptors = new List<AdaptorRecord>();
		
		override public void Run()
		{
			if (Errors.Count != 0)
				return;

			Visit(CompileUnit);

			CheckFieldInvocations();
		}

		private void CheckFieldInvocations()
		{
			var invocations = ContextAnnotations.GetFieldInvocations();
			if (invocations == null) return;

			foreach (var node in invocations)
			{
				var et = node.Target.ExpressionType;
				if (et is AnonymousCallableType)
				{
					et = ((AnonymousCallableType) et).ConcreteType;
					node.Target.ExpressionType = et;
				}
				var invoke = NameResolutionService.Resolve(et, "Invoke") as IMethod;
				if (invoke == null)
					throw new System.NotSupportedException("Invoke method on callable field not found");
				node.Target = CodeBuilder.CreateMemberReference(node.Target.LexicalInfo, node.Target, invoke);
			}
		}

		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			// allow interactive evaluation of closures (see booish)
			var converted = ConvertExpression(node.Expression);
			if (converted != null)
				node.Expression = converted;
		}

		override public void LeaveReturnStatement(ReturnStatement node)
		{
			Expression expression = node.Expression;
			if (null == expression)
				return;

			if (!HasReturnType(_current))
				return;

			Expression newExpression = Convert(_current.ReturnType, expression);
			if (null == newExpression)
				return;

			node.Expression = newExpression;
		}

		override public void LeaveExpressionPair(ExpressionPair pair)
		{
			Expression converted = ConvertExpression(pair.First);
			if (null != converted)
			{
				pair.First = converted;
			}

			converted = ConvertExpression(pair.Second);
			if (null != converted)
			{
				pair.Second = converted;
			}
		}

		override public void LeaveListLiteralExpression(ListLiteralExpression node)
		{
			ConvertExpressions(node.Items);
		}

		override public void LeaveArrayLiteralExpression(ArrayLiteralExpression node)
		{
			IType elementType = GetExpressionType(node).ElementType;
			for (int i = 0; i < node.Items.Count; ++i)
			{
				Expression converted = Convert(elementType, node.Items[i]);
				if (null != converted)
				{
					node.Items.ReplaceAt(i, converted);
				}
			}
		}

		override public void LeaveMethodInvocationExpression(MethodInvocationExpression node)
		{
			var parameters = ParametersFor(node.Target);
			if (parameters == null)
				return;
			ConvertMethodInvocation(node, parameters);
		}

		private static IParameter[] ParametersFor(Expression callableExpression)
		{
			var entity = callableExpression.Entity as IMethod;
			if (entity != null)
				return entity.GetParameters();

			var type = callableExpression.ExpressionType as ICallableType;
			if (type != null)
				return type.GetSignature().Parameters;

			return null;
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (IsEndInvokeOnStandaloneMethodReference(node) && AstUtil.IsTargetOfMethodInvocation(node))
			{
				ReplaceEndInvokeTargetByGetAsyncDelegate((MethodInvocationExpression)node.ParentNode);
				return;
			}

			var newTarget = ConvertExpression(node.Target);
			if (null != newTarget)
				node.Target = newTarget;
		}

		override public void LeaveCastExpression(CastExpression node)
		{
			var newExpression = Convert(node.ExpressionType, node.Target);
			if (newExpression != null)
				node.Target = newExpression;
		}

		public override void LeaveConditionalExpression(ConditionalExpression node)
		{
			var newTrueValue = Convert(node.ExpressionType, node.TrueValue);
			if (newTrueValue != null)
				node.TrueValue = newTrueValue;
			var newFalseValue = Convert(node.ExpressionType, node.FalseValue);
			if (newFalseValue != null)
				node.FalseValue = newFalseValue;
		}

		override public void LeaveTryCastExpression(TryCastExpression node)
		{
			Expression newExpression = Convert(node.ExpressionType, node.Target);
			if (null != newExpression)
				node.Target = newExpression;
		}

		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign != node.Operator)
				return;

			Expression newRight = Convert(node.Left.ExpressionType, node.Right);
			if (null != newRight)
				node.Right = newRight;
		}

		override public void LeaveGeneratorExpression(GeneratorExpression node)
		{
			Expression newExpression = Convert(
										GetConcreteExpressionType(node.Expression),
										node.Expression);
			if (null != newExpression)
			{
				node.Expression = newExpression;
			}
		}
		
		void InitializeAsyncResultType()
		{
			if (_asyncResultType != null)
				return;

			var type = typeof(AsyncResult);
			_asyncResultType = TypeSystemServices.Map(type);
			_asyncResultTypeAsyncDelegateGetter = TypeSystemServices.Map(Methods.GetterOf<AsyncResult, object>(r => r.AsyncDelegate));
		}
		
		override public void Dispose()
		{
			_asyncResultType = null;
			_asyncResultTypeAsyncDelegateGetter = null;
			_adaptors.Clear();
			base.Dispose();
		}
		
		override public void OnMethod(Method node)
		{
			_current = GetEntity(node);
			Visit(node.Body);
		}
		
		bool HasReturnType(IMethod method)
		{
			return TypeSystemServices.VoidType != method.ReturnType;
		}
		
		bool IsMethodReference(Expression node)
		{
			IEntity entity = GetEntity(node);
			return EntityType.Method == entity.EntityType;
		}

		static bool IsNotTargetOfMethodInvocation(Expression node)
		{
			var mie = node.ParentNode as MethodInvocationExpression;
			return mie == null || mie.Target != node;
		}

		bool IsStandaloneMethodReference(Expression node)
		{
			return
				(node is ReferenceExpression || node is GenericReferenceExpression)
                && IsMethodReference(node)
                && IsNotTargetOfMethodInvocation(node);
		}
		
		private void ConvertMethodInvocation(MethodInvocationExpression node, IParameter[] parameters)
		{
			ExpressionCollection arguments = node.Arguments;
			for (int i=0; i<parameters.Length; ++i)
			{
				Expression newArgument = Convert(parameters[i].Type, arguments[i]);
				if (null != newArgument)
				{
					arguments.ReplaceAt(i, newArgument);
				}
			}
		}

		void ConvertExpressions(ExpressionCollection items)
		{
			for (int i=0; i<items.Count; ++i)
			{
				Expression converted = ConvertExpression(items[i]);
				if (null != converted)
				{
					items.ReplaceAt(i, converted);
				}
			}
		}
		
		Expression ConvertExpression(Expression expression)
		{
			return Convert(expression.ExpressionType, expression);
		}
		
		Expression Convert(IType expectedType, Expression argument)
		{
			if (IsStandaloneMethodReference(argument))
				return ConvertMethodReference(expectedType, argument);

			var callableType = expectedType as ICallableType;
			if (callableType != null)
			{
				var argumentType = GetExpressionType(argument);
				if (expectedType != argumentType && !argumentType.IsNull())
					return Adapt(callableType, argument);
			}
			return null;
		}

		private Expression ConvertMethodReference(IType expectedType, Expression argument)
		{
			var expectedCallable = expectedType as ICallableType;
			if (expectedCallable != null)
			{
				var argumentType = (ICallableType) GetExpressionType(argument);
                var expectedSig = expectedCallable.GetSignature();
                var argSig = argumentType.GetSignature();
                if (argSig != expectedSig)
                {
                    if (TypeSystemServices.CompatibleSignatures(argSig, expectedSig) ||
						(TypeSystemServices.CompatibleGenericSignatures(argSig, expectedSig) /*&& IsUnspecializedGenericMethodReference(argument)*/)
					   )
                    {
                        argument.ExpressionType = expectedType;
                        return CreateDelegate(expectedType, argument);
                    }
                    return Adapt(expectedCallable, CreateDelegate(GetConcreteExpressionType(argument), argument));
                }
				return CreateDelegate(expectedType, argument);
			}
			return CreateDelegate(GetConcreteExpressionType(argument), argument);
		}

	    private static bool IsUnspecializedGenericMethodReference(Expression argument)
	    {
	        if (argument.NodeType != NodeType.MemberReferenceExpression)
	            return false;
	        var target = ((MemberReferenceExpression) argument).Target;
            if (target.NodeType != NodeType.MethodInvocationExpression)
                return false;
            target = ((MethodInvocationExpression)target).Target;
	        if (target.Entity.EntityType != EntityType.Constructor)
	            return false;
	        var cls = ((IConstructor) target.Entity).DeclaringType;
	        return cls.GenericInfo != null && (cls.ConstructedInfo == null || !cls.ConstructedInfo.FullyConstructed);
	    }

	    Expression Adapt(ICallableType expected, Expression callable)
		{
			ICallableType actual = GetExpressionType(callable) as ICallableType;
			if (null == actual)
			{
				// TODO: should we adapt System.Object, System.Delegate,
				// System.MulticastDelegate and ICallable as well?
				return null;
			}
			ClassDefinition adaptor = GetAdaptor(expected, actual);
			Method adapt = (Method)adaptor.Members["Adapt"];
			return CodeBuilder.CreateMethodInvocation((IMethod)adapt.Entity, callable);
		}
		
		ClassDefinition GetAdaptor(ICallableType to, ICallableType from)
		{
			return FindAdaptor(to, from) ?? CreateAdaptor(to, from);
		}
		
		sealed class AdaptorRecord
		{
			public readonly ICallableType To;
			public readonly ICallableType From;
			public readonly ClassDefinition Adaptor;
			
			public AdaptorRecord(ICallableType to, ICallableType from, ClassDefinition adaptor)
			{
				To = to;
				From = from;
				Adaptor = adaptor;
			}
		}
		
		ClassDefinition FindAdaptor(ICallableType to, ICallableType from)
		{
			foreach (AdaptorRecord record in _adaptors)
				if (from == record.From && to == record.To)
					return record.Adaptor;
			return null;
		}
		
		ClassDefinition CreateAdaptor(ICallableType to, ICallableType from)
		{
			BooClassBuilder adaptor = CodeBuilder.CreateClass("$adaptor$" + from.Name + "$" + to.Name + "$" + _adaptors.Count);
			adaptor.AddBaseType(TypeSystemServices.ObjectType);
			adaptor.Modifiers = TypeMemberModifiers.Final|TypeMemberModifiers.Internal;
			
			Field callable = adaptor.AddField("$from", from);
			
			BooMethodBuilder constructor = adaptor.AddConstructor();
			ParameterDeclaration param = constructor.AddParameter("from", from);
			constructor.Body.Add(
				CodeBuilder.CreateSuperConstructorInvocation(TypeSystemServices.ObjectType));
			constructor.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(callable),
					CodeBuilder.CreateReference(param)));
					
			CallableSignature signature = to.GetSignature();
			BooMethodBuilder invoke = adaptor.AddMethod("Invoke", signature.ReturnType);
			foreach (IParameter parameter in signature.Parameters)
			{
				invoke.AddParameter(parameter.Name, parameter.Type, parameter.IsByRef);
			}
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
							CodeBuilder.CreateReference(callable),
							GetInvokeMethod(from));
			int fromParameterCount = from.GetSignature().Parameters.Length;
			for (int i=0; i<fromParameterCount; ++i)
			{
				mie.Arguments.Add(
					CodeBuilder.CreateReference(invoke.Parameters[i]));
			}
			if (signature.ReturnType != TypeSystemServices.VoidType &&
				from.GetSignature().ReturnType != TypeSystemServices.VoidType)
			{
				invoke.Body.Add(new ReturnStatement(mie));
			}
			else
			{
				invoke.Body.Add(mie);
			}
			
			BooMethodBuilder adapt = adaptor.AddMethod("Adapt", to);
			adapt.Modifiers = TypeMemberModifiers.Static|TypeMemberModifiers.Public;
			param = adapt.AddParameter("from", from);
			adapt.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateConstructorInvocation(
						to.GetConstructors().First(),
						CodeBuilder.CreateConstructorInvocation(
							(IConstructor)constructor.Entity,
							CodeBuilder.CreateReference(param)),
						CodeBuilder.CreateAddressOfExpression(invoke.Entity))));
			
			var collector = new GenericTypeCollector(this.CodeBuilder);
			collector.Process(adaptor.ClassDefinition);
			RegisterAdaptor(to, from, adaptor.ClassDefinition);
			
			return adaptor.ClassDefinition;
		}
		
		void RegisterAdaptor(ICallableType to, ICallableType from, ClassDefinition adaptor)
		{
			_adaptors.Add(new AdaptorRecord(to, from, adaptor));
			TypeSystemServices.GetCompilerGeneratedTypesModule().Members.Add(adaptor);
		}
		
		bool IsEndInvokeOnStandaloneMethodReference(MemberReferenceExpression node)
		{
			if (IsStandaloneMethodReference(node.Target))
			{
				return node.Entity.Name == "EndInvoke";
			}
			return false;
		}
		
		void ReplaceEndInvokeTargetByGetAsyncDelegate(MethodInvocationExpression node)
		{
			InitializeAsyncResultType();

			var asyncResult = node.Arguments.Last;
			var endInvoke = (MemberReferenceExpression)node.Target;
			var callableType = ((IMember)endInvoke.Entity).DeclaringType;
			
			endInvoke.Target = CodeBuilder.CreateCast(callableType,
									CodeBuilder.CreateMethodInvocation(
										CodeBuilder.CreateCast(_asyncResultType, asyncResult.CloneNode()),
										_asyncResultTypeAsyncDelegateGetter));
		}

		Expression CreateDelegate(IType type, Expression source)
		{
			var method = (IMethod)GetEntity(source);
			
			Expression target = method.IsStatic
                ? CodeBuilder.CreateNullLiteral()
                : ((MemberReferenceExpression)source).Target;

			var cType = GetConcreteType(type) ?? 
				TypeSystemServices.GetConcreteCallableType(source, (AnonymousCallableType) type);
			return CodeBuilder.CreateConstructorInvocation(cType.GetConstructors().First(),
									target,
									CodeBuilder.CreateAddressOfExpression(method));
		}

		static IType GetConcreteType(IType type)
		{
			var anonymous = type as AnonymousCallableType;
			return null == anonymous ? type : anonymous.ConcreteType;
		}
		
		IMethod GetInvokeMethod(ICallableType type)
		{
			return NameResolutionService.ResolveMethod(type, "Invoke");
		}
	}
}
