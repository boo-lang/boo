using System;

namespace Boo.Lang.Compiler.Steps
{
	public class MacroAndAttributeExpansion : AbstractCompilerStep
	{
		private BindAndApplyAttributes _attributes = new BindAndApplyAttributes();
		private ExpandMacros _macros = new ExpandMacros();

		public override void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_attributes.Initialize(context);
			_macros.Initialize(context);
		}

		public override void Run()
		{
			int iteration = 0;
			while (iteration < Parameters.MaxAttributeSteps)
			{
				bool attributesApplied = _attributes.BindAndApply();
				bool macrosExpanded = _macros.ExpandAll();
				if (!attributesApplied && !macrosExpanded)
				{
					break;
				}
				++iteration;
			}
		}
	}
}
