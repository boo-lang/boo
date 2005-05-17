// $ANTLR 2.7.2: "pascal.tree.g" -> "PascalTreeParserSuper.java"$

import antlr.TreeParser;
import antlr.Token;
import antlr.collections.AST;
import antlr.RecognitionException;
import antlr.ANTLRException;
import antlr.NoViableAltException;
import antlr.MismatchedTokenException;
import antlr.SemanticException;
import antlr.collections.impl.BitSet;
import antlr.ASTPair;
import antlr.collections.impl.ASTArray;


public class PascalTreeParserSuper extends antlr.TreeParser       implements PascalTreeParserSuperTokenTypes
 {
public PascalTreeParserSuper() {
	tokenNames = _tokenNames;
}

	public final void program(AST _t) throws RecognitionException {
		
		PascalAST program_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			programHeading(_t);
			_t = _retTree;
			block(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void programHeading(AST _t) throws RecognitionException {
		
		PascalAST programHeading_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case PROGRAM:
			{
				AST __t3 = _t;
				PascalAST tmp1_AST_in = (PascalAST)_t;
				match(_t,PROGRAM);
				_t = _t.getFirstChild();
				PascalAST tmp2_AST_in = (PascalAST)_t;
				match(_t,IDENT);
				_t = _t.getNextSibling();
				identifierList(_t);
				_t = _retTree;
				_t = __t3;
				_t = _t.getNextSibling();
				break;
			}
			case UNIT:
			{
				AST __t4 = _t;
				PascalAST tmp3_AST_in = (PascalAST)_t;
				match(_t,UNIT);
				_t = _t.getFirstChild();
				PascalAST tmp4_AST_in = (PascalAST)_t;
				match(_t,IDENT);
				_t = _t.getNextSibling();
				_t = __t4;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void block(AST _t) throws RecognitionException {
		
		PascalAST block_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			{
			_loop8:
			do {
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case LABEL:
				{
					labelDeclarationPart(_t);
					_t = _retTree;
					break;
				}
				case CONST:
				{
					constantDefinitionPart(_t);
					_t = _retTree;
					break;
				}
				case TYPE:
				{
					typeDefinitionPart(_t);
					_t = _retTree;
					break;
				}
				case VAR:
				{
					variableDeclarationPart(_t);
					_t = _retTree;
					break;
				}
				case FUNCTION:
				case PROCEDURE:
				{
					procedureAndFunctionDeclarationPart(_t);
					_t = _retTree;
					break;
				}
				case USES:
				{
					usesUnitsPart(_t);
					_t = _retTree;
					break;
				}
				case IMPLEMENTATION:
				{
					PascalAST tmp5_AST_in = (PascalAST)_t;
					match(_t,IMPLEMENTATION);
					_t = _t.getNextSibling();
					break;
				}
				default:
				{
					break _loop8;
				}
				}
			} while (true);
			}
			compoundStatement(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void identifierList(AST _t) throws RecognitionException {
		
		PascalAST identifierList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t104 = _t;
			PascalAST tmp6_AST_in = (PascalAST)_t;
			match(_t,IDLIST);
			_t = _t.getFirstChild();
			{
			int _cnt106=0;
			_loop106:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==IDENT)) {
					PascalAST tmp7_AST_in = (PascalAST)_t;
					match(_t,IDENT);
					_t = _t.getNextSibling();
				}
				else {
					if ( _cnt106>=1 ) { break _loop106; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt106++;
			} while (true);
			}
			_t = __t104;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void identifier(AST _t) throws RecognitionException {
		
		PascalAST identifier_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			PascalAST tmp8_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void labelDeclarationPart(AST _t) throws RecognitionException {
		
		PascalAST labelDeclarationPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t12 = _t;
			PascalAST tmp9_AST_in = (PascalAST)_t;
			match(_t,LABEL);
			_t = _t.getFirstChild();
			{
			int _cnt14=0;
			_loop14:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==NUM_INT)) {
					label(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt14>=1 ) { break _loop14; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt14++;
			} while (true);
			}
			_t = __t12;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void constantDefinitionPart(AST _t) throws RecognitionException {
		
		PascalAST constantDefinitionPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t17 = _t;
			PascalAST tmp10_AST_in = (PascalAST)_t;
			match(_t,CONST);
			_t = _t.getFirstChild();
			{
			int _cnt19=0;
			_loop19:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==EQUAL)) {
					constantDefinition(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt19>=1 ) { break _loop19; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt19++;
			} while (true);
			}
			_t = __t17;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void typeDefinitionPart(AST _t) throws RecognitionException {
		
		PascalAST typeDefinitionPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t31 = _t;
			PascalAST tmp11_AST_in = (PascalAST)_t;
			match(_t,TYPE);
			_t = _t.getFirstChild();
			{
			int _cnt33=0;
			_loop33:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==TYPEDECL)) {
					typeDefinition(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt33>=1 ) { break _loop33; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt33++;
			} while (true);
			}
			_t = __t31;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
/** Yields a list of VARDECL-rooted subtrees with VAR at the overall root */
	public final void variableDeclarationPart(AST _t) throws RecognitionException {
		
		PascalAST variableDeclarationPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t83 = _t;
			PascalAST tmp12_AST_in = (PascalAST)_t;
			match(_t,VAR);
			_t = _t.getFirstChild();
			{
			int _cnt85=0;
			_loop85:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==VARDECL)) {
					variableDeclaration(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt85>=1 ) { break _loop85; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt85++;
			} while (true);
			}
			_t = __t83;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void procedureAndFunctionDeclarationPart(AST _t) throws RecognitionException {
		
		PascalAST procedureAndFunctionDeclarationPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			procedureOrFunctionDeclaration(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void usesUnitsPart(AST _t) throws RecognitionException {
		
		PascalAST usesUnitsPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t10 = _t;
			PascalAST tmp13_AST_in = (PascalAST)_t;
			match(_t,USES);
			_t = _t.getFirstChild();
			identifierList(_t);
			_t = _retTree;
			_t = __t10;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void compoundStatement(AST _t) throws RecognitionException {
		
		PascalAST compoundStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			statements(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void label(AST _t) throws RecognitionException {
		
		PascalAST label_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			PascalAST tmp14_AST_in = (PascalAST)_t;
			match(_t,NUM_INT);
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void constantDefinition(AST _t) throws RecognitionException {
		
		PascalAST constantDefinition_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t21 = _t;
			PascalAST tmp15_AST_in = (PascalAST)_t;
			match(_t,EQUAL);
			_t = _t.getFirstChild();
			PascalAST tmp16_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
			constant(_t);
			_t = _retTree;
			_t = __t21;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void constant(AST _t) throws RecognitionException {
		
		PascalAST constant_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case NUM_INT:
			{
				PascalAST tmp17_AST_in = (PascalAST)_t;
				match(_t,NUM_INT);
				_t = _t.getNextSibling();
				break;
			}
			case NUM_REAL:
			{
				PascalAST tmp18_AST_in = (PascalAST)_t;
				match(_t,NUM_REAL);
				_t = _t.getNextSibling();
				break;
			}
			case PLUS:
			{
				AST __t23 = _t;
				PascalAST tmp19_AST_in = (PascalAST)_t;
				match(_t,PLUS);
				_t = _t.getFirstChild();
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case NUM_INT:
				{
					PascalAST tmp20_AST_in = (PascalAST)_t;
					match(_t,NUM_INT);
					_t = _t.getNextSibling();
					break;
				}
				case NUM_REAL:
				{
					PascalAST tmp21_AST_in = (PascalAST)_t;
					match(_t,NUM_REAL);
					_t = _t.getNextSibling();
					break;
				}
				case IDENT:
				{
					PascalAST tmp22_AST_in = (PascalAST)_t;
					match(_t,IDENT);
					_t = _t.getNextSibling();
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t23;
				_t = _t.getNextSibling();
				break;
			}
			case MINUS:
			{
				AST __t25 = _t;
				PascalAST tmp23_AST_in = (PascalAST)_t;
				match(_t,MINUS);
				_t = _t.getFirstChild();
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case NUM_INT:
				{
					PascalAST tmp24_AST_in = (PascalAST)_t;
					match(_t,NUM_INT);
					_t = _t.getNextSibling();
					break;
				}
				case NUM_REAL:
				{
					PascalAST tmp25_AST_in = (PascalAST)_t;
					match(_t,NUM_REAL);
					_t = _t.getNextSibling();
					break;
				}
				case IDENT:
				{
					PascalAST tmp26_AST_in = (PascalAST)_t;
					match(_t,IDENT);
					_t = _t.getNextSibling();
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t25;
				_t = _t.getNextSibling();
				break;
			}
			case IDENT:
			{
				PascalAST tmp27_AST_in = (PascalAST)_t;
				match(_t,IDENT);
				_t = _t.getNextSibling();
				break;
			}
			case STRING_LITERAL:
			{
				PascalAST tmp28_AST_in = (PascalAST)_t;
				match(_t,STRING_LITERAL);
				_t = _t.getNextSibling();
				break;
			}
			case CHR:
			{
				AST __t27 = _t;
				PascalAST tmp29_AST_in = (PascalAST)_t;
				match(_t,CHR);
				_t = _t.getFirstChild();
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case NUM_INT:
				{
					PascalAST tmp30_AST_in = (PascalAST)_t;
					match(_t,NUM_INT);
					_t = _t.getNextSibling();
					break;
				}
				case NUM_REAL:
				{
					PascalAST tmp31_AST_in = (PascalAST)_t;
					match(_t,NUM_REAL);
					_t = _t.getNextSibling();
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t27;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void string(AST _t) throws RecognitionException {
		
		PascalAST string_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			PascalAST tmp32_AST_in = (PascalAST)_t;
			match(_t,STRING_LITERAL);
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void typeDefinition(AST _t) throws RecognitionException {
		
		PascalAST typeDefinition_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t35 = _t;
			PascalAST tmp33_AST_in = (PascalAST)_t;
			match(_t,TYPEDECL);
			_t = _t.getFirstChild();
			PascalAST tmp34_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case SCALARTYPE:
			case IDENT:
			case DOTDOT:
			case CHAR:
			case BOOLEAN:
			case INTEGER:
			case REAL:
			case STRING:
			case PACKED:
			case ARRAY:
			case RECORD:
			case SET:
			case FILE:
			case POINTER:
			{
				type(_t);
				_t = _retTree;
				break;
			}
			case FUNCTION:
			{
				AST __t37 = _t;
				PascalAST tmp35_AST_in = (PascalAST)_t;
				match(_t,FUNCTION);
				_t = _t.getFirstChild();
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case ARGDECLS:
				{
					formalParameterList(_t);
					_t = _retTree;
					break;
				}
				case IDENT:
				case CHAR:
				case BOOLEAN:
				case INTEGER:
				case REAL:
				case STRING:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				resultType(_t);
				_t = _retTree;
				_t = __t37;
				_t = _t.getNextSibling();
				break;
			}
			case PROCEDURE:
			{
				AST __t39 = _t;
				PascalAST tmp36_AST_in = (PascalAST)_t;
				match(_t,PROCEDURE);
				_t = _t.getFirstChild();
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case ARGDECLS:
				{
					formalParameterList(_t);
					_t = _retTree;
					break;
				}
				case 3:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t39;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			_t = __t35;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void type(AST _t) throws RecognitionException {
		
		PascalAST type_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case SCALARTYPE:
			{
				AST __t42 = _t;
				PascalAST tmp37_AST_in = (PascalAST)_t;
				match(_t,SCALARTYPE);
				_t = _t.getFirstChild();
				identifierList(_t);
				_t = _retTree;
				_t = __t42;
				_t = _t.getNextSibling();
				break;
			}
			case DOTDOT:
			{
				AST __t43 = _t;
				PascalAST tmp38_AST_in = (PascalAST)_t;
				match(_t,DOTDOT);
				_t = _t.getFirstChild();
				constant(_t);
				_t = _retTree;
				constant(_t);
				_t = _retTree;
				_t = __t43;
				_t = _t.getNextSibling();
				break;
			}
			case IDENT:
			case CHAR:
			case BOOLEAN:
			case INTEGER:
			case REAL:
			case STRING:
			{
				typeIdentifier(_t);
				_t = _retTree;
				break;
			}
			case PACKED:
			case ARRAY:
			case RECORD:
			case SET:
			case FILE:
			{
				structuredType(_t);
				_t = _retTree;
				break;
			}
			case POINTER:
			{
				AST __t44 = _t;
				PascalAST tmp39_AST_in = (PascalAST)_t;
				match(_t,POINTER);
				_t = _t.getFirstChild();
				typeIdentifier(_t);
				_t = _retTree;
				_t = __t44;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void formalParameterList(AST _t) throws RecognitionException {
		
		PascalAST formalParameterList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t94 = _t;
			PascalAST tmp40_AST_in = (PascalAST)_t;
			match(_t,ARGDECLS);
			_t = _t.getFirstChild();
			{
			int _cnt96=0;
			_loop96:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_tokenSet_0.member(_t.getType()))) {
					formalParameterSection(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt96>=1 ) { break _loop96; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt96++;
			} while (true);
			}
			_t = __t94;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void resultType(AST _t) throws RecognitionException {
		
		PascalAST resultType_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			typeIdentifier(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void typeIdentifier(AST _t) throws RecognitionException {
		
		PascalAST typeIdentifier_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case IDENT:
			{
				PascalAST tmp41_AST_in = (PascalAST)_t;
				match(_t,IDENT);
				_t = _t.getNextSibling();
				break;
			}
			case CHAR:
			{
				PascalAST tmp42_AST_in = (PascalAST)_t;
				match(_t,CHAR);
				_t = _t.getNextSibling();
				break;
			}
			case BOOLEAN:
			{
				PascalAST tmp43_AST_in = (PascalAST)_t;
				match(_t,BOOLEAN);
				_t = _t.getNextSibling();
				break;
			}
			case INTEGER:
			{
				PascalAST tmp44_AST_in = (PascalAST)_t;
				match(_t,INTEGER);
				_t = _t.getNextSibling();
				break;
			}
			case REAL:
			{
				PascalAST tmp45_AST_in = (PascalAST)_t;
				match(_t,REAL);
				_t = _t.getNextSibling();
				break;
			}
			case STRING:
			{
				AST __t46 = _t;
				PascalAST tmp46_AST_in = (PascalAST)_t;
				match(_t,STRING);
				_t = _t.getFirstChild();
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case IDENT:
				{
					PascalAST tmp47_AST_in = (PascalAST)_t;
					match(_t,IDENT);
					_t = _t.getNextSibling();
					break;
				}
				case NUM_INT:
				{
					PascalAST tmp48_AST_in = (PascalAST)_t;
					match(_t,NUM_INT);
					_t = _t.getNextSibling();
					break;
				}
				case NUM_REAL:
				{
					PascalAST tmp49_AST_in = (PascalAST)_t;
					match(_t,NUM_REAL);
					_t = _t.getNextSibling();
					break;
				}
				case 3:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t46;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void structuredType(AST _t) throws RecognitionException {
		
		PascalAST structuredType_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case PACKED:
			{
				AST __t49 = _t;
				PascalAST tmp50_AST_in = (PascalAST)_t;
				match(_t,PACKED);
				_t = _t.getFirstChild();
				unpackedStructuredType(_t);
				_t = _retTree;
				_t = __t49;
				_t = _t.getNextSibling();
				break;
			}
			case ARRAY:
			case RECORD:
			case SET:
			case FILE:
			{
				unpackedStructuredType(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void unpackedStructuredType(AST _t) throws RecognitionException {
		
		PascalAST unpackedStructuredType_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case ARRAY:
			{
				arrayType(_t);
				_t = _retTree;
				break;
			}
			case RECORD:
			{
				recordType(_t);
				_t = _retTree;
				break;
			}
			case SET:
			{
				setType(_t);
				_t = _retTree;
				break;
			}
			case FILE:
			{
				fileType(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
/** Note here that the syntactic diff between brackets disappears.
 *  If the brackets mean different things semantically, we need
 *  two different alternatives here.
 */
	public final void arrayType(AST _t) throws RecognitionException {
		
		PascalAST arrayType_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t52 = _t;
			PascalAST tmp51_AST_in = (PascalAST)_t;
			match(_t,ARRAY);
			_t = _t.getFirstChild();
			typeList(_t);
			_t = _retTree;
			type(_t);
			_t = _retTree;
			_t = __t52;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void recordType(AST _t) throws RecognitionException {
		
		PascalAST recordType_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t58 = _t;
			PascalAST tmp52_AST_in = (PascalAST)_t;
			match(_t,RECORD);
			_t = _t.getFirstChild();
			fieldList(_t);
			_t = _retTree;
			_t = __t58;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void setType(AST _t) throws RecognitionException {
		
		PascalAST setType_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t78 = _t;
			PascalAST tmp53_AST_in = (PascalAST)_t;
			match(_t,SET);
			_t = _t.getFirstChild();
			type(_t);
			_t = _retTree;
			_t = __t78;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void fileType(AST _t) throws RecognitionException {
		
		PascalAST fileType_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t80 = _t;
			PascalAST tmp54_AST_in = (PascalAST)_t;
			match(_t,FILE);
			_t = _t.getFirstChild();
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case SCALARTYPE:
			case IDENT:
			case DOTDOT:
			case CHAR:
			case BOOLEAN:
			case INTEGER:
			case REAL:
			case STRING:
			case PACKED:
			case ARRAY:
			case RECORD:
			case SET:
			case FILE:
			case POINTER:
			{
				type(_t);
				_t = _retTree;
				break;
			}
			case 3:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			_t = __t80;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void typeList(AST _t) throws RecognitionException {
		
		PascalAST typeList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t54 = _t;
			PascalAST tmp55_AST_in = (PascalAST)_t;
			match(_t,TYPELIST);
			_t = _t.getFirstChild();
			{
			int _cnt56=0;
			_loop56:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_tokenSet_1.member(_t.getType()))) {
					type(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt56>=1 ) { break _loop56; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt56++;
			} while (true);
			}
			_t = __t54;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void fieldList(AST _t) throws RecognitionException {
		
		PascalAST fieldList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t60 = _t;
			PascalAST tmp56_AST_in = (PascalAST)_t;
			match(_t,FIELDLIST);
			_t = _t.getFirstChild();
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case FIELD:
			{
				fixedPart(_t);
				_t = _retTree;
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case CASE:
				{
					variantPart(_t);
					_t = _retTree;
					break;
				}
				case 3:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				break;
			}
			case CASE:
			{
				variantPart(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			_t = __t60;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void fixedPart(AST _t) throws RecognitionException {
		
		PascalAST fixedPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			{
			int _cnt65=0;
			_loop65:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==FIELD)) {
					recordSection(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt65>=1 ) { break _loop65; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt65++;
			} while (true);
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void variantPart(AST _t) throws RecognitionException {
		
		PascalAST variantPart_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t69 = _t;
			PascalAST tmp57_AST_in = (PascalAST)_t;
			match(_t,CASE);
			_t = _t.getFirstChild();
			tag(_t);
			_t = _retTree;
			{
			int _cnt71=0;
			_loop71:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==VARIANT_CASE)) {
					variant(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt71>=1 ) { break _loop71; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt71++;
			} while (true);
			}
			_t = __t69;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void recordSection(AST _t) throws RecognitionException {
		
		PascalAST recordSection_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t67 = _t;
			PascalAST tmp58_AST_in = (PascalAST)_t;
			match(_t,FIELD);
			_t = _t.getFirstChild();
			identifierList(_t);
			_t = _retTree;
			type(_t);
			_t = _retTree;
			_t = __t67;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void tag(AST _t) throws RecognitionException {
		
		PascalAST tag_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case VARIANT_TAG:
			{
				AST __t73 = _t;
				PascalAST tmp59_AST_in = (PascalAST)_t;
				match(_t,VARIANT_TAG);
				_t = _t.getFirstChild();
				identifier(_t);
				_t = _retTree;
				typeIdentifier(_t);
				_t = _retTree;
				_t = __t73;
				_t = _t.getNextSibling();
				break;
			}
			case VARIANT_TAG_NO_ID:
			{
				AST __t74 = _t;
				PascalAST tmp60_AST_in = (PascalAST)_t;
				match(_t,VARIANT_TAG_NO_ID);
				_t = _t.getFirstChild();
				typeIdentifier(_t);
				_t = _retTree;
				_t = __t74;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void variant(AST _t) throws RecognitionException {
		
		PascalAST variant_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t76 = _t;
			PascalAST tmp61_AST_in = (PascalAST)_t;
			match(_t,VARIANT_CASE);
			_t = _t.getFirstChild();
			constList(_t);
			_t = _retTree;
			fieldList(_t);
			_t = _retTree;
			_t = __t76;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void constList(AST _t) throws RecognitionException {
		
		PascalAST constList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t108 = _t;
			PascalAST tmp62_AST_in = (PascalAST)_t;
			match(_t,CONSTLIST);
			_t = _t.getFirstChild();
			{
			int _cnt110=0;
			_loop110:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_tokenSet_2.member(_t.getType()))) {
					constant(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt110>=1 ) { break _loop110; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt110++;
			} while (true);
			}
			_t = __t108;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void variableDeclaration(AST _t) throws RecognitionException {
		
		PascalAST variableDeclaration_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t87 = _t;
			PascalAST tmp63_AST_in = (PascalAST)_t;
			match(_t,VARDECL);
			_t = _t.getFirstChild();
			identifierList(_t);
			_t = _retTree;
			type(_t);
			_t = _retTree;
			_t = __t87;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void procedureOrFunctionDeclaration(AST _t) throws RecognitionException {
		
		PascalAST procedureOrFunctionDeclaration_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case PROCEDURE:
			{
				procedureDeclaration(_t);
				_t = _retTree;
				break;
			}
			case FUNCTION:
			{
				functionDeclaration(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void procedureDeclaration(AST _t) throws RecognitionException {
		
		PascalAST procedureDeclaration_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t91 = _t;
			PascalAST tmp64_AST_in = (PascalAST)_t;
			match(_t,PROCEDURE);
			_t = _t.getFirstChild();
			PascalAST tmp65_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case ARGDECLS:
			{
				formalParameterList(_t);
				_t = _retTree;
				break;
			}
			case BLOCK:
			case IMPLEMENTATION:
			case USES:
			case LABEL:
			case CONST:
			case TYPE:
			case FUNCTION:
			case PROCEDURE:
			case VAR:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			block(_t);
			_t = _retTree;
			_t = __t91;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void functionDeclaration(AST _t) throws RecognitionException {
		
		PascalAST functionDeclaration_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t112 = _t;
			PascalAST tmp66_AST_in = (PascalAST)_t;
			match(_t,FUNCTION);
			_t = _t.getFirstChild();
			PascalAST tmp67_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case ARGDECLS:
			{
				formalParameterList(_t);
				_t = _retTree;
				break;
			}
			case IDENT:
			case CHAR:
			case BOOLEAN:
			case INTEGER:
			case REAL:
			case STRING:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			resultType(_t);
			_t = _retTree;
			block(_t);
			_t = _retTree;
			_t = __t112;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void formalParameterSection(AST _t) throws RecognitionException {
		
		PascalAST formalParameterSection_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case ARGDECL:
			{
				parameterGroup(_t);
				_t = _retTree;
				break;
			}
			case VAR:
			{
				AST __t98 = _t;
				PascalAST tmp68_AST_in = (PascalAST)_t;
				match(_t,VAR);
				_t = _t.getFirstChild();
				parameterGroup(_t);
				_t = _retTree;
				_t = __t98;
				_t = _t.getNextSibling();
				break;
			}
			case FUNCTION:
			{
				AST __t99 = _t;
				PascalAST tmp69_AST_in = (PascalAST)_t;
				match(_t,FUNCTION);
				_t = _t.getFirstChild();
				parameterGroup(_t);
				_t = _retTree;
				_t = __t99;
				_t = _t.getNextSibling();
				break;
			}
			case PROCEDURE:
			{
				AST __t100 = _t;
				PascalAST tmp70_AST_in = (PascalAST)_t;
				match(_t,PROCEDURE);
				_t = _t.getFirstChild();
				parameterGroup(_t);
				_t = _retTree;
				_t = __t100;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void parameterGroup(AST _t) throws RecognitionException {
		
		PascalAST parameterGroup_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t102 = _t;
			PascalAST tmp71_AST_in = (PascalAST)_t;
			match(_t,ARGDECL);
			_t = _t.getFirstChild();
			identifierList(_t);
			_t = _retTree;
			typeIdentifier(_t);
			_t = _retTree;
			_t = __t102;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void statement(AST _t) throws RecognitionException {
		
		PascalAST statement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case COLON:
			{
				AST __t116 = _t;
				PascalAST tmp72_AST_in = (PascalAST)_t;
				match(_t,COLON);
				_t = _t.getFirstChild();
				label(_t);
				_t = _retTree;
				unlabelledStatement(_t);
				_t = _retTree;
				_t = __t116;
				_t = _t.getNextSibling();
				break;
			}
			case BLOCK:
			case PROC_CALL:
			case CASE:
			case ASSIGN:
			case GOTO:
			case IF:
			case WHILE:
			case REPEAT:
			case FOR:
			case WITH:
			{
				unlabelledStatement(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void unlabelledStatement(AST _t) throws RecognitionException {
		
		PascalAST unlabelledStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case PROC_CALL:
			case ASSIGN:
			case GOTO:
			{
				simpleStatement(_t);
				_t = _retTree;
				break;
			}
			case BLOCK:
			case CASE:
			case IF:
			case WHILE:
			case REPEAT:
			case FOR:
			case WITH:
			{
				structuredStatement(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void simpleStatement(AST _t) throws RecognitionException {
		
		PascalAST simpleStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case ASSIGN:
			{
				assignmentStatement(_t);
				_t = _retTree;
				break;
			}
			case PROC_CALL:
			{
				procedureStatement(_t);
				_t = _retTree;
				break;
			}
			case GOTO:
			{
				gotoStatement(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void structuredStatement(AST _t) throws RecognitionException {
		
		PascalAST structuredStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case BLOCK:
			{
				compoundStatement(_t);
				_t = _retTree;
				break;
			}
			case CASE:
			case IF:
			{
				conditionalStatement(_t);
				_t = _retTree;
				break;
			}
			case WHILE:
			case REPEAT:
			case FOR:
			{
				repetetiveStatement(_t);
				_t = _retTree;
				break;
			}
			case WITH:
			{
				withStatement(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void assignmentStatement(AST _t) throws RecognitionException {
		
		PascalAST assignmentStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t120 = _t;
			PascalAST tmp73_AST_in = (PascalAST)_t;
			match(_t,ASSIGN);
			_t = _t.getFirstChild();
			variable(_t);
			_t = _retTree;
			expression(_t);
			_t = _retTree;
			_t = __t120;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void procedureStatement(AST _t) throws RecognitionException {
		
		PascalAST procedureStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t166 = _t;
			PascalAST tmp74_AST_in = (PascalAST)_t;
			match(_t,PROC_CALL);
			_t = _t.getFirstChild();
			PascalAST tmp75_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case ARGLIST:
			{
				parameterList(_t);
				_t = _retTree;
				break;
			}
			case 3:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			_t = __t166;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void gotoStatement(AST _t) throws RecognitionException {
		
		PascalAST gotoStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t170 = _t;
			PascalAST tmp76_AST_in = (PascalAST)_t;
			match(_t,GOTO);
			_t = _t.getFirstChild();
			label(_t);
			_t = _retTree;
			_t = __t170;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
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
	public final void variable(AST _t) throws RecognitionException {
		
		PascalAST variable_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case LBRACK:
			{
				AST __t122 = _t;
				PascalAST tmp77_AST_in = (PascalAST)_t;
				match(_t,LBRACK);
				_t = _t.getFirstChild();
				variable(_t);
				_t = _retTree;
				{
				int _cnt124=0;
				_loop124:
				do {
					if (_t==null) _t=ASTNULL;
					if ((_tokenSet_3.member(_t.getType()))) {
						expression(_t);
						_t = _retTree;
					}
					else {
						if ( _cnt124>=1 ) { break _loop124; } else {throw new NoViableAltException(_t);}
					}
					
					_cnt124++;
				} while (true);
				}
				_t = __t122;
				_t = _t.getNextSibling();
				break;
			}
			case LBRACK2:
			{
				AST __t125 = _t;
				PascalAST tmp78_AST_in = (PascalAST)_t;
				match(_t,LBRACK2);
				_t = _t.getFirstChild();
				variable(_t);
				_t = _retTree;
				{
				int _cnt127=0;
				_loop127:
				do {
					if (_t==null) _t=ASTNULL;
					if ((_tokenSet_3.member(_t.getType()))) {
						expression(_t);
						_t = _retTree;
					}
					else {
						if ( _cnt127>=1 ) { break _loop127; } else {throw new NoViableAltException(_t);}
					}
					
					_cnt127++;
				} while (true);
				}
				_t = __t125;
				_t = _t.getNextSibling();
				break;
			}
			case DOT:
			{
				AST __t128 = _t;
				PascalAST tmp79_AST_in = (PascalAST)_t;
				match(_t,DOT);
				_t = _t.getFirstChild();
				variable(_t);
				_t = _retTree;
				PascalAST tmp80_AST_in = (PascalAST)_t;
				match(_t,IDENT);
				_t = _t.getNextSibling();
				_t = __t128;
				_t = _t.getNextSibling();
				break;
			}
			case POINTER:
			{
				AST __t129 = _t;
				PascalAST tmp81_AST_in = (PascalAST)_t;
				match(_t,POINTER);
				_t = _t.getFirstChild();
				variable(_t);
				_t = _retTree;
				_t = __t129;
				_t = _t.getNextSibling();
				break;
			}
			case AT:
			{
				AST __t130 = _t;
				PascalAST tmp82_AST_in = (PascalAST)_t;
				match(_t,AT);
				_t = _t.getFirstChild();
				PascalAST tmp83_AST_in = (PascalAST)_t;
				match(_t,IDENT);
				_t = _t.getNextSibling();
				_t = __t130;
				_t = _t.getNextSibling();
				break;
			}
			case IDENT:
			{
				PascalAST tmp84_AST_in = (PascalAST)_t;
				match(_t,IDENT);
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void expression(AST _t) throws RecognitionException {
		
		PascalAST expression_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case EQUAL:
			{
				AST __t132 = _t;
				PascalAST tmp85_AST_in = (PascalAST)_t;
				match(_t,EQUAL);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t132;
				_t = _t.getNextSibling();
				break;
			}
			case NOT_EQUAL:
			{
				AST __t133 = _t;
				PascalAST tmp86_AST_in = (PascalAST)_t;
				match(_t,NOT_EQUAL);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t133;
				_t = _t.getNextSibling();
				break;
			}
			case LT:
			{
				AST __t134 = _t;
				PascalAST tmp87_AST_in = (PascalAST)_t;
				match(_t,LT);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t134;
				_t = _t.getNextSibling();
				break;
			}
			case LE:
			{
				AST __t135 = _t;
				PascalAST tmp88_AST_in = (PascalAST)_t;
				match(_t,LE);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t135;
				_t = _t.getNextSibling();
				break;
			}
			case GE:
			{
				AST __t136 = _t;
				PascalAST tmp89_AST_in = (PascalAST)_t;
				match(_t,GE);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t136;
				_t = _t.getNextSibling();
				break;
			}
			case GT:
			{
				AST __t137 = _t;
				PascalAST tmp90_AST_in = (PascalAST)_t;
				match(_t,GT);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t137;
				_t = _t.getNextSibling();
				break;
			}
			case IN:
			{
				AST __t138 = _t;
				PascalAST tmp91_AST_in = (PascalAST)_t;
				match(_t,IN);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t138;
				_t = _t.getNextSibling();
				break;
			}
			case PLUS:
			{
				AST __t139 = _t;
				PascalAST tmp92_AST_in = (PascalAST)_t;
				match(_t,PLUS);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case FUNC_CALL:
				case DOT:
				case IDENT:
				case EQUAL:
				case CHR:
				case NUM_INT:
				case NUM_REAL:
				case PLUS:
				case MINUS:
				case STRING_LITERAL:
				case LBRACK:
				case LBRACK2:
				case SET:
				case POINTER:
				case AT:
				case NOT_EQUAL:
				case LT:
				case LE:
				case GE:
				case GT:
				case IN:
				case OR:
				case STAR:
				case SLASH:
				case DIV:
				case MOD:
				case AND:
				case NOT:
				case NIL:
				{
					expression(_t);
					_t = _retTree;
					break;
				}
				case 3:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t139;
				_t = _t.getNextSibling();
				break;
			}
			case MINUS:
			{
				AST __t141 = _t;
				PascalAST tmp93_AST_in = (PascalAST)_t;
				match(_t,MINUS);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case FUNC_CALL:
				case DOT:
				case IDENT:
				case EQUAL:
				case CHR:
				case NUM_INT:
				case NUM_REAL:
				case PLUS:
				case MINUS:
				case STRING_LITERAL:
				case LBRACK:
				case LBRACK2:
				case SET:
				case POINTER:
				case AT:
				case NOT_EQUAL:
				case LT:
				case LE:
				case GE:
				case GT:
				case IN:
				case OR:
				case STAR:
				case SLASH:
				case DIV:
				case MOD:
				case AND:
				case NOT:
				case NIL:
				{
					expression(_t);
					_t = _retTree;
					break;
				}
				case 3:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t141;
				_t = _t.getNextSibling();
				break;
			}
			case OR:
			{
				AST __t143 = _t;
				PascalAST tmp94_AST_in = (PascalAST)_t;
				match(_t,OR);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t143;
				_t = _t.getNextSibling();
				break;
			}
			case STAR:
			{
				AST __t144 = _t;
				PascalAST tmp95_AST_in = (PascalAST)_t;
				match(_t,STAR);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t144;
				_t = _t.getNextSibling();
				break;
			}
			case SLASH:
			{
				AST __t145 = _t;
				PascalAST tmp96_AST_in = (PascalAST)_t;
				match(_t,SLASH);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t145;
				_t = _t.getNextSibling();
				break;
			}
			case DIV:
			{
				AST __t146 = _t;
				PascalAST tmp97_AST_in = (PascalAST)_t;
				match(_t,DIV);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t146;
				_t = _t.getNextSibling();
				break;
			}
			case MOD:
			{
				AST __t147 = _t;
				PascalAST tmp98_AST_in = (PascalAST)_t;
				match(_t,MOD);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t147;
				_t = _t.getNextSibling();
				break;
			}
			case AND:
			{
				AST __t148 = _t;
				PascalAST tmp99_AST_in = (PascalAST)_t;
				match(_t,AND);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t148;
				_t = _t.getNextSibling();
				break;
			}
			case NOT:
			{
				AST __t149 = _t;
				PascalAST tmp100_AST_in = (PascalAST)_t;
				match(_t,NOT);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				_t = __t149;
				_t = _t.getNextSibling();
				break;
			}
			case DOT:
			case IDENT:
			case LBRACK:
			case LBRACK2:
			case POINTER:
			case AT:
			{
				variable(_t);
				_t = _retTree;
				break;
			}
			case FUNC_CALL:
			{
				functionDesignator(_t);
				_t = _retTree;
				break;
			}
			case SET:
			{
				set(_t);
				_t = _retTree;
				break;
			}
			case NUM_INT:
			{
				PascalAST tmp101_AST_in = (PascalAST)_t;
				match(_t,NUM_INT);
				_t = _t.getNextSibling();
				break;
			}
			case NUM_REAL:
			{
				PascalAST tmp102_AST_in = (PascalAST)_t;
				match(_t,NUM_REAL);
				_t = _t.getNextSibling();
				break;
			}
			case CHR:
			{
				AST __t150 = _t;
				PascalAST tmp103_AST_in = (PascalAST)_t;
				match(_t,CHR);
				_t = _t.getFirstChild();
				{
				if (_t==null) _t=ASTNULL;
				switch ( _t.getType()) {
				case NUM_INT:
				{
					PascalAST tmp104_AST_in = (PascalAST)_t;
					match(_t,NUM_INT);
					_t = _t.getNextSibling();
					break;
				}
				case NUM_REAL:
				{
					PascalAST tmp105_AST_in = (PascalAST)_t;
					match(_t,NUM_REAL);
					_t = _t.getNextSibling();
					break;
				}
				default:
				{
					throw new NoViableAltException(_t);
				}
				}
				}
				_t = __t150;
				_t = _t.getNextSibling();
				break;
			}
			case STRING_LITERAL:
			{
				string(_t);
				_t = _retTree;
				break;
			}
			case NIL:
			{
				PascalAST tmp106_AST_in = (PascalAST)_t;
				match(_t,NIL);
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void functionDesignator(AST _t) throws RecognitionException {
		
		PascalAST functionDesignator_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t153 = _t;
			PascalAST tmp107_AST_in = (PascalAST)_t;
			match(_t,FUNC_CALL);
			_t = _t.getFirstChild();
			PascalAST tmp108_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case ARGLIST:
			{
				parameterList(_t);
				_t = _retTree;
				break;
			}
			case 3:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			_t = __t153;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void set(AST _t) throws RecognitionException {
		
		PascalAST set_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t160 = _t;
			PascalAST tmp109_AST_in = (PascalAST)_t;
			match(_t,SET);
			_t = _t.getFirstChild();
			{
			_loop162:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_tokenSet_4.member(_t.getType()))) {
					element(_t);
					_t = _retTree;
				}
				else {
					break _loop162;
				}
				
			} while (true);
			}
			_t = __t160;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void parameterList(AST _t) throws RecognitionException {
		
		PascalAST parameterList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t156 = _t;
			PascalAST tmp110_AST_in = (PascalAST)_t;
			match(_t,ARGLIST);
			_t = _t.getFirstChild();
			{
			int _cnt158=0;
			_loop158:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_tokenSet_3.member(_t.getType()))) {
					actualParameter(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt158>=1 ) { break _loop158; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt158++;
			} while (true);
			}
			_t = __t156;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void actualParameter(AST _t) throws RecognitionException {
		
		PascalAST actualParameter_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			expression(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void element(AST _t) throws RecognitionException {
		
		PascalAST element_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case DOTDOT:
			{
				AST __t164 = _t;
				PascalAST tmp111_AST_in = (PascalAST)_t;
				match(_t,DOTDOT);
				_t = _t.getFirstChild();
				expression(_t);
				_t = _retTree;
				expression(_t);
				_t = _retTree;
				_t = __t164;
				_t = _t.getNextSibling();
				break;
			}
			case FUNC_CALL:
			case DOT:
			case IDENT:
			case EQUAL:
			case CHR:
			case NUM_INT:
			case NUM_REAL:
			case PLUS:
			case MINUS:
			case STRING_LITERAL:
			case LBRACK:
			case LBRACK2:
			case SET:
			case POINTER:
			case AT:
			case NOT_EQUAL:
			case LT:
			case LE:
			case GE:
			case GT:
			case IN:
			case OR:
			case STAR:
			case SLASH:
			case DIV:
			case MOD:
			case AND:
			case NOT:
			case NIL:
			{
				expression(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void conditionalStatement(AST _t) throws RecognitionException {
		
		PascalAST conditionalStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case IF:
			{
				ifStatement(_t);
				_t = _retTree;
				break;
			}
			case CASE:
			{
				caseStatement(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void repetetiveStatement(AST _t) throws RecognitionException {
		
		PascalAST repetetiveStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case WHILE:
			{
				whileStatement(_t);
				_t = _retTree;
				break;
			}
			case REPEAT:
			{
				repeatStatement(_t);
				_t = _retTree;
				break;
			}
			case FOR:
			{
				forStatement(_t);
				_t = _retTree;
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void withStatement(AST _t) throws RecognitionException {
		
		PascalAST withStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t201 = _t;
			PascalAST tmp112_AST_in = (PascalAST)_t;
			match(_t,WITH);
			_t = _t.getFirstChild();
			recordVariableList(_t);
			_t = _retTree;
			statement(_t);
			_t = _retTree;
			_t = __t201;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void statements(AST _t) throws RecognitionException {
		
		PascalAST statements_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t174 = _t;
			PascalAST tmp113_AST_in = (PascalAST)_t;
			match(_t,BLOCK);
			_t = _t.getFirstChild();
			{
			_loop176:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_tokenSet_5.member(_t.getType()))) {
					statement(_t);
					_t = _retTree;
				}
				else {
					break _loop176;
				}
				
			} while (true);
			}
			_t = __t174;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void ifStatement(AST _t) throws RecognitionException {
		
		PascalAST ifStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t179 = _t;
			PascalAST tmp114_AST_in = (PascalAST)_t;
			match(_t,IF);
			_t = _t.getFirstChild();
			expression(_t);
			_t = _retTree;
			statement(_t);
			_t = _retTree;
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case BLOCK:
			case PROC_CALL:
			case COLON:
			case CASE:
			case ASSIGN:
			case GOTO:
			case IF:
			case WHILE:
			case REPEAT:
			case FOR:
			case WITH:
			{
				statement(_t);
				_t = _retTree;
				break;
			}
			case 3:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			_t = __t179;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void caseStatement(AST _t) throws RecognitionException {
		
		PascalAST caseStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t182 = _t;
			PascalAST tmp115_AST_in = (PascalAST)_t;
			match(_t,CASE);
			_t = _t.getFirstChild();
			expression(_t);
			_t = _retTree;
			{
			int _cnt184=0;
			_loop184:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_t.getType()==COLON)) {
					caseListElement(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt184>=1 ) { break _loop184; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt184++;
			} while (true);
			}
			{
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case BLOCK:
			{
				statements(_t);
				_t = _retTree;
				break;
			}
			case 3:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
			}
			_t = __t182;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void caseListElement(AST _t) throws RecognitionException {
		
		PascalAST caseListElement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t187 = _t;
			PascalAST tmp116_AST_in = (PascalAST)_t;
			match(_t,COLON);
			_t = _t.getFirstChild();
			constList(_t);
			_t = _retTree;
			statement(_t);
			_t = _retTree;
			_t = __t187;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void whileStatement(AST _t) throws RecognitionException {
		
		PascalAST whileStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t190 = _t;
			PascalAST tmp117_AST_in = (PascalAST)_t;
			match(_t,WHILE);
			_t = _t.getFirstChild();
			expression(_t);
			_t = _retTree;
			statement(_t);
			_t = _retTree;
			_t = __t190;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void repeatStatement(AST _t) throws RecognitionException {
		
		PascalAST repeatStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t192 = _t;
			PascalAST tmp118_AST_in = (PascalAST)_t;
			match(_t,REPEAT);
			_t = _t.getFirstChild();
			statements(_t);
			_t = _retTree;
			expression(_t);
			_t = _retTree;
			_t = __t192;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void forStatement(AST _t) throws RecognitionException {
		
		PascalAST forStatement_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			AST __t194 = _t;
			PascalAST tmp119_AST_in = (PascalAST)_t;
			match(_t,FOR);
			_t = _t.getFirstChild();
			PascalAST tmp120_AST_in = (PascalAST)_t;
			match(_t,IDENT);
			_t = _t.getNextSibling();
			forList(_t);
			_t = _retTree;
			statement(_t);
			_t = _retTree;
			_t = __t194;
			_t = _t.getNextSibling();
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void forList(AST _t) throws RecognitionException {
		
		PascalAST forList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			if (_t==null) _t=ASTNULL;
			switch ( _t.getType()) {
			case TO:
			{
				AST __t196 = _t;
				PascalAST tmp121_AST_in = (PascalAST)_t;
				match(_t,TO);
				_t = _t.getFirstChild();
				initialValue(_t);
				_t = _retTree;
				finalValue(_t);
				_t = _retTree;
				_t = __t196;
				_t = _t.getNextSibling();
				break;
			}
			case DOWNTO:
			{
				AST __t197 = _t;
				PascalAST tmp122_AST_in = (PascalAST)_t;
				match(_t,DOWNTO);
				_t = _t.getFirstChild();
				initialValue(_t);
				_t = _retTree;
				finalValue(_t);
				_t = _retTree;
				_t = __t197;
				_t = _t.getNextSibling();
				break;
			}
			default:
			{
				throw new NoViableAltException(_t);
			}
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void initialValue(AST _t) throws RecognitionException {
		
		PascalAST initialValue_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			expression(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void finalValue(AST _t) throws RecognitionException {
		
		PascalAST finalValue_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			expression(_t);
			_t = _retTree;
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	public final void recordVariableList(AST _t) throws RecognitionException {
		
		PascalAST recordVariableList_AST_in = (PascalAST)_t;
		
		try {      // for error handling
			{
			int _cnt204=0;
			_loop204:
			do {
				if (_t==null) _t=ASTNULL;
				if ((_tokenSet_6.member(_t.getType()))) {
					variable(_t);
					_t = _retTree;
				}
				else {
					if ( _cnt204>=1 ) { break _loop204; } else {throw new NoViableAltException(_t);}
				}
				
				_cnt204++;
			} while (true);
			}
		}
		catch (RecognitionException ex) {
			reportError(ex);
			if (_t!=null) {_t = _t.getNextSibling();}
		}
		_retTree = _t;
	}
	
	
	public static final String[] _tokenNames = {
		"<0>",
		"EOF",
		"<2>",
		"NULL_TREE_LOOKAHEAD",
		"BLOCK",
		"IDLIST",
		"ELIST",
		"FUNC_CALL",
		"PROC_CALL",
		"SCALARTYPE",
		"TYPELIST",
		"VARIANT_TAG",
		"VARIANT_TAG_NO_ID",
		"VARIANT_CASE",
		"CONSTLIST",
		"FIELDLIST",
		"ARGDECLS",
		"VARDECL",
		"ARGDECL",
		"ARGLIST",
		"TYPEDECL",
		"FIELD",
		"\"interface\"",
		"DOT",
		"\"program\"",
		"LPAREN",
		"RPAREN",
		"SEMI",
		"\"unit\"",
		"IDENT",
		"\"implementation\"",
		"\"uses\"",
		"\"label\"",
		"COMMA",
		"\"const\"",
		"EQUAL",
		"\"chr\"",
		"NUM_INT",
		"NUM_REAL",
		"PLUS",
		"MINUS",
		"STRING_LITERAL",
		"\"type\"",
		"\"function\"",
		"COLON",
		"\"procedure\"",
		"DOTDOT",
		"\"char\"",
		"\"boolean\"",
		"\"integer\"",
		"\"real\"",
		"\"string\"",
		"\"packed\"",
		"LBRACK",
		"RBRACK",
		"\"array\"",
		"\"of\"",
		"LBRACK2",
		"RBRACK2",
		"\"record\"",
		"\"end\"",
		"\"case\"",
		"\"set\"",
		"\"file\"",
		"POINTER",
		"\"var\"",
		"ASSIGN",
		"AT",
		"NOT_EQUAL",
		"LT",
		"LE",
		"GE",
		"GT",
		"\"in\"",
		"\"or\"",
		"STAR",
		"SLASH",
		"\"div\"",
		"\"mod\"",
		"\"and\"",
		"\"not\"",
		"\"nil\"",
		"\"goto\"",
		"\"begin\"",
		"\"if\"",
		"\"then\"",
		"\"else\"",
		"\"while\"",
		"\"do\"",
		"\"repeat\"",
		"\"until\"",
		"\"for\"",
		"\"to\"",
		"\"downto\"",
		"\"with\"",
		"METHOD",
		"ADDSUBOR",
		"ASSIGNEQUAL",
		"SIGN",
		"FUNC",
		"NODE_NOT_EMIT",
		"MYASTVAR",
		"LF",
		"LCURLY",
		"RCURLY",
		"WS",
		"COMMENT_1",
		"COMMENT_2",
		"EXPONENT"
	};
	
	private static final long[] mk_tokenSet_0() {
		long[] data = { 43980465373184L, 2L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_0 = new BitSet(mk_tokenSet_0());
	private static final long[] mk_tokenSet_1() {
		long[] data = { -3990259638057565696L, 1L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_1 = new BitSet(mk_tokenSet_1());
	private static final long[] mk_tokenSet_2() {
		long[] data = { 4329863905280L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_2 = new BitSet(mk_tokenSet_2());
	private static final long[] mk_tokenSet_3() {
		long[] data = { 4764812769990017152L, 262137L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_3 = new BitSet(mk_tokenSet_3());
	private static final long[] mk_tokenSet_4() {
		long[] data = { 4764883138734194816L, 262137L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_4 = new BitSet(mk_tokenSet_4());
	private static final long[] mk_tokenSet_5() {
		long[] data = { 2305860601399738640L, 1251213316L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_5 = new BitSet(mk_tokenSet_5());
	private static final long[] mk_tokenSet_6() {
		long[] data = { 153122387875856384L, 9L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_6 = new BitSet(mk_tokenSet_6());
	}
	
