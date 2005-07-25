import System
import System.IO
import System.Text.RegularExpressions
import Useful.IO from Boo.Lang.Useful
import ICSharpCode.SharpRefactory.Parser from "ICSharpCode.SharpRefactory"
import ICSharpCode.SharpRefactory.Parser.AST
import ICSharpCode.SharpRefactory.PrettyPrinter

class PascalCasePrinter(PrettyPrintVisitor):
	def constructor(originalFileName as string):
		super(originalFileName)
		
	override def Visit(method as MethodDeclaration, data):
		method.Name = ToPascalCase(method.Name)
		return super(method, data)
		
	override def Visit(invocation as InvocationExpression, data):
		memberRef = invocation.TargetObject as FieldReferenceExpression
		if memberRef is not null:
			memberRef.FieldName = ToPascalCase(memberRef.FieldName)
		else:
			identifier = invocation.TargetObject as IdentifierExpression
			if identifier is not null:
				identifier.Identifier = ToPascalCase(identifier.Identifier)
		return super(invocation, data)
		
	def ToPascalCase(name as string):
		return name[:1].ToUpper() + name[1:]
		
def preprocess(code as string):
	pp = PreProcessor()
	pp.Define("foo")
	return pp.Process(code)

code = """
class YapFoo {
#if foo
	// a comment
	public void bar() {
	}
#endif

	public string Prop	{ get { return null; } }
	public void baz() {
		     this.bar();
		bar();
	}
	
#if bang
	void bang() {
	}
#endif
}
"""

p = Parser()
p.Parse(Lexer(StringReader(preprocess(code))))

printer = PascalCasePrinter("code.cs")
options = printer.PrettyPrintOptions
options.MethodBraceStyle = BraceStyle.NextLine
printer.Visit(p.compilationUnit, null)

print printer.Text
