using System;
using System.Xml.Serialization;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	/// <summary>
	/// Writes a xml representation of the AST to the console.
	/// </summary>
	public class XmlStep : ICompilerStep
	{
		CompilerContext _context;
		
		public XmlStep()
		{
		}
		
		public void Initialize(CompilerContext context)
		{
			_context = context;
		}
		
		public void Dispose()
		{
			_context = null;
		}

		public void Run()
		{
			CompileUnit cu = _context.CompileUnit;
			new XmlSerializer(cu.GetType()).Serialize(Console.Out, cu);
			Console.WriteLine();
		}
	}
}
