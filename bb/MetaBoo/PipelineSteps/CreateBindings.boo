namespace BooInBoo.PipelineSteps

import BooInBoo

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

class CreateBindings(AbstractCompilerPipelineStep):
	
	override def Run():
		pass
