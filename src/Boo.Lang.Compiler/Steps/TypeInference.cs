using System;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// Marker step for code that depends to run after type inference has been completed.
	/// 
	/// <example>
	/// my(CompilerPipeline).InsertAfter(TypeInference, ActionStep({ PerformTypeDependentOperation() })
	/// </example>
	/// 
	/// <example>
	/// my(CompilerPipeline).AfterStep += def (sender, args as CompilerStepEventArgs):
	///     if args.Step isa TypeInference: PerformTypeDependentOperation()
	/// </example>
	/// 
	/// </summary>
	public class TypeInference : ICompilerStep
	{
		void IDisposable.Dispose()
		{
		}

		void ICompilerComponent.Initialize(CompilerContext context)
		{
		}

		void ICompilerStep.Run()
		{
		}
	}
}
