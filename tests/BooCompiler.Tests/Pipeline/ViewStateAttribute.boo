namespace BooCompiler.Tests

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class ViewStateAttribute(AbstractAstAttribute):
	private _default as Expression

	def constructor():
		pass

	public Default as Expression:
		get: return _default
		set: _default = value

	override public def Apply(node as Node):
		f = node as Field

		p = Property()
		p.Name = f.Name
		p.Type = f.Type
		p.Setter = CreateSetter(f)
		p.Getter = CreateGetter(f)

		(f.ParentNode as TypeDefinition).Members.Replace(f, p)

	private def CreateSetter(f as Field) as Method:
		m = Method()
		m.Name = "set"
		m.Body.Statements.Add(
			ExpressionStatement(
				BinaryExpression(
					BinaryOperatorType.Assign,
					CreateViewStateSlice(f),
					ReferenceExpression("value"))))
		return m

	private def CreateGetter(f as Field) as Method:
		m = Method()
		m.Name = "get"

		# value = ViewState["<f.Name>"]
		m.Body.Statements.Add(
			ExpressionStatement(
				BinaryExpression(
					BinaryOperatorType.Assign,
					ReferenceExpression("value"),
					CreateViewStateSlice(f))))

		if _default is not null:
			# return <_default> unless value
			rs = ReturnStatement(_default)
			rs.Modifier = StatementModifier(StatementModifierType.Unless, ReferenceExpression("value"))
			m.Body.Statements.Add(rs)

		# return value
		m.Body.Statements.Add(
			ReturnStatement(
				ReferenceExpression("value")))

		return m

	private def CreateViewStateSlice(f as Field) as Expression:
		# ViewState["<f.Name>"]
		return SlicingExpression(
			ReferenceExpression("ViewState"),
			StringLiteralExpression(f.Name))
