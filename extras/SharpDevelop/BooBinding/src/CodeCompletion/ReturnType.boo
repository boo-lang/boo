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
	PointerNestingLevel as int:
		get:
			return super.pointerNestingLevel
	
	ArrayDimensions as (int):
		get:
			return super.arrayDimensions
	
	def constructor(fullyQualifiedName as string):
		self.FullyQualifiedName = fullyQualifiedName
	
	def constructor(fullyQualifiedName as string, arrayDimensions as (int), pointerNestingLevel as int):
		self(fullyQualifiedName)
		self.arrayDimensions = arrayDimensions
		self.pointerNestingLevel = pointerNestingLevel
	
	def constructor(t as AST.TypeReference):
		super.pointerNestingLevel = 0
		if t isa AST.SimpleTypeReference:
			super.arrayDimensions = array(int, 0)
			super.FullyQualifiedName = cast(AST.SimpleTypeReference, t).Name
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
		super.FullyQualifiedName = t.FullName
		super.arrayDimensions = array(int, 0)
		super.pointerNestingLevel = 0
	
	def constructor(c as IClass):
		super.FullyQualifiedName = c.FullyQualifiedName
		super.arrayDimensions = array(int, 0)
		super.pointerNestingLevel = 0
	
	def Clone() as ReturnType:
		return ReturnType(FullyQualifiedName, arrayDimensions, pointerNestingLevel)
