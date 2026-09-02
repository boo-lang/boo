namespace BooCompiler.Tests.SupportingClasses

class ExtendsOverridenBoolOperator(OverrideBoolOperator):
	[Boo.Lang.DuckTyped]
	public def GetFoo() as ExtendsOverridenBoolOperator:
		return ExtendsOverridenBoolOperator()
