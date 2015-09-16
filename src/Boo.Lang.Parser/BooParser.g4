parser grammar BooParser;

options {
	tokenVocab = BooLexer;
}

start
	:	parse_module EOF
	;

parse_module
	:	eos?
		docstring
		eos?
		namespace_directive?
		import_directive*
		(	{IsValidMacroArgument(_input.La(2))}? module_macro
		|	type_member
		)*
		globals
		(	(	assembly_attribute
			|	module_attribute
			)
			eos
		)*
	;

module_macro
	:	macro_stmt
	;

docstring
	:	(	TRIPLE_QUOTED_STRING eos?
		)?
	;

eos
	:	(	EOL
		|	EOS
		)+
	;

import_directive
	:	(	import_directive_
		|	import_directive_from_
		)
		eos
	;

identifier_expression
	:	identifier
	;

namespace_expression
	:	identifier_expression
		(	LPAREN
			expression_list
			RPAREN
		)?
	;

import_directive_
	:	IMPORT namespace_expression
		(	FROM
			(	identifier
			|	DOUBLE_QUOTED_STRING
			|	SINGLE_QUOTED_STRING
			)
		)?
		(	AS ID
		)?
	;

import_directive_from_
	:	FROM identifier_expression IMPORT
		(	MULTIPLY
		|	expression_list
		)
	;

namespace_directive
	:	NAMESPACE identifier
		eos
		docstring
	;

type_member
	:	attributes modifiers
		(	type_definition
		|	method
		)
	;

type_definition
	:	class_definition
	|	interface_definition
	|	enum_definition
	|	callable_definition
	;
	
callable_definition
	:	CALLABLE ID
		(	LBRACK OF? generic_parameter_declaration_list RBRACK
		)?
		LPAREN parameter_declaration_list RPAREN
		(	AS type_reference
		)?
		eos
		docstring
	;

enum_definition
	:	ENUM ID
		begin_with_doc
		(	PASS eos
		|	any_enum_member+
		)
		end
	;

any_enum_member
	:	(	enum_member
		|	splice_type_definition_body
		)
	;

enum_member
	:	attributes ID (ASSIGN simple_initializer)?
		eos
		docstring
	;

attributes
	:	(	LBRACK
			(	attribute
				(	COMMA attribute
				)*
			)?
			RBRACK
			eos?
		)*
	;

attribute
	:	(	identifier
		|	TRANSIENT
		)
		(	LPAREN
			argument_list
			RPAREN
		)?
	;

module_attribute
	:	MODULE_ATTRIBUTE_BEGIN
		attribute
		RBRACK
	;

assembly_attribute
	:	ASSEMBLY_ATTRIBUTE_BEGIN
		attribute
		RBRACK
	;

any_type_definition_member
	:	(	splice_type_definition_body
		|	type_definition_member
		)
	;

class_definition
	:	(	CLASS
		|	STRUCT
		)
		(	ID
		|	SPLICE_BEGIN atom
		)
		(	LBRACK OF? generic_parameter_declaration_list RBRACK
		)?
		base_types?
		begin_with_doc
		(	PASS eos
		|	eos?
			any_type_definition_member+
		)
		end
	;

splice_type_definition_body
	:	SPLICE_BEGIN atom eos
	;

type_definition_member
	:	attributes
		modifiers
		(	method
		|	event_declaration
		|	field_or_property
		|	type_definition
		)
	;

any_intf_type_member
	:	(	attributes
			(	interface_method
			|	event_declaration
			|	interface_property
			)
		)
	;

interface_definition
	:	INTERFACE
		(	ID
		|	SPLICE_BEGIN atom
		)
		(	LBRACK OF? generic_parameter_declaration_list RBRACK
		)?
		base_types?
		begin_with_doc
		(	PASS eos
		|	any_intf_type_member+
		)
		end
	;

base_types
	:	LPAREN
		(	type_reference
			(	COMMA type_reference
			)*
		)?
		RPAREN
	;

interface_method
	:	DEF
		(	member
		|	SPLICE_BEGIN atom
		)
		(	LBRACK OF? generic_parameter_declaration_list RBRACK
		|	OF generic_parameter_declaration
		)?
		LPAREN parameter_declaration_list RPAREN
		(	AS type_reference
		)?
		(	eos docstring
		|	empty_block eos?
		)
	;

interface_property
	:	(ID | SELF)
		(	(LBRACK|LPAREN) parameter_declaration_list (RBRACK|RPAREN)
		)?
		(	AS type_reference
		)?
		begin_with_doc
		interface_property_accessor+
		end
	;

interface_property_accessor
	:	attributes
		(	GET
		|	SET
		)
		(	eos
		|	empty_block
		)
	;

empty_block
	:	begin
		PASS eos
		end
	;

event_declaration
	:	EVENT ID AS type_reference eos
		docstring
	;

explicit_member_info
	:	ID DOT
		(	ID DOT
		)*
	;

method
	:	DEF
		(	explicit_member_info?
			(	member
			|	SPLICE_BEGIN atom
			)
		|	CONSTRUCTOR
		|	DESTRUCTOR
		)
		(	LBRACK OF? generic_parameter_declaration_list RBRACK
		)?
		LPAREN parameter_declaration_list RPAREN
		attributes
		(	AS type_reference
		)?
		begin_block_with_doc
		block
		end
	;

field_or_property
	:	(
			explicit_member_info?
			(	ID
			|	SPLICE_BEGIN atom
			|	SELF
			)
			(	LPAREN parameter_declaration_list RPAREN
			|	LBRACK parameter_declaration_list RBRACK
			)?
			(	AS type_reference
			)?
			begin_with_doc
			property_accessor+
			end
		)
	|	member_macro
	|	(
			(	ID
			|	SPLICE_BEGIN atom
			)
			(	AS type_reference
			)?
			(	ASSIGN declaration_initializer
			|	eos
			)
			docstring
		)
	;

member_macro
	:	macro_stmt
	;

declaration_initializer
	:	slicing_expression method_invocation_block
	|	array_or_expression eos
	|	callable_expression
	;

simple_initializer
	:	array_or_expression
	|	callable_expression
	;

property_accessor
	:	attributes
		modifiers
		(	GET
		|	SET
		)
		(	eos
		|	compound_stmt
		)
	;

globals
	:	eos? stmt*
	;

block
	:	eos?
		(	PASS eos
		|	stmt+
		)
	; 

modifiers
	:	type_member_modifier*
	;

type_member_modifier
	:	STATIC
	|	PUBLIC
	|	PROTECTED
	|	PRIVATE
	|	INTERNAL
	|	FINAL
	|	TRANSIENT
	|	OVERRIDE
	|	ABSTRACT
	|	VIRTUAL
	|	NEW
	|	PARTIAL
	;

parameter_modifier
	:	REF
	;

parameter_declaration_list
	:	(	parameter_declaration
			(	COMMA parameter_declaration
			)*
		)?
	;

parameter_declaration
	:	attributes
		(	MULTIPLY
			(	ID
			|	SPLICE_BEGIN atom
			)
			(	AS array_type_reference
			)?
		|	parameter_modifier?
			(	ID
			|	SPLICE_BEGIN atom
			)
			(	AS type_reference
			)?
		)
	;

callable_parameter_declaration_list
	:	(	callable_parameter_declaration
			(	COMMA callable_parameter_declaration
			)*
		)?
	;

callable_parameter_declaration
	:	MULTIPLY type_reference
	|	parameter_modifier? type_reference
	;

generic_parameter_declaration_list
	:	generic_parameter_declaration
		(	COMMA generic_parameter_declaration
		)*
	;

generic_parameter_declaration
	:	ID 
		(	LPAREN generic_parameter_constraints RPAREN
		)?
	;

generic_parameter_constraints
	:	(	CLASS
		|	STRUCT
		|	CONSTRUCTOR
		|	type_reference
		)
		(	COMMA generic_parameter_constraints
		)?
	;

callable_type_reference
	:	CALLABLE LPAREN
		callable_parameter_declaration_list
		RPAREN
		(	AS type_reference
		)?
	;

array_type_reference
	:	LPAREN
		type_reference
		(	COMMA integer_literal
		)?
		RPAREN
	;

type_reference_list
	:	type_reference
		(	COMMA type_reference
		)*
	;

splice_type_reference
	:	SPLICE_BEGIN atom
	;

type_reference
	:	(	splice_type_reference
		|	array_type_reference
		|	callable_type_reference
		|	type_name
			(	LBRACK OF?
				(	MULTIPLY
					(	COMMA MULTIPLY
					)*
					RBRACK
				|	type_reference_list
					RBRACK
				)
			|	OF MULTIPLY
			|	OF type_reference
			|
			)
			NULLABLE_SUFFIX?
		)
		type_degree
	;

type_degree
	:	(	MULTIPLY
		|	EXPONENTIATION
		)*
	;

type_name
	:	identifier
	|	CALLABLE
	|	CHAR
	;

begin
	:	COLON INDENT
	;

begin_with_doc
	:	COLON
		(eos docstring)?
		INDENT
	;

begin_block_with_doc
	:	COLON
		(eos docstring)?
		INDENT
	;

end
	:	DEDENT
		eos?
	;

compound_stmt
	:	single_line_block
	|	COLON INDENT block end
	;

single_line_block
	:	COLON
		(	PASS
		|	simple_stmt
			(	EOS simple_stmt?
			)*
		)
		EOL+
	;

closure_macro_stmt
	:	macro_name expression_list
	;

any_macro_stmt
	:	(	stmt
		|	type_member_stmt
		)
	;

macro_block
	:	eos?
		(	PASS eos
		|	any_macro_stmt+
		)
	;

type_member_stmt
	:	type_definition_member
	;

macro_compound_stmt
	:	single_line_block
	|	COLON INDENT macro_block end
	;

macro_stmt
	:	macro_name expression_list
		(	begin_with_doc macro_block end
		|	macro_compound_stmt
		|	(	eos
			|	stmt_modifier eos
			)
			docstring
		)
	;

macro_name
	:	ID
	|	THEN
	;

goto_stmt
	:	GOTO ID
	;

label_stmt
	:	COLON ID
	;
	
nested_function
	:	DEF ID
		(	LPAREN parameter_declaration_list RPAREN
			(	AS type_reference
			)?
		)?
		compound_stmt
	;

stmt
	:	nested_function
	|	for_stmt
	|	while_stmt
	|	if_stmt
	|	unless_stmt
	|	try_stmt
	|	{IsValidMacroArgument(_input.La(2))}? macro_stmt
	|	assignment_or_method_invocation_with_block_stmt
	|	return_stmt
	|	unpack_stmt
	|	declaration_stmt
	|	(	goto_stmt
		|	label_stmt
		|	yield_stmt
		|	break_stmt
		|	continue_stmt
		|	raise_stmt
		|	expression_stmt
		)
		stmt_modifier?
		eos
	;

simple_stmt
	:	{IsValidMacroArgument(_input.La(2))}? closure_macro_stmt
	|	assignment_or_method_invocation
	|	return_expression_stmt
	|	unpack
	|	declaration_stmt
	|	(	goto_stmt
		|	label_stmt
		|	yield_stmt
		|	break_stmt
		|	continue_stmt
		|	raise_stmt
		|	expression_stmt
		)
	;

stmt_modifier
	:	(	IF
		|	UNLESS
		|	WHILE
		)
		boolean_expression
	;

callable_or_expression
	:	callable_expression
	|	array_or_expression
	;

internal_closure_stmt
	:	return_expression_stmt
	|	(	unpack
		|	{IsValidClosureMacroArgument(_input.La(2))}? closure_macro_stmt
		|	closure_expression_stmt
		|	raise_stmt
		|	yield_stmt
		)
		stmt_modifier?
	;

closure_expression_stmt
	:	array_or_expression
	;

closure_expression
	:	LBRACE
		(	parameter_declaration_list BITWISE_OR
		)?
		internal_closure_stmt
		(	eos internal_closure_stmt?
		)*
		RBRACE
	;

callable_expression
	:	compound_stmt
	|	(	DO
		|	DEF
		)
		(	LPAREN parameter_declaration_list RPAREN
			(AS type_reference)?
		)?
		compound_stmt
	;

try_stmt
	:	TRY compound_stmt
		(	exception_handler
		)*
		(	FAILURE compound_stmt
		)?
		(	ENSURE compound_stmt
		)?
	;

exception_handler
	:	EXCEPT ID?
		(	AS type_reference
		)?
		(	(IF|UNLESS) boolean_expression
		)?
		compound_stmt
	;

raise_stmt
	:	RAISE expression?
	;

declaration_stmt
	:	ID AS type_reference
		(	ASSIGN
			(	declaration_initializer
			|	simple_initializer
			)
		|	stmt_modifier? eos
		)
	;

expression_stmt
	:	assignment_expression
	;

return_expression_stmt
	:	RETURN array_or_expression? stmt_modifier?
	;

return_stmt
	:	RETURN
		(	array_or_expression
			(	method_invocation_block
			|	stmt_modifier? eos
			)
		|	callable_expression
		|	stmt_modifier? eos
		)
	;

yield_stmt
	:	YIELD array_or_expression?
	;

break_stmt
	:	BREAK
	;

continue_stmt
	:	CONTINUE
	;

unless_stmt
	:	UNLESS expression
		compound_stmt
	;

for_stmt
	:	FOR declaration_list IN array_or_expression
		compound_stmt
		(	OR compound_stmt
		)?
		(	THEN compound_stmt
		)?
	;

while_stmt
	:	WHILE expression
		compound_stmt
		(	OR compound_stmt
		)?
		(	THEN compound_stmt
		)?
	;

if_stmt
	:	IF expression
		compound_stmt
		(	ELIF expression compound_stmt
		)*
		(	ELSE compound_stmt
		)?
	;

unpack_stmt
	:	unpack stmt_modifier? eos
	;

unpack
	:	declaration COMMA
		declaration_list?
		ASSIGN array_or_expression
	;

declaration_list
	:	declaration
		(	COMMA declaration
		)*
	;

declaration
	:	ID (AS type_reference)?
	;

array_or_expression
	:	(	// empty tuple: , or (,)
			COMMA
		)
	|	expression
		(	COMMA expression
		)*
		COMMA?
	;

expression
	:	boolean_expression
		(	FOR generator_expression_body
		)*
	;

generator_expression_body
	:	declaration_list IN boolean_expression
		stmt_modifier?
	;

boolean_expression
	:	boolean_term
		(	OR boolean_term
		)*
	;

boolean_term
	:	not_expression
		(	AND not_expression
		)*
	;

method_invocation_block
	:	callable_expression
	;

ast_literal_expression
	:	QQ_BEGIN
		(	INDENT ast_literal_block DEDENT eos?
		|	ast_literal_closure
		)
		QQ_END
	;

ast_literal_module
	:	parse_module
	;

ast_literal_block
	:	type_definition_member+
	|	stmt+
	|	ast_literal_module
	;

ast_literal_closure
	:	expression
		(	COLON expression
		)?
	|	import_directive_
	|	internal_closure_stmt
		(	eos internal_closure_stmt?
		)*
	;

assignment_or_method_invocation_with_block_stmt
	:	slicing_expression
		(	method_invocation_block
		|	ASSIGN
			(	array_or_expression
				(	method_invocation_block
				|	stmt_modifier eos
				|	eos
				)?
			|	callable_expression
			)
		)
	;

assignment_or_method_invocation
	:	slicing_expression ASSIGN array_or_expression
	;

not_expression
	:	NOT not_expression
	|	assignment_expression
	;

assignment_expression
	:	conditional_expression
		(	(	ASSIGN
			|	INPLACE_BITWISE_OR
			|	INPLACE_EXCLUSIVE_OR
			|	INPLACE_BITWISE_AND
			|	INPLACE_SHIFT_LEFT
			|	INPLACE_SHIFT_RIGHT
			)
			assignment_expression
		)?
	;

any_cond_expr_value
	:	(	(	CMP_OPERATOR
			|	GREATER_THAN
			|	LESS_THAN
			|	IS NOT
			|	IS
			|	NOT IN
			|	IN
			)
			sum
		|	ISA type_reference
		)
	;

conditional_expression
	:	sum
		any_cond_expr_value*
	;

any_sum_value
	:		(	(	ADD
			|	SUBTRACT
			|	BITWISE_OR
			|	EXCLUSIVE_OR
			)
			term
		)
	;

sum
	:	term
		any_sum_value*
	;

any_term_value
	:	(	(	MULTIPLY
			|	DIVISION
			|	MODULUS
			|	BITWISE_AND
			)
			factor
		)
	;

term
	:	factor
		any_term_value*
	;

any_factor_value
	:	(	(	SHIFT_LEFT
			|	SHIFT_RIGHT
			)
			exponentiation
		)
	;

factor
	:	exponentiation
		any_factor_value*
	;

exponentiation
	:	unary_expression
		(	AS type_reference
		|	CAST type_reference
		)?
		(	EXPONENTIATION exponentiation
		)*
	;

unary_expression
	:	(	SUBTRACT
		|	INCREMENT
		|	DECREMENT
		|	ONES_COMPLEMENT
		|	MULTIPLY
		)
		unary_expression
	|	integer_literal
	|	slicing_expression
		(	INCREMENT
		|	DECREMENT
		)?
	;

atom
	:	literal
	|	char_literal
	|	reference_expression
	|	paren_expression
	|	cast_expression
	|	typeof_expression
	|	splice_expression
	|	omitted_member_expression
	;

omitted_member_expression
	:	DOT member
	;

splice_expression
	:	SPLICE_BEGIN atom
	;

char_literal
	:	CHAR LPAREN
		(	SINGLE_QUOTED_STRING
		|	INT
		)?
		RPAREN
	;

cast_expression
	:	CAST LPAREN type_reference COMMA expression RPAREN
	;

typeof_expression
	:	TYPEOF LPAREN type_reference RPAREN
	;

reference_expression
	:	macro_name
	|	CHAR
	;

paren_expression
	:	typed_array
	|	LPAREN array_or_expression
		(	IF boolean_expression ELSE array_or_expression
		)?
		RPAREN
	;

typed_array
	:	LPAREN OF type_reference COLON
		(	COMMA
		|	expression
			(	COMMA expression
			)*
			COMMA?
		)
		RPAREN
	;

member
	:	ID
	|	SET
	|	GET
	|	INTERNAL
	|	PUBLIC
	|	PROTECTED
	|	EVENT
	|	REF
	|	YIELD
	;

slice_no_begin
	:	// [:
		COLON
		(	// [:end]
			expression
		|	// [::step]
			COLON expression
		|	// [:]
		)
	;

slice_with_begin
	:	// [begin
		expression
		(	// [begin:
			COLON
			expression?
			(	COLON expression
			)?
		)?
	;

slice
	:	slice_no_begin
	|	slice_with_begin
	;

safe_atom
	:	atom NULLABLE_SUFFIX?
	;

any_slice_expr_value
	:	(	LBRACK
			(	OF type_reference_list
			|	slice (COMMA slice)*
			)
			RBRACK
			NULLABLE_SUFFIX?
		|	OF type_reference
		|	DOT
			(	member
			|	SPLICE_BEGIN atom
			)
			NULLABLE_SUFFIX?
		|	LPAREN
			(	argument
				(	COMMA argument
				)*
			)?
			RPAREN
			NULLABLE_SUFFIX?
			(	hash_literal
			|	list_initializer
			)?
		)
	;

slicing_expression
	:	safe_atom
		any_slice_expr_value*
	;

list_initializer
	:	LBRACE list_items RBRACE
	;

literal
	:	integer_literal
	|	string_literal
	|	list_literal
	|	hash_literal
	|	closure_expression
	|	ast_literal_expression
	|	re_literal
	|	bool_literal
	|	null_literal
	|	self_literal
	|	super_literal
	|	double_literal
	|	timespan_literal
	;

self_literal
	:	SELF
	;

super_literal
	:	SUPER
	;

null_literal
	:	NULL
	;

bool_literal
	:	TRUE
	|	FALSE
	;

integer_literal
	:	SUBTRACT?
		(	INT
		|	LONG
		)
	;

string_literal
	:	expression_interpolation
	|	DOUBLE_QUOTED_STRING
	|	SINGLE_QUOTED_STRING
	|	TRIPLE_QUOTED_STRING
	|	BACKTICK_QUOTED_STRING
	;

any_expr_interpolation_item
	:	(	ESEPARATOR
			expression
			(	COLON? ID
			)?
			ESEPARATOR
		)
	;

expression_interpolation
	:	ESEPARATOR?
		any_expr_interpolation_item+
		ESEPARATOR?
	;

list_literal
	:	LBRACK list_items RBRACK
	;

list_items
	:	(	expression
			(	COMMA expression
			)*
			COMMA?
		)?
	;

hash_literal
	:	LBRACE
		(	expression_pair
			(	COMMA expression_pair
			)*
			COMMA?
		)?
		RBRACE
	;

expression_pair
	:	expression COLON expression
	;

re_literal
	:	RE_LITERAL
	;

double_literal
	:	SUBTRACT? DOUBLE
	|	FLOAT
	;

timespan_literal
	:	SUBTRACT? TIMESPAN
	;

expression_list
	:	(	expression
			(	COMMA expression
			)*
		)?
	;

argument_list
	:	(	argument
			(	COMMA argument
			)*
		)?
	;

argument
	:	expression_pair
	|	expression
	;

identifier
	:	macro_name
		(	DOT member
		)*
	;
