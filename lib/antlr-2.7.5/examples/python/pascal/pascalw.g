// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

//
// Pascal Tree Super Grammar (symtab.g derives from this)
//
// Adapted from,
// Pascal User Manual And Report (Second Edition-1978)
// Kathleen Jensen - Niklaus Wirth
//
// By
//
// Hakki Dogusan dogusanh@tr-net.net.tr
//
// Then significantly enhanced by Piet Schoutteten
// with some guidance by Terence Parr.  Piet added tree
// construction, and some tree walkers.
//

options {
    language=Python;
}

class pascal_w extends TreeParser;

options {
	importVocab = Pascal;
    ASTLabelType = "pascal.PascalAST";
}

program
    : programHeading
      block
    ;

programHeading
    : #(PROGRAM IDENT identifierList)
    | #(UNIT IDENT)
    ;

identifier
    : IDENT
    ;

block
    : ( labelDeclarationPart
      | constantDefinitionPart
      | typeDefinitionPart
      | variableDeclarationPart
      | procedureAndFunctionDeclarationPart
      | usesUnitsPart
      | IMPLEMENTATION
      )*
      compoundStatement
    ;

usesUnitsPart
    : #(USES identifierList)
    ;

labelDeclarationPart
    : #(LABEL ( label )+)
    ;

label
    : NUM_INT
    ;

constantDefinitionPart
    : #(CONST ( constantDefinition )+ )
    ;

constantDefinition
    : #(EQUAL IDENT constant)
    ;

constant
    : NUM_INT
    | NUM_REAL
    | #( PLUS
         ( NUM_INT
         | NUM_REAL
         | IDENT
         )
       )
    | #( MINUS
         ( NUM_INT
         | NUM_REAL
         | IDENT
         )
       )
    | IDENT
    | STRING_LITERAL
    | #(CHR (NUM_INT|NUM_REAL))
    ;

string
    : STRING_LITERAL
    ;

typeDefinitionPart
    : #(TYPE ( typeDefinition )+)
    ;

typeDefinition
    : #(TYPEDECL IDENT
      ( type 
      | #(FUNCTION (formalParameterList)? resultType)
      | #(PROCEDURE (formalParameterList)?)
      )
      )
    ;

type
    : #(SCALARTYPE identifierList)
    | #(DOTDOT constant constant)
    | typeIdentifier
    | structuredType
    | #(POINTER typeIdentifier)
    ;

typeIdentifier
    : IDENT
    | CHAR
    | BOOLEAN
    | INTEGER
    | REAL
    | #( STRING
         ( IDENT
         | NUM_INT
         | NUM_REAL
         |
         )
       )
    ;

structuredType
    : #(PACKED unpackedStructuredType)
    | unpackedStructuredType
    ;

unpackedStructuredType
    : arrayType
    | recordType
    | setType
    | fileType
    ;

/** Note here that the syntactic diff between brackets disappears.
 *  If the brackets mean different things semantically, we need
 *  two different alternatives here.
 */
arrayType
    : #(ARRAY typeList type)
    ;

typeList
    : #( TYPELIST ( type )+ )
    ;

recordType
    : #(RECORD fieldList)
    ;

fieldList
    : #( FIELDLIST
         ( fixedPart ( variantPart )?
         | variantPart
         )
       )
    ;

fixedPart
    : ( recordSection )+
    ;

recordSection
    : #(FIELD identifierList type)
    ;

variantPart
    : #( CASE tag ( variant )+ )
    ;

tag
    : #(VARIANT_TAG identifier typeIdentifier)
    | #(VARIANT_TAG_NO_ID typeIdentifier)
    ;

variant
    : #(VARIANT_CASE constList fieldList)
    ;

setType
    : #(SET type)
    ;

fileType
    : #(FILE (type)?)
    ;

/** Yields a list of VARDECL-rooted subtrees with VAR at the overall root */
variableDeclarationPart
    : #( VAR ( variableDeclaration )+ )
    ;

variableDeclaration
    : #(VARDECL identifierList type)
    ;

procedureAndFunctionDeclarationPart
    : procedureOrFunctionDeclaration
    ;

procedureOrFunctionDeclaration
    : procedureDeclaration
    | functionDeclaration
    ;

procedureDeclaration
    : #(PROCEDURE IDENT (formalParameterList)? block )
    ;

formalParameterList
    : #(ARGDECLS ( formalParameterSection )+)
    ;

formalParameterSection
    : parameterGroup
    | #(VAR parameterGroup)
    | #(FUNCTION parameterGroup)
    | #(PROCEDURE parameterGroup)
    ;

parameterGroup
    : #(ARGDECL identifierList typeIdentifier)
    ;

identifierList
    : #(IDLIST (IDENT)+)
    ;

constList
    : #(CONSTLIST ( constant )+)
    ;

functionDeclaration
    : #(FUNCTION IDENT (formalParameterList)? resultType block)
    ;

resultType
    : typeIdentifier
    ;

statement
    : #(COLON label unlabelledStatement)
    | unlabelledStatement
    ;

unlabelledStatement
    : simpleStatement
    | structuredStatement
    ;

simpleStatement
    : assignmentStatement
    | procedureStatement
    | gotoStatement
    ;

assignmentStatement
    : #(ASSIGN variable expression)
    ;

/** A variable is an id with a suffix and can look like:
 *  id
 *  id[expr,...]
 *  id.id
 *  id.id[expr,...]
 *  id^
 *  id^.id
 *  id^.id[expr,...]
 *  ...
 *
 *  LL has a really hard time with this construct as it's naturally
 *  left-recursive.  We have to turn into a simple loop rather than
 *  recursive loop, hence, the suffixes.  I keep in the same rule
 *  for easy tree construction.
 */
variable
    : #(LBRACK variable (expression)+)
    | #(LBRACK2 variable (expression)+)
    | #(DOT variable IDENT)
    | #(POINTER variable)
    | #(AT IDENT)
    | IDENT
    ;

expression
    : #(EQUAL expression expression)
    | #(NOT_EQUAL expression expression)
    | #(LT expression expression)
    | #(LE expression expression)
    | #(GE expression expression)
    | #(GT expression expression)
    | #(IN expression expression)
    | #(PLUS expression (expression)?)
    | #(MINUS expression (expression)?)
    | #(OR expression expression)
    | #(STAR expression expression)
    | #(SLASH expression expression)
    | #(DIV expression expression)
    | #(MOD expression expression)
    | #(AND expression expression)
    | #(NOT expression)
    | variable
    | functionDesignator
    | set
    | NUM_INT
    | NUM_REAL
    | #(CHR (NUM_INT|NUM_REAL))
    | string
    | NIL
    ;

functionDesignator
    : #(FUNC_CALL IDENT (parameterList)?)
    ;

parameterList
    : #( ARGLIST (actualParameter)+ )
    ;

set
    : #(SET (element)*)
    ;

element
    : #(DOTDOT expression expression)
    | expression
    ;

procedureStatement
    : #(PROC_CALL IDENT ( parameterList )?)
    ;

actualParameter
    : expression
    ;

gotoStatement
    : #(GOTO label)
    ;

structuredStatement
    : compoundStatement
    | conditionalStatement
    | repetetiveStatement
    | withStatement
    ;

compoundStatement
    : statements
    ;

statements
    : #(BLOCK (statement)*)
    ;

conditionalStatement
    : ifStatement
    | caseStatement
    ;

ifStatement
    : #(IF expression statement (statement)?)
    ;

caseStatement //pspsps ???
    : #(CASE expression
        ( caseListElement )+
        ( statements )?
       )
    ;

caseListElement
    : #(COLON constList statement)
    ;

repetetiveStatement
    : whileStatement
    | repeatStatement
    | forStatement
    ;

whileStatement
    : #(WHILE expression statement)
    ;

repeatStatement
    : #(REPEAT statements expression)
    ;

forStatement
    : #(FOR IDENT forList statement)
    ;

forList
    : #(TO initialValue finalValue)
    | #(DOWNTO initialValue finalValue)
    ;

initialValue
    : expression
    ;

finalValue
    : expression
    ;

withStatement
    : #(WITH recordVariableList statement)
    ;

recordVariableList
    : ( variable )+
    ;
