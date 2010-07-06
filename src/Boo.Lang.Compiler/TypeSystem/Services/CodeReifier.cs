using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	/// <summary>
	/// Implemented by compiler steps that need to take part in type member
	/// reification.
	/// </summary>
	public interface ITypeMemberReifier
	{
		void Reify(TypeMember member);
	}

	/// <summary>
	/// Reifies ast nodes.
	/// 
	/// Reification of an ast node means compiling it up to the point of being ready for inclusion into <see cref="CompilerContext.CompileUnit"/>.
	/// 
	/// This allows code generated through the AST api or code literals to be added to the current CompileUnit without having to go through
	/// bureaucratic <see cref="CompilerContext.CodeBuilder"/> calls.
	/// </summary>
	public class CodeReifier : AbstractCompilerComponent
	{
		public void ReifyInto(TypeDefinition parentType, TypeMember member)
		{	
			if (member == null)
				throw new ArgumentNullException("member");
			if (parentType == null)
				throw new ArgumentNullException("parentType");
			if (member.ParentNode != null)
				throw new ArgumentException("ParentNode must be null for member to be reified.", "member");
			
			parentType.Members.Add(member);

			var pipeline = Parameters.Pipeline;
			var currentStep = pipeline.CurrentStep;
			foreach (var step in pipeline)
			{
				if (step == currentStep)
					break;

				var typeMemberReification = step as ITypeMemberReifier;
				if (typeMemberReification == null)
					continue;

				typeMemberReification.Reify(member);
			}
		}
	}
}
