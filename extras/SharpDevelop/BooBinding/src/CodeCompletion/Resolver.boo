namespace BooBinding.CodeCompletion

import System
import System.Collections
import System.Diagnostics
import System.IO
import ICSharpCode.SharpDevelop.Services
import ICSharpCode.SharpDevelop.Internal.Project
import SharpDevelop.Internal.Parser
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class Resolver:
	def CtrlSpace(parserService as IParserService, caretLine as int, caretColumn as int, fileName as string) as ArrayList:
		return ArrayList()
	
	def Resolve(parserService as IParserService, expression as string, caretLineNumber as int, caretColumn as int, fileName as string, fileContent as string) as ResolveResult:
		return null
	
