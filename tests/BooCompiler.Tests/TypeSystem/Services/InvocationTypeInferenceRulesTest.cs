using System;
using Boo.Lang;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
	[TestFixture]
	public class InvocationTypeInferenceRulesTest : AbstractTypeSystemTest
	{
		[Test]
		public void TypeReferencedByFirstArgumentRule()
		{
			RunInCompilerContextEnvironment(()=>
			{
				var type = TypeSystemServices.Map(typeof(string));
				var method = TypeSystemServices.Map(Methods.Of<Type, object>(Create));
				var invocation = CodeBuilder.CreateMethodInvocation(method, CodeBuilder.CreateReference(type));

				Assert.AreSame(type, Subject.ApplyTo(invocation, method));
			});
		}

		[Test]
		public void TypeOfFirstArgumentRule()
		{
			RunInCompilerContextEnvironment(() =>
			{
				var type = TypeSystemServices.Map(typeof(string));
				var method = TypeSystemServices.Map(Methods.Of<object, object>(Instantiate));
				var invocation = CodeBuilder.CreateMethodInvocation(method, CodeBuilder.CreateStringLiteral("foo"));

				Assert.AreSame(type, Subject.ApplyTo(invocation, method));
			});
		}

		[Test]
		public void MisspelledRuleNameCausesWarning()
		{
			RunInCompilerContextEnvironment(() =>
			{
				var method = TypeSystemServices.Map(Methods.Of<object>(MethodWithMisspelledRule));
				var invocation = CodeBuilder.CreateMethodInvocation(method);

				Assert.IsNull(Subject.ApplyTo(invocation, method));

				var warnings = My<CompilerWarningCollection>.Instance;
				Assert.AreEqual(1, warnings.Count);

				var message = warnings[0].Message;
				Assert.IsTrue(message.Contains("UnknownRule"));
				Assert.IsTrue(message.Contains("MethodWithMisspelledRule"));
			});
		}

		private static InvocationTypeInferenceRules Subject
		{
			get { return My<InvocationTypeInferenceRules>.Instance; }
		}

		[TypeInferenceRule("TypeOfFirstArgument")]
		private static object Instantiate(object prototype) { return null;  }

		[TypeInferenceRule("TypeReferencedByFirstArgument")]
		private static object Create(Type type) { return null; }

		[TypeInferenceRule("UnknownRule")]
		private static object MethodWithMisspelledRule() { return null; }
	}
}
