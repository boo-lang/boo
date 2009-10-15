namespace BooCompiler.Tests.SupportingClasses
{
	public class ExtendsOverridenBoolOperator : OverrideBoolOperator
	{
		[Boo.Lang.DuckTyped]
		public ExtendsOverridenBoolOperator GetFoo()
		{
			return new ExtendsOverridenBoolOperator();
		}
	}
}