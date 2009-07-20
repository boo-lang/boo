"""
The classes in this module model the entire AST for the language.

The actual AST classes and supporting modules are generated
by a boo script.
"""
namespace Boo.Ast

class CompileUnit(Node):
	Modules as ModuleCollection	

[Flags]
enum TypeMemberModifiers:
	None = 0
	Private = 1
	Internal = 2	
	Protected = 4
	Public = 8
	Transient = 16
	Static = 32
	Final = 64
	Virtual = 128
	Override = 256
	Abstract = 512
	Partial = 1024
	VisibilityMask = 15
	
enum MethodImplementationFlags:
	None = 0
	Runtime = 1

enum ParameterModifiers:
	None = 0
	Val = 0
	Ref = 1

[Flags]
enum ExceptionHandlerFlags:
	None = 0
	Anonymous = 1
	Untyped = 2
	Filter = 4

[Flags]
enum GenericParameterConstraints:
	None = 0
	ValueType = 1
	ReferenceType = 2
	Constructable = 4
	Covariant = 8
	Contravariant = 16
	
abstract class TypeMember(Node, INodeWithAttributes):
	Modifiers as TypeMemberModifiers
	Name as string
	Attributes as AttributeCollection

class ExplicitMemberInfo(Node):
	InterfaceType as SimpleTypeReference

[collection(TypeMember)]
class TypeMemberCollection:
	pass

abstract class TypeReference(Node):
	IsPointer as bool

class SimpleTypeReference(TypeReference):
	Name as string

class ArrayTypeReference(TypeReference):
	ElementType as TypeReference
	Rank as IntegerLiteralExpression
	
class CallableTypeReference(TypeReference, INodeWithParameters):
	Parameters as ParameterDeclarationCollection
	ReturnType as TypeReference
	
class GenericTypeReference(SimpleTypeReference):
	GenericArguments as TypeReferenceCollection

class GenericTypeDefinitionReference(SimpleTypeReference):
	GenericPlaceholders as int
	
[collection(TypeReference)]
class TypeReferenceCollection:
	pass

class CallableDefinition(TypeMember, INodeWithParameters, INodeWithGenericParameters):
	Parameters as ParameterDeclarationCollection
	GenericParameters as GenericParameterDeclarationCollection
	ReturnType as TypeReference
	ReturnTypeAttributes as AttributeCollection

abstract class TypeDefinition(TypeMember, INodeWithGenericParameters):
	Members as TypeMemberCollection
	BaseTypes as TypeReferenceCollection
	GenericParameters as GenericParameterDeclarationCollection

[collection(TypeDefinition)]
class TypeDefinitionCollection:
	pass

class NamespaceDeclaration(Node):
	Name as string

class Import(Node):
	Namespace as string
	AssemblyReference as ReferenceExpression
	Alias as ReferenceExpression

[collection(Import)]
class ImportCollection:
	pass

class Module(TypeDefinition):
	Namespace as NamespaceDeclaration
	Imports as ImportCollection
	[auto]
	Globals as Block
	AssemblyAttributes as AttributeCollection

[collection(Module)]
class ModuleCollection:
	pass

class ClassDefinition(TypeDefinition):
	pass

class StructDefinition(TypeDefinition):
	pass

class InterfaceDefinition(TypeDefinition):
	pass

class EnumDefinition(TypeDefinition):
	pass

class EnumMember(TypeMember):
	Initializer as Expression

class Field(TypeMember):
	Type as TypeReference
	Initializer as Expression
	IsVolatile as bool

class Property(TypeMember, INodeWithParameters, IExplicitMember):
	Parameters as ParameterDeclarationCollection
	Getter as Method
	Setter as Method
	Type as TypeReference
	ExplicitInfo as ExplicitMemberInfo
	
class Event(TypeMember):
	Add as Method
	Remove as Method
	Raise as Method
	Type as TypeReference

class Local(Node):
	Name as string

[collection(Local)]
class LocalCollection:
	pass
	
class BlockExpression(Expression, INodeWithParameters, INodeWithBody):
	Parameters as ParameterDeclarationCollection
	ReturnType as TypeReference
	[auto]
	Body as Block

class Method(CallableDefinition, IExplicitMember, INodeWithBody):
	[auto]
	Body as Block
	Locals as LocalCollection
	ImplementationFlags as MethodImplementationFlags
	ExplicitInfo as ExplicitMemberInfo

class Constructor(Method):
	pass

class Destructor(Method):
	pass

class ParameterDeclaration(Node, INodeWithAttributes):
	Name as string
	Type as TypeReference
	Modifiers as ParameterModifiers
	Attributes as AttributeCollection

[collection(ParameterDeclaration)]
class ParameterDeclarationCollection:
	pass

class GenericParameterDeclaration(Node):
	Name as string
	BaseTypes as TypeReferenceCollection
	Constraints as GenericParameterConstraints

[collection(GenericParameterDeclaration)]
class GenericParameterDeclarationCollection:
	pass

class Declaration(Node):
	Name as string
	Type as TypeReference

[collection(Declaration)]
class DeclarationCollection:
	pass

class Attribute(Node, INodeWithArguments):
	Name as string
	Arguments as ExpressionCollection
	NamedArguments as ExpressionPairCollection

[collection(Attribute)]
class AttributeCollection:
	pass

enum StatementModifierType:
	None
	If
	Unless
	While

class StatementModifier(Node):
	Type as StatementModifierType
	Condition as Expression

abstract class Statement(Node):
	Modifier as StatementModifier
	
class GotoStatement(Statement):
	Label as ReferenceExpression
	
class LabelStatement(Statement):
	Name as string

class Block(Statement):
	Statements as StatementCollection

[collection(Statement)]
class StatementCollection:
	pass

class DeclarationStatement(Statement):
	Declaration as Declaration
	Initializer as Expression

class MacroStatement(Statement, INodeWithBody):
	Name as string
	Arguments as ExpressionCollection
	[auto]
	Body as Block

class TryStatement(Statement):
	[auto]
	ProtectedBlock as Block
	ExceptionHandlers as ExceptionHandlerCollection
	FailureBlock as Block
	EnsureBlock as Block
	
class ExceptionHandler(Node):
	Declaration as Declaration
	FilterCondition as Expression
	Flags as ExceptionHandlerFlags
	
	[auto]
	Block as Block

[collection(ExceptionHandler)]
class ExceptionHandlerCollection:
	pass

class IfStatement(Statement):
	Condition as Expression
	TrueBlock as Block
	FalseBlock as Block

class UnlessStatement(Statement):
	Condition as Expression
	
	[auto]
	Block as Block

class ForStatement(Statement):
	Declarations as DeclarationCollection
	Iterator as Expression
	
	[auto]
	Block as Block
	OrBlock as Block
	ThenBlock as Block

class WhileStatement(Statement):
	Condition as Expression
	
	[auto]
	Block as Block
	OrBlock as Block
	ThenBlock as Block

class BreakStatement(Statement):
	pass

class ContinueStatement(Statement):
	pass

class ReturnStatement(Statement):
	Expression as Expression

class YieldStatement(Statement):
	Expression as Expression

class RaiseStatement(Statement):
	Exception as Expression

class UnpackStatement(Statement):
	Declarations as DeclarationCollection
	Expression as Expression

class ExpressionStatement(Statement):
	[LexicalInfo]
	Expression as Expression

abstract class Expression(Node):
	pass

[collection(Expression)]
class ExpressionCollection:
	pass
	
[ignore]
class OmittedExpression(Expression):
	pass

class ExpressionPair(Node):
	First as Expression
	Second as Expression

[collection(ExpressionPair)]
class ExpressionPairCollection:
	pass

class MethodInvocationExpression(Expression, INodeWithArguments):
	Target as Expression
	Arguments as ExpressionCollection
	NamedArguments as ExpressionPairCollection

enum BinaryOperatorType:
	None
	Addition
	Subtraction
	Multiply
	Division
	Modulus
	Exponentiation
	LessThan
	LessThanOrEqual
	GreaterThan
	GreaterThanOrEqual
	Equality
	Inequality
	Match
	NotMatch
	Assign
	InPlaceAddition
	InPlaceSubtraction
	InPlaceMultiply
	InPlaceDivision
	InPlaceModulus
	InPlaceBitwiseAnd
	InPlaceBitwiseOr
	ReferenceEquality
	ReferenceInequality
	TypeTest
	Member
	NotMember
	Or
	And
	BitwiseOr
	BitwiseAnd
	ExclusiveOr
	InPlaceExclusiveOr
	ShiftLeft
	InPlaceShiftLeft
	ShiftRight
	InPlaceShiftRight

#values are ready to be used as mask if/when BinaryOperatorType
#is changed as in patch attached to BOO-1123 (breaking change)
enum BinaryOperatorKind:
	Arithmetic = 0xF
	Comparison = 0xFF0
	TypeComparison = 0xF00
	Assignment = 0xFF000
	InPlaceAssignment = 0xF0000
	Logical = 0x0F00000
	Bitwise = 0xF000000

enum UnaryOperatorType:
	None
	UnaryNegation
	Increment
	Decrement
	PostIncrement
	PostDecrement
	LogicalNot
	Explode
	OnesComplement
	AddressOf
	Indirection

class UnaryExpression(Expression):
	Operator as UnaryOperatorType
	Operand as Expression

class BinaryExpression(Expression):
	Operator as BinaryOperatorType
	Left as Expression
	Right as Expression
	
class ConditionalExpression(Expression):
	Condition as Expression
	TrueValue as Expression
	FalseValue as Expression

class ReferenceExpression(Expression):
	Name as string

class MemberReferenceExpression(ReferenceExpression):
	Target as Expression
	
class GenericReferenceExpression(Expression):
	Target as Expression
	GenericArguments as TypeReferenceCollection

abstract class LiteralExpression(Expression):
	pass
	
class QuasiquoteExpression(LiteralExpression):
	Node as Node

class StringLiteralExpression(LiteralExpression):
	Value as string
	
class CharLiteralExpression(StringLiteralExpression):
	pass

class TimeSpanLiteralExpression(LiteralExpression):
	Value as System.TimeSpan

class IntegerLiteralExpression(LiteralExpression):
	Value as long
	IsLong as bool

class DoubleLiteralExpression(LiteralExpression):
	Value as double
	IsSingle as bool

class NullLiteralExpression(LiteralExpression):
	pass

class SelfLiteralExpression(LiteralExpression):
	pass

class SuperLiteralExpression(LiteralExpression):
	pass

class BoolLiteralExpression(LiteralExpression):
	Value as bool

class RELiteralExpression(LiteralExpression):
	Value as string
	
class SpliceExpression(Expression):
	Expression as Expression
	
class SpliceTypeReference(TypeReference):
	Expression as Expression
	
class SpliceMemberReferenceExpression(Expression):
	Target as Expression
	NameExpression as Expression
	
class SpliceTypeMember(TypeMember):
	TypeMember as TypeMember
	NameExpression as Expression
	
class SpliceParameterDeclaration(ParameterDeclaration):
	ParameterDeclaration as ParameterDeclaration
	NameExpression as Expression

class ExpressionInterpolationExpression(Expression):
	Expressions as ExpressionCollection

class HashLiteralExpression(LiteralExpression):
	Items as ExpressionPairCollection

class ListLiteralExpression(LiteralExpression):
	Items as ExpressionCollection

class ArrayLiteralExpression(ListLiteralExpression):
	Type as ArrayTypeReference
	
class GeneratorExpression(Expression):
	Expression as Expression
	Declarations as DeclarationCollection
	Iterator as Expression
	Filter as StatementModifier
	
class ExtendedGeneratorExpression(Expression):
	Items as GeneratorExpressionCollection
	
[collection(GeneratorExpression)]
class GeneratorExpressionCollection:
	pass
	
class Slice(Node):
	Begin as Expression
	End as Expression
	Step as Expression
	
[collection(Slice)]
class SliceCollection:
	pass

class SlicingExpression(Expression):
	Target as Expression
	Indices as SliceCollection

class TryCastExpression(Expression):
	Target as Expression
	Type as TypeReference
	
class CastExpression(Expression):
	Target as Expression
	Type as TypeReference
	
class TypeofExpression(Expression):
	Type as TypeReference

class CustomStatement(Statement):
	pass
	
class StatementTypeMember(TypeMember):
"""
Allow for macros and initializing statements inside type definition bodies.
"""
	Statement as Statement

