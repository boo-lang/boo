namespace Boo.Lang.Compiler.Steps
{
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
			if (IsAssignmentToMemberOfValueTypeProperty(node))
			{
				ProcessAssignmentToMemberOfValueTypeProperty(node);
			}
		}

		bool IsAssignmentToMemberOfValueTypeProperty(BinaryExpression node)
		{	
			if (BinaryOperatorType.Assign == node.Operator &&
				NodeType.MemberReferenceExpression == node.Left.NodeType)
			{
				MemberReferenceExpression outter = node.Left as MemberReferenceExpression;
				if (NodeType.MemberReferenceExpression == outter.Target.NodeType)
				{
					return EntityType.Property == outter.Target.Entity.EntityType &&
						outter.Target.ExpressionType.IsValueType;	
				}
			}
			return false;
		}
		
		// BOO-313
		// TODO: do it recursively for internal struct properties
		void ProcessAssignmentToMemberOfValueTypeProperty(BinaryExpression node)
		{	
			Node parentNode = node.ParentNode;
			
			MemberReferenceExpression memberRef = (MemberReferenceExpression)node.Left;
			MemberReferenceExpression property = (MemberReferenceExpression)memberRef.Target;			
			if (!CheckLValue(property))
			{
				return;
			}
			
			InternalLocal temp = DeclareTempLocal(property.ExpressionType);
			
			BinaryExpression tempInitialization = CodeBuilder.CreateAssignment(
				node.LexicalInfo,
				CodeBuilder.CreateReference(temp),
				property.CloneNode());
				
			memberRef.Target = CodeBuilder.CreateReference(temp);
			
			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
			eval.Arguments.Add(tempInitialization);
			eval.Arguments.Add(
				CodeBuilder.CreateAssignment(node.LexicalInfo,
				node.Left, node.Right));
			eval.Arguments.Add(
				CodeBuilder.CreatePropertySet(
				property.Target,
				(IProperty)property.Entity,
				CodeBuilder.CreateReference(temp)));
					
			parentNode.Replace(node, eval);
			
			if (NodeType.ExpressionStatement != parentNode.NodeType)
			{
				// TODO: add the expression value as the return value
				// of __eval__
				NotImplemented(node, "BOO-313");
			}
		}

		InternalLocal DeclareTempLocal(IType localType)
		{
			return CodeBuilder.DeclareTempLocal(_currentMethod, localType);
		}
	}
}
