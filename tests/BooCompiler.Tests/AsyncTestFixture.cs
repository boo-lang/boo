// Test suite ported from Roslyn tests found at
// https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Test/Emit/CodeGen/CodeGenAsyncTests.cs

namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
    public class AsyncTestFixture : AbstractCompilerTestCase
    {
		[Test]
		public void async_conformance_awaiting_indexer()
		{
			RunCompilerTestCase(@"async-conformance-awaiting-indexer.boo");
		}

        [Test]
		public void async_delegates()
		{
			RunCompilerTestCase(@"async-delegates.boo");
		}
		
        [Test]
		public void async_extension_add_method()
		{
			RunCompilerTestCase(@"async-extension-add-method.boo");
		}
		
        [Test]
		public void async_hello_world()
		{
			RunCompilerTestCase(@"async-hello-world.boo");
		}
		
        [Test]
		public void async_method_only_writes_to_enclosing_struct()
		{
			RunCompilerTestCase(@"async-method-only-writes-to-enclosing-struct.boo");
		}
		
        [Test]
		public void async_state_machine_struct_task_t()
		{
			RunCompilerTestCase(@"async-state-machine-struct-task-t.boo");
		}
		
        [Test]
		public void await_in_delegate_constructor()
		{
			RunCompilerTestCase(@"await-in-delegate-constructor.boo");
		}
		
        [Test]
		public void await_in_obj_initializer()
		{
			RunCompilerTestCase(@"await-in-obj-initializer.boo");
		}
		
        [Test]
		public void await_in_using_and_for()
		{
			RunCompilerTestCase(@"await-in-using-and-for.boo");
		}
		
        [Test]
		public void await_switch()
		{
			RunCompilerTestCase(@"await-switch.boo");
		}
		
        [Test]
		public void await_void()
		{
			RunCompilerTestCase(@"await-void.boo");
		}
		
        [Test] [Ignore("Requires better closure signature inferring")]
		public void better_conversion_from_async_lambda()
		{
			RunCompilerTestCase(@"better-conversion-from-async-lambda.boo");
		}
		
        [Test]
		public void conformance_awaiting_methods_accessible()
		{
			RunCompilerTestCase(@"conformance-awaiting-methods-accessible.boo");
		}
		
        [Test]
		public void conformance_awaiting_methods_generic()
		{
			RunCompilerTestCase(@"conformance-awaiting-methods-generic.boo");
		}

		[Test][Ignore("This will fail until Run and RunEx are merged back together")]
		public void conformance_awaiting_methods_method()
		{
			RunCompilerTestCase(@"conformance-awaiting-methods-method.boo");
		}
		
        [Test]
		public void conformance_awaiting_methods_method02()
		{
			RunCompilerTestCase(@"conformance-awaiting-methods-method02.boo");
		}
		
        [Test]
		public void conformance_awaiting_methods_parameter()
		{
			RunCompilerTestCase(@"conformance-awaiting-methods-parameter.boo");
		}
		
        [Test]
		public void conformance_exceptions_async_await_names()
		{
			RunCompilerTestCase(@"conformance-exceptions-async-await-names.boo");
		
        }

		[Test][Ignore("Requires better closure signature inferring")]
		public void conformance_overload_resolution_class_generic_regular_method()
		{
			RunCompilerTestCase(@"conformance-overload-resolution-class-generic-regular-method.boo");
		}
		
        [Test]
		public void cs_bug_602246()
		{
			RunCompilerTestCase(@"cs-bug-602246.boo");
		}
		
        [Test]
		public void cs_bug_748527()
		{
			RunCompilerTestCase(@"cs-bug-748527.boo");
		}
		
        [Test]
		public void delegate_async()
		{
			RunCompilerTestCase(@"delegate-async.boo");
		}
		
        [Test]
		public void generic_async_lambda()
		{
			RunCompilerTestCase(@"generic-async-lambda.boo");
		}

        [Test]
		public void generic_task_returning_async()
		{
			RunCompilerTestCase(@"generic-task-returning-async.boo");
		}
		
        [Test]
		public void generic()
		{
			RunCompilerTestCase(@"generic.boo");
		}
		
        [Test]
		public void hoist_structure()
		{
			RunCompilerTestCase(@"hoist-structure.boo");
		}
		
        [Test]
		public void hoist_using_1()
		{
			RunCompilerTestCase(@"hoist-using-1.boo");
		}
		
        [Test]
		public void hoist_using_2()
		{
			RunCompilerTestCase(@"hoist-using-2.boo");
		}
		
        [Test]
		public void hoist_using_3()
		{
			RunCompilerTestCase(@"hoist-using-3.boo");
		}
		
        [Test]
		public void infer_from_async_lambda()
		{
			RunCompilerTestCase(@"infer-from-async-lambda.boo");
		}
		
        [Test]
		public void inference()
		{
			RunCompilerTestCase(@"inference.boo");
		}
		
        [Test]
		public void init()
		{
			RunCompilerTestCase(@"init.boo");
		}
		
        [Test]
		public void is_and_as_operators()
		{
			RunCompilerTestCase(@"is-and-as-operators.boo");
		}
		
        [Test]
		public void mutating_array_of_structs()
		{
			RunCompilerTestCase(@"mutating-array-of-structs.boo");
		}
		
        [Test]
		public void mutating_struct_with_using()
		{
			RunCompilerTestCase(@"mutating-struct-with-using.boo");
		}
		
        [Test]
		public void my_task_2()
		{
			RunCompilerTestCase(@"my-task-2.boo");
		}
		
        [Test]
		public void my_task()
		{
			RunCompilerTestCase(@"my-task.boo");
		}
		
        [Test]
		public void premature_null()
		{
			RunCompilerTestCase(@"premature-null.boo");
		}
		
        [Test]
		public void property()
		{
			RunCompilerTestCase(@"property.boo");
		}
		
        [Test]
		public void struct_async()
		{
            RunCompilerTestCase(@"struct-async.boo");
		}
		
        [Test]
		public void switch_on_awaited_value_async()
		{
			RunCompilerTestCase(@"switch-on-awaited-value-async.boo");
		}
		
        [Test]
		public void task_returning_async()
		{
			RunCompilerTestCase(@"task-returning-async.boo");
		}

		[Test]
		public void void_returning_async()
		{
			RunCompilerTestCase(@"void-returning-async.boo");
		}

        protected override string GetRelativeTestCasesPath()
        {
            return "async";
        }
    }
}