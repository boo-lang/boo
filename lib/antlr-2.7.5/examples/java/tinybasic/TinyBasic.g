header{
	package tinybasic;
	import java.util.*;
}
class TinyBasicParser extends Parser;
options {
	k = 4;				// two token lookahead
	exportVocab=TinyBasic;		// Call its vocabulary "TinyBasic"
	//codeGenMakeSwitchThreshold = 2;  // Some optimizations
	//codeGenBitsetTestThreshold = 3;
	defaultErrorHandler = false;     // Don't generate parser error handlers
	//analyzerDebug=true;
	buildAST = true;
}
{
	Context theContext=null;

}
imaginaryTokenDefinitions
	:	
		SLIST
		TYPE
		PROGRAM_DEF	SUBROUTINE_DEF	FUNCTION_DEF
		EXIT_MODULE
		PARAMETERS	PARAMETER_DEF
		LABELED_STAT	NUMBERED_STAT
		UNARY_MINUS	UNARY_PLUS
		CASE_GROUP	ARGLIST
		FOR_LOOP	FOR_FROM	FOR_TO
		FOR_BY		FOR_BY_ONE	FOR_BODY
		
		INT_FN_EXECUTE	FLT_FN_EXECUTE	STR_FN_EXECUTE
		SUB_EXECUTE
	
		EQ_COMP
		INDEX_OP	SUBSTRING_OP 	DOT
		ARRAY1D		ARRAY2D		ARRAY3D
		ARRAY1D_PROXY	ARRAY2D_PROXY	ARRAY3D_PROXY
		VAR_PROXY

		WHEN_ERROR_CALL	WHEN_ERROR_IN
		
		PRINT_ASCII	PRINT_TAB
		PRINT_NUMERIC	PRINT_STRING
		PRINT_COMMA	PRINT_SEMI
		IF_THEN_BLOCK	IF_BLOCK	ELSE_IF_BLOCK	ELSE_BLOCK
		CODE_BLOCK	CONDITION
	;
	
// Compilation Unit: In TinyBasic, this is a single file.  This is the start
//   rule for this parser
compilationUnit[Context context]
	{
		theContext=context; //new Context();
	}
	:
		// A compilation unit starts with an optional program definition
		(	programDefinition
		|	/* nothing */
		)

		// Next we have a series of zero or more sub/function blocks
		(	subroutineDefinition
		|	functionDefinition
		)*

		EOF!
	;


// PROGRAM ( parameter, parameter)
programDefinition
	options {defaultErrorHandler = true;} // let ANTLR handle errors
	{ Vector pVector=null; }
	:	"program"!
			{
				//#p.setType(PROGRAM_DEF,"PROGRAM_DEF");
				theContext.setProgramScope(new ProgramScope(theContext.getCurrentScope()));

			}
			pVector=parameters	eol!
		// now parse the body
			cb:procedureBlock
			quit:"end" 		eol!
			{
				#quit.setType(EXIT_MODULE);
				#programDefinition = #(#[PROGRAM_DEF,"PROGRAM_DEF"],#programDefinition);
				theContext.popScope();
			}

	;

// SUB IDENT ( parameter)*
subroutineDefinition
	options {defaultErrorHandler = true;} // let ANTLR handle errors
	{ Vector pVector=null; }
	:	p:"sub"! n:subName
			{
				theContext.pushScope(new SubroutineScope(theContext.getCurrentScope()));
			}
			pVector=params:parameters	eol!
		// now parse the body of the class
		cb:procedureBlock
		quit:"end" "sub"! eol!
		{
			#quit.setType(EXIT_MODULE);
			DTCodeType sub;
			#subroutineDefinition = #(#[SUBROUTINE_DEF,"SUBROUTINE_DEF"],#subroutineDefinition);
			sub=new DTSubroutine(#subroutineDefinition,
						#cb,theContext.getCurrentScope(),pVector,#n.getText());
			theContext.popScope();
			theContext.insertSubroutine(#n.getText(),sub);
		}

	;

// FUNCTION IDENT ( parameter)*
functionDefinition
	options {defaultErrorHandler = true;} // let ANTLR handle errors
	{
		int fnType=0;
		Vector pVector=null;
	}
	:	p:"function"^ fnType=n:newFunction {#p.setType(FUNCTION_DEF);}
			{
				theContext.pushScope(new FunctionScope(theContext.getCurrentScope()));
			}
			pVector=params:parameters	eol!
		// now parse the body of the class
		cb:procedureBlock
		quit:"end" "function"! eol!
		{
			#quit.setType(EXIT_MODULE);
			DTCodeType fnc;
			fnc=new DTFunction(fnType,#params,#cb,theContext.getCurrentScope(),pVector,#n.getText());
			#functionDefinition = #(#[FUNCTION_DEF,"FUNCTION_DEF"],#functionDefinition);
			theContext.popScope();
			theContext.insertFunction(#n.getText(),fnc);
		}
	;
	


//funcName
//	:
//		INT_FN
//	|	FLT_FN
//	|	STR_FN
//	;

newFunction returns [int t]
	{ t=0;}
	:
		INT_FN		{ t=INT_FN; }
	|	STR_FN		{ t=STR_FN; }
	|	FLT_FN		{ t=FLT_FN; }
	;



// This is the body of a procedure.  
procedureBlock
	:
		codeBlock
		//{#procedureBlock = #([OBJBLOCK, "OBJBLOCK"], #procedureBlock);}
	;


statement
	:
	nl
	(
		singleStatement
	|	ifStatements
	|	compoundStatement
	)
	;


parameters returns [ Vector v ]
	{ v=new Vector(); }
	:
		(	(LPAREN)=>
			LPAREN!	parameterDeclarationList[v] RPAREN!
		|
		)
		;


// A list of formal parameters
parameterDeclarationList [ Vector v]
	{ DTDataType tbd=null; }
	:	tbd=parameterDeclaration
			{
				v.addElement(tbd);
			}
			( COMMA! tbd=parameterDeclaration 
				{
					v.addElement(tbd);
				}
			)*
		{#parameterDeclarationList = #(#[PARAMETERS,"PARAMETERS"],
									#parameterDeclarationList);}
	;



parameterDeclaration returns [DTDataType arg]
		{
			int varType=0;
			arg=null;
		}
	:
		varType=v:newVariable
			(	LPAREN!	//d1:integerExpression
				(
					COMMA!	//d2:integerExpression
					(
						COMMA!	//d3:integerExpression
						{
							arg=new DTDataTypeProxy(varType,theContext.getCurrentScope(),3);
							//arg=new DTArray3DProxy(varType,theContext.getCurrentScope());
							//#parameterDeclaration = #([ARRAY3D_PROXY], #parameterDeclaration);
						}
					|
						{
							arg=new DTDataTypeProxy(varType,theContext.getCurrentScope(),2);
							//arg=new DTArray2DProxy(varType,theContext.getCurrentScope());
							//#parameterDeclaration = #([ARRAY2D_PROXY], #parameterDeclaration);
						}
					)
				|
					{
						arg=new DTDataTypeProxy(varType,theContext.getCurrentScope(),1);
						//arg=new DTArray1DProxy(varType,theContext.getCurrentScope());
						//#parameterDeclaration = #([ARRAY1D_PROXY], #parameterDeclaration);
					}
				)	RPAREN!
			|
				{
					arg=new DTDataTypeProxy(varType,theContext.getCurrentScope(),0);
					//#parameterDeclaration = #([VAR_PROXY], #parameterDeclaration);
				}
			)
			{
				#parameterDeclaration = #([VAR_PROXY], #parameterDeclaration);
				theContext.insertVariable(#v.getText(),arg);
			}
	;
	


compoundStatement
	:
		forNextBlock
	|	doUntilLoopBlock
	|	doLoopUntilBlock
	|	selectCaseBlock
	|	eventCompoundStatements
	;

ifThenBlock
	:
		ifBlock
			(
				options {
					warnWhenFollowAmbig = false;
				}
				:
				elseIfBlock
			)*
			
			(
				options {
					warnWhenFollowAmbig = false;
				}
				:
				elseBlock 
			)?
		endIfBlock
		{ #ifThenBlock = #(#[IF_THEN_BLOCK,"IF_THEN_BLOCK"],#ifThenBlock);}
	;

ifStatements
	:
	
		(ifStatement)=>	ifStatement
	|	ifThenBlock
	;

ifStatement
	:
		"if"!  condition "then"! singleStatement eol!
	;
	
ifBlock
	:
		"if"!  condition "then"! eol!
		codeBlock
		{ #ifBlock = #(#[IF_BLOCK,"IF_BLOCK"],#ifBlock);}
	;

elseIfBlock
	:
		nl ("else"! "if"! | "elseif"! ) condition "then"! eol!
		codeBlock
		{ #elseIfBlock = #(#[ELSE_IF_BLOCK,"ELSE_IF_BLOCK"],#elseIfBlock);}
	;

elseBlock
	:
		nl "else"! eol!
		codeBlock
		{ #elseBlock = #(#[ELSE_BLOCK,"ELSE_BLOCK"],#elseBlock);}
	;

endIfBlock
	:
		nl ("end"! "if"! | "endif"! ) eol!
	;

condition
	:
		relationalExpression
		{ #condition = #(#[CONDITION,"CONDITION"],#condition);}
	;
	
codeBlock
	:
		(
			options {
				warnWhenFollowAmbig = false;
			}
			:
			statement
		)*
		{#codeBlock = #(#[CODE_BLOCK,"CODE_BLOCK"],#codeBlock);}
	;

forNextBlock
	:
		"for"!	(
				// I=1    TO 2    (BY 1)?
				forFrom   forTo	  forBy        eol!
				forBody
			)
		{#forNextBlock = #(#[FOR_LOOP,"FOR_LOOP"],#forNextBlock);}
	;

// The initializer for a for loop
forFrom
	:	numericStore EQ^ numericExpression
		{#forFrom = #(#[FOR_FROM,"FOR_FROM"],#forFrom);}
	;

forTo
	:	 "to"! numericExpression
		{#forTo = #(#[FOR_TO,"FOR_TO"],#forTo);}
	;

forBy
	:
		(	"by"! numericExpression
			{#forBy = #(#[FOR_BY,"FOR_BY"],#forBy);}
		|
			{#forBy = #(#[FOR_BY_ONE,"FOR_BY_ONE"],#forBy);}
		)
	;

forBody
	:
				codeBlock
				nextStatement!
				{#forBody = #(#[FOR_BODY,"FOR_BODY"],#forBody);}
	;

nextStatement
	:
		nl "next" numericStore eol!
	;
doUntilLoopBlock
	:
		"do"! "until"^  condition eol!
				codeBlock
		nl "loop"! eol!
	;

doLoopUntilBlock
	:
		"do"^  eol!
				codeBlock
		nl "loop"! "until"!  condition eol!
	;

selectCaseBlock
	:
		"select"^ "case"! expression eol
			(casesGroup)*
		nl "end" "select" eol!
	;

singleStatement
	:
	(
		"library"^	STR_CONST
	|	"dim"^		dimensionedVariables
	|	"global"^	parameterDeclarationList[new Vector()]
	|	"beep"
	|	"chain"^	stringExpression	("with"	LPAREN!	argList	RPAREN!)?
	|	"gosub"^	lineLabel
	|	"goto"^	lineLabel
	|	callSubroutineStatement
	|	"return"^ (expression)?
	|	ex:"exit"^ "sub"!	{#ex.setType(EXIT_MODULE);}

	|	("let"!)?	assignmentExpression
	|	("on" numericExpression)=>
		"on"^ numericExpression	("goto"^	| "gosub"^ ) lineLabel  (COMMA! lineLabel)*
	|	eventSingleStatements
	|	"option"^	"base"	INT_CONST
	|	"out"^	integerExpression COMMA! integerExpression
	|	"pause"^	(numericExpression)?
	|	"redim"^	dimensionedVariables
	|	"poke"^
			integerExpression COMMA!
			integerExpression COMMA! 
			integerExpression
	|	"randomize"^	integerExpression
	|	graphicsOutput
	|	inputOutput
	|	line_stuff
	|	set_stuff
	)	eol!
	;


callSubroutineStatement
	:
		call:"call"^ subName (LPAREN! argList RPAREN!)?
		{ #call.setType(SUB_EXECUTE); }
	;
dimensionedVariables
	{ DTDataType av=null; int varType=0;}
	:
	(
		varType=v:newVariable	LPAREN!	d1:integerExpression
			(
				COMMA!	d2:integerExpression
				(
					COMMA!	d3:integerExpression
					{
						av=new DTArray3D(varType,theContext.getCurrentScope());
						#dimensionedVariables = #([ARRAY3D, "ARRAY3D"], #dimensionedVariables);
					}
				|
					{
						av=new DTArray2D(varType,theContext.getCurrentScope());
						#dimensionedVariables = #([ARRAY2D, "ARRAY2D"], #dimensionedVariables);
					}
				)
			|
				{
					av=new DTArray1D(varType,theContext.getCurrentScope());
					#dimensionedVariables = #([ARRAY1D, "ARRAY1D"], #dimensionedVariables);
				}
			)	RPAREN!
			{ theContext.insertVariable(#v.getText(),av);}
	)
	(
		COMMA dimensionedVariables
	)?
	;
	


lineLabel
	:
		INT_CONST
	|	IDENT
	;

nl
	:
	(
	options {
			warnWhenFollowAmbig = false;
		}
		:
		IDENT^ c:COLON! {#c.setType(LABELED_STAT);}
	|	INT_CONST^	{#c.setType(NUMBERED_STAT);}
	)?
	;

constant
	:
		stringConstant
	|	floatNumber
	;

binaryReadVariables
	:
		(	numericStore
		|	stringStore "until" integerExpression
		)	(COMMA binaryReadVariables)?
	;

printList
	:
		(
			tabExpression
		|	printString
		|	printNumeric
		)	(
				(	c:COMMA { #c.setType(PRINT_COMMA);}
				|	s:SEMI	{ #s.setType(PRINT_SEMI);}
				) (printList)?
			)?
	
	;
tabExpression
	:
		"tab"! LPAREN! numericExpression RPAREN!
		{ #tabExpression = #(#[PRINT_TAB,"PRINT_TAB"],#tabExpression);}
	;

printString
	:
		stringExpression	{ #printString = #(#[PRINT_STRING,"PRINT_STRING"],#printString);}
	;

printNumeric
	:
		numericExpression	{ #printNumeric = #(#[PRINT_NUMERIC,"PRINT_NUMERIC"],#printNumeric);}
	;
	
inputList
	:
		(	numericStore
		|	stringStore
		)	(COMMA inputList)?
	
	;
	
inputOutput
	:
		"close"^ (POUND! integerExpression)?
		//|	"cominfo"
	|	"data"^	constant (COMMA! constant)*
	|	"deletefile"	stringExpression
		//|	"fileinfo"
	|	"input"
		(
			"binary"	(chanNumber)? binaryReadVariables
		|	chanAndPrompt inputList
		)
	|	"open"	chanNumber stringExpression 
		(
			COMMA
			(
				"access"
					(
						"input"
					|	"output"
					|	"outin"
					|	"append"
					)
			|	"organization" 
					(
						"sequential"
					|	"random"
					|	"stream"
					|	"append"
					)
			|	"recsize" integerExpression
			)
		)+
	//|	"output"
	|	print_ascii
	|	"print"	"binary" (chanNumber)? printList
	|	"read"	inputList
	|	"restore"
	;

set_stuff
	:
		"set"
		(
			"timer"	numericExpression
		|	"loc"	LPAREN integerExpression COMMA integerExpression RPAREN
		|	(chanNumber)? specifier integerExpression
		)
	;
print_ascii
	:
		"print"!	(chanNumber)?	("using" stringExpression)? printList
		{#print_ascii = #([PRINT_ASCII, "PRINT_ASCII"], #print_ascii);}
	;	
specifier
	:
		"margin"
	|	"zonewidth"
	|	"address"
	|	"record"
	;

chanNumber
	:
		POUND	integerExpression COLON
	;
	
prompt
	:
		"prompt" stringExpression COLON
	;

chanAndPrompt
	:
		(chanNumber)? (prompt)? 
	;
casesGroup
	:
		aCase
		codeBlock
		{#casesGroup = #([CASE_GROUP, "CASE_GROUP"], #casesGroup);}
	;
	


integerArray
	:
		argArray
	;

symbolicAddress
	:
		stringExpression
	;

deviceAddress
	:
		(adapterAddress COMMA!)?	primaryAddress	(COMMA!	secondaryAddress)?
	;
	
primaryAddress
	:
		integerExpression
	;
	
secondaryAddress
	:
		integerExpression
	;
	

adapterAddress
	:
		stringExpression
	|	"@" integerExpression
	;


combinationAddress
	:
		(deviceAddress)=>	deviceAddress
	|	adapterAddress
		
	;

aCase
	:	"case"^ expression (COMMA! expression)*	eol!
	;



integerArrayVariable
	:
		integerVariable
	;

stringArrayVariable
	:
		stringVariable
	;

floatArrayVariable
	:
		floatVariable
	;
	

arrayVariable
	:
		integerArrayVariable
	|	stringArrayVariable
	|	floatArrayVariable
	;	

graphicsOutput
	:
		"brush"^	integerExpression
	|	"circle"^	LPAREN!	integerExpression COMMA integerExpression	RPAREN!
				COMMA	integerExpression ( COMMA integerExpression )?
	|	"clear"^	("metafileon" | "metafileoff" )?
	|	"ellipse"^	LPAREN!	integerExpression COMMA integerExpression	RPAREN!
			MINUS	LPAREN!	integerExpression COMMA integerExpression	RPAREN!
				( COMMA integerExpression )?
	|	"font"^		integerExpression
				( COMMA integerExpression ( COMMA integerExpression )? )? 
	|	"loc"^		integerStore COMMA integerStore
	|	"pen"^		integerExpression COMMA integerExpression COMMA integerExpression
	|	"picture"^	stringExpression
				COMMA LPAREN!	integerExpression COMMA integerExpression	RPAREN!
				( COMMA integerExpression )?
	|	"polyline"^	integerArrayVariable LPAREN COMMA RPAREN
				( COMMA integerExpression )?			
	|	"rectangle"^	LPAREN!	integerExpression COMMA integerExpression	RPAREN!
			MINUS	LPAREN!	integerExpression COMMA integerExpression	RPAREN!
				( COMMA integerExpression )?
	|	"screen"^	( "normal" | "condensed" | "display" | "zoom" | "unzoom" | "close_basic" )
	;
	
line_stuff	// ambiguity forced left factoring
	:
		"line"
			(
				"input" (chanNumber)? stringStore
			|	"enter"			combinationAddress
						(prompt)?	stringStore ("until" integerExpression)?
			|	(LPAREN!	integerExpression COMMA integerExpression	RPAREN!)?
				MINUS	LPAREN!	integerExpression COMMA integerExpression	RPAREN!
				( COMMA integerExpression )?
				
			)
	
	;



eventSingleStatements
	:
		"cause"		("error")?	integerExpression
	|	"cause"		"event"		integerExpression
	|	("disable" | "enable") ("srq"|"timer"|"gpib") ("discard")?
	|	("disable" | "enable") "event"	integerExpression ("discard")?
	|	"error"
			(
				"abort"	integerExpression
			|	"retry"
			|	"continue"
			|	"stop"
			)
	|	"on"
		(
			"event"	integerExpression
		|	"srq"
		|	"timer"
		|	"gpib"
		)	"call"	subName
	;

eventCompoundStatements
	:
		w:"when"^ "error"	
		(
			"call"^	subName	(LPAREN!	argList RPAREN!)? eol!
			{#w.setType(WHEN_ERROR_CALL);}
		|	"in"!		eol!
			{#w.setType(WHEN_ERROR_IN);}
				(singleStatement)+
			"use"^				eol!
				(singleStatement)+
			("end"! "when"! | "endwhen"!) 	eol
		)
	;


subName
	:
		IDENT
	;

expression
	:
		numericExpression
	|	stringExpression
	;

argList
	:	arg ( COMMA! arg )*
		{#argList = #(#[ARGLIST,"ARGLIST"], argList);}
	;
	
arg
	:
			//(variable LPAREN COMMA)=>
			//variable LPAREN COMMA {dimCount=2;} (  COMMA {dimCount++;} )* RPAREN
		(argArray)=>argArray
			//|	(variable LPAREN RPAREN)=>
			//variable LPAREN   RPAREN
	|	expression
	;

argArray
	:
		(variable LPAREN COMMA)=>
		v23:variable LPAREN! COMMA!
				( 	COMMA!
					{ #v23.setType(ARRAY3D); }
				| 
					{ #v23.setType(ARRAY2D); }
				) RPAREN!
	|	//(variable LPAREN RPAREN)=>
		v1:variable LPAREN   RPAREN
			{ #v1.setType(ARRAY1D); }
	;
	


// assignment expression (level 13)
assignmentExpression
	:
		stringStore		EQ^ 	stringExpression
	|	integerStore		EQ^	integerExpression
	|	floatStore		EQ^	numericExpression
	;

stringStore
	:
		(stringVariable LPAREN)=>
		{theContext.isArrayVariable(LT(1).getText())}?
		stringVariable	lp:LPAREN^	{#lp.setType(INDEX_OP);}
					indices
				RPAREN
	|	(stringVariable LBRACK)=>
		stringVariable	lb:LBRACK^	{#lb.setType(SUBSTRING_OP);}
					integerExpression COLON!  integerExpression
				RBRACK!
	|	stringVariable
	;
	
integerStore
	:
		( integerVariable LPAREN )=>
		{theContext.isArrayVariable(LT(1).getText())}?
		integerVariable	lp:LPAREN^	{#lp.setType(INDEX_OP);}
					indices
				RPAREN!
	|	integerVariable
	;
	
floatStore
	:
		( floatVariable LPAREN )=>
		{theContext.isArrayVariable(LT(1).getText())}?
		floatVariable	lp:LPAREN^	{#lp.setType(INDEX_OP);}
					indices
				RPAREN!
	|	floatVariable
	;
	
numericStore
	:
		integerStore
	|	floatStore
	;

stringVariable
	:
		STR_VAR
	;
	
integerVariable
	:
		INT_VAR
	;
	
floatVariable
	:
	(
		FLT_VAR
	|	IDENT
	)
	;


// boolean relational expressions (level 5)

relationalExpression
	:		relationalXORExpression
	;

relationalXORExpression
	:		relationalORExpression	(	"xor"^	relationalORExpression	)*
	;



relationalORExpression
	:		relationalANDExpression		(	"or"^ 	relationalANDExpression 	)*
	;


relationalANDExpression
	:		relationalNOTExpression		(	"and"^	relationalNOTExpression		)*
	;

relationalNOTExpression
	: ("not"^)?	primaryRelationalExpression
	;



primaryRelationalExpression
	:
		(numericExpression)=>
		numericExpression
			(	LT^
			|	GT^
			|	LE^
			|	GE^
			|	e1:EQ^	{#e1.setType( EQ_COMP );}
			|	NE_COMP^
			)
			numericExpression
	|	stringExpression
			(	LT^
			|	GT^
			|	LE^
			|	GE^
			|	e2:EQ^	{#e2.setType( EQ_COMP );}
			|	NE_COMP^
			)
			stringExpression
	|	LPAREN!	relationalExpression	RPAREN!
	;
	

numericValuedFunctionExpression
	:
		"abs"^		LPAREN!	numericExpression	RPAREN!
	|	"acos"^		LPAREN!	numericExpression	RPAREN!
	|	"asc"^		LPAREN!	stringExpression	RPAREN!
	|	"atn"^		LPAREN!	numericExpression	RPAREN!
	|	"cos"^		LPAREN!	numericExpression	RPAREN!
	|	"dround"^	LPAREN!	numericExpression COMMA! integerExpression	RPAREN!
	|	"errl"^
	|	"errn"^
	|	"exp"^		LPAREN!	numericExpression	RPAREN!
	|	"fract"^	LPAREN!	numericExpression	RPAREN!
	|	"get_event"^	LPAREN!	numericExpression	RPAREN!
	|	"in"^		LPAREN!	numericExpression	RPAREN!
	|	"instr"^	LPAREN!	stringExpression COMMA!	stringExpression	RPAREN!
	|	"int"^		LPAREN!	numericExpression	RPAREN!
	|	"ival"^		LPAREN!	stringExpression	RPAREN!
	|	"len"^		LPAREN!	stringExpression	RPAREN!
	|	"lgt"^		LPAREN!	numericExpression	RPAREN!
	|	"log"^		LPAREN!	numericExpression	RPAREN!
	|	"max"^		LPAREN!	(numericExpression)+	RPAREN!
	|	"min"^		LPAREN!	(numericExpression)+	RPAREN!
	|	"peek"^		LPAREN!	numericExpression COMMA! integerExpression	RPAREN!
	|	"pi"^
	|	"rnd"^
	|	"sgn"^		LPAREN!	numericExpression	RPAREN!
	|	"signed"^	LPAREN!	integerExpression	RPAREN!
	|	"sin"^		LPAREN!	numericExpression	RPAREN!
	|	"sqr"^		LPAREN!	numericExpression	RPAREN!
	|	"tan"^		LPAREN!	numericExpression	RPAREN!
	|	"time"^
	|	"ubound"^	LPAREN!	stringExpression	COMMA!	integerExpression	RPAREN!
	|	"val"^		LPAREN!	stringExpression	RPAREN!
	// BIT Functions
	|	"andb"^		LPAREN!	integerExpression COMMA! integerExpression		RPAREN!
	|	"orb"^		LPAREN!	integerExpression COMMA! integerExpression		RPAREN!
	|	"notb"^		LPAREN!	integerExpression	RPAREN!
	|	"shiftb"^	LPAREN!	integerExpression COMMA! integerExpression		RPAREN!
	|	"xorb"^		LPAREN!	integerExpression COMMA! integerExpression		RPAREN!
	;

integerExpression
	:
		numericExpression
	;

stringValuedFunctionExpression
	:
		"chr$"^		LPAREN!	integerExpression	RPAREN!
	|	"date$"^
	|	"dround$"^	LPAREN!	numericExpression	COMMA! integerExpression	RPAREN!
	|	"errl$"^
	|	"errn$"^	LPAREN!	integerExpression	RPAREN!
	|	"inchr$"^
	|	"ival$"^	LPAREN!	integerExpression	COMMA! integerExpression	RPAREN!
	|	"lwc$"^		LPAREN!	stringExpression	RPAREN!
	|	"rpt$"^		LPAREN!	stringExpression	COMMA! integerExpression	RPAREN!
	|	"time$"^
	|	"upc$"^		LPAREN!	stringExpression	RPAREN!
	|	"val$"^		LPAREN!	numericExpression	RPAREN!
	;

//numericExpression
//	:	numericAdditiveExpression
//	;


// binary addition/subtraction (level 3)
numericExpression
	:
		numericMultiplicativeExpression 
		(
			options {
				warnWhenFollowAmbig = false;
			}
			:
			(PLUS^ | MINUS^) numericMultiplicativeExpression
		)*
	;


// multiplication/division/modulo (level 2) 
numericMultiplicativeExpression
	:	numericExponentialExpression ((STAR^ | "div"^ | "mod"^ | SLASH^ ) numericExponentialExpression)*
	;

numericExponentialExpression
	:		numericUnaryExpression ( EXPO^  numericUnaryExpression)*
	;

numericUnaryExpression
:
		(
			p:PLUS^	{#p.setType(UNARY_PLUS);}
		|	m:MINUS^	{#m.setType(UNARY_PLUS);}
		)? 	numericPrimaryExpression
	;

numericPrimaryExpression
	:
		floatNumber
	|	numericStore
	|	//(FLT_FN|INT_FN)=>
		(	FLT_FN^		{#FLT_FN.setType(FLT_FN_EXECUTE);}
		|	INT_FN^		{#INT_FN.setType(INT_FN_EXECUTE);}
		)
			(
				(LPAREN)=>
				LPAREN	argList RPAREN
			|
			)
	|	numericValuedFunctionExpression
	|	e:LPAREN! numericExpression RPAREN!
	;

floatNumber
	:
		integerNumber
	|	FLT_CONST
	;


stringExpression
	:	stringConcatanateExpression
	;

// binary addition/subtraction (level 3)
stringConcatanateExpression
	:	stringPrimaryExpression ( AMPERSAND^ stringConcatanateExpression)?
	;


stringPrimaryExpression
	:
		stringStore
	|	stringConstant
	|	STR_FN^	((LPAREN)=>LPAREN!	argList	RPAREN!)? {#STR_FN.setType(STR_FN_EXECUTE);}
	|	stringValuedFunctionExpression
	;


indices
	:
		numericExpression (COMMA! indices)?
	;

stringConstant
	:
		STR_CONST
	;




integerNumber
	:
		INT_CONST
	|	BINARY_INTEGER
	|	OCTAL_INTEGER
	|	HEXADECIMAL_INTEGER
	;



newVariable returns [int t]
	{ t=0;}
	:
		INT_VAR		{ t=INT_VAR; }
	|	STR_VAR		{ t=STR_VAR; }
	|	FLT_VAR		{ t=FLT_VAR; }
	|	IDENT		{ t=FLT_VAR; }
	;
	
variable
	:
		numericStore
	|	stringStore
	;
	
	
	
eol!
	:
	(
	options {
			warnWhenFollowAmbig = false;
		}
		:
		EOL!
	)+
	;

//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
// The TinyBasic scanner
//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
class TinyBasicLexer extends Lexer;

options {
	importVocab=TinyBasic;  // call the vocabulary "TinyBasic"
	testLiterals=true;     // automatically test for literals
	k=6;                   // four characters of lookahead
	caseSensitive=false;
	caseSensitiveLiterals = false;
}



// OPERATORS
AMPERSAND		:	'&'		;
LPAREN			:	'('		;
RPAREN			:	')'		;
LBRACK			:	'['		;
RBRACK			:	']'		;
COLON			:	':'		;
COMMA			:	','		;
//DOT			:	'.'		;
EQ			:	'='		;
NE_COMP			:	"<>"		;
//BNOT			:	'~'		;
SLASH			:	'/'		;
PLUS			:	'+'		;
MINUS			:	'-'		;
STAR			:	'*'		;
GE			:	">="		;
GT			:	">"		;
LE			:	"<="		;
LT			:	'<'		;
SEMI			:	';'		;
POUND			:	'#'		;

	
BINARY_INTEGER
	:
		"&b" ('0' | '1' ) +
	;

OCTAL_INTEGER
	:
		"&o" ('0'..'7' ) +
	;

HEXADECIMAL_INTEGER
	:
		"&h" ('0'..'9' | 'a'..'f' ) +
	;

// Whitespace -- ignored
WS	:
		(	' '
		|	'\t'
		|	'\f'
		)
		{ _ttype = Token.SKIP; }
	;

EOL
	:
		(	"\r\n"  // Evil DOS
		|	'\r'    // Macintosh
		|	'\n'    // Unix (the right way)
		)
		{ newline(); }
	;

// Single-line comments
SL_COMMENT
	:	'!'
		(~('\n'|'\r'))*
		//('\n'|'\r'('\n')?)
		{
			$setType(Token.SKIP);
			//newline();
		}
		
	;


// character literals
CHAR_LITERAL
	:	'\'' ( (ESCc)=> ESCc | ~'\'' ) '\''
	;

// string literals
STR_CONST
	:	'"'! ( (ESCs)=> ESCs | (ESCqs)=> ESCqs | ~('"'))* '"'!
	;

protected
ESCc
	:	'<'	('0'..'9')+ '>'
	;

protected
ESCs
	:	"<<"	('0'..'9')+ ">>"
	;

protected
ESCqs
	:
		'"' '"'!
	;
// hexadecimal digit (again, note it's protected!)
protected
HEX_DIGIT
	:	('0'..'9'|'a'..'f')
	;


// a dummy rule to force vocabulary to be all characters (except special
//   ones that ANTLR uses internally (0 to 2)
protected
VOCAB
	:	'\3'..'\377'
	;


// an identifier.  Note that testLiterals is set to true!  This means
// that after we match the rule, we look in the literals table to see
// if it's a literal or really an identifer
IDENT
	options {testLiterals=true;}
	:	('a'..'z') ('a'..'z'|'0'..'9'|'_'|'.')*
			(
				'$'
			 	{ 
					if($getText.substring(0,2).toLowerCase().equals("fn")){
						_ttype=STR_FN;
					} else {
						_ttype=STR_VAR;
					}
				}
			|	'%'
			 	{ 
					if($getText.substring(0,2).toLowerCase().equals("fn")){
						_ttype=INT_FN;
					} else {
						_ttype=INT_VAR;
					}
				}
			|	'#'
			 	{ 
					if($getText.substring(0,2).toLowerCase().equals("fn")){
						_ttype=FLT_FN;
					} else {
						_ttype=FLT_VAR;
					}
				}
			|
			 	{ 
					if($getText.substring(0,2).toLowerCase().equals("fn")){
						_ttype=FLT_FN;
					//} else {
					//	_ttype=FLT_VAR;
					}
				}

			)
	;


// a numeric literal
INT_CONST
	{boolean isDecimal=false;}
	:	'.' {_ttype = DOT;}
			(('0'..'9')+ (EXPONENT)? (FLT_SUFFIX)? { _ttype = FLT_CONST; })?
	|	(	'0' {isDecimal = true;} // special case for just '0'
			(	('x')
				(											// hex
					// the 'e'|'E' and float suffix stuff look
					// like hex digits, hence the (...)+ doesn't
					// know when to stop: ambig.  ANTLR resolves
					// it correctly by matching immediately.  It
					// is therefor ok to hush warning.
					options {
						warnWhenFollowAmbig=false;
					}
				:	HEX_DIGIT
				)+
			|	('0'..'7')+									// octal
			)?
		|	('1'..'9') ('0'..'9')*  {isDecimal=true;}		// non-zero decimal
		)
		(	('l')
		
		// only check to see if it's a float if looks like decimal so far
		|	{isDecimal}?
			(	'.' ('0'..'9')* (EXPONENT)? (FLT_SUFFIX)?
			|	EXPONENT (FLT_SUFFIX)?
			|	FLT_SUFFIX
			)
			{ _ttype = FLT_CONST; }
		)?
	;


// a couple protected methods to assist in matching floating point numbers
protected
EXPONENT
	:	('e') ('+'|'-')? ('0'..'9')+
	;


protected
FLT_SUFFIX
	:	'f'|'d'
	;

