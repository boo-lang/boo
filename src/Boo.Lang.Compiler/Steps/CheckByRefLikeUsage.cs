#region license
// Copyright (c) 2026 the Boo contributors
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

using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// Reports the places where a byreflike value would reach the heap.
	/// Runs after closures and generators are expanded so the fields they
	/// hoist locals into are visible here.
	/// </summary>
	public class CheckByRefLikeUsage : AbstractFastVisitorCompilerStep
	{
		// One array type shows up at several nodes on its way through a method:
		// the literal that mints it, the local it lands in, the parameter it is
		// passed as. They are all the same mistake, so only the first is worth
		// reporting. Cleared per method; a field is reported once per type.
		private readonly List<IType> _reportedArrayTypes = new List<IType>();

		/// <summary>
		/// Nothing to say about a compilation that is already broken: the
		/// earlier error is the one worth reading, and the boxing it rejected
		/// has been reported by the type checker already.
		/// </summary>
		public override void Run()
		{
			if (Errors.Count > 0)
				return;
			base.Run();
		}

		public override void OnMethod(Method node)
		{
			_reportedArrayTypes.Clear();
			base.OnMethod(node);
		}

		public override void OnField(Field node)
		{
			var field = node.Entity as IField;
			if (field == null)
				return;

			if (TypeSystemServices.IsByRefLike(field.Type) && !IsInstanceFieldOfByRefLikeType(field))
			{
				// A closure or a generator hoists the locals it needs into a
				// synthetic class, which is a heap allocation like any other.
				if (node.DeclaringType != null && node.DeclaringType.IsSynthetic)
					Errors.Add(CompilerErrorFactory.ByRefLikeTypeCaptured(node, field.Type));
				else
					Errors.Add(CompilerErrorFactory.ByRefLikeFieldType(node, field.Type, field.DeclaringType));
				return;
			}

			CheckForByRefLikeArray(node, field.Type);
		}

		/// <summary>
		/// A byreflike struct may hold another one, since it stays on the stack
		/// with its owner. A static field does not, wherever it is declared.
		/// </summary>
		private static bool IsInstanceFieldOfByRefLikeType(IField field)
		{
			return !field.IsStatic && TypeSystemServices.IsByRefLike(field.DeclaringType);
		}

		/// <summary>
		/// Every array type the source spells out, wherever it appears: a field,
		/// a parameter, a local, a return type, the target of a cast.
		/// </summary>
		public override void OnArrayTypeReference(ArrayTypeReference node)
		{
			base.OnArrayTypeReference(node);
			CheckForByRefLikeArray(node, node.Entity as IType);
		}

		/// <summary>
		/// A declaration loses its type reference once the local is created, so
		/// the local itself is where a declared array type is still readable.
		/// </summary>
		public override void OnLocal(Local node)
		{
			base.OnLocal(node);
			var local = node.Entity as ILocalEntity;
			if (local != null)
				CheckForByRefLikeArray(node, local.Type);
		}

		/// <summary>
		/// The parameter's own type covers what its type reference would say,
		/// so the children are left unvisited rather than reported twice.
		/// </summary>
		public override void OnParameterDeclaration(ParameterDeclaration node)
		{
			var parameter = node.Entity as IParameter;
			if (parameter != null)
				CheckForByRefLikeArray(node, parameter.Type);
		}

		public override void OnBinaryExpression(BinaryExpression node)
		{
			base.OnBinaryExpression(node);

			// Assignment carries a value into the slot the left hand side names,
			// and a duck slot is an object slot like any other. A type test asks
			// the same of its left hand side, against object.
			if (BinaryOperatorType.Assign == node.Operator)
				CheckForBoxing(node.Right, node.Left.ExpressionType, node.Right.ExpressionType);
			else if (BinaryOperatorType.TypeTest == node.Operator)
				CheckForBoxing(node, TypeSystemServices.ObjectType, node.Left.ExpressionType);
		}

		public override void OnArrayLiteralExpression(ArrayLiteralExpression node)
		{
			base.OnArrayLiteralExpression(node);

			var arrayType = node.ExpressionType as IArrayType;
			if (arrayType == null)
				return;

			foreach (var item in node.Items)
				CheckForBoxing(item, arrayType.ElementType, item.ExpressionType);
		}

		/// <summary>
		/// Naming a reference type to cast to does not make the value reach it
		/// any differently. The using macro casts to IDisposable this way.
		/// </summary>
		public override void OnCastExpression(CastExpression node)
		{
			base.OnCastExpression(node);
			CheckForBoxing(node, GetType(node.Type), node.Target.ExpressionType);
		}

		public override void OnTryCastExpression(TryCastExpression node)
		{
			base.OnTryCastExpression(node);
			CheckForBoxing(node, GetType(node.Type), node.Target.ExpressionType);
		}

		/// <summary>
		/// A list and a hash hold object, so everything put in one is boxed.
		/// </summary>
		public override void OnListLiteralExpression(ListLiteralExpression node)
		{
			base.OnListLiteralExpression(node);
			foreach (var item in node.Items)
				CheckForBoxing(item, TypeSystemServices.ObjectType, item.ExpressionType);
		}

		public override void OnHashLiteralExpression(HashLiteralExpression node)
		{
			base.OnHashLiteralExpression(node);
			foreach (var pair in node.Items)
			{
				CheckForBoxing(pair.First, TypeSystemServices.ObjectType, pair.First.ExpressionType);
				CheckForBoxing(pair.Second, TypeSystemServices.ObjectType, pair.Second.ExpressionType);
			}
		}

		/// <summary>
		/// Every argument the call ends up passing, once the vararg and property
		/// rewrites have settled on a signature.
		/// </summary>
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.OnMethodInvocationExpression(node);
			CheckForInheritedMemberCall(node);

			var callable = node.Target.Entity as IMethodBase;
			if (callable == null)
				return;

			var parameters = callable.GetParameters();
			for (var i = 0; i < parameters.Length && i < node.Arguments.Count; ++i)
				CheckForBoxing(node.Arguments[i], parameters[i].Type, node.Arguments[i].ExpressionType);
		}

		/// <summary>
		/// A byreflike value reaches an overridden member directly and an
		/// interface member by a constrained call. One it merely inherits, such
		/// as an object member it leaves alone, can only be reached by boxing.
		/// </summary>
		private void CheckForInheritedMemberCall(MethodInvocationExpression node)
		{
			var member = node.Target as MemberReferenceExpression;
			if (member == null)
				return;

			var method = member.Entity as IMethod;
			if (method == null || method.IsStatic)
				return;

			var targetType = member.Target.ExpressionType;
			if (!TypeSystemServices.IsByRefLike(targetType))
				return;

			var declaringType = method.DeclaringType;
			if (declaringType == null || declaringType.IsValueType || declaringType.IsInterface)
				return;

			Errors.Add(CompilerErrorFactory.CannotBoxByRefLikeType(node, targetType, declaringType));
		}

		private void CheckForBoxing(Node node, IType expectedType, IType actualType)
		{
			if (TypeSystemServices.WouldBoxByRefLikeType(expectedType, actualType))
				Errors.Add(CompilerErrorFactory.CannotBoxByRefLikeType(node, actualType, expectedType));
		}

		private void CheckForByRefLikeArray(Node node, IType type)
		{
			while (type != null && type.IsArray)
			{
				type = type.ElementType;
				if (TypeSystemServices.IsByRefLike(type))
				{
					if (!_reportedArrayTypes.Contains(type))
					{
						_reportedArrayTypes.Add(type);
						Errors.Add(CompilerErrorFactory.ByRefLikeArrayElementType(node, type));
					}
					return;
				}
			}
		}
	}
}
