namespace BooInBoo.CompilerSteps

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipeline

enum BindingType:
	CompileUnit
	Module
	Class
	Interface
	Enum
	Callable
	Constructor
	Method
	Field
	Property
	Namespace

interface IBinding:
	ParentNamespace as INamespace:
		get
	BindingType as BindingType:
		get

interface INamespace(IBinding):	
	def Resolve(name as string) as IBinding
	
interface INameResolutionService(ICompilerComponent):
	def EnterNamespace(ns as INamespace)
	def LeaveNamespace()
	def Resolve(name as string) as List

class CreateBindings(AbstractSwitcherCompilerStep):
	
	override def Run():
		pass
