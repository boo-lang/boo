namespace BooBinding.CodeCompletion

import BooBinding
import System
import System.Collections
import System.Diagnostics
import System.IO
import ICSharpCode.SharpDevelop.Services
import SharpDevelop.Internal.Parser
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast as AST
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps

class Resolver:
	_parserService as IParserService
	
	def CtrlSpace(parserService as IParserService, caretLine as int, caretColumn as int, fileName as string) as ArrayList:
		_parserService = parserService
		result = ArrayList(BooAmbience.TypeConversionTable.Values)
		result.Add("System") // system namespace can be used everywhere
		
		parseInfo = parserService.GetParseInformation(fileName)
		cu = parseInfo.MostRecentCompilationUnit as CompilationUnit
		if cu != null:
			curClass = parserService.GetInnermostClass(cu, caretLine, caretColumn) as IClass
			result = AddCurrentClassMembers(result, curClass) if curClass != null
		
		return result
	
	def AddCurrentClassMembers(result as ArrayList, curClass as IClass) as ArrayList:
		result = _parserService.ListMembers(result, curClass, curClass, false)
		// Add static members, but only from this class (not from base classes)
		for method as IMethod in curClass.Methods:
			result.Add(method) if (method.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		for field as IField in curClass.Fields:
			result.Add(field) if (field.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		for property as IProperty in curClass.Properties:
			result.Add(property) if (property.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		for e as Event in curClass.Events:
			result.Add(e) if (e.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		return result
	
	def Resolve(parserService as IParserService, expression as string, caretLine as int, caretColumn as int, fileName as string, fileContent as string) as ResolveResult:
		if expression == null or expression == '':
			return null
		
		if expression.StartsWith("import "):
			expression = expression.Substring(7).Trim()
			if parserService.NamespaceExists(expression):
				return ResolveResult(parserService.GetNamespaceList(expression))
			return null
		
		_parserService = parserService
		try:
			int.Parse(expression)
			return null
		except exception as Exception:
			pass
		
		parseInfo = parserService.GetParseInformation(fileName)
		cu = parseInfo.MostRecentCompilationUnit as CompilationUnit
		if cu == null:
			print "BooResolver: No parse information!"
			return null
		
		callingClass as IClass = parserService.GetInnermostClass(cu, caretLine, caretColumn)
		returnClass as IClass = null
		if expression == "self":
			returnClass = callingClass
		elif expression == "super":
			if callingClass.BaseTypes.Count > 0:
				print callingClass.BaseTypes[0]
				returnClass = parserService.SearchType(callingClass.BaseTypes[0], callingClass, caretLine, caretColumn)
		else:
			// try looking if the expression is the name of a class
			expressionClass = parserService.SearchType(expression, callingClass, caretLine, caretColumn)
			if expressionClass != null:
				return ResolveResult(expressionClass, parserService.ListMembers(ArrayList(), expressionClass, callingClass, true))
			expandedExpression = BooAmbience.ReverseTypeConversionTable[expression]
			if expandedExpression != null:
				expressionClass = parserService.GetClass(expandedExpression)
				if expressionClass != null:
					return ResolveResult(expressionClass, parserService.ListMembers(ArrayList(), expressionClass, callingClass, true))
			
			// try if it is the name of a namespace
			if parserService.NamespaceExists(expression):
				return ResolveResult(array(string, 0), parserService.GetNamespaceContents(expression))
			
			expr = Boo.Lang.Parser.BooParser.ParseExpression("expression", expression)
			visitor = ExpressionTypeVisitor()
			visitor.Visit(expr)
			Print ("result", visitor.ReturnType)
			if visitor.ReturnType != null:
				returnClass = parserService.SearchType(visitor.ReturnType.FullyQualifiedName, callingClass, caretLine, caretColumn)
		
		return null if returnClass == null
		return ResolveResult(returnClass, parserService.ListMembers(ArrayList(), returnClass, callingClass, false))
	
	private def Print(name as string, obj):
		Console.Write(name);
		Console.Write(' = ');
		if obj == null:
			Console.WriteLine('null')
		else:
			Console.WriteLine('{0} ({1})', obj, obj.GetType().FullName)
