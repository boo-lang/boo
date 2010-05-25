using Boo.Lang.Compiler.Ast;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
    public class CompilerErrorEmitter
    {
    	public void GenericArgumentsCountMismatch(Node anchor, IType type)
        {
            CompilerErrors.Add(CompilerErrorFactory.GenericDefinitionArgumentCount(anchor, type.FullName, type.GenericInfo.GenericParameters.Length));
        }

    	private static CompilerErrorCollection CompilerErrors
    	{
    		get { return My<CompilerErrorCollection>.Instance; }
    	}
    }
}
