namespace BooCompiler.Tests

import NUnit.Framework

partial class AsyncTestFixture:

	[Test]
	def @async_conformance_awaiting_indexer():
		RunCompilerTestCase("async-conformance-awaiting-indexer.boo")

	[Test]
	def @async_delegates():
		RunCompilerTestCase("async-delegates.boo")

	[Test]
	def @async_extension_add_method():
		RunCompilerTestCase("async-extension-add-method.boo")

	[Test]
	def @async_hello_world():
		RunCompilerTestCase("async-hello-world.boo")

	[Test]
	def @async_method_only_writes_to_enclosing_struct():
		RunCompilerTestCase("async-method-only-writes-to-enclosing-struct.boo")

	[Test]
	def @async_state_machine_struct_task_t():
		RunCompilerTestCase("async-state-machine-struct-task-t.boo")

	[Test]
	def @await_in_delegate_constructor():
		RunCompilerTestCase("await-in-delegate-constructor.boo")

	[Test]
	def @await_in_obj_initializer():
		RunCompilerTestCase("await-in-obj-initializer.boo")

	[Test]
	def @await_in_using_and_for():
		RunCompilerTestCase("await-in-using-and-for.boo")

	[Test]
	def @await_switch():
		RunCompilerTestCase("await-switch.boo")

	[Test]
	def @await_void():
		RunCompilerTestCase("await-void.boo")

	[Ignore("Requires better closure signature inferring")][Test]
	def @better_conversion_from_async_lambda():
		RunCompilerTestCase("better-conversion-from-async-lambda.boo")

	[Test]
	def @conformance_awaiting_methods_accessible():
		RunCompilerTestCase("conformance-awaiting-methods-accessible.boo")

	[Test]
	def @conformance_awaiting_methods_generic():
		RunCompilerTestCase("conformance-awaiting-methods-generic.boo")

	[Ignore("This will fail until Run and RunEx are merged back together")][Test]
	def @conformance_awaiting_methods_method():
		RunCompilerTestCase("conformance-awaiting-methods-method.boo")

	[Test]
	def @conformance_awaiting_methods_method02():
		RunCompilerTestCase("conformance-awaiting-methods-method02.boo")

	[Test]
	def @conformance_awaiting_methods_parameter():
		RunCompilerTestCase("conformance-awaiting-methods-parameter.boo")

	[Test]
	def @conformance_exceptions_async_await_names():
		RunCompilerTestCase("conformance-exceptions-async-await-names.boo")

	[Ignore("Requires better closure signature inferring")][Test]
	def @conformance_overload_resolution_class_generic_regular_method():
		RunCompilerTestCase("conformance-overload-resolution-class-generic-regular-method.boo")

	[Test]
	def @cs_bug_602246():
		RunCompilerTestCase("cs-bug-602246.boo")

	[Test]
	def @cs_bug_748527():
		RunCompilerTestCase("cs-bug-748527.boo")

	[Test]
	def @delegate_async():
		RunCompilerTestCase("delegate-async.boo")

	[Test]
	def @generic_async_lambda():
		RunCompilerTestCase("generic-async-lambda.boo")

	[Test]
	def @generic_task_returning_async():
		RunCompilerTestCase("generic-task-returning-async.boo")

	[Test]
	def @generic():
		RunCompilerTestCase("generic.boo")

	[Test]
	def @hoist_structure():
		RunCompilerTestCase("hoist-structure.boo")

	[Test]
	def @hoist_using_1():
		RunCompilerTestCase("hoist-using-1.boo")

	[Test]
	def @hoist_using_2():
		RunCompilerTestCase("hoist-using-2.boo")

	[Test]
	def @hoist_using_3():
		RunCompilerTestCase("hoist-using-3.boo")

	[Test]
	def @infer_from_async_lambda():
		RunCompilerTestCase("infer-from-async-lambda.boo")

	[Test]
	def @inference():
		RunCompilerTestCase("inference.boo")

	[Test]
	def @init():
		RunCompilerTestCase("init.boo")

	[Test]
	def @is_and_as_operators():
		RunCompilerTestCase("is-and-as-operators.boo")

	[Test]
	def @mutating_array_of_structs():
		RunCompilerTestCase("mutating-array-of-structs.boo")

	[Test]
	def @mutating_struct_with_using():
		RunCompilerTestCase("mutating-struct-with-using.boo")

	[Test]
	def @my_task_2():
		RunCompilerTestCase("my-task-2.boo")

	[Test]
	def @my_task():
		RunCompilerTestCase("my-task.boo")

	[Test]
	def @premature_null():
		RunCompilerTestCase("premature-null.boo")

	[Test]
	def @property():
		RunCompilerTestCase("property.boo")

	[Test]
	def @struct_async():
		RunCompilerTestCase("struct-async.boo")

	[Test]
	def @switch_on_awaited_value_async():
		RunCompilerTestCase("switch-on-awaited-value-async.boo")

	[Test]
	def @task_returning_async():
		RunCompilerTestCase("task-returning-async.boo")

	[Test]
	def @void_returning_async():
		RunCompilerTestCase("void-returning-async.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "async"
