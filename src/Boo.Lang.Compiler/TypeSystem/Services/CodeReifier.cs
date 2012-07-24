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
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	/// <summary>
	/// Implemented by compiler steps that need to take part in type member
	/// reification.
	/// </summary>
	public interface ITypeMemberReifier : INodeReifier<TypeMember>
	{
	}

	/// <summary>
	/// Implemented by compiler steps that need to take part in type reference
	/// reification.
	/// </summary>
	public interface ITypeReferenceReifier
	{
		void Reify(TypeReference node);
	}

	/// <summary>
	/// Implemented by compiler steps that need to take part in statement
	/// reification.
	/// </summary>
	public interface IStatementReifier : INodeReifier<Statement>
	{	
	}

	/// <summary>
	/// Implemented by compiler steps that need to take part in expression
	/// reification.
	/// </summary>
	public interface IExpressionReifier : INodeReifier<Expression>
	{
	}

	public interface INodeReifier<T> where T: Node
	{
		T Reify(T node);
	}

	/// <summary>
	/// Reifies ast nodes.
	/// 
	/// Reification of an ast node means compiling it up to the point of being ready for inclusion into <see cref="CompilerContext.CompileUnit"/>.
	/// 
	/// This allows code generated through the AST api or code literals to be added to the current CompileUnit without having to go through
	/// bureaucratic <see cref="CompilerContext.CodeBuilder"/> calls.
	/// </summary>
	public class CodeReifier : AbstractCompilerComponent
	{
		public Statement Reify(Statement node)
		{
			return ReifyNode(node);
		}

		public Expression Reify(Expression node)
		{
			return ReifyNode(node);
		}

		public void ReifyInto(TypeDefinition parentType, TypeMember member)
		{	
			if (member == null)
				throw new ArgumentNullException("member");
			if (parentType == null)
				throw new ArgumentNullException("parentType");
			if (member.ParentNode != null)
				throw new ArgumentException("ParentNode must be null for member to be reified.", "member");
			
			parentType.Members.Add(member);
			ReifyNode(member);
		}

		public void MergeInto(TypeDefinition targetType, TypeDefinition mixin)
		{
			if (null == targetType)
				throw new ArgumentNullException("targetType");
			if (null == mixin)
				throw new ArgumentNullException("mixin");

			targetType.BaseTypes.AddRange(mixin.BaseTypes);
			foreach (var baseType in mixin.BaseTypes)
				Reify(baseType);

			targetType.Members.AddRange(mixin.Members);
			foreach (var member in mixin.Members)
				ReifyNode(member);
		}

		private void Reify(TypeReference node)
		{
			ForEachReifier<ITypeReferenceReifier>(r => r.Reify(node));
		}

		private T ReifyNode<T>(T node) where T : Node
		{
			var original = node;
			var originalParent = original.ParentNode;
			if (null == originalParent)
				throw new ArgumentException(string.Format("ParentNode must be set on {0}.", typeof(T).Name), "node");

			ForEachReifier<INodeReifier<T>>(r =>
			{
				node = r.Reify(node);
				if (node != original)
				{
					originalParent.Replace(original, node);
					original = node;
					originalParent = original.ParentNode;
				}
			});
			return node;
		}

		private void ForEachReifier<T>(Action<T> action) where T: class
		{
			var currentScope = NameResolutionService.CurrentNamespace;
			try
			{
				var pipeline = Parameters.Pipeline;
				var currentStep = pipeline.CurrentStep;
				foreach (var step in pipeline)
				{
					if (step == currentStep)
						break;

					var reifier = step as T;
					if (reifier == null)
						continue;

					action(reifier);
				}
			}
			finally
			{
				NameResolutionService.EnterNamespace(currentScope);
			}
		}
	}
}
