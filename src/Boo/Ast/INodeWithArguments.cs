using System;

namespace Boo.Ast
{
	/// <summary>
	/// Uma AST para nós que armazenam parâmetros como atributos e
	/// invocações de método.
	/// </summary>
	public interface INodeWithArguments
	{
		ExpressionCollection Arguments
		{
			get;
		}

		ExpressionPairCollection NamedArguments
		{
			get;
		}
	}
}
