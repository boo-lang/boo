using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Diagnostics
{
	public static class DiagnosticFactory
	{	
		public static Diagnostic Error(int code, string msg, Node node, params object[] args)
		{
			var diag = new Diagnostic();
			diag.Level = DiagnosticLevel.Error;
			diag.Message = msg;
			diag.Code = code;
			diag.Caret = node.LexicalInfo;
			diag.Arguments = args;
			return diag;
		}

		public static Diagnostic MemberNameConflict(Node node, IType declaringType, string memberName)
		{
			var diag = Error(89, "{0:type} already has a definition for {1:ident}", node, declaringType, memberName);
			diag.Ranges = new Range[] { new Range(node.LexicalInfo, memberName.Length) };
			return diag;
		}
	}
}