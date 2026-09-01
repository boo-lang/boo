namespace Boo.Lang.Runtime.Tests

import Boo.Lang.Runtime
import NUnit.Framework

abstract class AbstractDispatcherFactoryTestCase:
	protected _extensions as ExtensionRegistry

	[SetUp]
	def SetUp():
		_extensions = ExtensionRegistry()
		_extensions.Register(FooExtensions)
