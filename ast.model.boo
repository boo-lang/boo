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
	
enum MethodImplementationFlags:
	None = 0
	Runtime = 1

abstract class TypeMember(Node, INodeWithAttributes):
	Modifiers as TypeMemberModifiers
	Name as string
	Attributes as AttributeCollection

[collection(TypeMember)]
class TypeMemberCollection:
	pass

abstract class TypeReference(Node):
	pass

class SimpleTypeReference(TypeReference):
	Name as string

class ArrayTypeReference(TypeReference):
	ElementType as TypeReference
	
class CallableTypeReference(TypeReference):
	Parameters as TypeReferenceCollection
	ReturnType as TypeReference

[collection(TypeReference)]
class TypeReferenceCollection:
	pass

class CallableDefinition(TypeMember, INodeWithParameters):
	Parameters as ParameterDeclarationCollection
	ReturnType as TypeReference
	ReturnTypeAttributes as AttributeCollection
	VariableArguments as bool

abstract class TypeDefinition(TypeMember):
	Members as TypeMemberCollection
	BaseTypes as TypeReferenceCollection

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

class InterfaceDefinition(TypeDefinition):
	pass

class EnumDefinition(TypeDefinition):
	pass

class EnumMember(TypeMember):
	Initializer as IntegerLiteralExpression

class Field(TypeMember):
	Type as TypeReference
	Initializer as Expression

class Property(TypeMember, INodeWithParameters):
	Parameters as ParameterDeclarationCollection
	Getter as Method
	Setter as Method
	Type as TypeReference
	
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
	
class CallableBlockExpression(Expression, INodeWithParameters):
	Parameters as ParameterDeclarationCollection
	ReturnType as TypeReference
	[auto]
	Body as Block

class Method(CallableDefinition):	
	[auto]
	Body as Block
	Locals as LocalCollection
	ImplementationFlags as MethodImplementationFlags

class Constructor(Method):
	pass

class ParameterDeclaration(Node, INodeWithAttributes):
	Name as string
	Type as TypeReference
	Attributes as AttributeCollection

[collection(ParameterDeclaration)]
class ParameterDeclarationCollection:
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
	Uninitialized
	If
	Unless
	While

class StatementModifier(Node):
	Type as StatementModifierType
	Condition as Expression

abstract class Statement(Node):
	Modifier as StatementModifier

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
	SuccessBlock as Block
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

class GivenStatement(Statement):
	Expression as Expression
	WhenClauses as WhenClauseCollection
	OtherwiseBlock as Block

class WhenClause(Node):
	Condition as Expression
	
	[auto]
	Block as Block

[collection(WhenClause)]
class WhenClauseCollection:
	pass

class BreakStatement(Statement):
	pass

class ContinueStatement(Statement):
	pass

class RetryStatement(Statement):
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
	InPlaceAdd
	InPlaceSubtract
	InPlaceMultiply
	InPlaceDivide
	ReferenceEquality
	ReferenceInequality
	TypeTest
	Member
	NotMember
	Or
	And
	BitwiseOr

enum UnaryOperatorType:
	None
	UnaryNegation
	Increment
	Decrement
	LogicalNot

class UnaryExpression(Expression):
	Operator as UnaryOperatorType
	Operand as Expression

class BinaryExpression(Expression):
	Operator as BinaryOperatorType
	Left as Expression
	Right as Expression

class ReferenceExpression(Expression):
	Name as string

class MemberReferenceExpression(ReferenceExpression):
	Target as Expression

abstract class LiteralExpression(Expression):
	pass

class StringLiteralExpression(LiteralExpression):
	Value as string

class TimeSpanLiteralExpression(LiteralExpression):
	Value as System.TimeSpan

class IntegerLiteralExpression(LiteralExpression):
	Value as long
	IsLong as bool

class DoubleLiteralExpression(LiteralExpression):
	Value as double

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

class ExpressionInterpolationExpression(Expression):
	Expressions as ExpressionCollection

class HashLiteralExpression(LiteralExpression):
	Items as ExpressionPairCollection

class ListLiteralExpression(LiteralExpression):
	Items as ExpressionCollection

class ArrayLiteralExpression(ListLiteralExpression):
	pass

class GeneratorExpression(Expression):
	Expression as Expression
	Declarations as DeclarationCollection
	Iterator as Expression
	Filter as StatementModifier

class SlicingExpression(Expression):
	Target as Expression
	Begin as Expression
	End as Expression
	Step as Expression

class AsExpression(Expression):
	Target as Expression
	Type as TypeReference
	
class CastExpression(Expression):
	Type as TypeReference
	Target as Expression
	
class TypeofExpression(Expression):
	Type as TypeReference

