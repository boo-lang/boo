namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
partial class GenericsTestFixture(AbstractCompilerTestCase):
	override protected def RunCompilerTestCase(name as string):
		Assert.Ignore("Test requires .net 2.") if System.Environment.Version.Major < 2
		resolver as System.ResolveEventHandler = InstallAssemblyResolver(BaseTestCasesPath)
		try:
			super.RunCompilerTestCase(name)
		ensure:
			RemoveAssemblyResolver(resolver)

	override protected def CopyDependencies():
		CopyAssembliesFromTestCasePath()
