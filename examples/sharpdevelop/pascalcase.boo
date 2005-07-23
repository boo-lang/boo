import System
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

code = """
class YapFoo {
	public void bar() {
	}
	
	public string Prop	{ get { return null; } }
	public void baz() {
		     this.bar();
		bar();
	}
}
"""

p = Parser()
p.Parse(Lexer(StringReader(code)))

printer = PascalCasePrinter("foo.cs")
options = printer.PrettyPrintOptions
options.MethodBraceStyle = BraceStyle.NextLine
printer.Visit(p.compilationUnit, null)

print printer.Text
