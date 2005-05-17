//
// Pascal Tree Grammar that walks tree, building symtab
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

{
import java.util.*;
import java.io.*;
}

class SymtabPhase extends PascalTreeParserSuper;

options {
        importVocab = Pascal;
        ASTLabelType = "PascalAST";
}

{
Stack scopes = new Stack();
Stack usesScopes = new Stack();
//public static File thisUnit;
public File thisUnit;
}

program
    : programHeading
      block
      {System.out.println(scopes.peek());}
    ;

programHeading
    : #(PROGRAM IDENT identifierList)
      {Scope root = new Scope(null); 
       scopes.push(root);}
    | #(UNIT IDENT)
      {
      Scope root = new Scope(null); // create new scope
      scopes.push(root);            // enter new scope :)
      root.addSymbol(new Unit(#IDENT.getText())); // create unit symbol entry

      String tUnit = new String (#IDENT.getText());
      tUnit = tUnit.concat(".sym");
      thisUnit = new File (PascalParser.translateFilePath, tUnit);
      }
    ;

block
    : ( labelDeclarationPart
      | constantDefinitionPart
      | typeDefinitionPart
      | variableDeclarationPart
      | procedureAndFunctionDeclarationPart
      | usesUnitsPart
      | IMPLEMENTATION
        { System.out.println(scopes.peek()); 
            //write symbol table and exit when currentFileName != translateFileName
            if  (PascalParser.currentFileName.compareTo(PascalParser.translateFileName) != 0) {
                try{
                    ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream(thisUnit));
                    oos.writeObject(scopes);
                    oos.close();
                }
                catch (IOException e) {
                    System.err.println("IOexception: "+e); }
                _t = null;
                throw new RecognitionException();
            }
        }
      )*
      compoundStatement
    ;

usesUnitsPart
{
String usesFile, usesSymtab;
String oldUsesFile;
File f, symFile;
Object readSymtab = new Stack();
}
    : #(USES usesList:identifierList)
    // walk identifierList and make symbol table
    {
        usesList = (PascalAST) usesList.getFirstChild();
        oldUsesFile = PascalParser.currentFileName;
        while (usesList !=null) {
            // make symboltable for usesFile
            usesFile = usesList.getText();
            usesFile = usesFile.concat(".pas");
            usesSymtab = usesList.getText();
            usesSymtab = usesSymtab.concat(".sym");

            f = new File(PascalParser.translateFilePath,usesFile);
            symFile = new File(PascalParser.translateFilePath,usesSymtab);

			// we have to build the symbol table when ...
			if (( !(symFile.exists() ) )
			// this .sym file (usesSymtyb) does not exist or ...
			   || ((f.lastModified()) > (symFile.lastModified()) ) ){
			// the date of this .sym file (usesSymtyb) is older than the .pas file (usesFile)

                try {
                PascalParser.parseFile(f.getName(),new FileInputStream(f));
                }
                catch (Exception e) {
                    System.err.println("parser exception: "+e);
                    e.printStackTrace();   // so we can get stack trace
                }
            }

            // read serialized symbol table and add to symbol table "usesScopes"
            File symTab = new File (PascalParser.translateFilePath , usesSymtab);
            try {
                ObjectInputStream ois = new ObjectInputStream(new FileInputStream(symTab));
                readSymtab = ois.readObject();
                ois.close();
            }
            catch (ClassNotFoundException cnfe) {
                System.err.println("parser exception: "+cnfe);
                cnfe.printStackTrace();   // so we can get stack trace
            }
            catch (FileNotFoundException e) {
                System.err.println("parser exception: "+e);
                e.printStackTrace();   // so we can get stack trace
			}
            catch (IOException e) {
                System.err.println("parser exception: "+e);
                e.printStackTrace();   // so we can get stack trace
            }

            Stack symTabToPop = (Stack) readSymtab;

            // add uses symbol table "readSymtab" to symboltable "usesScopes"
            while (!(symTabToPop.empty())) {
                usesScopes.push(symTabToPop.pop());
            }

            usesList = (PascalAST) usesList.getNextSibling();
        }
        PascalParser.currentFileName = oldUsesFile;
    }
    ;

constantDefinition
{
Constant c = null;
}
    : #(r:EQUAL IDENT c=constant)
      {
      if ( c!=null ) {
        Scope sc = (Scope)scopes.peek();
        c.setName(#IDENT.getText());
        sc.addSymbol(c);
        r.symbol = c; // AST def root points to symbol table entry now
      }
      }
    ;

constant returns [Constant c=null]
    : NUM_INT
      {c = new IntegerConstant(#NUM_INT.getText());}
    | NUM_REAL
      {c = new RealConstant(#NUM_REAL.getText());}
    | #( PLUS
         ( NUM_INT
           {c = new IntegerConstant(#NUM_INT.getText());}
         | NUM_REAL
           {c = new RealConstant(#NUM_REAL.getText());}
         | IDENT
         )
       )
    | #( MINUS
         ( NUM_INT
           {c = new IntegerConstant(#NUM_INT.getText());}
         | NUM_REAL
           {c = new RealConstant(#NUM_REAL.getText());}
         | IDENT
         )
       )
    | IDENT
    | STRING_LITERAL
    | #(CHR (NUM_INT|NUM_REAL))
    ;

typeDefinition
{
TypeSpecifier t=null;
}
    : #(TYPEDECL IDENT
      ( type 
      | #(r:FUNCTION (formalParameterList)? t=resultType)
      {
      if ( t!=null ) {r.symbol = t;}
      }

      | #(PROCEDURE (formalParameterList)?)
      )
      )
    ;

type returns [TypeSpecifier ts=null]
    : #(SCALARTYPE identifierList)
    | #(DOTDOT constant constant)
    | ts=typeIdentifier
    | structuredType
    | #(POINTER typeIdentifier)
    ;

typeIdentifier returns [TypeSpecifier ts=null]
    : IDENT // lookup and return type spec
    | CHAR
    | BOOLEAN
    | INTEGER {ts=PascalParser.symbolTable.getPredefinedType("integer");}
    | REAL {ts=PascalParser.symbolTable.getPredefinedType("real");}
    | #( STRING
         ( IDENT
         | NUM_INT
         | NUM_REAL
         |
         )
       )
    ;

variableDeclaration
{
Vector ids=null;
TypeSpecifier t=null;
}
    : #(r:VARDECL ids=identifierList t=type)
      {
      // walk list of identifiers, creating variable syms and setting types
      if ( t!=null ) {
        Scope sc = (Scope)scopes.peek();
        for (int i=0; ids!=null && i<ids.size(); i++) {
          String id = (String)ids.elementAt(i);
          Variable v = new Variable(id,t);
          sc.addSymbol(v);
          r.symbol = t; // AST def root points to symbol table entry now
       }
      }
      }
    ;

parameterGroup
{
Vector ids=null;
TypeSpecifier t=null;
}
    : #(r:ARGDECL ids=identifierList t=typeIdentifier)
      {
      // walk list of identifiers, creating variable syms and setting types
      if ( t!=null ) {
        Scope sc = (Scope)scopes.peek();
        for (int i=0; ids!=null && i<ids.size(); i++) {
          String id = (String)ids.elementAt(i);
          Variable v = new Variable(id,t);
          sc.addSymbol(v);
          r.symbol = t; // AST def root points to symbol table entry now
       }
      }
      }

    ;

identifierList returns [Vector ids=new Vector()]
    : #( IDLIST ( IDENT {ids.addElement(#IDENT.getText());} )+ )
    ;

functionDeclaration
{
TypeSpecifier t=null;
}
    : #(r:FUNCTION IDENT (formalParameterList)? t=resultType block)
      {
      if ( t!=null ) {r.symbol = t;}
      }
    ;

resultType returns [TypeSpecifier ts=null]
    : ts=typeIdentifier
    ;
