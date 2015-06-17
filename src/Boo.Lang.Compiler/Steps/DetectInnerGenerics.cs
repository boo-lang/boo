using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class DetectInnerGenerics: FastDepthFirstVisitor
	{
		private IType _currentType;
		private IMethod _currentMethod;
		private List<Node> _values = new List<Node>();

		public IEnumerable<Node> Values
		{
			get {return _values;}
		}

		public override void OnClassDefinition(ClassDefinition node)
		{
			OnTypeDefinition(node);
		}

		public override void OnInterfaceDefinition(InterfaceDefinition node)
		{
			OnTypeDefinition(node);
		}

		public override void OnStructDefinition(StructDefinition node)
		{
			OnTypeDefinition(node);
		}

		private void OnTypeDefinition(TypeDefinition node)
		{
			var old = _currentType;
			_currentType = (IType)node.Entity;
			Visit(node.Attributes);
			Visit(node.BaseTypes);
			Visit(node.Members);
			Visit(node.GenericParameters);
			_currentType = old;
		}

		public override void OnMethod(Method node)
		{
			var old = _currentMethod;
			_currentMethod = (IMethod) node.Entity;
			base.OnMethod(node);
			_currentMethod = old;
		}

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			CheckForInnerGenericParameterReferenceUsage(node);
		}

		public override void OnSimpleTypeReference(SimpleTypeReference node)
		{
			CheckForInnerGenericParameterReferenceUsage(node);
		}

		private void CheckForInnerGenericParameterReferenceUsage(Node node)
		{
			var genericParameterRef = node.Entity as IGenericParameter;
			if (genericParameterRef == null)
				return;

			var declaringEntity = genericParameterRef.DeclaringEntity;
			if (declaringEntity == _currentMethod || declaringEntity == _currentType)
				return;
			
			_values.Add(node);

		}
	}
}
