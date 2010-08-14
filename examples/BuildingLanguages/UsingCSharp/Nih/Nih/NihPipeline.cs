using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Nih
{
	public class NihPipeline : Boo.Lang.Compiler.Pipelines.CompileToMemory
	{
		public NihPipeline()
		{	
			Replace(typeof(Parsing), new NihParsingStep());
			InsertAfter(typeof(NihParsingStep), new AddRuntimeImport());
		}

		public class NihParsingStep : AbstractCompilerStep
		{
			public override void Run()
			{
				foreach (var input in Parameters.Input)
					using (var reader = input.Open())
						CompileUnit.Modules.Add(Parser.ParseModule(reader.ReadToEnd()));
			}
		}

		public class AddRuntimeImport : AbstractCompilerStep
		{
			public override void Run()
			{
				foreach (var module in CompileUnit.Modules)
					module.Imports.Add(new Import { Namespace = "Nih.Runtime" });
			}
		}
	}
}