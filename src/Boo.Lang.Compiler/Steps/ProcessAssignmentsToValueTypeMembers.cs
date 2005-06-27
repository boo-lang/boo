using System;

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	public class ProcessAssignmentsToValueTypeMembers : AbstractVisitorCompilerStep
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
			// BOO-313
			if (IsAssignmentToMemberOfValueType(node))
			{
				ProcessAssignmentToMemberOfValueTypeProperty(node);
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
		
		// BOO-313
		void ProcessAssignmentToMemberOfValueTypeProperty(BinaryExpression node)
		{	
			MemberReferenceExpression memberRef = (MemberReferenceExpression)node.Left;
			List chain = WalkMemberChain(memberRef);
			if (null == chain || 0 == chain.Count) return;

			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
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
				node.Left, node.Right));

			foreach (ChainItem item in chain.Reversed)
			{
				eval.Arguments.Add(
					CodeBuilder.CreateAssignment(
						item.Container.CloneNode(),
						CodeBuilder.CreateReference(item.Local)));
			}
					
			Node parentNode = node.ParentNode;
			parentNode.Replace(node, eval);
			
			if (NodeType.ExpressionStatement != parentNode.NodeType)
			{
				// TODO: add the expression value as the return value
				// of __eval__
				throw CompilerErrorFactory.NotImplemented(node, "BOO-313");
			}
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
					return IsReadOnlyField((IField)container.Entity);
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
