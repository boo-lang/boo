using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
	public class CacheRegularExpressionsInStaticFields : AbstractTransformerCompilerStep
	{
		private ClassDefinition _currentType;

		public override bool EnterClassDefinition(ClassDefinition node)
		{
			_currentType = node;
			return base.EnterClassDefinition(node);
		}

		override public void OnRELiteralExpression(RELiteralExpression node)
		{
			if (AstUtil.IsRhsOfAssignment(node))
				return;

			var field = CreateRegexFieldFor(node);
			AddFieldInitializerToStaticConstructor(field, node);

			ReplaceCurrentNode(CodeBuilder.CreateReference(field));
		}

		private Field CreateRegexFieldFor(RELiteralExpression node)
		{
			var field = CodeBuilder.CreateField(Context.GetUniqueName("re"), TypeSystemServices.RegexType);
			field.Modifiers = TypeMemberModifiers.Internal | TypeMemberModifiers.Static;
			field.LexicalInfo = node.LexicalInfo;
			_currentType.Members.Add(field);
			return field;
		}

		void AddFieldInitializerToStaticConstructor(Field node, Expression initializer)
		{
			var constructor = CodeBuilder.GetOrCreateStaticConstructorFor(_currentType);
			constructor.Body.Statements.Insert(0, CodeBuilder.CreateFieldAssignment(node, initializer));
		}
	}
}
