#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	/// <summary>
	/// see BOO-313
	/// </summary>
	public class ProcessAssignmentsToValueTypeMembers : AbstractTransformerCompilerStep
	{
		Method _currentMethod;

		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}

		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{	
		}

		override public void OnEnumDefinition(EnumDefinition node)
		{	
		}

		override public void OnMethod(Method node)
		{	
			_currentMethod = node;
			Visit(node.Body);
		}

		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (IsAssignmentToMemberOfValueType(node))
			{
				ProcessAssignmentToMemberOfValueType(node);
			}
		}

		bool IsAssignmentToMemberOfValueType(BinaryExpression node)
		{	
			if (BinaryOperatorType.Assign == node.Operator &&
				NodeType.MemberReferenceExpression == node.Left.NodeType)
			{
				MemberReferenceExpression memberRef = node.Left as MemberReferenceExpression;
				Expression container = memberRef.Target;
				return !IsTerminalReferenceNode(container)
					&& null != container.ExpressionType
					&& container.ExpressionType.IsValueType;	
			}
			return false;
		}

		class ChainItem
		{
			public MemberReferenceExpression Container;
			public InternalLocal Local;

			public ChainItem(MemberReferenceExpression container)
			{
				this.Container = container;
			}
		}
		
		void ProcessAssignmentToMemberOfValueType(BinaryExpression node)
		{	
			MemberReferenceExpression memberRef = (MemberReferenceExpression)node.Left;
			List chain = WalkMemberChain(memberRef);
			if (null == chain || 0 == chain.Count) return;
			
			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);

			// right hand side should always be executed before
			// left hand side
			InternalLocal value = DeclareTempLocal(GetExpressionType(node.Right));
			eval.Arguments.Add(
				CodeBuilder.CreateAssignment(
				CodeBuilder.CreateReference(value),
				node.Right));

			foreach (ChainItem item in chain)
			{
				item.Local = DeclareTempLocal(item.Container.ExpressionType);
				BinaryExpression tempInitialization = CodeBuilder.CreateAssignment(
					node.LexicalInfo,
					CodeBuilder.CreateReference(item.Local),
					item.Container.CloneNode());
				item.Container.ParentNode.Replace(item.Container,
					CodeBuilder.CreateReference(item.Local));
				eval.Arguments.Add(tempInitialization);
			}

			eval.Arguments.Add(
				CodeBuilder.CreateAssignment(node.LexicalInfo,
				node.Left,
				CodeBuilder.CreateReference(value)));

			foreach (ChainItem item in chain.Reversed)
			{
				eval.Arguments.Add(
					CodeBuilder.CreateAssignment(
						item.Container.CloneNode(),
						CodeBuilder.CreateReference(item.Local)));
			}		
			
			if (NodeType.ExpressionStatement != node.ParentNode.NodeType)
			{
				eval.Arguments.Add(CodeBuilder.CreateReference(value));	
				BindExpressionType(eval, value.Type);
			}
			
			ReplaceCurrentNode(eval);
		}

		List WalkMemberChain(MemberReferenceExpression memberRef)
		{
			List chain = new List();
			while (true)
			{	
				MemberReferenceExpression container = memberRef.Target as MemberReferenceExpression;
				if (null == container || IsReadOnlyMember(container))
				{	
					Warnings.Add(
						CompilerWarningFactory.AssignmentToTemporary(memberRef));
					return null;
				}
				if (EntityType.Field != container.Entity.EntityType)
				{
					chain.Insert(0, new ChainItem(container));
				}
				if (IsTerminalReferenceNode(container.Target))
				{
					break;	
				}
				memberRef = container;
			}
			return chain;
		}

		private bool IsTerminalReferenceNode(Expression target)
		{
			NodeType type = target.NodeType;
			return 
				NodeType.ReferenceExpression == type ||
				NodeType.SelfLiteralExpression == type ||
				NodeType.SuperLiteralExpression == type;
		}

		private bool IsReadOnlyMember(MemberReferenceExpression container)
		{
			switch (container.Entity.EntityType)
			{
				case EntityType.Property:
				{
					return ((IProperty)container.Entity).GetSetMethod() == null;
				}
				case EntityType.Field:
				{
					return TypeSystemServices.IsReadOnlyField((IField)container.Entity);
				}
			}
			return true;
		}

		InternalLocal DeclareTempLocal(IType localType)
		{
			return CodeBuilder.DeclareTempLocal(_currentMethod, localType);
		}
	}
}
