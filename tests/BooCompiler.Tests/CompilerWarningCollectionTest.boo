namespace BooCompiler.Tests

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class CompilerWarningCollectionTest:
	[Test]
	def TestSuppressWarning():
		warnings = CompilerWarningCollection()
		warnings.Adding += def (sender as object, args as CompilerWarningEventArgs):
			args.Cancel() if args.Warning.Code == "foo"
		warnings.Add(CompilerWarning(LexicalInfo.Empty, "foo", "foo"))
		Assert.AreEqual(0, warnings.Count)
		warnings.Add(CompilerWarning(LexicalInfo.Empty, "bar", "bar"))
		Assert.AreEqual(1, warnings.Count)
