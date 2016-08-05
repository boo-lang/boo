namespace Boo.Lang.Extensions

import Boo.Lang.Compiler.Ast

macro initialization:
	macro order(value as IntegerLiteralExpression):
		initialization['Ordering'] = value.Value
	
	name = Context.GetUniqueName('module_ctor')
	mCons = [|
		static internal def $name():
			$(initialization.Body)
	|]
	mCons['Ordering'] = (initialization['Ordering'] if initialization.ContainsAnnotation('Ordering') else int.MaxValue)
	mCons.IsSynthetic = true
	yield mCons