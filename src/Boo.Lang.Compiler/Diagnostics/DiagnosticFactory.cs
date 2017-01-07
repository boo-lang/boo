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

		public static Diagnostic Warning(int code, string msg, Node node, params object[] args)
		{
			var diag = new Diagnostic();
			diag.Level = DiagnosticLevel.Warning;
			diag.Message = msg;
			diag.Code = code;
			diag.Caret = node.LexicalInfo;
			diag.Arguments = args;
			return diag;
		}

		public static Diagnostic MemberNameConflict(Node node, IType declaringType, string memberName)
		{
			var diag = Error(89, "{0:type} already has a definition for {1:id}", node, declaringType, memberName);
			diag.Ranges = new Range[] { new Range(node.LexicalInfo, memberName.Length) };
			return diag;
		}

		public static Diagnostic MethodSignature(Node node, IEntity expected, string actualSignature)
		{
			var diag = Error(17, "no overload found for {0:id} with arguments {1}", node, expected, actualSignature);
			diag.Ranges = new Range[] { new Range(node) };
			return diag;
		}

		public static Diagnostic InvalidOperatorForType(UnaryExpression node, string opName, IType operand)
		{
			var diag = Error(50, "operator {0:id} cannot be used with {1:type}", node, opName, operand);
			diag.Ranges = new Range[] {
				new Range(node.Operand)
			};
			return diag;
		}

		public static Diagnostic InvalidOperatorForTypes(BinaryExpression node, string opName, IType lhs, IType rhs)
		{
			var diag = Error(51, "operator {0:id} cannot be used between {1:type} and {2:type}", node, opName, lhs, rhs);
			diag.Ranges = new Range[] {
				new Range(node.Left),
				new Range(node.Right)
			};
			return diag;
		}

		public static Diagnostic EqualsInsteadOfAssign(BinaryExpression node)
		{
			var diag = Warning(1007, "assignment inside a conditional", node);
			diag.Ranges = new Range[] {
				new Range(node.Left),
				new Range(node.Right)
			};
			diag.Hints = new Hint[] {
				new Hint(new SourceLocation(node.LexicalInfo.Line, node.LexicalInfo.Column + 1), "=")
			};
			return diag;
		}

		public static Diagnostic NamespaceNeverUsed(Import node)
		{
			var diag = Warning(1016, "namespace {0:id} is not used in this module", node.Expression, node.Expression.ToCodeString());
			diag.Ranges = new Range[] {
				new Range(node.Expression)
			};
			return diag;
		}

	}
}