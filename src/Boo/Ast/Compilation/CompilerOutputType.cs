using System.Reflection.Emit;

namespace Boo.Ast.Compilation
{
	public enum CompilerOutputType
	{
		Library = PEFileKinds.Dll,
		ConsoleApplication = PEFileKinds.ConsoleApplication,
		WindowsApplication = PEFileKinds.WindowApplication
	}
}
