package Boo.Web

import Boo.Ast

class ViewStateAttribute(AstAttribute):

	_default as Expression
	
	Default:
		get:
			return _default
			
		set:
			_default = value
			
	// Invocado pelo compilador, recebe como parâmetro
	// o nó da AST ao qual foi aplicado
	def Apply(node):
		
		assert("ViewState pode ser aplicado somente a campos!", node kindof Field)			
		
		f as Field = node
		
		p = Property()
		p.Name = f.Name
		p.Type = f.Type
		p.Getter = CreateGetter(f)
		p.Setter = CreateSetter(f)

		f.ReplaceBy(p)

	protected def CreateGetter(f as Field):

		getter = Method()
		getter.ReturnType = f.Type
		
		if _default:
			
			// value = ViewState[<FieldName>]
			getter.Body.Add(
						BinaryExpression
						(
							BinaryOperatorType.Assign,
							ReferenceExpression("value"),
							CreateViewStateSlice(f)
						)
					)
							
			// return value ? value : <_default>
			getter.Body.Add(
						ReturnStatement
						(
							TernaryExpression
							(
								ReferenceExpression("value"),
								ReferenceExpression("value"),
								_default
							)
						)
					)				
			
		else:			
			// return ViewState[<FieldName>]
			getter.Body.Add(ReturnStatement(CreateViewStateSlice(f)))
		
		return getter
		
	protected def CreateSetter(f as Field):
		
		setter = Method()
		
		// ViewState[<FieldName>] = value
		setter.Body.Add(BinaryExpression(
						BinaryOperatorType.Assign,
						CreateViewStateSlice(f),
						ReferenceExpression("value")
						)
					)
		
		return setter
		
	protected def CreateViewStateSlice(f as Field):
		// ViewState["<f.Name>"]
		slice = SlicingExpression()
		slice.Target = ReferenceExpression("ViewState")
		slice.Begin = StringLiteralExpression(f.Name)
		return slice						
