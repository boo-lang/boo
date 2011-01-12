using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class DetectNotImplementedFeatureUsage : AbstractFastVisitorCompilerStep
	{
		private IType _currentType;
		private IMethod _currentMethod;

		public override void Run()
		{
			if (Errors.Count > 0)
				return;
			base.Run();
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
			CheckForInvalidGenericParameterReferenceUsage(node);
		}

		public override void OnSimpleTypeReference(SimpleTypeReference node)
		{
			CheckForInvalidGenericParameterReferenceUsage(node);
		}

		private void CheckForInvalidGenericParameterReferenceUsage(Node node)
		{
			var genericParameterRef = node.Entity as IGenericParameter;
			if (genericParameterRef == null)
				return;

			var declaringEntity = genericParameterRef.DeclaringEntity;
			if (declaringEntity == _currentMethod || declaringEntity == _currentType)
				return;

			Errors.Add(CompilerErrorFactory.NotImplemented(node, "referencing generic parameter of outer type"));
		}
	}
}
