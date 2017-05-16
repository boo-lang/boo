using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class TypeFinder : AbstractFastVisitorCompilerStep
	{
		protected TypeCollector _collector;

		public TypeFinder(TypeCollector collector)
		{
			_collector = collector;
		}

		public IEnumerable<IType> Results
		{
			get { return _collector.Matches; }
		}

		private void OnTypeReference(TypeReference node)
		{
			var type = (IType)node.Entity;
			_collector.Visit(type);
		}

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			var te = node.Entity as ITypedEntity;
			if (te != null)
			{
				_collector.Visit(te.Type);
				if (te.EntityType == EntityType.Constructor)
					_collector.Visit(((IConstructor) te).DeclaringType);
			}
		}

		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			base.OnMemberReferenceExpression(node);
			OnReferenceExpression(node);
		}

		public override void OnSimpleTypeReference(SimpleTypeReference node)
		{
			OnTypeReference(node);
		}

		public override void OnArrayTypeReference(ArrayTypeReference node)
		{
			base.OnArrayTypeReference(node);
			OnTypeReference(node);
		}

		public override void OnCallableTypeReference(CallableTypeReference node)
		{
			base.OnCallableTypeReference(node);
			OnTypeReference(node);
		}

		public override void OnGenericTypeReference(GenericTypeReference node)
		{
			base.OnGenericTypeReference(node);
			OnTypeReference(node);
		}

		public override void OnGenericTypeDefinitionReference(GenericTypeDefinitionReference node)
		{
			base.OnGenericTypeDefinitionReference(node);
			OnTypeReference(node);
		}
	}
}