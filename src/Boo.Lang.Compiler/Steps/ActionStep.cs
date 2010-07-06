using System;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// Promotes a closure to a compiler step.
	/// </summary>
	public class ActionStep : ICompilerStep
	{
		private readonly Action _action;

		public ActionStep(Action action) { _action = action; }

		public void Run() { _action(); }

		public void Initialize(CompilerContext context) { }

		public void Dispose() { }
	}
}