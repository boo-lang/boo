namespace Boo.Lang.Extensions

import Boo.Lang.Compiler.Ast

macro initialization:
	macro order:
		assert len(order.Arguments) == 1 and order.Arguments[0] isa IntegerLiteralExpression, "order <integer>"
		value as IntegerLiteralExpression = order.Arguments[0] 
		initialization['Ordering'] = value.Value
	
	name = Context.GetUniqueName('module_ctor')
	mCons = [|
		static internal def $name():
			$(initialization.Body)
	|]
	mCons['Ordering'] = (initialization['Ordering'] if initialization.ContainsAnnotation('Ordering') else int.MaxValue)
	mCons.IsSynthetic = true
	yield mCons