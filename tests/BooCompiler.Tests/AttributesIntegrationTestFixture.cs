namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class AttributesIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void assembly_attributes_1()
		{
			RunCompilerTestCase(@"assembly-attributes-1.boo");
		}
		
		[Test]
		public void assembly_attributes_2()
		{
			RunCompilerTestCase(@"assembly-attributes-2.boo");
		}
		
		[Test]
		public void attributes_1()
		{
			RunCompilerTestCase(@"attributes-1.boo");
		}
		
		[Test]
		public void attributes_2()
		{
			RunCompilerTestCase(@"attributes-2.boo");
		}
		
		[Test]
		public void attributes_3()
		{
			RunCompilerTestCase(@"attributes-3.boo");
		}
		
		[Test]
		public void attributes_4()
		{
			RunCompilerTestCase(@"attributes-4.boo");
		}
		
		[Test]
		public void attributes_5()
		{
			RunCompilerTestCase(@"attributes-5.boo");
		}
		
		[Test]
		public void attributes_6()
		{
			RunCompilerTestCase(@"attributes-6.boo");
		}
		
		[Test]
		public void attributes_7()
		{
			RunCompilerTestCase(@"attributes-7.boo");
		}
		
		[Test]
		public void attributes_8()
		{
			RunCompilerTestCase(@"attributes-8.boo");
		}
		
		[Test]
		public void conditionalattribute_1()
		{
			RunCompilerTestCase(@"conditionalattribute-1.boo");
		}
		
		[Test]
		public void conditionalattribute_2()
		{
			RunCompilerTestCase(@"conditionalattribute-2.boo");
		}
		
		[Test]
		public void conditionalattribute_3()
		{
			RunCompilerTestCase(@"conditionalattribute-3.boo");
		}
		
		[Test]
		public void conditionalattribute_4()
		{
			RunCompilerTestCase(@"conditionalattribute-4.boo");
		}
		
		[Test]
		public void ns_alias_on_attribute()
		{
			RunCompilerTestCase(@"ns_alias_on_attribute.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/attributes";
		}
	}
}
