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
		// Add special boo types (types defined in Boo.dll)
		result.Add("hash")
		result.Add("list")
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
		
		callingClass as Class = parserService.GetInnermostClass(cu, caretLine, caretColumn)
		returnClass as Class = null
		returnClass = callingClass if expression == "self"
		return null if returnClass == null
		return ResolveResult(returnClass, parserService.ListMembers(ArrayList(), returnClass, callingClass, false))
