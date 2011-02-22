using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace booc.Tests
{
	[TestFixture]
	public class CommandLineParserTest
	{
		[Test]
		public void LibCanBeWrappedInDoubleQuotes()
		{
			var libPath = Path.GetTempPath();
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"""-lib:{0}""", libPath));
			Assert.AreEqual(libPath, compilerParameters.LibPaths[0]);
		}

		[Test]
		public void LibValueCanBeWrappedInDoubleQuotes()
		{
			var libPath = Path.GetTempPath();
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"-lib:""{0}""", libPath));
			Assert.AreEqual(libPath, compilerParameters.LibPaths[0]);
		}

		[Test]
		public void WorkaroundForNAntBugCausingAdditionalDoubleQuoteSuffixOnLibValue()
		{
			var libPath = Path.GetTempPath();
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"-lib:{0}""", libPath));
			Assert.AreEqual(libPath, compilerParameters.LibPaths[0]);
		}

		[Test]
		public void SingleQuotesCanBeUsedAroundFileNames()
		{
			var fileName = "foo.boo";
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"'{0}'", fileName));
			Assert.AreEqual(fileName, compilerParameters.Input[0].Name);
		}

		[Test]
		public void CustomTypeInferenceRuleAttribute()
		{
			var customAttributeName = typeof(CustomAttribute).FullName.Replace('+', '.');
			
			var parameters = new CompilerParameters();
			parameters.References.Add(typeof(CustomAttribute).Assembly);

			CommandLineParser.ParseInto(parameters, "-x-type-inference-rule-attribute:" + customAttributeName);
			ActiveEnvironment.With(new CompilerContext(parameters).Environment, () =>
			{
				var m = Methods.Of<object>(MethodWithCustomTypeInferenceRule);
				Assert.AreEqual("custom", My<TypeInferenceRuleProvider>.Instance.TypeInferenceRuleFor(m));	
			});
		}

		[CustomAttribute]
		public static object MethodWithCustomTypeInferenceRule()
		{
			return null; 
		}

		public class CustomAttribute : System.Attribute
		{
			override public string ToString()
			{
				return "custom";
			}
		}
	}
}
