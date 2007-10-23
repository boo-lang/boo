"""
The classes in this module model the entire AST for the language.

The actual AST classes and supporting modules are generated
by a boo script.
"""
namespace Boo.Ast

class CompileUnit(Node):
	Modules as ModuleCollection	

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
	pass

class SimpleTypeReference(TypeReference):
	Name as string

class ArrayTypeReference(TypeReference):
	ElementType as TypeReference
	Rank as IntegerLiteralExpression
	
class CallableTypeReference(TypeReference):
	Parameters as ParameterDeclarationCollection
	ReturnType as TypeReference
	
class GenericTypeReference(SimpleTypeReference):
	GenericArguments as TypeReferenceCollection

class GenericTypeDefinitionReference(SimpleTypeReference):
	GenericPlaceholders as int
	
[collection(TypeReference)]
class TypeReferenceCollection:
	pass

class CallableDefinition(TypeMember, INodeWithParameters):
	Parameters as ParameterDeclarationCollection
	GenericParameters as GenericParameterDeclarationCollection
	ReturnType as TypeReference
	ReturnTypeAttributes as AttributeCollection

abstract class TypeDefinition(TypeMember):
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
	Initializer as IntegerLiteralExpression

class Field(TypeMember):
	Type as TypeReference
	Initializer as Expression

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
	
class BlockExpression(Expression, INodeWithParameters):
	Parameters as ParameterDeclarationCollection
	ReturnType as TypeReference
	[auto]
	Body as Block

class Method(CallableDefinition, IExplicitMember):
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

class MacroStatement(Statement):
	Name as string
	Arguments as ExpressionCollection
	
	[auto]
	Block as Block

class TryStatement(Statement):
	[auto]
	ProtectedBlock as Block
	ExceptionHandlers as ExceptionHandlerCollection
	EnsureBlock as Block
	
class ExceptionHandler(Node):
	Declaration as Declaration
	
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

class WhileStatement(Statement):
	Condition as Expression
	
	[auto]
	Block as Block

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

