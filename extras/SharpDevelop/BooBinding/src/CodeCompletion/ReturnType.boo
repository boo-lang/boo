namespace BooBinding.CodeCompletion

import System
import System.Collections
import System.Diagnostics
import SharpDevelop.Internal.Parser
import Boo.Lang.Compiler.Ast as AST

/////////////////////////////////////
///          Return Type          ///
/////////////////////////////////////
class ReturnType(AbstractReturnType):
	def constructor(fullyQualifiedName as string):
		self(fullyQualifiedName, array(int, 0), 0)
	
	def constructor(fullyQualifiedName as string, arrayDimensions as (int), pointerNestingLevel as int):
		self.FullyQualifiedName = fullyQualifiedName
		self.arrayDimensions = arrayDimensions
		self.pointerNestingLevel = pointerNestingLevel
	
	def constructor(t as AST.TypeReference):
		super.pointerNestingLevel = 0
		if t isa AST.SimpleTypeReference:
			super.arrayDimensions = array(int, 0)
			name = cast(AST.SimpleTypeReference, t).Name
			expandedName = BooBinding.BooAmbience.ReverseTypeConversionTable[name]
			name = expandedName if expandedName != null
			super.FullyQualifiedName = name
		elif t isa AST.ArrayTypeReference:
			ar as AST.ArrayTypeReference = t
			depth = 1
			while ar.ElementType isa AST.ArrayTypeReference:
				depth += 1
				ar = ar.ElementType
			dimensions = array(int, depth)
			for i as int in range(depth):
				dimensions[i] = 1
			self.arrayDimensions = dimensions
			if ar.ElementType isa AST.SimpleTypeReference:
				super.FullyQualifiedName = cast(AST.SimpleTypeReference, ar.ElementType).Name
			else:
				print ("Got unknown TypeReference in Array: ${t}")
				super.FullyQualifiedName = "<Error>"
		else:
			super.arrayDimensions = array(int, 0)
			super.FullyQualifiedName = "<Error>"
			print ("Got unknown TypeReference ${t}")
	
	def constructor(t as AST.TypeDefinition):
		self(t.FullName)
	
	def constructor(c as IClass):
		self(c.FullyQualifiedName)
	
	def Clone() as ReturnType:
		return ReturnType(FullyQualifiedName, arrayDimensions, pointerNestingLevel)
	
	override def ToString():
		return "[${GetType().Name} Name=${FullyQualifiedName}]"

/////////////////////////////////////
///     Namespace Return Type     ///
/////////////////////////////////////
class NamespaceReturnType(AbstractReturnType):
	def constructor(fullyQualifiedName as string):
		self.FullyQualifiedName = fullyQualifiedName
		self.arrayDimensions = array(int, 0)
		self.pointerNestingLevel = 0
	
	override def ToString():
		return "[${GetType().Name} Name=${FullyQualifiedName}]"
