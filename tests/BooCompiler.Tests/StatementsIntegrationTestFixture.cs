namespace BooCompiler.Tests	
{
	using NUnit.Framework;

	[TestFixture]
	public class StatementsIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void break_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\break-1.boo");
		}
		
		[Test]
		public void break_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\break-2.boo");
		}
		
		[Test]
		public void continue_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\continue-1.boo");
		}
		
		[Test]
		public void continue_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\continue-2.boo");
		}
		
		[Test]
		public void declaration_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\declaration-1.boo");
		}
		
		[Test]
		public void declaration_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\declaration-2.boo");
		}
		
		[Test]
		public void declaration_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\declaration-3.boo");
		}
		
		[Test]
		public void for_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-1.boo");
		}
		
		[Test]
		public void for_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-2.boo");
		}
		
		[Test]
		public void for_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-3.boo");
		}
		
		[Test]
		public void for_4()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-4.boo");
		}
		
		[Test]
		public void for_5()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-5.boo");
		}
		
		[Test]
		public void for_6()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-6.boo");
		}
		
		[Test]
		public void for_7()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-7.boo");
		}
		
		[Test]
		public void for_8()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-8.boo");
		}
		
		[Test]
		public void for_9()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-9.boo");
		}
		
		[Test]
		public void for_array_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-array-1.boo");
		}
		
		[Test]
		public void for_array_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\for-array-2.boo");
		}
		
		[Test]
		public void goto_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\goto-1.boo");
		}
		
		[Test]
		public void goto_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\goto-2.boo");
		}
		
		[Test]
		public void goto_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\goto-3.boo");
		}
		
		[Test]
		public void goto_4()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\goto-4.boo");
		}
		
		[Test]
		public void goto_5()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\goto-5.boo");
		}
		
		[Test]
		public void goto_6()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\goto-6.boo");
		}
		
		[Test]
		public void reraise_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\reraise-1.boo");
		}
		
		[Test]
		public void reraise_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\reraise-2.boo");
		}
		
		[Test]
		public void try_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\try-1.boo");
		}
		
		[Test]
		public void try_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\try-2.boo");
		}
		
		[Test]
		public void try_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\try-3.boo");
		}
		
		[Test]
		public void try_4()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\try-4.boo");
		}
		
		[Test]
		public void unpack_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-1.boo");
		}
		
		[Test]
		public void unpack_10()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-10.boo");
		}
		
		[Test]
		public void unpack_11()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-11.boo");
		}
		
		[Test]
		public void unpack_12()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-12.boo");
		}
		
		[Test]
		public void unpack_13()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-13.boo");
		}
		
		[Test]
		public void unpack_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-2.boo");
		}
		
		[Test]
		public void unpack_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-3.boo");
		}
		
		[Test]
		public void unpack_4()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-4.boo");
		}
		
		[Test]
		public void unpack_5()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-5.boo");
		}
		
		[Test]
		public void unpack_6()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-6.boo");
		}
		
		[Test]
		public void unpack_7()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-7.boo");
		}
		
		[Test]
		public void unpack_8()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-8.boo");
		}
		
		[Test]
		public void unpack_9()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\unpack-9.boo");
		}
		
		[Test]
		public void while_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-1.boo");
		}
		
		[Test]
		public void while_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-2.boo");
		}
		
		[Test]
		public void while_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-3.boo");
		}
		
		[Test]
		public void while_4()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-4.boo");
		}
		
		[Test]
		public void while_5()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-5.boo");
		}
		
		[Test]
		public void while_6()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-6.boo");
		}
		
		[Test]
		public void while_7()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-7.boo");
		}
		
		[Test]
		public void while_8()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\statements\while-8.boo");
		}
		
	}
}
