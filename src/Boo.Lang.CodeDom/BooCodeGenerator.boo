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

import System.CodeDom
import System.CodeDom.Compiler
import System.Reflection
import System.Collections
import System.Text.RegularExpressions
import System.Globalization

class BooCodeGenerator(CodeGenerator):
	static _NET_2_0 as bool = System.Environment.Version >= System.Version(2, 0)
	
	static primitives = { "System.Double" : "double",
						"System.Single" : "single",
						"System.Int32" : "int",
						"System.Int64" : "long",
						"System.Int16" : "short",
						"System.UInt16" : "ushort",
						"System.UInt32" : "uint",
						"System.Uint64" : "ulong",
						"System.Byte": "byte",
						"System.Sbyte": "sbyte",
						"System.Decimal": "decimal",
						"System.Boolean" : "bool",
						"System.Char" : "char",
						"System.String" : "string",
						"System.Object" : "object",
						"System.Void" : "void",
						"System.DateTime" : "date",
						"System.TimeSpan" : "timespan" }
						
	protected NullToken as string:
		get:
			return "null"	

	// workaround for MS's implementation around code snippets
	protected BooIndent as int:
		get:
			return _booIndent
		set:
			_booIndent = value
			Indent = value
	_booIndent = 0

	protected override def OutputType(typeRef as CodeTypeReference):
		Output.Write(GetTypeOutput(typeRef))


	protected override def GenerateArrayCreateExpression(exp as CodeArrayCreateExpression) :
		if exp.Initializers is not null and exp.Size == 0 and exp.SizeExpression is null:
			Output.Write("(")
			if exp.CreateType is not null:
				Output.Write("of ")
				OutputType(exp.CreateType)
				Output.Write(": ")
			OutputExpressionList(exp.Initializers)
			//no trailing comma needed for single item with "of" type
			if exp.Initializers.Count==0 or (exp.Initializers.Count==1 and not exp.CreateType):
				Output.Write(",")
			Output.Write(")")
		else:
			Output.Write("array(")
			OutputType(exp.CreateType)
			Output.Write(", ")
			if exp.SizeExpression is not null:
				GenerateExpression(exp.SizeExpression)
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
		Output.WriteLine()

	protected override def GenerateIterationStatement(e as CodeIterationStatement):
		if e.InitStatement:
			GenerateStatement(e.InitStatement)
		Output.Write("while ")
		GenerateExpression(e.TestExpression)
		BeginBlock()
		if e.Statements and e.Statements.IsValid():
			GenerateStatements(e.Statements)
		if e.IncrementStatement:
			GenerateStatement(e.IncrementStatement)
		else:
			passcheck(e.Statements)
		EndBlock()

	protected override def GenerateThrowExceptionStatement(e as CodeThrowExceptionStatement) :
		Output.Write("raise ")
		GenerateExpression(e.ToThrow)
		Output.WriteLine()

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
		if e.Expression is null:
			Output.WriteLine("return")			
			return
		Output.Write("return ")
		GenerateExpression(e.Expression)
		Output.WriteLine()

	protected override def GenerateConditionStatement(e as CodeConditionStatement) :
		Output.Write("if ")
		GenerateExpression(e.Condition)
		BeginBlock()
		GenerateStatements(e.TrueStatements)
		passcheck(e.TrueStatements)
		EndBlock(false)
		if e.FalseStatements and e.FalseStatements.IsValid():
			Output.Write("else")
			BeginBlock()
			GenerateStatements(e.FalseStatements)
			passcheck(e.FalseStatements)
			EndBlock(false)
		End()

	protected override def GenerateTryCatchFinallyStatement(e as CodeTryCatchFinallyStatement) :
		Output.Write("try")
		BeginBlock()
		GenerateStatements(e.TryStatements)
		passcheck(e.TryStatements)
		EndBlock(false)
		for exp as CodeCatchClause in e.CatchClauses:
			Output.Write("except ")
			Output.Write("${exp.LocalName} as ")
			OutputType(exp.CatchExceptionType)
			BeginBlock()
			GenerateStatements(exp.Statements)
			passcheck(exp.Statements)
			EndBlock(false)
		
		if e.FinallyStatements and e.FinallyStatements.Count > 0:
			Output.Write("ensure")
			BeginBlock()
			GenerateStatements(e.FinallyStatements)
			passcheck(e.FinallyStatements)
			EndBlock(false)
		End()
		

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
		if e.Statement:
			GenerateStatement(e.Statement)
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
		
	protected override def GenerateField(e as CodeMemberField):
		if CurrentClass.IsEnum:
			Output.Write(e.Name)
		else:
			ModifiersAndAttributes(e)
			Output.Write("${e.Name} as ")
			OutputType(e.Type)
			
		if e.InitExpression:
			Output.Write(" = ")
			GenerateExpression(e.InitExpression)
		
	protected override def GenerateSnippetMember(e as CodeSnippetTypeMember) :
		if e.CustomAttributes:
			OutputAttributes(e.CustomAttributes, null, false)
		Output.WriteLine(FixIndent(e.Text, Options.IndentString, BooIndent, false))
	
	protected override def GenerateSnippetExpression(e as CodeSnippetExpression) :
		//Output.Write("("+e.Value+")")
		Output.Write(e.Value)
		
	protected override def GenerateSnippetCompileUnit(e as CodeSnippetCompileUnit) :
		Output.WriteLine(FixIndent(e.Value, Options.IndentString, BooIndent, false))
		
	protected override def GenerateSnippetStatement(e as CodeSnippetStatement) :
		if _NET_2_0:
			Output.WriteLine(e.Value)
		else:
			Output.WriteLine(FixIndent(e.Value, Options.IndentString, BooIndent, false))
		
	protected override def GenerateEntryPointMethod(e as CodeEntryPointMethod, c as CodeTypeDeclaration) :
		Method(e, "Main")
		
	protected override def GenerateMethod(e as CodeMemberMethod, c as CodeTypeDeclaration) :
		Method(e, e.Name)
	
	protected override def GeneratePrimitiveExpression(e as CodePrimitiveExpression) :
		if e.Value isa char:
			Output.Write("char(")
			super.GeneratePrimitiveExpression(e)
			Output.Write(")")
		else:
			super.GeneratePrimitiveExpression(e)
	
	protected override def GenerateProperty(e as CodeMemberProperty, c as CodeTypeDeclaration) :		
		ModifiersAndAttributes(e)
		
		if (e.Parameters.Count > 0
			and string.Compare(e.Name, "Item", true, CultureInfo.InvariantCulture) == 0):
			Output.Write(" self")
		else:
			Output.Write(" ${e.Name}")
		if len(e.Parameters) > 0:
			Output.Write("[")
			OutputParameters(e.Parameters)
			Output.Write("]")
		Output.Write(" as ")		
		OutputType(e.Type)
		BeginBlock()
		if e.HasGet:
			Output.Write("get")
			BeginBlock()
			GenerateStatements(e.GetStatements)
			passcheck(e.GetStatements)
			EndBlock() //TODO: Change to EndBlock(false) when BOO-631 accepted
		if e.HasSet:
			Output.Write("set")
			BeginBlock()
			GenerateStatements(e.SetStatements)
			passcheck(e.SetStatements)
			EndBlock() //TODO: Change to EndBlock(false) when BOO-631 accepted
		EndBlock()
		
	protected override def GenerateConstructor(e as CodeConstructor, c as CodeTypeDeclaration) :
		e.Attributes |= MemberAttributes.Final
		Method(e, "constructor")
		

	protected override def GenerateTypeConstructor(e as CodeTypeConstructor) :		
		e.Attributes |= MemberAttributes.Static
		e.Attributes |= MemberAttributes.Public
		e.Parameters.Clear()
		Method(e, "constructor")

	protected override def GenerateTypeStart(e as CodeTypeDeclaration):		
		if e.CustomAttributes:
			OutputAttributes(e.CustomAttributes, null, false)
		if e isa CodeTypeDelegate:
			GenerateDelegate(e)
			return
			
		//TODO: move this to an override of OutputTypeAttributes
		
		//FIXME: When boo gets preprocessor directives, i.e. #if NET_2_0
		//Til then we have to partially rely on super.OutputTypeAttributes
		
		//output "final", since super.OutputTypeAttributes outputs "sealed"
		if e.TypeAttributes & TypeAttributes.Sealed == TypeAttributes.Sealed:
			Output.Write("final ")
			
			//remove sealed from attributes:
				
			//FIXME: when boo enum processing fixed
			//e.TypeAttributes &= ~TypeAttributes.Sealed
			e.TypeAttributes &= cast(TypeAttributes,~cast(int,TypeAttributes.Sealed))
			
		visibility = e.TypeAttributes & TypeAttributes.VisibilityMask
		if visibility == TypeAttributes.Public or visibility == TypeAttributes.NestedPublic:
			pass //Output.Write("public ") //public by default
		elif visibility == TypeAttributes.NestedPrivate:
			Output.Write("private ")
		elif (visibility == TypeAttributes.NotPublic or
			TypeAttributes.NestedFamANDAssem or
			TypeAttributes.NestedAssembly):
			Output.Write("internal ")
		elif visibility == TypeAttributes.NestedFamily:
			//FIXME: BOO-666
			Output.Write("protected ")
		elif visibility == TypeAttributes.NestedFamORAssem:
			//FIXME: BOO-666
			Output.Write("internal ")

		if e.IsPartial:
			Output.Write("partial ")
			
		//super OutputTypeAttributes ignores TypeAttributes.NotPublic (internal)
		//FIXME: when boo enum processing fixed
		//e.TypeAttributes &= ~TypeAttributes.VisibilityMask
		e.TypeAttributes &= cast(TypeAttributes,~cast(int,TypeAttributes.VisibilityMask))
		e.TypeAttributes |= TypeAttributes.NotPublic //not really nec. since val is 0
		
		//TODO: Remove call to OutputTypeAttributes
		OutputTypeAttributes(e.TypeAttributes, e.IsStruct, e.IsEnum)
		/////////////////////////////////////
		
		Output.Write("${e.Name}")
		if len(e.BaseTypes):
			Output.Write("(")
			for index in range(len(e.BaseTypes)):
				var as CodeTypeReference = e.BaseTypes[index]
				OutputType(var)
				continue unless index + 1 != e.BaseTypes.Count
				Output.Write(",")
			Output.Write(")")
			
		BeginBlock()
		#Ah hah hah, yeah, no. Empty class.
		passcheck(e.Members)

		
	private def GenerateDelegate(e as CodeTypeDelegate):
		if e.TypeAttributes & TypeAttributes.Sealed == TypeAttributes.Sealed:
			Output.Write("final ")
		if e.TypeAttributes & TypeAttributes.NestedFamORAssem == TypeAttributes.Public:
			Output.Write("public ")
		Output.Write("callable ${e.Name}(")
		OutputParameters(e.Parameters)
		Output.Write(") as ")
		OutputType(e.ReturnType)
		
	protected override def GenerateTypeEnd(e as CodeTypeDeclaration) :
		EndBlock()

	protected override def GenerateNamespaceStart(e as CodeNamespace) :
		if e is null or string.IsNullOrEmpty(e.Name):
			return
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
		
	def NonGenericName(typeName as string):
		return typeName[:typeName.IndexOf(char('`'))]

	protected override def GetTypeOutput(typeRef as CodeTypeReference) as string:
		if typeRef.ArrayElementType:
			out = GetTypeOutput(typeRef.ArrayElementType)
		elif typeRef.BaseType in primitives:
			out = primitives[typeRef.BaseType]
		else:
			// BaseType uses .NET syntax for inner classes, so we have to replace '+' with '.'
			out = typeRef.BaseType.Replace('+', '.')
			typeArgs = typeRef.TypeArguments
			if len(typeArgs) > 0:
				out = "${NonGenericName(out)}[of ${join(GetTypeOutput(arg) for arg in typeArgs, ', ')}]"
			
		if typeRef.ArrayRank == 0:
			return out
		elif typeRef.ArrayRank == 1:
			return "(${out})"
		else:
			return "(${out}, ${typeRef.ArrayRank})"

	protected override def QuoteSnippetString(snippet as string) as string:		
		s = snippet.Replace("\\", "\\\\")
		s = s.Replace("\"", "\\\"")
		s = s.Replace("\t", "\\t")
		s = s.Replace("\r", "\\r")
		s = s.Replace("\n", "\\n")
		return "\"" + s + "\""

	def Method(method as duck, name as string):		
		ModifiersAndAttributes(method)		
		Output.Write("def ${name}(")
		OutputParameters(method.Parameters)		
		Output.Write(")")
		if method.ReturnTypeCustomAttributes:
			OutputAttributes(method.ReturnTypeCustomAttributes, null, true)
		unless GetTypeOutput(method.ReturnType) == "void":
			Output.Write(" as ")
			OutputType(method.ReturnType)
		BeginBlock()
		ctor = method as CodeConstructor
		do_pass = true
		if ctor:
			if ctor.BaseConstructorArgs.IsValid():
				Output.Write("super(")
				OutputExpressionList(ctor.BaseConstructorArgs)
				Output.WriteLine(")")
				do_pass = false
			if ctor.ChainedConstructorArgs.IsValid():
				Output.Write("self(")
				OutputExpressionList(ctor.ChainedConstructorArgs)			
				Output.WriteLine(")")
				do_pass = false
		GenerateStatements(method.Statements)
		if do_pass:
			passcheck(method.Statements)
		EndBlock()
		
	def MemberReference(target as CodeExpression, member as string):
		if target: #target.method( ... )
			GenerateExpression(target)
			Output.Write(".${member}")
		else: #method( ... )
			Output.Write(member)
		
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
		
	protected override def OutputExpressionList(exps as CodeExpressionCollection):
		if exps is null or exps.Count == 0:
			return
		total = exps.Count
		i = 0
		while i < total:
			e as CodeExpression = exps[i]
			if AsBool(e.UserData["Explode"]):
				Output.Write("*")
			GenerateExpression(e)
			if i < total-1:
				Output.Write(", ")
			++i
		
	def passcheck(stuff as ICollection):
		return if stuff.IsValid()
		if not AsBool(Options["WhiteSpaceAgnostic"]):
			Output.WriteLine("pass")
		
	override def OutputTypeNamePair(type as CodeTypeReference, name as string):
		Output.Write("${name} as ")
		OutputType(type)
		
	protected override def GenerateParameterDeclarationExpression(e as CodeParameterDeclarationExpression):
		name = e.Name
		if e.Type.ArrayElementType:
			paramarray as CodeAttributeDeclaration = null
			for att as CodeAttributeDeclaration in e.CustomAttributes:
				if att.Name.EndsWith("ParamArrayAttribute") or att.Name.EndsWith("ParamArray"):
					paramarray = att
					break
			if paramarray:
				e.CustomAttributes.Remove(paramarray)
				name = "*"+e.Name
				
		OutputAttributes(e.CustomAttributes, null, true)
		OutputDirection(e.Direction)
		OutputTypeNamePair(e.Type, name)
	
	private def OutputAttributes(attributes as CodeAttributeDeclarationCollection, prefix as string, inline as bool):
		for att as CodeAttributeDeclaration in attributes:
			GenerateAttributeDeclarationsStart(attributes)
			Output.Write(prefix) if prefix
			OutputAttribute(att)
			GenerateAttributeDeclarationsEnd(attributes)
			if inline:  //for parameter and return type attributes
				pass //Output.Write(" ")
			else:
				Output.WriteLine()
	
	private def OutputAttribute(attribute as CodeAttributeDeclaration):
		Output.Write(attribute.Name.Replace ('+', '.'))
		if len(attribute.Arguments) == 0:
			return
			
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
		if typeMember.CustomAttributes:
			OutputAttributes(typeMember.CustomAttributes, null, false)
		
		//FIXME: We just ignore "new" methods, since they are unsupported in boo
		if typeMember.Attributes & MemberAttributes.VTableMask == MemberAttributes.New:
			typeMember.Attributes &= cast(MemberAttributes,~cast(int,MemberAttributes.New))
			
		//TODO: Override these.  Right now we are lucky C# and boo are the same
		//with respect to keywords and defaults (such as final/sealed by default).
		//We should override for example to not print out "public" since it is boo default.
		OutputMemberAccessModifier(typeMember.Attributes)
		OutputMemberScopeModifier(typeMember.Attributes)
		
	def OutputAttributeArgument(arg as CodeAttributeArgument):
		if arg.Name and arg.Name != "":
			Output.Write(arg.Name)
			Output.Write(":")
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
		if AsBool(Options["WhiteSpaceAgnostic"]):
			Output.WriteLine("//option WhiteSpaceAgnostic")
			Output.WriteLine()
			
		//super.GenerateCompileUnitStart(compileUnit)

	protected override def GenerateCompileUnit(compileUnit as CodeCompileUnit):
		GenerateCompileUnitStart(compileUnit)
		
		for ns as CodeNamespace in compileUnit.Namespaces:
			GenerateNamespace(ns)
			
		if compileUnit.AssemblyCustomAttributes.Count > 0:
			OutputAttributes(compileUnit.AssemblyCustomAttributes, 
					"assembly:", false)
			Output.WriteLine("")
		
		GenerateCompileUnitEnd(compileUnit)
		
	protected virtual def BeginBlock():
		Output.WriteLine(":")
		++BooIndent
		
	protected virtual def EndBlock():
		EndBlock(true)
		
	protected virtual def EndBlock(realend as bool):
		--BooIndent
		if realend:
			End()
			
	protected virtual def End():
		if AsBool(Options["WhiteSpaceAgnostic"]):
			Output.WriteLine("end")
		
#region FixIndent
	static newlinePattern = Regex("\\n",RegexOptions.Compiled)
	static whiteSpacePattern = Regex("(\\s*)",RegexOptions.Compiled)
	
	static public def FixIndent(code as string, indentstring as string,
				indentlevel as int, indentfirst as bool) as string:
		//how much the code should be indented:
		if indentlevel > 0:
			indentprefix = indentstring * indentlevel
			nonEmptyPrefix = (indentprefix != string.Empty)
		else:
			indentprefix = string.Empty
			nonEmptyPrefix = false
		
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
					buffer.Append(line + System.Environment.NewLine)
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
			
			buffer.Append(System.Environment.NewLine)
			
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

