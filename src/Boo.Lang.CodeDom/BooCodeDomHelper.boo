namespace Boo.Lang.CodeDom

import System
import System.CodeDom
import System.Collections
import System.IO

// Original code converted from XSharpCodeDomHelper
// https://github.com/X-Sharp/XSharpPublic/blob/master/VisualStudio/ProjectPackage/CodeDomProvider/XSharpCodeDomHelper.cs

public static class BooCodeDomHelper:

	#region Dump Tools
	private indent = 0

	private writer as StringWriter

	
	private Indent as String:
		get:
			return string(char(' '), (indent * 3))

	
	private def WriteLineIndent(str as string):
		writer.WriteLine((Indent + str))

	
	private def WriteLine(str as string):
		writer.WriteLine(str)

	
	private def Write(str as string):
		writer.Write(str)


	public def FindFirstClass(ccu as CodeCompileUnit) as CodeTypeDeclaration:
		namespaceName as CodeNamespace
		return FindFirstClass(ccu, namespaceName)

	public def FindFirstClass(ccu as CodeCompileUnit, ref namespaceName as CodeNamespace) as CodeTypeDeclaration:
		namespaceName = null
		rstClass as CodeTypeDeclaration = null
		if ccu != null:
			for namespace2 as CodeNamespace in ccu.Namespaces:
				for declaration as CodeTypeDeclaration in namespace2.Types:
					//  The first Type == The first Class declaration
					if declaration.IsClass:
						namespaceName = namespace2
						rstClass = declaration
						break;
		return rstClass;

	public def DumpCodeCompileUnit(ccu as CodeCompileUnit) as string:
		writer = StringWriter()
		//
		Delimiter = String(char('-'), 5)
		Line = String(char('='), 25)
		indent = 0
		WriteLine(Line)
		//
		WriteLine(DateTime.Now.ToString())
		WriteLine(Delimiter)
		WriteLine('CodeCompileUnit UserData :')
		
		for value as DictionaryEntry in ccu.UserData:
		//
			WriteLine(((value.Key.ToString() + '  = ') + value.Value.ToString()))
		ctd as CodeTypeDeclaration = FindFirstClass(ccu)
		WriteLine(Line)
		Write('CodeTypeDeclaration : ')
		WriteLine(ctd.Name)
		WriteLine(Delimiter)
		//
		WriteLine('UserData')
		for value as DictionaryEntry in ctd.UserData:
			WriteLine(((value.Key.ToString() + '  = ') + value.Value.ToString()))
		for member as CodeTypeMember in ctd.Members:
		//
			WriteLine(Delimiter)
			Write('CodeTypeMember : ')
			WriteLine(member.Name)
			WriteLine(Delimiter)
			WriteLine('CodeTypeMember UserData :')
			for value as DictionaryEntry in member.UserData:
			//
				WriteLine(((value.Key.ToString() + '  = ') + value.Value.ToString()))
			WriteLine(Delimiter)
			if member isa CodeMemberField:
				cmf = (member cast CodeMemberField)
				Write(' -=> CodeMemberField : ')
				WriteLine(cmf.Type.BaseType)
				WriteLine(cmf.Name)
				WriteLine(Delimiter)
			elif member isa CodeMemberMethod:
				cmm = (member cast CodeMemberMethod)
				Write(' -=> CodeMemberMethod : ')
				WriteLine(cmm.Name)
				for stmt as CodeStatement in cmm.Statements:
					WriteLine(stmt.GetType().ToString())
					DumpStatement(writer, stmt)
				WriteLine(Delimiter)
		writer.Close()
		return writer.ToString()
	
	private def DumpStatement(writer as StringWriter, s as CodeStatement):
		indent += 1
		if s isa CodeAssignStatement:
			stmt = (s cast CodeAssignStatement)
			WriteLineIndent(stmt.Left.GetType().ToString())
			DumpExpression(stmt.Left)
			WriteLineIndent(stmt.Right.GetType().ToString())
			DumpExpression(stmt.Right)
		elif s isa CodeExpressionStatement:
			stmt__2 = (s cast CodeExpressionStatement)
			WriteLineIndent(stmt__2.Expression.GetType().ToString())
			DumpExpression(stmt__2.Expression)
		indent -= 1

	
	private def DumpExpression(e as CodeExpression):
		indent += 1
		if e isa CodeFieldReferenceExpression:
			exp = (e cast CodeFieldReferenceExpression)
			WriteLineIndent((exp.TargetObject.ToString() if exp.TargetObject else null))
		elif e isa CodeObjectCreateExpression:
			exp__2 = (e cast CodeObjectCreateExpression)
			WriteLineIndent(exp__2.CreateType.ToString())
		elif e isa CodeMethodInvokeExpression:
			exp__3 = (e cast CodeMethodInvokeExpression)
			WriteLineIndent((exp__3.Method.TargetObject.ToString() if exp__3.Method.TargetObject else null))
			WriteLineIndent(exp__3.Method.MethodName)
		elif e isa CodePropertyReferenceExpression:
			exp__4 = (e cast CodePropertyReferenceExpression)
			WriteLineIndent((exp__4.TargetObject.ToString() if exp__4.TargetObject else null))
			WriteLineIndent(exp__4.PropertyName)
		elif e isa CodePrimitiveExpression:
			exp__5 = (e cast CodePrimitiveExpression)
			WriteLineIndent(exp__5.ToString())
			WriteLineIndent(exp__5.Value.ToString())
		indent -= 1
	
	
	#endregion

