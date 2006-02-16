#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

// authors:
// Arron Washington
// Ian MacLean (original C# version)

//See also:
//http://svn.myrealbox.com/viewcvs/trunk/mcs/class/System/Microsoft.CSharp/CSharpCodeGenerator.cs?view=auto
//http://svn.myrealbox.com/viewcvs/trunk/mcs/class/System/Microsoft.VisualBasic/VBCodeGenerator.cs?view=auto

namespace Boo.Lang.CodeDom

import System
import System.CodeDom
import System.CodeDom.Compiler
import System.IO
import System.Reflection
import System.Collections
import System.Text.RegularExpressions

class BooCodeGenerator(CodeGenerator):
	static primitives = { "System.Double" : "double",
						"System.Single" : "single",
						"System.Int32" : "int",
						"System.Int64" : "long",
						"System.Int16" : "short",
						"System.UInt16" : "ushort",
						"System.Boolean" : "bool",
						"System.Char" : "char",
						"System.String" : "string",
						"System.Object" : "object",
						"System.Void" : "void"}
						
	protected NullToken as string:
		get:
			return "null"	

	protected override def OutputType(typeRef as CodeTypeReference):
		Output.Write(GetTypeOutput(typeRef))

	protected override def GenerateArrayCreateExpression(exp as CodeArrayCreateExpression) :
		if exp.Initializers:
			Output.Write("(")
			if exp.CreateType:
				Output.Write("of ")
				OutputType(exp.CreateType)
				Output.Write(": ")
			OutputExpressionList(exp.Initializers)
			if exp.Initializers.Count == 1:
				Output.Write(",")
			Output.Write(")")
		else:
			Output.Write("array(")
			OutputType(exp.CreateType)
			Output.Write(",")
			if exp.SizeExpression:
				GenerateExpression(exp)
			else:
				Output.Write(exp.Size)
			Output.Write(")")
		
	protected override def GenerateBaseReferenceExpression(e as CodeBaseReferenceExpression) :
		Output.Write("super")

	protected override def GenerateCastExpression(e as CodeCastExpression) :
		Output.Write("cast(")
		OutputType(e.TargetType)
		Output.Write(",")
		GenerateExpression(e.Expression)
		Output.Write(")")

	protected override def GenerateDelegateCreateExpression(e as CodeDelegateCreateExpression) :
		GenerateExpression(e.TargetObject)
		Output.Write(".${e.MethodName} as ")
		OutputType(e.DelegateType)
		
	protected override def GenerateFieldReferenceExpression(e as CodeFieldReferenceExpression):		
		MemberReference(e.TargetObject, e.FieldName)
	
	protected override def GenerateArgumentReferenceExpression(e as CodeArgumentReferenceExpression) :
		Output.Write(e.ParameterName)		
	
	protected override def GenerateVariableReferenceExpression(e as CodeVariableReferenceExpression) :
		Output.Write(e.VariableName)
	
	protected override def GenerateIndexerExpression(e as CodeIndexerExpression) :
		Indexer(e.TargetObject, e.Indices)
	
	protected override def GenerateArrayIndexerExpression(e as CodeArrayIndexerExpression) :
		Indexer(e.TargetObject, e.Indices)
	
	protected override def GenerateMethodInvokeExpression(e as CodeMethodInvokeExpression) :
		Invocation(e.Method, e.Parameters)

	protected override def GenerateMethodReferenceExpression(e as CodeMethodReferenceExpression) :
		MemberReference(e.TargetObject, e.MethodName)		
	protected override def GenerateEventReferenceExpression(e as CodeEventReferenceExpression) :
		MemberReference(e.TargetObject, e.EventName)

	protected override def GenerateDelegateInvokeExpression(e as CodeDelegateInvokeExpression) :
		Invocation(e.TargetObject, e.Parameters)
		
	protected override def GenerateObjectCreateExpression(e as CodeObjectCreateExpression) :
		OutputType(e.CreateType)
		Output.Write("(")
		OutputExpressionList(e.Parameters)
		Output.Write(")")

	protected override def GeneratePropertyReferenceExpression(e as CodePropertyReferenceExpression) :
		MemberReference(e.TargetObject, e.PropertyName)

	protected override def GeneratePropertySetValueReferenceExpression(e as CodePropertySetValueReferenceExpression) :
		Output.Write("value")

	protected override def GenerateThisReferenceExpression(e as CodeThisReferenceExpression) :
		Output.Write("self")

	protected override def GenerateExpressionStatement(e as CodeExpressionStatement) :
		GenerateExpression(e.Expression)
		Output.WriteLine();

	protected override def GenerateIterationStatement(e as CodeIterationStatement) :
		GenerateStatement(e.InitStatement)
		Output.Write("while ")
		GenerateExpression(e.TestExpression)
		Output.Write(":")
		Indent++
		GenerateStatements(e.Statements)
		GenerateStatement(e.IncrementStatement)
		Indent--

	protected override def GenerateThrowExceptionStatement(e as CodeThrowExceptionStatement) :
		Output.Write("raise ")
		GenerateExpression(e.ToThrow)

	protected override def GenerateComment(comment as CodeComment) :
		//TODO: check comment.DocComment when boo doc format is available
		commentSymbol = "#"
		txt = comment.Text
		
		Output.Write(commentSymbol + " ")
		
		i = 0
		while i<len(txt):
			rawArrayIndexing:
				Output.Write(txt[i])
				if txt[i] == char('\r'):
					if i<len(txt)-1 and txt[i+1]==char('\n'):
						continue
					Output.Write(commentSymbol)
				elif txt[i] == char('\n'):
					Output.Write(commentSymbol)
			++i
		Output.WriteLine()

	protected override def GenerateMethodReturnStatement(e as CodeMethodReturnStatement) :
		Output.Write("return ")
		GenerateExpression(e.Expression)
		Output.WriteLine()

	protected override def GenerateConditionStatement(e as CodeConditionStatement) :
		Output.Write("if ")
		GenerateExpression(e.Condition)
		Output.WriteLine(":")
		Indent++
		GenerateStatements(e.TrueStatements)
		Output.WriteLine ("pass") if len(e.TrueStatements) == 0
		Indent--
		return unless e.FalseStatements.IsValid()
		Output.WriteLine("else:")
		Indent++
		GenerateStatements(e.FalseStatements)
		Output.WriteLine ("pass") if len(e.TrueStatements) == 0
		Indent--

	protected override def GenerateTryCatchFinallyStatement(e as CodeTryCatchFinallyStatement) :
		Output.Write("try:")
		Indent++
		GenerateStatements(e.TryStatements)
		Indent--
		for exp as CodeCatchClause in e.CatchClauses:
			Output.Write("except ")
			Output.Write("${exp.LocalName} as ")
			OutputType(exp.CatchExceptionType)
			Output.Write(":")
			Indent++
			GenerateStatements(exp.Statements)
			Indent--
		GenerateStatements(e.FinallyStatements)
		

	protected override def GenerateAssignStatement(e as CodeAssignStatement) :
		GenerateExpression(e.Left)
		Output.Write(" = ")
		GenerateExpression(e.Right)
		Output.WriteLine()
	protected override def GenerateAttachEventStatement(e as CodeAttachEventStatement) :
		GenerateExpression(e.Event)
		Output.Write(" += ")
		GenerateExpression(e.Listener)
		Output.WriteLine()
	protected override def GenerateRemoveEventStatement(e as CodeRemoveEventStatement) :
		GenerateExpression(e.Event)
		Output.Write(" -= ")
		GenerateExpression(e.Listener)
		Output.WriteLine()
	protected override def GenerateGotoStatement(e as CodeGotoStatement) :
		Output.WriteLine("goto ${e.Label}")		
	protected override def GenerateLabeledStatement(e as CodeLabeledStatement) :
		Output.WriteLine(":${e.Label}")
		GenerateStatement(e.Statement) if e.Statement		
	protected override def GenerateVariableDeclarationStatement(e as CodeVariableDeclarationStatement) :		
		Output.Write("${e.Name} as ")
		OutputType(e.Type)
		if e.InitExpression:
			Output.Write(" = ")
			GenerateExpression(e.InitExpression)
		Output.WriteLine()
	protected override def GenerateLinePragmaStart(e as CodeLinePragma) :
		pass

	protected override def GenerateLinePragmaEnd(e as CodeLinePragma) :
		pass	

	protected override def GenerateEvent(e as CodeMemberEvent, c as CodeTypeDeclaration) :
		
		if e.PrivateImplementationType:
			raise "Does not support private implementation type."
		if e.ImplementationTypes.IsValid():			
			raise "Does not implement these types: ${join(e.ImplementationTypes)}"
		e.Attributes |= MemberAttributes.Final
		
		ModifiersAndAttributes(e)
		
		Output.Write("event ${e.Name} as ")
		OutputType(e.Type)
		Output.WriteLine()
	protected override def GenerateField(e as CodeMemberField) :
		
		ModifiersAndAttributes(e)
		Output.Write("${e.Name} as ")
		OutputType(e.Type)
		if e.InitExpression:
			Output.Write(" = ")
			GenerateExpression(e.InitExpression)		
		Output.WriteLine()
		
	protected override def GenerateSnippetMember(e as CodeSnippetTypeMember) :
		OutputAttributeDeclarations(e.CustomAttributes) if e.CustomAttributes		
		Output.Write(FixIndent(e.Text, Options.IndentString, Indent, false))
	
	protected override def GenerateSnippetExpression(e as CodeSnippetExpression) :
		//Output.Write("("+e.Value+")")
		Output.Write(e.Value)
		
	protected override def GenerateSnippetCompileUnit(e as CodeSnippetCompileUnit) :
		Output.Write(FixIndent(e.Value, Options.IndentString, Indent, false))
		
	protected override def GenerateSnippetStatement(e as CodeSnippetStatement) :
		Output.Write(FixIndent(e.Value, Options.IndentString, Indent, false))
		
	protected override def GenerateEntryPointMethod(e as CodeEntryPointMethod, c as CodeTypeDeclaration) :
		Method(e, "Main")
		
	protected override def GenerateMethod(e as CodeMemberMethod, c as CodeTypeDeclaration) :
		Method(e, e.Name)
		
	protected override def GenerateProperty(e as CodeMemberProperty, c as CodeTypeDeclaration) :		
		ModifiersAndAttributes(e)
		
		Output.Write(" ${e.Name} ")
		if len(e.Parameters) > 1:
			Output.Write("(")
			OutputParameters(e.Parameters)
			Output.Write(")")
		Output.Write(" as ")		
		OutputType(e.Type)
		Output.WriteLine(":")		
		Indent++
		if e.HasGet:
			Output.WriteLine("get:")
			Indent++
			GenerateStatements(e.GetStatements)
			passcheck(e.GetStatements)
			Indent--
			Output.WriteLine()
		if e.HasSet:
			Output.WriteLine("set:")
			Indent++
			GenerateStatements(e.SetStatements)
			passcheck(e.SetStatements)
			Indent--
			Output.WriteLine()
		Indent--
		Output.WriteLine()
	protected override def GenerateConstructor(e as CodeConstructor, c as CodeTypeDeclaration) :
		Method(e, "constructor")
		

	protected override def GenerateTypeConstructor(e as CodeTypeConstructor) :		
		e.Attributes |= MemberAttributes.Static
		e.Parameters.Clear()
		Method(e, "constructor")

	protected override def GenerateTypeStart(e as CodeTypeDeclaration):		
		Output.WriteLine()
		OutputAttributeDeclarations(e.CustomAttributes)
		Output.WriteLine()
		if e.TypeAttributes & TypeAttributes.Sealed:
			e.TypeAttributes = (e.TypeAttributes.ToInt() & ~TypeAttributes.Sealed.ToInt()).ToEnum(TypeAttributes)
			Output.Write("final ")
		if e isa CodeTypeDelegate:		
			GenerateDelegate(e)
			return
		OutputTypeAttributes(e.TypeAttributes, e.IsStruct, e.IsEnum)
		Output.Write(" ${e.Name}(")
		for index in range(len(e.BaseTypes)):
			var as CodeTypeReference = e.BaseTypes[index]
			OutputType(var)
			continue unless index + 1 != e.BaseTypes.Count
			Output.Write(",")
		Output.Write("):")
		Indent++		
		#Ah hah hah, yeah, no. Empty class.
		passcheck(e.Members)

		
	private def GenerateDelegate(e as CodeTypeDelegate):
		if e.TypeAttributes & TypeAttributes.NestedFamORAssem == TypeAttributes.Public:
			Output.Write("public ")
		Output.Write("callable ${e.Name}(")
		OutputParameters(e.Parameters)
		Output.Write(") as ")
		OutputType(e.ReturnType)
		
	protected override def GenerateTypeEnd(e as CodeTypeDeclaration) :
		Indent--

	protected override def GenerateNamespaceStart(e as CodeNamespace) :
		if e and e.Name and e.Name != string.Empty:
			Output.WriteLine("namespace ${e.Name}")	
		
	protected override def GenerateNamespaceEnd(e as CodeNamespace) :
		pass

	protected override def GenerateNamespaceImport(e as CodeNamespaceImport) :
		Output.WriteLine("import ${e.Namespace}")

	protected override def GenerateAttributeDeclarationsStart(attributes as CodeAttributeDeclarationCollection) :		
		Output.Write("[")
		
	protected override def GenerateAttributeDeclarationsEnd(attributes as CodeAttributeDeclarationCollection) :
		Output.Write("]")

	protected override def Supports(support as GeneratorSupport) as bool:
		return true

	protected override def IsValidIdentifier(value as string) as bool:
		return true

	protected override def CreateEscapedIdentifier(value as string) as string:
		return value

	protected override def CreateValidIdentifier(value as string) as string:
		return value

	protected override def GetTypeOutput(typeRef as CodeTypeReference):
		if typeRef.ArrayElementType:
			out = GetTypeOutput(typeRef.ArrayElementType)
		elif typeRef.BaseType in primitives:
			out = primitives[typeRef.BaseType]
		else:
			out = typeRef.BaseType
		
		if typeRef.ArrayRank > 0:
			if typeRef.ArrayRank > 1:
				return "(${out}, ${typeRef.ArrayRank})"
			return "(${out})"
		return out

	protected override def QuoteSnippetString(snippet as string) as string:		
		s = snippet.Replace("\\", "\\\\")
		s = s.Replace("\"", "\\\"")
		s = s.Replace("\t", "\\t")
		s = s.Replace("\r", "\\r")
		s = s.Replace("\n", "\\n")
		return "\"" + s + "\""

	def Method(method as duck, name as string):		
		ModifiersAndAttributes(method)		
		Output.Write("def ${name} (")
		OutputParameters(method.Parameters)		
		Output.Write(")")
		unless GetTypeOutput(method.ReturnType) == "void":
			Output.Write(" as ")
			OutputType(method.ReturnType)
			OutputAttributeDeclarations(method.ReturnTypeCustomAttributes) if method.ReturnTypeCustomAttributes				
		Output.WriteLine(":")
		Indent++
		if bugged = method as CodeConstructor:
			if bugged.BaseConstructorArgs.IsValid():
				Output.Write("super(")
				OutputExpressionList(bugged.BaseConstructorArgs)
				Output.WriteLine(")")
			if bugged.ChainedConstructorArgs.IsValid():
				Output.Write("self(")
				OutputExpressionList(bugged.ChainedConstructorArgs)			
				Output.WriteLine(")")
		GenerateStatements(method.Statements)
		passcheck(method.Statements)
		Indent--
		
	def MemberReference(target as CodeExpression, member as string):
		GenerateExpression(target)
		Output.Write(".${member}")
		
	def Indexer(target as CodeExpression, indices as CodeExpressionCollection):
		GenerateExpression(target)
		Output.Write("[")
		OutputExpressionList(indices)
		Output.Write("]")
		
	def Invocation(target as CodeExpression, parameters as CodeExpressionCollection):
		GenerateExpression(target)
		Output.Write("(")
		OutputExpressionList(parameters)
		Output.Write(")")
		
	def passcheck(stuff as ICollection):
		return if stuff.IsValid()
		Output.Write("pass")
		
	override def OutputTypeNamePair(type as CodeTypeReference, name as string):
		Output.Write("${name} as ")
		OutputType(type)
		
	protected override def GenerateParameterDeclarationExpression(e as CodeParameterDeclarationExpression):
		OutputAttributes(e.CustomAttributes, null, true);
		OutputDirection(e.Direction)
		OutputTypeNamePair(e.Type, e.Name)
	
	private def OutputAttributes(attributes as CodeAttributeDeclarationCollection, prefix as string, inline as bool):
		for att as CodeAttributeDeclaration in attributes:
			GenerateAttributeDeclarationsStart(attributes)
			Output.Write(prefix) if prefix
			OutputAttributeDeclaration(att)
			GenerateAttributeDeclarationsEnd(attributes)
			if inline:
				Output.Write(" ")
			else:
				Output.WriteLine()
	
	private def OutputAttributeDeclaration(attribute as CodeAttributeDeclaration):
		Output.Write(attribute.Name.Replace ('+', '.'))
		Output.Write('(')
		first = true
		for argument as CodeAttributeArgument in attribute.Arguments:
			if first:
				OutputAttributeArgument(argument)
				first = false
			else:
				Output.Write(", ")
				OutputAttributeArgument(argument)
		Output.Write(')')
		
	def ModifiersAndAttributes(typeMember as CodeTypeMember):		
		OutputAttributeDeclarations(typeMember.CustomAttributes) if typeMember.CustomAttributes
		Output.WriteLine()
		to = { ziggy| return cast(int, ziggy) }
		fro = { ziggy| return cast(MemberAttributes, ziggy) }
		if typeMember.Attributes & MemberAttributes.VTableMask == MemberAttributes.New:
			typeMember.Attributes = fro(to(typeMember.Attributes) & ~ to(MemberAttributes.New))		
		OutputMemberAccessModifier(typeMember.Attributes)
		OutputMemberScopeModifier(typeMember.Attributes)
		
	def OutputAttributeArgument(arg as CodeAttributeArgument):
		if arg.Name and arg.Name != "":
			Output.Write(arg.Name)
			Output.Write(" : ")
		GenerateExpression(arg.Value)
		
	protected override def OutputDirection(direction as FieldDirection):
		if direction == FieldDirection.Ref or direction == FieldDirection.Out:
			Output.Write("ref ")
		
	protected override def OutputFieldScopeModifier(attributes as MemberAttributes):
		scope = attributes & MemberAttributes.ScopeMask
		if scope == MemberAttributes.Static:
			Output.Write("static ")
		elif scope == MemberAttributes.Const:
			Output.Write("static final ")
			
	private def AsBool(data):
		return data is not null and data isa bool and cast(bool,data)
		
	protected override def GenerateCompileUnitStart(compileUnit as CodeCompileUnit):
		GenerateComment(CodeComment("------------------------------------------------------------------------------"))
		GenerateComment(CodeComment("This code was automatically generated by Boo.Lang.CodeDom v.${BooVersion}."))
		GenerateComment(CodeComment(""))
		GenerateComment(CodeComment("     Changes to this file may cause incorrect behavior and will be lost if "))
		GenerateComment(CodeComment("     the code is regenerated."))
		GenerateComment(CodeComment("------------------------------------------------------------------------------"))
		Output.WriteLine()
		
		//for future:
		//if AsBool(compileUnit.UserData["WhiteSpaceAgnostic"]):
		//	Output.WriteLine("Option WhiteSpaceAgnostic")
			
		super.GenerateCompileUnitStart(compileUnit)

	protected override def GenerateCompileUnit(compileUnit as CodeCompileUnit):
		GenerateCompileUnitStart(compileUnit)
		
		for ns as CodeNamespace in compileUnit.Namespaces:
			GenerateNamespace(ns)
			
		if compileUnit.AssemblyCustomAttributes.Count > 0:
			OutputAttributes(compileUnit.AssemblyCustomAttributes, 
					"assembly:", false)
			Output.WriteLine("")
		
		GenerateCompileUnitEnd(compileUnit)
		
#region FixIndent
	static newlinePattern = Regex("\\n",RegexOptions.Compiled)
	static whiteSpacePattern = Regex("(\\s*)",RegexOptions.Compiled)
	
	static public def FixIndent(code as string, indentstring as string,
				indentlevel as int, indentfirst as bool) as string:
		//how much the code should be indented:
		indentprefix = string.Empty
		indentprefix = indentstring * indentlevel if indentlevel > 0
		nonEmptyPrefix = (indentprefix != string.Empty)
		
		if code is null or code==string.Empty:
			if indentfirst:
				return indentprefix
			else:
				return string.Empty
		
		lines = newlinePattern.Split(code.Replace("\r\n","\n"))
		
		foundFirstCodeLine = false //first line of real code
		insidecomment = 0 //inside /* */
		insidestring = false  //inside """ """
		firstline = true
		buffer = System.Text.StringBuilder()
		indent = ""
		indentSize = 0
		toTabs = indentprefix.StartsWith("\t") //otherwise, assume spaces
		for line in lines:
			//ignore comments before the first line of real code
			if not foundFirstCodeLine:
				trimmed = line.Trim()
				startmultiline = trimmed.StartsWith("/*")
				
				//if not whitespace and not a comment...
				if (insidecomment <= 0 and
						not startmultiline and
						not trimmed == string.Empty and
						not trimmed.StartsWith("#") and
						not trimmed.StartsWith("//")):
					foundFirstCodeLine = true
					indent = whiteSpacePattern.Match(line).Groups[0].Value
					indentSize = len(indent)
					insidecomment = 0
				
				//special stuff for handling multiline comments before code
				if startmultiline or insidecomment > 0:
					j = 0
					linelen = len(trimmed) - 1
					while j < linelen:
						rawArrayIndexing:
							c = trimmed[j]
						if c==char('/'):
							rawArrayIndexing:
								nextc = trimmed[j+1]
							if nextc==char('*'):
								++insidecomment
								++j
						elif c==char('*') and insidecomment > 0:
							rawArrayIndexing:
								nextc = trimmed[j+1]
							if nextc==char('/'):
								--insidecomment
								++j
						++j
			
			if not foundFirstCodeLine or not insidestring:
				if firstline:
					firstline = false
					if indentfirst and nonEmptyPrefix:
						buffer.Append(indentprefix)
				else:
					if nonEmptyPrefix:
						buffer.Append(indentprefix)
						
				if not foundFirstCodeLine:
					buffer.Append(line + "\n")
					continue
				
				if line.StartsWith(indent):
					if indentSize > 0:
						rest = line[indentSize:]
					else:
						rest = line
					if len(rest) > 0:
						//convert any more leading whitespace to tabs or spaces
						//so we don't mix tabs and spaces on same line
						ws = whiteSpacePattern.Match(rest).Groups[0].Value
						sz = len(ws)
						if len(ws):
							rawArrayIndexing:
								c = rest[0]
							if toTabs:
								if c==char(' '):
									rest = rest[sz:]
									ws = ws.Replace("    ","\t")
									ws = ws.Replace("   ","\t")
									ws = ws.Replace("  ","\t")
									ws = ws.Replace(" ","\t")
									buffer.Append(ws)
							else:
								if c==char('\t'):
									rest = rest[sz:]
									ws = ws.Replace("\t","    ")
									buffer.Append(ws)
						buffer.Append(rest)
				else: //line is not indented as expected, pass it on as is:
					buffer.Append(line)
			else: //we are inside a triple quoted string:
				buffer.Append(line)
			
			buffer.Append("\n")
			
			//scan for triple quoted string literals:
			//   for speed reasons, I'm not also checking for insidecomment,
			//   so if you have an odd # of triple quotes inside a comment,
			//   you can break this.
			j = 0
			linelen = len(line) - 2
			while j < linelen:
				rawArrayIndexing:
					c1 = line[j]
				if c1==char('"'):
					rawArrayIndexing:
						c2 = line[j+1]
					if c2==char('"'):
						rawArrayIndexing:
							c3 = line[j+2]
						if c3==char('"'):
							insidestring = not insidestring
							j += 2
				++j
				
		return buffer.ToString()
#endregion

