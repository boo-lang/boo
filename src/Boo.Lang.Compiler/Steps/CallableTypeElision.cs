using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	class CallableTypeElision : AbstractFastVisitorCompilerStep
	{
		public override void Run()
		{
			if (!TypeSystemServices.CompilerGeneratedTypesModuleExists())
				return;

			var cgm = TypeSystemServices.GetCompilerGeneratedTypesModule();
			var callableFinder = new TypeFinder(new TypeCollector(type => type.ParentNamespace == cgm.Entity));
			foreach (var module in CompileUnit.Modules)
			{
				if (module != cgm)
					module.Accept(callableFinder);
			}

			var foundSet = new HashSet<IType>(callableFinder.Results);
			var count = 0;
			while (foundSet.Count > count)
			{
				count = foundSet.Count;
				var sweeper = new TypeFinder(new TypeCollector(type => foundSet.Contains(type)));
				cgm.Accept(sweeper);
				foreach (var swept in sweeper.Results)
					foundSet.Add(swept);
			}

			var rejects = cgm.Members
				.Cast<TypeDefinition>()
				.Where(td => !td.Name.Contains("$adaptor$") && !foundSet.Contains(td.Entity));
			foreach (var type in rejects)
			{
				cgm.Members.Remove(type);
			}
		}
	}
}
