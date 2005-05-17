// $ANTLR 2.7.2: "pascal.g" -> "PascalParser.java"$

import antlr.TokenBuffer;
import antlr.TokenStreamException;
import antlr.TokenStreamIOException;
import antlr.ANTLRException;
import antlr.LLkParser;
import antlr.Token;
import antlr.TokenStream;
import antlr.RecognitionException;
import antlr.NoViableAltException;
import antlr.MismatchedTokenException;
import antlr.SemanticException;
import antlr.ParserSharedInputState;
import antlr.collections.impl.BitSet;
import antlr.collections.AST;
import java.util.Hashtable;
import antlr.ASTFactory;
import antlr.ASTPair;
import antlr.collections.impl.ASTArray;

import java.util.*;
import java.io.*;
import antlr.collections.AST;
import antlr.collections.impl.*;
import antlr.debug.misc.*;
import antlr.*;

public class PascalParser extends antlr.LLkParser       implements PascalTokenTypes
 {

    /** Overall symbol table for translator */
    public static SymbolTable symbolTable = new SymbolTable();

    // This method decides what action to take based on the type of
    //   file we are looking at
    public  static void doFile(File f) throws Exception {
      // If this is a directory, walk each file/dir in that directory
      translateFilePath = f.getParent();
      if (f.isDirectory()) {
        String files[] = f.list();
        for(int i=0; i < files.length; i++)
        {
          doFile(new File(f, files[i]));
        }
      }
      // otherwise, if this is a Pascal file, parse it!
      else if ((f.getName().length()>4) &&
             f.getName().substring(f.getName().length()-4).toLowerCase().equals(".pas")) {
        System.err.println("   "+f.getAbsolutePath());

        if (translateFileName == null) {
          translateFileName = f.getName(); //set this file as the one to translate
          currentFileName = f.getName();
        }

        parseFile(f.getName(),new FileInputStream(f));
      }
      else {
        System.err.println("Can not parse:   "+f.getAbsolutePath());
      }
    }

    // Here's where we do the real work...
    public  static void parseFile(String f,InputStream s) throws Exception {
      try {
        currentFileName = f; // set this File as the currentFileName

        // Create a scanner that reads from the input stream passed to us
         PascalLexer lexer = new PascalLexer(s);

        // Create a parser that reads from the scanner
         PascalParser parser = new PascalParser(lexer);

        // set AST type to PascalAST (has symbol)
        parser.setASTNodeClass("PascalAST");

        // start parsing at the program rule
        parser.program(); 

        CommonAST t = (CommonAST)parser.getAST();

        // do something with the tree
        parser.doTreeAction(f, parser.getAST(), parser.getTokenNames());
        //System.out.println(parser.getAST().toStringList());
            


// build symbol table
        
        // Get the tree out of the parser
        AST resultTree1 = parser.getAST();

        // Make an instance of the tree parser
        // PascalTreeParserSuper treeParser1 = new PascalTreeParserSuper();
        SymtabPhase treeParser1 = new SymtabPhase();

        treeParser1.setASTNodeClass("PascalAST");

        // Begin tree parser at only rule
        treeParser1.program(resultTree1);



//        parser.doTreeAction(f, treeParser1.getAST(), treeParser1.getTokenNames());



       
      }
      catch (Exception e) {
        System.err.println("parser exception: "+e);
        e.printStackTrace();   // so we can get stack trace
      }
    }
    public void doTreeAction(String f, AST t, String[] tokenNames) {
      if ( t==null ) return;
      if ( showTree ) {
         ((CommonAST)t).setVerboseStringConversion(true, tokenNames);
         ASTFactory factory = new ASTFactory();
         AST r = factory.create(0,"AST ROOT");
         r.setFirstChild(t);
         ASTFrame frame = new ASTFrame("Pascal AST", r);
         frame.setVisible(true);
         //System.out.println(t.toStringList());
      }

    }
  static boolean showTree = true;
  public static String translateFilePath;
  public static String translateFileName;
  public static String currentFileName; // not static, recursive USES ... other FileName in currentFileName
  public static String oldtranslateFileName;


// main
  public static void main(String[] args) {
    // Use a try/catch block for parser exceptions
    try {
      // if we have at least one command-line argument
      if (args.length > 0 ) {

        // for each directory/file specified on the command line
        for(int i=0; i< args.length;i++)
{
	  if ( args[i].equals("-showtree") ) {
             showTree = true;
          }
          else {
            System.err.println("Parsing...");
            doFile(new File(args[i])); // parse it
          }
        }
      }
      else
        System.err.println("Usage: java PascalParser <file/directory name>");

    }
    catch(Exception e) {
      System.err.println("exception: "+e);
      e.printStackTrace(System.err);   // so we can get stack trace
    }
  }


protected PascalParser(TokenBuffer tokenBuf, int k) {
  super(tokenBuf,k);
  tokenNames = _tokenNames;
  buildTokenTypeASTClassMap();
  astFactory = new ASTFactory(getTokenTypeToASTClassMap());
}

public PascalParser(TokenBuffer tokenBuf) {
  this(tokenBuf,2);
}

protected PascalParser(TokenStream lexer, int k) {
  super(lexer,k);
  tokenNames = _tokenNames;
  buildTokenTypeASTClassMap();
  astFactory = new ASTFactory(getTokenTypeToASTClassMap());
}

public PascalParser(TokenStream lexer) {
  this(lexer,2);
}

public PascalParser(ParserSharedInputState state) {
  super(state,2);
  tokenNames = _tokenNames;
  buildTokenTypeASTClassMap();
  astFactory = new ASTFactory(getTokenTypeToASTClassMap());
}

	public final void program() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST program_AST = null;
		
		programHeading();
		astFactory.addASTChild(currentAST, returnAST);
		{
		switch ( LA(1)) {
		case INTERFACE:
		{
			match(INTERFACE);
			break;
		}
		case IMPLEMENTATION:
		case USES:
		case LABEL:
		case CONST:
		case TYPE:
		case FUNCTION:
		case PROCEDURE:
		case VAR:
		case BEGIN:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		block();
		astFactory.addASTChild(currentAST, returnAST);
		match(DOT);
		program_AST = (PascalAST)currentAST.root;
		returnAST = program_AST;
	}
	
	public final void programHeading() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST programHeading_AST = null;
		
		switch ( LA(1)) {
		case PROGRAM:
		{
			PascalAST tmp3_AST = null;
			tmp3_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp3_AST);
			match(PROGRAM);
			identifier();
			astFactory.addASTChild(currentAST, returnAST);
			match(LPAREN);
			identifierList();
			astFactory.addASTChild(currentAST, returnAST);
			match(RPAREN);
			match(SEMI);
			programHeading_AST = (PascalAST)currentAST.root;
			break;
		}
		case UNIT:
		{
			PascalAST tmp7_AST = null;
			tmp7_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp7_AST);
			match(UNIT);
			identifier();
			astFactory.addASTChild(currentAST, returnAST);
			match(SEMI);
			programHeading_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = programHeading_AST;
	}
	
	public final void block() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST block_AST = null;
		
		{
		_loop7:
		do {
			switch ( LA(1)) {
			case LABEL:
			{
				labelDeclarationPart();
				astFactory.addASTChild(currentAST, returnAST);
				break;
			}
			case CONST:
			{
				constantDefinitionPart();
				astFactory.addASTChild(currentAST, returnAST);
				break;
			}
			case TYPE:
			{
				typeDefinitionPart();
				astFactory.addASTChild(currentAST, returnAST);
				break;
			}
			case VAR:
			{
				variableDeclarationPart();
				astFactory.addASTChild(currentAST, returnAST);
				break;
			}
			case FUNCTION:
			case PROCEDURE:
			{
				procedureAndFunctionDeclarationPart();
				astFactory.addASTChild(currentAST, returnAST);
				break;
			}
			case USES:
			{
				usesUnitsPart();
				astFactory.addASTChild(currentAST, returnAST);
				break;
			}
			case IMPLEMENTATION:
			{
				PascalAST tmp9_AST = null;
				tmp9_AST = (PascalAST)astFactory.create(LT(1));
				astFactory.addASTChild(currentAST, tmp9_AST);
				match(IMPLEMENTATION);
				break;
			}
			default:
			{
				break _loop7;
			}
			}
		} while (true);
		}
		compoundStatement();
		astFactory.addASTChild(currentAST, returnAST);
		block_AST = (PascalAST)currentAST.root;
		returnAST = block_AST;
	}
	
	public final void identifier() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST identifier_AST = null;
		
		PascalAST tmp10_AST = null;
		tmp10_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.addASTChild(currentAST, tmp10_AST);
		match(IDENT);
		identifier_AST = (PascalAST)currentAST.root;
		returnAST = identifier_AST;
	}
	
	public final void identifierList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST identifierList_AST = null;
		
		identifier();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop80:
		do {
			if ((LA(1)==COMMA)) {
				match(COMMA);
				identifier();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop80;
			}
			
		} while (true);
		}
		identifierList_AST = (PascalAST)currentAST.root;
		identifierList_AST = (PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(IDLIST)).add(identifierList_AST));
		currentAST.root = identifierList_AST;
		currentAST.child = identifierList_AST!=null &&identifierList_AST.getFirstChild()!=null ?
			identifierList_AST.getFirstChild() : identifierList_AST;
		currentAST.advanceChildToEnd();
		identifierList_AST = (PascalAST)currentAST.root;
		returnAST = identifierList_AST;
	}
	
	public final void labelDeclarationPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST labelDeclarationPart_AST = null;
		
		PascalAST tmp12_AST = null;
		tmp12_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp12_AST);
		match(LABEL);
		label();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop11:
		do {
			if ((LA(1)==COMMA)) {
				match(COMMA);
				label();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop11;
			}
			
		} while (true);
		}
		match(SEMI);
		labelDeclarationPart_AST = (PascalAST)currentAST.root;
		returnAST = labelDeclarationPart_AST;
	}
	
	public final void constantDefinitionPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST constantDefinitionPart_AST = null;
		
		PascalAST tmp15_AST = null;
		tmp15_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp15_AST);
		match(CONST);
		constantDefinition();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop15:
		do {
			if ((LA(1)==SEMI) && (LA(2)==IDENT)) {
				match(SEMI);
				constantDefinition();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop15;
			}
			
		} while (true);
		}
		match(SEMI);
		constantDefinitionPart_AST = (PascalAST)currentAST.root;
		returnAST = constantDefinitionPart_AST;
	}
	
	public final void typeDefinitionPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST typeDefinitionPart_AST = null;
		
		PascalAST tmp18_AST = null;
		tmp18_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp18_AST);
		match(TYPE);
		typeDefinition();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop26:
		do {
			if ((LA(1)==SEMI) && (LA(2)==IDENT)) {
				match(SEMI);
				typeDefinition();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop26;
			}
			
		} while (true);
		}
		match(SEMI);
		typeDefinitionPart_AST = (PascalAST)currentAST.root;
		returnAST = typeDefinitionPart_AST;
	}
	
/** Yields a list of VARDECL-rooted subtrees with VAR at the overall root */
	public final void variableDeclarationPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST variableDeclarationPart_AST = null;
		
		PascalAST tmp21_AST = null;
		tmp21_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp21_AST);
		match(VAR);
		variableDeclaration();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop67:
		do {
			if ((LA(1)==SEMI) && (LA(2)==IDENT)) {
				match(SEMI);
				variableDeclaration();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop67;
			}
			
		} while (true);
		}
		match(SEMI);
		variableDeclarationPart_AST = (PascalAST)currentAST.root;
		returnAST = variableDeclarationPart_AST;
	}
	
	public final void procedureAndFunctionDeclarationPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST procedureAndFunctionDeclarationPart_AST = null;
		
		procedureOrFunctionDeclaration();
		astFactory.addASTChild(currentAST, returnAST);
		match(SEMI);
		procedureAndFunctionDeclarationPart_AST = (PascalAST)currentAST.root;
		returnAST = procedureAndFunctionDeclarationPart_AST;
	}
	
	public final void usesUnitsPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST usesUnitsPart_AST = null;
		
		PascalAST tmp25_AST = null;
		tmp25_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp25_AST);
		match(USES);
		identifierList();
		astFactory.addASTChild(currentAST, returnAST);
		match(SEMI);
		usesUnitsPart_AST = (PascalAST)currentAST.root;
		returnAST = usesUnitsPart_AST;
	}
	
	public final void compoundStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST compoundStatement_AST = null;
		
		match(BEGIN);
		statements();
		astFactory.addASTChild(currentAST, returnAST);
		match(END);
		compoundStatement_AST = (PascalAST)currentAST.root;
		returnAST = compoundStatement_AST;
	}
	
	public final void label() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST label_AST = null;
		
		unsignedInteger();
		astFactory.addASTChild(currentAST, returnAST);
		label_AST = (PascalAST)currentAST.root;
		returnAST = label_AST;
	}
	
	public final void unsignedInteger() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST unsignedInteger_AST = null;
		
		PascalAST tmp29_AST = null;
		tmp29_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.addASTChild(currentAST, tmp29_AST);
		match(NUM_INT);
		unsignedInteger_AST = (PascalAST)currentAST.root;
		returnAST = unsignedInteger_AST;
	}
	
	public final void constantDefinition() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST constantDefinition_AST = null;
		
		identifier();
		astFactory.addASTChild(currentAST, returnAST);
		PascalAST tmp30_AST = null;
		tmp30_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp30_AST);
		match(EQUAL);
		constant();
		astFactory.addASTChild(currentAST, returnAST);
		constantDefinition_AST = (PascalAST)currentAST.root;
		returnAST = constantDefinition_AST;
	}
	
	public final void constant() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST constant_AST = null;
		PascalAST s_AST = null;
		PascalAST n_AST = null;
		PascalAST s2_AST = null;
		PascalAST id_AST = null;
		
		switch ( LA(1)) {
		case NUM_INT:
		case NUM_REAL:
		{
			unsignedNumber();
			astFactory.addASTChild(currentAST, returnAST);
			constant_AST = (PascalAST)currentAST.root;
			break;
		}
		case IDENT:
		{
			identifier();
			astFactory.addASTChild(currentAST, returnAST);
			constant_AST = (PascalAST)currentAST.root;
			break;
		}
		case STRING_LITERAL:
		{
			string();
			astFactory.addASTChild(currentAST, returnAST);
			constant_AST = (PascalAST)currentAST.root;
			break;
		}
		case CHR:
		{
			constantChr();
			astFactory.addASTChild(currentAST, returnAST);
			constant_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
			if ((LA(1)==PLUS||LA(1)==MINUS) && (LA(2)==NUM_INT||LA(2)==NUM_REAL)) {
				sign();
				s_AST = (PascalAST)returnAST;
				unsignedNumber();
				n_AST = (PascalAST)returnAST;
				constant_AST = (PascalAST)currentAST.root;
				constant_AST=(PascalAST)astFactory.make( (new ASTArray(2)).add(s_AST).add(n_AST));
				currentAST.root = constant_AST;
				currentAST.child = constant_AST!=null &&constant_AST.getFirstChild()!=null ?
					constant_AST.getFirstChild() : constant_AST;
				currentAST.advanceChildToEnd();
			}
			else if ((LA(1)==PLUS||LA(1)==MINUS) && (LA(2)==IDENT)) {
				sign();
				s2_AST = (PascalAST)returnAST;
				identifier();
				id_AST = (PascalAST)returnAST;
				constant_AST = (PascalAST)currentAST.root;
				constant_AST=(PascalAST)astFactory.make( (new ASTArray(2)).add(s2_AST).add(id_AST));
				currentAST.root = constant_AST;
				currentAST.child = constant_AST!=null &&constant_AST.getFirstChild()!=null ?
					constant_AST.getFirstChild() : constant_AST;
				currentAST.advanceChildToEnd();
			}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = constant_AST;
	}
	
	public final void constantChr() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST constantChr_AST = null;
		
		PascalAST tmp31_AST = null;
		tmp31_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp31_AST);
		match(CHR);
		match(LPAREN);
		unsignedInteger();
		astFactory.addASTChild(currentAST, returnAST);
		match(RPAREN);
		constantChr_AST = (PascalAST)currentAST.root;
		returnAST = constantChr_AST;
	}
	
	public final void unsignedNumber() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST unsignedNumber_AST = null;
		
		switch ( LA(1)) {
		case NUM_INT:
		{
			unsignedInteger();
			astFactory.addASTChild(currentAST, returnAST);
			unsignedNumber_AST = (PascalAST)currentAST.root;
			break;
		}
		case NUM_REAL:
		{
			unsignedReal();
			astFactory.addASTChild(currentAST, returnAST);
			unsignedNumber_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = unsignedNumber_AST;
	}
	
	public final void sign() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST sign_AST = null;
		
		switch ( LA(1)) {
		case PLUS:
		{
			PascalAST tmp34_AST = null;
			tmp34_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp34_AST);
			match(PLUS);
			sign_AST = (PascalAST)currentAST.root;
			break;
		}
		case MINUS:
		{
			PascalAST tmp35_AST = null;
			tmp35_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp35_AST);
			match(MINUS);
			sign_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = sign_AST;
	}
	
	public final void string() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST string_AST = null;
		
		PascalAST tmp36_AST = null;
		tmp36_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.addASTChild(currentAST, tmp36_AST);
		match(STRING_LITERAL);
		string_AST = (PascalAST)currentAST.root;
		returnAST = string_AST;
	}
	
	public final void unsignedReal() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST unsignedReal_AST = null;
		
		PascalAST tmp37_AST = null;
		tmp37_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.addASTChild(currentAST, tmp37_AST);
		match(NUM_REAL);
		unsignedReal_AST = (PascalAST)currentAST.root;
		returnAST = unsignedReal_AST;
	}
	
	public final void typeDefinition() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST typeDefinition_AST = null;
		Token  e = null;
		PascalAST e_AST = null;
		
		identifier();
		astFactory.addASTChild(currentAST, returnAST);
		e = LT(1);
		e_AST = (PascalAST)astFactory.create(e);
		astFactory.makeASTRoot(currentAST, e_AST);
		match(EQUAL);
		e_AST.setType(TYPEDECL);
		{
		switch ( LA(1)) {
		case LPAREN:
		case IDENT:
		case CHR:
		case NUM_INT:
		case NUM_REAL:
		case PLUS:
		case MINUS:
		case STRING_LITERAL:
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
			type();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case FUNCTION:
		{
			functionType();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case PROCEDURE:
		{
			procedureType();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		typeDefinition_AST = (PascalAST)currentAST.root;
		returnAST = typeDefinition_AST;
	}
	
	public final void type() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST type_AST = null;
		
		switch ( LA(1)) {
		case LPAREN:
		case IDENT:
		case CHR:
		case NUM_INT:
		case NUM_REAL:
		case PLUS:
		case MINUS:
		case STRING_LITERAL:
		case CHAR:
		case BOOLEAN:
		case INTEGER:
		case REAL:
		case STRING:
		{
			simpleType();
			astFactory.addASTChild(currentAST, returnAST);
			type_AST = (PascalAST)currentAST.root;
			break;
		}
		case PACKED:
		case ARRAY:
		case RECORD:
		case SET:
		case FILE:
		{
			structuredType();
			astFactory.addASTChild(currentAST, returnAST);
			type_AST = (PascalAST)currentAST.root;
			break;
		}
		case POINTER:
		{
			pointerType();
			astFactory.addASTChild(currentAST, returnAST);
			type_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = type_AST;
	}
	
	public final void functionType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST functionType_AST = null;
		
		PascalAST tmp38_AST = null;
		tmp38_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp38_AST);
		match(FUNCTION);
		{
		switch ( LA(1)) {
		case LPAREN:
		{
			formalParameterList();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case COLON:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		match(COLON);
		resultType();
		astFactory.addASTChild(currentAST, returnAST);
		functionType_AST = (PascalAST)currentAST.root;
		returnAST = functionType_AST;
	}
	
	public final void procedureType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST procedureType_AST = null;
		
		PascalAST tmp40_AST = null;
		tmp40_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp40_AST);
		match(PROCEDURE);
		{
		switch ( LA(1)) {
		case LPAREN:
		{
			formalParameterList();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case SEMI:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		procedureType_AST = (PascalAST)currentAST.root;
		returnAST = procedureType_AST;
	}
	
	public final void formalParameterList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST formalParameterList_AST = null;
		
		PascalAST tmp41_AST = null;
		tmp41_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp41_AST);
		match(LPAREN);
		formalParameterSection();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop75:
		do {
			if ((LA(1)==SEMI)) {
				match(SEMI);
				formalParameterSection();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop75;
			}
			
		} while (true);
		}
		match(RPAREN);
		formalParameterList_AST = (PascalAST)currentAST.root;
		formalParameterList_AST.setType(ARGDECLS);
		formalParameterList_AST = (PascalAST)currentAST.root;
		returnAST = formalParameterList_AST;
	}
	
	public final void resultType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST resultType_AST = null;
		
		typeIdentifier();
		astFactory.addASTChild(currentAST, returnAST);
		resultType_AST = (PascalAST)currentAST.root;
		returnAST = resultType_AST;
	}
	
	public final void simpleType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST simpleType_AST = null;
		
		if ((LA(1)==LPAREN)) {
			scalarType();
			astFactory.addASTChild(currentAST, returnAST);
			simpleType_AST = (PascalAST)currentAST.root;
		}
		else if ((_tokenSet_0.member(LA(1))) && (_tokenSet_1.member(LA(2)))) {
			subrangeType();
			astFactory.addASTChild(currentAST, returnAST);
			simpleType_AST = (PascalAST)currentAST.root;
		}
		else if ((_tokenSet_2.member(LA(1))) && (_tokenSet_3.member(LA(2)))) {
			typeIdentifier();
			astFactory.addASTChild(currentAST, returnAST);
			simpleType_AST = (PascalAST)currentAST.root;
		}
		else if ((LA(1)==STRING) && (LA(2)==LBRACK)) {
			stringtype();
			astFactory.addASTChild(currentAST, returnAST);
			simpleType_AST = (PascalAST)currentAST.root;
		}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		
		returnAST = simpleType_AST;
	}
	
	public final void structuredType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST structuredType_AST = null;
		
		switch ( LA(1)) {
		case PACKED:
		{
			PascalAST tmp44_AST = null;
			tmp44_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp44_AST);
			match(PACKED);
			unpackedStructuredType();
			astFactory.addASTChild(currentAST, returnAST);
			structuredType_AST = (PascalAST)currentAST.root;
			break;
		}
		case ARRAY:
		case RECORD:
		case SET:
		case FILE:
		{
			unpackedStructuredType();
			astFactory.addASTChild(currentAST, returnAST);
			structuredType_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = structuredType_AST;
	}
	
	public final void pointerType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST pointerType_AST = null;
		
		PascalAST tmp45_AST = null;
		tmp45_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp45_AST);
		match(POINTER);
		typeIdentifier();
		astFactory.addASTChild(currentAST, returnAST);
		pointerType_AST = (PascalAST)currentAST.root;
		returnAST = pointerType_AST;
	}
	
	public final void scalarType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST scalarType_AST = null;
		
		PascalAST tmp46_AST = null;
		tmp46_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp46_AST);
		match(LPAREN);
		identifierList();
		astFactory.addASTChild(currentAST, returnAST);
		match(RPAREN);
		scalarType_AST = (PascalAST)currentAST.root;
		scalarType_AST.setType(SCALARTYPE);
		scalarType_AST = (PascalAST)currentAST.root;
		returnAST = scalarType_AST;
	}
	
	public final void subrangeType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST subrangeType_AST = null;
		
		constant();
		astFactory.addASTChild(currentAST, returnAST);
		PascalAST tmp48_AST = null;
		tmp48_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp48_AST);
		match(DOTDOT);
		constant();
		astFactory.addASTChild(currentAST, returnAST);
		subrangeType_AST = (PascalAST)currentAST.root;
		returnAST = subrangeType_AST;
	}
	
	public final void typeIdentifier() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST typeIdentifier_AST = null;
		
		switch ( LA(1)) {
		case IDENT:
		{
			identifier();
			astFactory.addASTChild(currentAST, returnAST);
			typeIdentifier_AST = (PascalAST)currentAST.root;
			break;
		}
		case CHAR:
		{
			PascalAST tmp49_AST = null;
			tmp49_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp49_AST);
			match(CHAR);
			typeIdentifier_AST = (PascalAST)currentAST.root;
			break;
		}
		case BOOLEAN:
		{
			PascalAST tmp50_AST = null;
			tmp50_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp50_AST);
			match(BOOLEAN);
			typeIdentifier_AST = (PascalAST)currentAST.root;
			break;
		}
		case INTEGER:
		{
			PascalAST tmp51_AST = null;
			tmp51_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp51_AST);
			match(INTEGER);
			typeIdentifier_AST = (PascalAST)currentAST.root;
			break;
		}
		case REAL:
		{
			PascalAST tmp52_AST = null;
			tmp52_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp52_AST);
			match(REAL);
			typeIdentifier_AST = (PascalAST)currentAST.root;
			break;
		}
		case STRING:
		{
			PascalAST tmp53_AST = null;
			tmp53_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp53_AST);
			match(STRING);
			typeIdentifier_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = typeIdentifier_AST;
	}
	
	public final void stringtype() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST stringtype_AST = null;
		
		PascalAST tmp54_AST = null;
		tmp54_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp54_AST);
		match(STRING);
		match(LBRACK);
		{
		switch ( LA(1)) {
		case IDENT:
		{
			identifier();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case NUM_INT:
		case NUM_REAL:
		{
			unsignedNumber();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		match(RBRACK);
		stringtype_AST = (PascalAST)currentAST.root;
		returnAST = stringtype_AST;
	}
	
	public final void unpackedStructuredType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST unpackedStructuredType_AST = null;
		
		switch ( LA(1)) {
		case ARRAY:
		{
			arrayType();
			astFactory.addASTChild(currentAST, returnAST);
			unpackedStructuredType_AST = (PascalAST)currentAST.root;
			break;
		}
		case RECORD:
		{
			recordType();
			astFactory.addASTChild(currentAST, returnAST);
			unpackedStructuredType_AST = (PascalAST)currentAST.root;
			break;
		}
		case SET:
		{
			setType();
			astFactory.addASTChild(currentAST, returnAST);
			unpackedStructuredType_AST = (PascalAST)currentAST.root;
			break;
		}
		case FILE:
		{
			fileType();
			astFactory.addASTChild(currentAST, returnAST);
			unpackedStructuredType_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = unpackedStructuredType_AST;
	}
	
	public final void arrayType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST arrayType_AST = null;
		
		if ((LA(1)==ARRAY) && (LA(2)==LBRACK)) {
			PascalAST tmp57_AST = null;
			tmp57_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp57_AST);
			match(ARRAY);
			match(LBRACK);
			typeList();
			astFactory.addASTChild(currentAST, returnAST);
			match(RBRACK);
			match(OF);
			componentType();
			astFactory.addASTChild(currentAST, returnAST);
			arrayType_AST = (PascalAST)currentAST.root;
		}
		else if ((LA(1)==ARRAY) && (LA(2)==LBRACK2)) {
			PascalAST tmp61_AST = null;
			tmp61_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp61_AST);
			match(ARRAY);
			match(LBRACK2);
			typeList();
			astFactory.addASTChild(currentAST, returnAST);
			match(RBRACK2);
			match(OF);
			componentType();
			astFactory.addASTChild(currentAST, returnAST);
			arrayType_AST = (PascalAST)currentAST.root;
		}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		
		returnAST = arrayType_AST;
	}
	
	public final void recordType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST recordType_AST = null;
		
		PascalAST tmp65_AST = null;
		tmp65_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp65_AST);
		match(RECORD);
		fieldList();
		astFactory.addASTChild(currentAST, returnAST);
		match(END);
		recordType_AST = (PascalAST)currentAST.root;
		returnAST = recordType_AST;
	}
	
	public final void setType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST setType_AST = null;
		
		PascalAST tmp67_AST = null;
		tmp67_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp67_AST);
		match(SET);
		match(OF);
		baseType();
		astFactory.addASTChild(currentAST, returnAST);
		setType_AST = (PascalAST)currentAST.root;
		returnAST = setType_AST;
	}
	
	public final void fileType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST fileType_AST = null;
		
		if ((LA(1)==FILE) && (LA(2)==OF)) {
			PascalAST tmp69_AST = null;
			tmp69_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp69_AST);
			match(FILE);
			match(OF);
			type();
			astFactory.addASTChild(currentAST, returnAST);
			fileType_AST = (PascalAST)currentAST.root;
		}
		else if ((LA(1)==FILE) && (_tokenSet_4.member(LA(2)))) {
			PascalAST tmp71_AST = null;
			tmp71_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp71_AST);
			match(FILE);
			fileType_AST = (PascalAST)currentAST.root;
		}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		
		returnAST = fileType_AST;
	}
	
	public final void typeList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST typeList_AST = null;
		
		indexType();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop45:
		do {
			if ((LA(1)==COMMA)) {
				match(COMMA);
				indexType();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop45;
			}
			
		} while (true);
		}
		typeList_AST = (PascalAST)currentAST.root;
		typeList_AST = (PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(TYPELIST)).add(typeList_AST));
		currentAST.root = typeList_AST;
		currentAST.child = typeList_AST!=null &&typeList_AST.getFirstChild()!=null ?
			typeList_AST.getFirstChild() : typeList_AST;
		currentAST.advanceChildToEnd();
		typeList_AST = (PascalAST)currentAST.root;
		returnAST = typeList_AST;
	}
	
	public final void componentType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST componentType_AST = null;
		
		type();
		astFactory.addASTChild(currentAST, returnAST);
		componentType_AST = (PascalAST)currentAST.root;
		returnAST = componentType_AST;
	}
	
	public final void indexType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST indexType_AST = null;
		
		simpleType();
		astFactory.addASTChild(currentAST, returnAST);
		indexType_AST = (PascalAST)currentAST.root;
		returnAST = indexType_AST;
	}
	
	public final void fieldList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST fieldList_AST = null;
		
		{
		switch ( LA(1)) {
		case IDENT:
		{
			fixedPart();
			astFactory.addASTChild(currentAST, returnAST);
			{
			if ((LA(1)==SEMI) && (LA(2)==CASE)) {
				match(SEMI);
				variantPart();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else if ((LA(1)==SEMI) && (LA(2)==RPAREN||LA(2)==END)) {
				match(SEMI);
			}
			else if ((LA(1)==RPAREN||LA(1)==END)) {
			}
			else {
				throw new NoViableAltException(LT(1), getFilename());
			}
			
			}
			break;
		}
		case CASE:
		{
			variantPart();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		fieldList_AST = (PascalAST)currentAST.root;
		fieldList_AST=(PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(FIELDLIST)).add(fieldList_AST));
		currentAST.root = fieldList_AST;
		currentAST.child = fieldList_AST!=null &&fieldList_AST.getFirstChild()!=null ?
			fieldList_AST.getFirstChild() : fieldList_AST;
		currentAST.advanceChildToEnd();
		fieldList_AST = (PascalAST)currentAST.root;
		returnAST = fieldList_AST;
	}
	
	public final void fixedPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST fixedPart_AST = null;
		
		recordSection();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop54:
		do {
			if ((LA(1)==SEMI) && (LA(2)==IDENT)) {
				match(SEMI);
				recordSection();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop54;
			}
			
		} while (true);
		}
		fixedPart_AST = (PascalAST)currentAST.root;
		returnAST = fixedPart_AST;
	}
	
	public final void variantPart() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST variantPart_AST = null;
		
		PascalAST tmp76_AST = null;
		tmp76_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp76_AST);
		match(CASE);
		tag();
		astFactory.addASTChild(currentAST, returnAST);
		match(OF);
		variant();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop58:
		do {
			if ((LA(1)==SEMI) && (_tokenSet_0.member(LA(2)))) {
				match(SEMI);
				variant();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else if ((LA(1)==SEMI) && (_tokenSet_4.member(LA(2)))) {
				match(SEMI);
			}
			else {
				break _loop58;
			}
			
		} while (true);
		}
		variantPart_AST = (PascalAST)currentAST.root;
		returnAST = variantPart_AST;
	}
	
	public final void recordSection() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST recordSection_AST = null;
		
		identifierList();
		astFactory.addASTChild(currentAST, returnAST);
		match(COLON);
		type();
		astFactory.addASTChild(currentAST, returnAST);
		recordSection_AST = (PascalAST)currentAST.root;
		recordSection_AST = (PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(FIELD)).add(recordSection_AST));
		currentAST.root = recordSection_AST;
		currentAST.child = recordSection_AST!=null &&recordSection_AST.getFirstChild()!=null ?
			recordSection_AST.getFirstChild() : recordSection_AST;
		currentAST.advanceChildToEnd();
		recordSection_AST = (PascalAST)currentAST.root;
		returnAST = recordSection_AST;
	}
	
	public final void tag() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST tag_AST = null;
		PascalAST id_AST = null;
		PascalAST t_AST = null;
		PascalAST t2_AST = null;
		
		if ((LA(1)==IDENT) && (LA(2)==COLON)) {
			identifier();
			id_AST = (PascalAST)returnAST;
			PascalAST tmp81_AST = null;
			tmp81_AST = (PascalAST)astFactory.create(LT(1));
			match(COLON);
			typeIdentifier();
			t_AST = (PascalAST)returnAST;
			tag_AST = (PascalAST)currentAST.root;
			tag_AST=(PascalAST)astFactory.make( (new ASTArray(3)).add((PascalAST)astFactory.create(VARIANT_TAG)).add(id_AST).add(t_AST));
			currentAST.root = tag_AST;
			currentAST.child = tag_AST!=null &&tag_AST.getFirstChild()!=null ?
				tag_AST.getFirstChild() : tag_AST;
			currentAST.advanceChildToEnd();
		}
		else if ((_tokenSet_2.member(LA(1))) && (LA(2)==OF)) {
			typeIdentifier();
			t2_AST = (PascalAST)returnAST;
			tag_AST = (PascalAST)currentAST.root;
			tag_AST=(PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(VARIANT_TAG_NO_ID)).add(t2_AST));
			currentAST.root = tag_AST;
			currentAST.child = tag_AST!=null &&tag_AST.getFirstChild()!=null ?
				tag_AST.getFirstChild() : tag_AST;
			currentAST.advanceChildToEnd();
		}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		
		returnAST = tag_AST;
	}
	
	public final void variant() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST variant_AST = null;
		Token  c = null;
		PascalAST c_AST = null;
		
		constList();
		astFactory.addASTChild(currentAST, returnAST);
		c = LT(1);
		c_AST = (PascalAST)astFactory.create(c);
		astFactory.makeASTRoot(currentAST, c_AST);
		match(COLON);
		c_AST.setType(VARIANT_CASE);
		match(LPAREN);
		fieldList();
		astFactory.addASTChild(currentAST, returnAST);
		match(RPAREN);
		variant_AST = (PascalAST)currentAST.root;
		returnAST = variant_AST;
	}
	
	public final void constList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST constList_AST = null;
		
		constant();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop83:
		do {
			if ((LA(1)==COMMA)) {
				match(COMMA);
				constant();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop83;
			}
			
		} while (true);
		}
		constList_AST = (PascalAST)currentAST.root;
		constList_AST = (PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(CONSTLIST)).add(constList_AST));
		currentAST.root = constList_AST;
		currentAST.child = constList_AST!=null &&constList_AST.getFirstChild()!=null ?
			constList_AST.getFirstChild() : constList_AST;
		currentAST.advanceChildToEnd();
		constList_AST = (PascalAST)currentAST.root;
		returnAST = constList_AST;
	}
	
	public final void baseType() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST baseType_AST = null;
		
		simpleType();
		astFactory.addASTChild(currentAST, returnAST);
		baseType_AST = (PascalAST)currentAST.root;
		returnAST = baseType_AST;
	}
	
	public final void variableDeclaration() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST variableDeclaration_AST = null;
		Token  c = null;
		PascalAST c_AST = null;
		
		identifierList();
		astFactory.addASTChild(currentAST, returnAST);
		c = LT(1);
		c_AST = (PascalAST)astFactory.create(c);
		astFactory.makeASTRoot(currentAST, c_AST);
		match(COLON);
		c_AST.setType(VARDECL);
		type();
		astFactory.addASTChild(currentAST, returnAST);
		variableDeclaration_AST = (PascalAST)currentAST.root;
		returnAST = variableDeclaration_AST;
	}
	
	public final void procedureOrFunctionDeclaration() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST procedureOrFunctionDeclaration_AST = null;
		
		switch ( LA(1)) {
		case PROCEDURE:
		{
			procedureDeclaration();
			astFactory.addASTChild(currentAST, returnAST);
			procedureOrFunctionDeclaration_AST = (PascalAST)currentAST.root;
			break;
		}
		case FUNCTION:
		{
			functionDeclaration();
			astFactory.addASTChild(currentAST, returnAST);
			procedureOrFunctionDeclaration_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = procedureOrFunctionDeclaration_AST;
	}
	
	public final void procedureDeclaration() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST procedureDeclaration_AST = null;
		
		PascalAST tmp85_AST = null;
		tmp85_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp85_AST);
		match(PROCEDURE);
		identifier();
		astFactory.addASTChild(currentAST, returnAST);
		{
		switch ( LA(1)) {
		case LPAREN:
		{
			formalParameterList();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case SEMI:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		match(SEMI);
		block();
		astFactory.addASTChild(currentAST, returnAST);
		procedureDeclaration_AST = (PascalAST)currentAST.root;
		returnAST = procedureDeclaration_AST;
	}
	
	public final void functionDeclaration() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST functionDeclaration_AST = null;
		
		PascalAST tmp87_AST = null;
		tmp87_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp87_AST);
		match(FUNCTION);
		identifier();
		astFactory.addASTChild(currentAST, returnAST);
		{
		switch ( LA(1)) {
		case LPAREN:
		{
			formalParameterList();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case COLON:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		match(COLON);
		resultType();
		astFactory.addASTChild(currentAST, returnAST);
		match(SEMI);
		block();
		astFactory.addASTChild(currentAST, returnAST);
		functionDeclaration_AST = (PascalAST)currentAST.root;
		returnAST = functionDeclaration_AST;
	}
	
	public final void formalParameterSection() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST formalParameterSection_AST = null;
		
		switch ( LA(1)) {
		case IDENT:
		{
			parameterGroup();
			astFactory.addASTChild(currentAST, returnAST);
			formalParameterSection_AST = (PascalAST)currentAST.root;
			break;
		}
		case VAR:
		{
			PascalAST tmp90_AST = null;
			tmp90_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp90_AST);
			match(VAR);
			parameterGroup();
			astFactory.addASTChild(currentAST, returnAST);
			formalParameterSection_AST = (PascalAST)currentAST.root;
			break;
		}
		case FUNCTION:
		{
			PascalAST tmp91_AST = null;
			tmp91_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp91_AST);
			match(FUNCTION);
			parameterGroup();
			astFactory.addASTChild(currentAST, returnAST);
			formalParameterSection_AST = (PascalAST)currentAST.root;
			break;
		}
		case PROCEDURE:
		{
			PascalAST tmp92_AST = null;
			tmp92_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp92_AST);
			match(PROCEDURE);
			parameterGroup();
			astFactory.addASTChild(currentAST, returnAST);
			formalParameterSection_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = formalParameterSection_AST;
	}
	
	public final void parameterGroup() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST parameterGroup_AST = null;
		PascalAST ids_AST = null;
		PascalAST t_AST = null;
		
		identifierList();
		ids_AST = (PascalAST)returnAST;
		match(COLON);
		typeIdentifier();
		t_AST = (PascalAST)returnAST;
		parameterGroup_AST = (PascalAST)currentAST.root;
		parameterGroup_AST = (PascalAST)astFactory.make( (new ASTArray(3)).add((PascalAST)astFactory.create(ARGDECL)).add(ids_AST).add(t_AST));
		currentAST.root = parameterGroup_AST;
		currentAST.child = parameterGroup_AST!=null &&parameterGroup_AST.getFirstChild()!=null ?
			parameterGroup_AST.getFirstChild() : parameterGroup_AST;
		currentAST.advanceChildToEnd();
		returnAST = parameterGroup_AST;
	}
	
	public final void statement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST statement_AST = null;
		
		switch ( LA(1)) {
		case NUM_INT:
		{
			label();
			astFactory.addASTChild(currentAST, returnAST);
			PascalAST tmp94_AST = null;
			tmp94_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp94_AST);
			match(COLON);
			unlabelledStatement();
			astFactory.addASTChild(currentAST, returnAST);
			statement_AST = (PascalAST)currentAST.root;
			break;
		}
		case SEMI:
		case IDENT:
		case END:
		case CASE:
		case AT:
		case GOTO:
		case BEGIN:
		case IF:
		case ELSE:
		case WHILE:
		case REPEAT:
		case UNTIL:
		case FOR:
		case WITH:
		{
			unlabelledStatement();
			astFactory.addASTChild(currentAST, returnAST);
			statement_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = statement_AST;
	}
	
	public final void unlabelledStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST unlabelledStatement_AST = null;
		
		switch ( LA(1)) {
		case SEMI:
		case IDENT:
		case END:
		case AT:
		case GOTO:
		case ELSE:
		case UNTIL:
		{
			simpleStatement();
			astFactory.addASTChild(currentAST, returnAST);
			unlabelledStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case CASE:
		case BEGIN:
		case IF:
		case WHILE:
		case REPEAT:
		case FOR:
		case WITH:
		{
			structuredStatement();
			astFactory.addASTChild(currentAST, returnAST);
			unlabelledStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = unlabelledStatement_AST;
	}
	
	public final void simpleStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST simpleStatement_AST = null;
		
		switch ( LA(1)) {
		case GOTO:
		{
			gotoStatement();
			astFactory.addASTChild(currentAST, returnAST);
			simpleStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case SEMI:
		case END:
		case ELSE:
		case UNTIL:
		{
			emptyStatement();
			astFactory.addASTChild(currentAST, returnAST);
			simpleStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
			if ((LA(1)==IDENT||LA(1)==AT) && (_tokenSet_5.member(LA(2)))) {
				assignmentStatement();
				astFactory.addASTChild(currentAST, returnAST);
				simpleStatement_AST = (PascalAST)currentAST.root;
			}
			else if ((LA(1)==IDENT) && (_tokenSet_6.member(LA(2)))) {
				procedureStatement();
				astFactory.addASTChild(currentAST, returnAST);
				simpleStatement_AST = (PascalAST)currentAST.root;
			}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = simpleStatement_AST;
	}
	
	public final void structuredStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST structuredStatement_AST = null;
		
		switch ( LA(1)) {
		case BEGIN:
		{
			compoundStatement();
			astFactory.addASTChild(currentAST, returnAST);
			structuredStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case CASE:
		case IF:
		{
			conditionalStatement();
			astFactory.addASTChild(currentAST, returnAST);
			structuredStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case WHILE:
		case REPEAT:
		case FOR:
		{
			repetetiveStatement();
			astFactory.addASTChild(currentAST, returnAST);
			structuredStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case WITH:
		{
			withStatement();
			astFactory.addASTChild(currentAST, returnAST);
			structuredStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = structuredStatement_AST;
	}
	
	public final void assignmentStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST assignmentStatement_AST = null;
		
		variable();
		astFactory.addASTChild(currentAST, returnAST);
		PascalAST tmp95_AST = null;
		tmp95_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp95_AST);
		match(ASSIGN);
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		assignmentStatement_AST = (PascalAST)currentAST.root;
		returnAST = assignmentStatement_AST;
	}
	
	public final void procedureStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST procedureStatement_AST = null;
		PascalAST id_AST = null;
		PascalAST args_AST = null;
		
		identifier();
		id_AST = (PascalAST)returnAST;
		{
		switch ( LA(1)) {
		case LPAREN:
		{
			match(LPAREN);
			parameterList();
			args_AST = (PascalAST)returnAST;
			match(RPAREN);
			break;
		}
		case SEMI:
		case END:
		case ELSE:
		case UNTIL:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		procedureStatement_AST = (PascalAST)currentAST.root;
		procedureStatement_AST = (PascalAST)astFactory.make( (new ASTArray(3)).add((PascalAST)astFactory.create(PROC_CALL)).add(id_AST).add(args_AST));
		currentAST.root = procedureStatement_AST;
		currentAST.child = procedureStatement_AST!=null &&procedureStatement_AST.getFirstChild()!=null ?
			procedureStatement_AST.getFirstChild() : procedureStatement_AST;
		currentAST.advanceChildToEnd();
		returnAST = procedureStatement_AST;
	}
	
	public final void gotoStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST gotoStatement_AST = null;
		
		PascalAST tmp98_AST = null;
		tmp98_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp98_AST);
		match(GOTO);
		label();
		astFactory.addASTChild(currentAST, returnAST);
		gotoStatement_AST = (PascalAST)currentAST.root;
		returnAST = gotoStatement_AST;
	}
	
	public final void emptyStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST emptyStatement_AST = null;
		
		emptyStatement_AST = (PascalAST)currentAST.root;
		returnAST = emptyStatement_AST;
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
	public final void variable() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST variable_AST = null;
		
		{
		switch ( LA(1)) {
		case AT:
		{
			PascalAST tmp99_AST = null;
			tmp99_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp99_AST);
			match(AT);
			identifier();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case IDENT:
		{
			identifier();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		{
		_loop98:
		do {
			switch ( LA(1)) {
			case LBRACK:
			{
				PascalAST tmp100_AST = null;
				tmp100_AST = (PascalAST)astFactory.create(LT(1));
				astFactory.makeASTRoot(currentAST, tmp100_AST);
				match(LBRACK);
				expression();
				astFactory.addASTChild(currentAST, returnAST);
				{
				_loop95:
				do {
					if ((LA(1)==COMMA)) {
						match(COMMA);
						expression();
						astFactory.addASTChild(currentAST, returnAST);
					}
					else {
						break _loop95;
					}
					
				} while (true);
				}
				match(RBRACK);
				break;
			}
			case LBRACK2:
			{
				PascalAST tmp103_AST = null;
				tmp103_AST = (PascalAST)astFactory.create(LT(1));
				astFactory.makeASTRoot(currentAST, tmp103_AST);
				match(LBRACK2);
				expression();
				astFactory.addASTChild(currentAST, returnAST);
				{
				_loop97:
				do {
					if ((LA(1)==COMMA)) {
						match(COMMA);
						expression();
						astFactory.addASTChild(currentAST, returnAST);
					}
					else {
						break _loop97;
					}
					
				} while (true);
				}
				match(RBRACK2);
				break;
			}
			case DOT:
			{
				PascalAST tmp106_AST = null;
				tmp106_AST = (PascalAST)astFactory.create(LT(1));
				astFactory.makeASTRoot(currentAST, tmp106_AST);
				match(DOT);
				identifier();
				astFactory.addASTChild(currentAST, returnAST);
				break;
			}
			case POINTER:
			{
				PascalAST tmp107_AST = null;
				tmp107_AST = (PascalAST)astFactory.create(LT(1));
				astFactory.makeASTRoot(currentAST, tmp107_AST);
				match(POINTER);
				break;
			}
			default:
			{
				break _loop98;
			}
			}
		} while (true);
		}
		variable_AST = (PascalAST)currentAST.root;
		returnAST = variable_AST;
	}
	
	public final void expression() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST expression_AST = null;
		
		simpleExpression();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop102:
		do {
			if ((_tokenSet_7.member(LA(1)))) {
				{
				switch ( LA(1)) {
				case EQUAL:
				{
					PascalAST tmp108_AST = null;
					tmp108_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp108_AST);
					match(EQUAL);
					break;
				}
				case NOT_EQUAL:
				{
					PascalAST tmp109_AST = null;
					tmp109_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp109_AST);
					match(NOT_EQUAL);
					break;
				}
				case LT:
				{
					PascalAST tmp110_AST = null;
					tmp110_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp110_AST);
					match(LT);
					break;
				}
				case LE:
				{
					PascalAST tmp111_AST = null;
					tmp111_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp111_AST);
					match(LE);
					break;
				}
				case GE:
				{
					PascalAST tmp112_AST = null;
					tmp112_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp112_AST);
					match(GE);
					break;
				}
				case GT:
				{
					PascalAST tmp113_AST = null;
					tmp113_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp113_AST);
					match(GT);
					break;
				}
				case IN:
				{
					PascalAST tmp114_AST = null;
					tmp114_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp114_AST);
					match(IN);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				}
				}
				simpleExpression();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop102;
			}
			
		} while (true);
		}
		expression_AST = (PascalAST)currentAST.root;
		returnAST = expression_AST;
	}
	
	public final void simpleExpression() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST simpleExpression_AST = null;
		
		term();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop106:
		do {
			if ((_tokenSet_8.member(LA(1)))) {
				{
				switch ( LA(1)) {
				case PLUS:
				{
					PascalAST tmp115_AST = null;
					tmp115_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp115_AST);
					match(PLUS);
					break;
				}
				case MINUS:
				{
					PascalAST tmp116_AST = null;
					tmp116_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp116_AST);
					match(MINUS);
					break;
				}
				case OR:
				{
					PascalAST tmp117_AST = null;
					tmp117_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp117_AST);
					match(OR);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				}
				}
				term();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop106;
			}
			
		} while (true);
		}
		simpleExpression_AST = (PascalAST)currentAST.root;
		returnAST = simpleExpression_AST;
	}
	
	public final void term() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST term_AST = null;
		
		signedFactor();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop110:
		do {
			if (((LA(1) >= STAR && LA(1) <= AND))) {
				{
				switch ( LA(1)) {
				case STAR:
				{
					PascalAST tmp118_AST = null;
					tmp118_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp118_AST);
					match(STAR);
					break;
				}
				case SLASH:
				{
					PascalAST tmp119_AST = null;
					tmp119_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp119_AST);
					match(SLASH);
					break;
				}
				case DIV:
				{
					PascalAST tmp120_AST = null;
					tmp120_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp120_AST);
					match(DIV);
					break;
				}
				case MOD:
				{
					PascalAST tmp121_AST = null;
					tmp121_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp121_AST);
					match(MOD);
					break;
				}
				case AND:
				{
					PascalAST tmp122_AST = null;
					tmp122_AST = (PascalAST)astFactory.create(LT(1));
					astFactory.makeASTRoot(currentAST, tmp122_AST);
					match(AND);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				}
				}
				signedFactor();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop110;
			}
			
		} while (true);
		}
		term_AST = (PascalAST)currentAST.root;
		returnAST = term_AST;
	}
	
	public final void signedFactor() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST signedFactor_AST = null;
		
		{
		switch ( LA(1)) {
		case PLUS:
		{
			PascalAST tmp123_AST = null;
			tmp123_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp123_AST);
			match(PLUS);
			break;
		}
		case MINUS:
		{
			PascalAST tmp124_AST = null;
			tmp124_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp124_AST);
			match(MINUS);
			break;
		}
		case LPAREN:
		case IDENT:
		case CHR:
		case NUM_INT:
		case NUM_REAL:
		case STRING_LITERAL:
		case LBRACK:
		case LBRACK2:
		case AT:
		case NOT:
		case NIL:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		factor();
		astFactory.addASTChild(currentAST, returnAST);
		signedFactor_AST = (PascalAST)currentAST.root;
		returnAST = signedFactor_AST;
	}
	
	public final void factor() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST factor_AST = null;
		
		switch ( LA(1)) {
		case LPAREN:
		{
			match(LPAREN);
			expression();
			astFactory.addASTChild(currentAST, returnAST);
			match(RPAREN);
			factor_AST = (PascalAST)currentAST.root;
			break;
		}
		case CHR:
		case NUM_INT:
		case NUM_REAL:
		case STRING_LITERAL:
		case NIL:
		{
			unsignedConstant();
			astFactory.addASTChild(currentAST, returnAST);
			factor_AST = (PascalAST)currentAST.root;
			break;
		}
		case LBRACK:
		case LBRACK2:
		{
			set();
			astFactory.addASTChild(currentAST, returnAST);
			factor_AST = (PascalAST)currentAST.root;
			break;
		}
		case NOT:
		{
			PascalAST tmp127_AST = null;
			tmp127_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp127_AST);
			match(NOT);
			factor();
			astFactory.addASTChild(currentAST, returnAST);
			factor_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
			if ((LA(1)==IDENT||LA(1)==AT) && (_tokenSet_9.member(LA(2)))) {
				variable();
				astFactory.addASTChild(currentAST, returnAST);
				factor_AST = (PascalAST)currentAST.root;
			}
			else if ((LA(1)==IDENT) && (LA(2)==LPAREN)) {
				functionDesignator();
				astFactory.addASTChild(currentAST, returnAST);
				factor_AST = (PascalAST)currentAST.root;
			}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = factor_AST;
	}
	
	public final void functionDesignator() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST functionDesignator_AST = null;
		PascalAST id_AST = null;
		PascalAST args_AST = null;
		
		identifier();
		id_AST = (PascalAST)returnAST;
		match(LPAREN);
		parameterList();
		args_AST = (PascalAST)returnAST;
		match(RPAREN);
		functionDesignator_AST = (PascalAST)currentAST.root;
		functionDesignator_AST = (PascalAST)astFactory.make( (new ASTArray(3)).add((PascalAST)astFactory.create(FUNC_CALL)).add(id_AST).add(args_AST));
		currentAST.root = functionDesignator_AST;
		currentAST.child = functionDesignator_AST!=null &&functionDesignator_AST.getFirstChild()!=null ?
			functionDesignator_AST.getFirstChild() : functionDesignator_AST;
		currentAST.advanceChildToEnd();
		returnAST = functionDesignator_AST;
	}
	
	public final void unsignedConstant() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST unsignedConstant_AST = null;
		
		switch ( LA(1)) {
		case NUM_INT:
		case NUM_REAL:
		{
			unsignedNumber();
			astFactory.addASTChild(currentAST, returnAST);
			unsignedConstant_AST = (PascalAST)currentAST.root;
			break;
		}
		case CHR:
		{
			constantChr();
			astFactory.addASTChild(currentAST, returnAST);
			unsignedConstant_AST = (PascalAST)currentAST.root;
			break;
		}
		case STRING_LITERAL:
		{
			string();
			astFactory.addASTChild(currentAST, returnAST);
			unsignedConstant_AST = (PascalAST)currentAST.root;
			break;
		}
		case NIL:
		{
			PascalAST tmp130_AST = null;
			tmp130_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.addASTChild(currentAST, tmp130_AST);
			match(NIL);
			unsignedConstant_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = unsignedConstant_AST;
	}
	
	public final void set() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST set_AST = null;
		
		switch ( LA(1)) {
		case LBRACK:
		{
			PascalAST tmp131_AST = null;
			tmp131_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp131_AST);
			match(LBRACK);
			elementList();
			astFactory.addASTChild(currentAST, returnAST);
			match(RBRACK);
			set_AST = (PascalAST)currentAST.root;
			set_AST.setType(SET);
			set_AST = (PascalAST)currentAST.root;
			break;
		}
		case LBRACK2:
		{
			PascalAST tmp133_AST = null;
			tmp133_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp133_AST);
			match(LBRACK2);
			elementList();
			astFactory.addASTChild(currentAST, returnAST);
			match(RBRACK2);
			set_AST = (PascalAST)currentAST.root;
			set_AST.setType(SET);
			set_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = set_AST;
	}
	
	public final void parameterList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST parameterList_AST = null;
		
		actualParameter();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop118:
		do {
			if ((LA(1)==COMMA)) {
				match(COMMA);
				actualParameter();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop118;
			}
			
		} while (true);
		}
		parameterList_AST = (PascalAST)currentAST.root;
		parameterList_AST = (PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(ARGLIST)).add(parameterList_AST));
		currentAST.root = parameterList_AST;
		currentAST.child = parameterList_AST!=null &&parameterList_AST.getFirstChild()!=null ?
			parameterList_AST.getFirstChild() : parameterList_AST;
		currentAST.advanceChildToEnd();
		parameterList_AST = (PascalAST)currentAST.root;
		returnAST = parameterList_AST;
	}
	
	public final void actualParameter() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST actualParameter_AST = null;
		
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		actualParameter_AST = (PascalAST)currentAST.root;
		returnAST = actualParameter_AST;
	}
	
	public final void elementList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST elementList_AST = null;
		
		switch ( LA(1)) {
		case LPAREN:
		case IDENT:
		case CHR:
		case NUM_INT:
		case NUM_REAL:
		case PLUS:
		case MINUS:
		case STRING_LITERAL:
		case LBRACK:
		case LBRACK2:
		case AT:
		case NOT:
		case NIL:
		{
			element();
			astFactory.addASTChild(currentAST, returnAST);
			{
			_loop122:
			do {
				if ((LA(1)==COMMA)) {
					match(COMMA);
					element();
					astFactory.addASTChild(currentAST, returnAST);
				}
				else {
					break _loop122;
				}
				
			} while (true);
			}
			elementList_AST = (PascalAST)currentAST.root;
			break;
		}
		case RBRACK:
		case RBRACK2:
		{
			elementList_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = elementList_AST;
	}
	
	public final void element() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST element_AST = null;
		
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		{
		switch ( LA(1)) {
		case DOTDOT:
		{
			PascalAST tmp137_AST = null;
			tmp137_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp137_AST);
			match(DOTDOT);
			expression();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case COMMA:
		case RBRACK:
		case RBRACK2:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		element_AST = (PascalAST)currentAST.root;
		returnAST = element_AST;
	}
	
	public final void empty() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST empty_AST = null;
		
		empty_AST = (PascalAST)currentAST.root;
		returnAST = empty_AST;
	}
	
	public final void conditionalStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST conditionalStatement_AST = null;
		
		switch ( LA(1)) {
		case IF:
		{
			ifStatement();
			astFactory.addASTChild(currentAST, returnAST);
			conditionalStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case CASE:
		{
			caseStatement();
			astFactory.addASTChild(currentAST, returnAST);
			conditionalStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = conditionalStatement_AST;
	}
	
	public final void repetetiveStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST repetetiveStatement_AST = null;
		
		switch ( LA(1)) {
		case WHILE:
		{
			whileStatement();
			astFactory.addASTChild(currentAST, returnAST);
			repetetiveStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case REPEAT:
		{
			repeatStatement();
			astFactory.addASTChild(currentAST, returnAST);
			repetetiveStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		case FOR:
		{
			forStatement();
			astFactory.addASTChild(currentAST, returnAST);
			repetetiveStatement_AST = (PascalAST)currentAST.root;
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		returnAST = repetetiveStatement_AST;
	}
	
	public final void withStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST withStatement_AST = null;
		
		PascalAST tmp138_AST = null;
		tmp138_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp138_AST);
		match(WITH);
		recordVariableList();
		astFactory.addASTChild(currentAST, returnAST);
		match(DO);
		statement();
		astFactory.addASTChild(currentAST, returnAST);
		withStatement_AST = (PascalAST)currentAST.root;
		returnAST = withStatement_AST;
	}
	
	public final void statements() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST statements_AST = null;
		
		statement();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop135:
		do {
			if ((LA(1)==SEMI)) {
				match(SEMI);
				statement();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop135;
			}
			
		} while (true);
		}
		statements_AST = (PascalAST)currentAST.root;
		statements_AST = (PascalAST)astFactory.make( (new ASTArray(2)).add((PascalAST)astFactory.create(BLOCK)).add(statements_AST));
		currentAST.root = statements_AST;
		currentAST.child = statements_AST!=null &&statements_AST.getFirstChild()!=null ?
			statements_AST.getFirstChild() : statements_AST;
		currentAST.advanceChildToEnd();
		statements_AST = (PascalAST)currentAST.root;
		returnAST = statements_AST;
	}
	
	public final void ifStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST ifStatement_AST = null;
		
		PascalAST tmp141_AST = null;
		tmp141_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp141_AST);
		match(IF);
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		match(THEN);
		statement();
		astFactory.addASTChild(currentAST, returnAST);
		{
		if ((LA(1)==ELSE) && (_tokenSet_10.member(LA(2)))) {
			match(ELSE);
			statement();
			astFactory.addASTChild(currentAST, returnAST);
		}
		else if ((_tokenSet_11.member(LA(1))) && (_tokenSet_12.member(LA(2)))) {
		}
		else {
			throw new NoViableAltException(LT(1), getFilename());
		}
		
		}
		ifStatement_AST = (PascalAST)currentAST.root;
		returnAST = ifStatement_AST;
	}
	
	public final void caseStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST caseStatement_AST = null;
		
		PascalAST tmp144_AST = null;
		tmp144_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp144_AST);
		match(CASE);
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		match(OF);
		caseListElement();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop141:
		do {
			if ((LA(1)==SEMI) && (_tokenSet_0.member(LA(2)))) {
				match(SEMI);
				caseListElement();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop141;
			}
			
		} while (true);
		}
		{
		switch ( LA(1)) {
		case SEMI:
		{
			match(SEMI);
			match(ELSE);
			statements();
			astFactory.addASTChild(currentAST, returnAST);
			break;
		}
		case END:
		{
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		match(END);
		caseStatement_AST = (PascalAST)currentAST.root;
		returnAST = caseStatement_AST;
	}
	
	public final void caseListElement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST caseListElement_AST = null;
		
		constList();
		astFactory.addASTChild(currentAST, returnAST);
		PascalAST tmp150_AST = null;
		tmp150_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp150_AST);
		match(COLON);
		statement();
		astFactory.addASTChild(currentAST, returnAST);
		caseListElement_AST = (PascalAST)currentAST.root;
		returnAST = caseListElement_AST;
	}
	
	public final void whileStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST whileStatement_AST = null;
		
		PascalAST tmp151_AST = null;
		tmp151_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp151_AST);
		match(WHILE);
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		match(DO);
		statement();
		astFactory.addASTChild(currentAST, returnAST);
		whileStatement_AST = (PascalAST)currentAST.root;
		returnAST = whileStatement_AST;
	}
	
	public final void repeatStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST repeatStatement_AST = null;
		
		PascalAST tmp153_AST = null;
		tmp153_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp153_AST);
		match(REPEAT);
		statements();
		astFactory.addASTChild(currentAST, returnAST);
		match(UNTIL);
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		repeatStatement_AST = (PascalAST)currentAST.root;
		returnAST = repeatStatement_AST;
	}
	
	public final void forStatement() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST forStatement_AST = null;
		
		PascalAST tmp155_AST = null;
		tmp155_AST = (PascalAST)astFactory.create(LT(1));
		astFactory.makeASTRoot(currentAST, tmp155_AST);
		match(FOR);
		identifier();
		astFactory.addASTChild(currentAST, returnAST);
		match(ASSIGN);
		forList();
		astFactory.addASTChild(currentAST, returnAST);
		match(DO);
		statement();
		astFactory.addASTChild(currentAST, returnAST);
		forStatement_AST = (PascalAST)currentAST.root;
		returnAST = forStatement_AST;
	}
	
	public final void forList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST forList_AST = null;
		
		initialValue();
		astFactory.addASTChild(currentAST, returnAST);
		{
		switch ( LA(1)) {
		case TO:
		{
			PascalAST tmp158_AST = null;
			tmp158_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp158_AST);
			match(TO);
			break;
		}
		case DOWNTO:
		{
			PascalAST tmp159_AST = null;
			tmp159_AST = (PascalAST)astFactory.create(LT(1));
			astFactory.makeASTRoot(currentAST, tmp159_AST);
			match(DOWNTO);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		}
		}
		finalValue();
		astFactory.addASTChild(currentAST, returnAST);
		forList_AST = (PascalAST)currentAST.root;
		returnAST = forList_AST;
	}
	
	public final void initialValue() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST initialValue_AST = null;
		
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		initialValue_AST = (PascalAST)currentAST.root;
		returnAST = initialValue_AST;
	}
	
	public final void finalValue() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST finalValue_AST = null;
		
		expression();
		astFactory.addASTChild(currentAST, returnAST);
		finalValue_AST = (PascalAST)currentAST.root;
		returnAST = finalValue_AST;
	}
	
	public final void recordVariableList() throws RecognitionException, TokenStreamException {
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		PascalAST recordVariableList_AST = null;
		
		variable();
		astFactory.addASTChild(currentAST, returnAST);
		{
		_loop155:
		do {
			if ((LA(1)==COMMA)) {
				match(COMMA);
				variable();
				astFactory.addASTChild(currentAST, returnAST);
			}
			else {
				break _loop155;
			}
			
		} while (true);
		}
		recordVariableList_AST = (PascalAST)currentAST.root;
		returnAST = recordVariableList_AST;
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
	
	protected void buildTokenTypeASTClassMap() {
		tokenTypeToASTClassMap=null;
	};
	
	private static final long[] mk_tokenSet_0() {
		long[] data = { 4329863905280L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_0 = new BitSet(mk_tokenSet_0());
	private static final long[] mk_tokenSet_1() {
		long[] data = { 70781631463424L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_1 = new BitSet(mk_tokenSet_1());
	private static final long[] mk_tokenSet_2() {
		long[] data = { 4362862675886080L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_2 = new BitSet(mk_tokenSet_2());
	private static final long[] mk_tokenSet_3() {
		long[] data = { 1459166288059301888L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_3 = new BitSet(mk_tokenSet_3());
	private static final long[] mk_tokenSet_4() {
		long[] data = { 1152921504808173568L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_4 = new BitSet(mk_tokenSet_4());
	private static final long[] mk_tokenSet_5() {
		long[] data = { 153122387875856384L, 5L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_5 = new BitSet(mk_tokenSet_5());
	private static final long[] mk_tokenSet_6() {
		long[] data = { 1152921504774619136L, 71303168L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_6 = new BitSet(mk_tokenSet_6());
	private static final long[] mk_tokenSet_7() {
		long[] data = { 34359738368L, 1008L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_7 = new BitSet(mk_tokenSet_7());
	private static final long[] mk_tokenSet_8() {
		long[] data = { 1649267441664L, 1024L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_8 = new BitSet(mk_tokenSet_8());
	private static final long[] mk_tokenSet_9() {
		long[] data = { 1684418322344443904L, 895549425L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_9 = new BitSet(mk_tokenSet_9());
	private static final long[] mk_tokenSet_10() {
		long[] data = { 3458764651930583040L, 1323040776L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_10 = new BitSet(mk_tokenSet_10());
	private static final long[] mk_tokenSet_11() {
		long[] data = { 1152921504741064704L, 71303168L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_11 = new BitSet(mk_tokenSet_11());
	private static final long[] mk_tokenSet_12() {
		long[] data = { 3611891231191203840L, 1323237384L, 0L, 0L};
		return data;
	}
	public static final BitSet _tokenSet_12 = new BitSet(mk_tokenSet_12());
	
	}
