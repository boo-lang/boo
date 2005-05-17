header{
	package tinybasic;
}
	
class TinyBasicTreeWalker extends TreeParser;

options {
	importVocab = TinyBasic;
}
{
	Context theContext=null;
	DTDataType zero=null,posOne=null,negOne=null ;
	
	//protected void blah() throws DTExecException{
	//	throw new DTExecException("oops");
	//}


}

compilationUnit[Context context]
	{
		theContext=context;
		theContext.initialize();
		zero	= new DTInteger(theContext.getCurrentScope(),0);
		posOne	= new DTInteger(theContext.getCurrentScope(),1);
		negOne	= new DTInteger(theContext.getCurrentScope(),-1);
	}
	:	
		pd:PROGRAM_DEF
		{
			try{
				programDefinition(pd);
			} catch(DTExecException didit){
				System.out.println("Yes it works!"+didit);
			}
		}
	;

programDefinition
	:	#( PROGRAM_DEF moduleBody )
	;


subroutineDefinition
	:	#( SUBROUTINE_DEF IDENT moduleBody )
	;
	
moduleBody
	:	#(PARAMETERS parameters )
		#(CODE_BLOCK ( statement )+ )
		EXIT_MODULE
		{
			throw new DTExitModuleException("Done folks");
		}
	;


parameters
	:
		(parameter)*
	;


parameter
	{ int argNum=0;}
	:
		#(VAR_PROXY arg[argNum]{ argNum++;} )
	;
	
arg[int argNum]
	{ DTDataType v=null; }
	:
	(
		s:STR_VAR	//{v=theContext.ensureVariable(s.getText(),STR_VAR);}
	|	i:INT_VAR	//{v=theContext.ensureVariable(i.getText(),INT_VAR);}
	|	f:FLT_VAR	//{v=theContext.ensureVariable(f.getText(),FLT_VAR);}
	)
	;


statement
	:
		when_error_call_statement
	|	dim_statement			// done
	|	assign_statement		// done
	|	forNextStatement		// done
	|	printAsciiStatement		// done
	|	ifThenBlock			// done
	|	doUntilLoop			// done
	|	doLoopUntil			// done
	|	subExecuteStatement		// done
	|	exitModuleStatement		// done
	;

exitModuleStatement
	:
	EXIT_MODULE
	{
		throw new DTExitModuleException("Asnychronous return");
	}
	;
subExecuteStatement
	{ DTCodeType sub=null; int argNum=0;DTDataType tbd=null; }
	:
		#(SUB_EXECUTE	i:IDENT
			{
				sub=theContext.getSubroutine(i.getText());
			}
			#(ARGLIST
				(
					tbd=argExpr
					{ sub.attachArg(argNum++,tbd); }
				)*
			
			)
			{
				try {
					sub.newCall(theContext);
					try{
						subroutineDefinition(sub.getAST());
					} catch(DTExitModuleException didit){
						System.out.println("Yes it works!"+didit);
					}
				} catch(ANTLRException ex){
				}
			}
		)
	;


argExpr	returns [DTDataType tbd]
	{tbd=null;}
	:
		a1d:ARRAY1D	{ tbd=theContext.getVariable(a1d.getText()); }
	|	a2d:ARRAY2D	{ tbd=theContext.getVariable(a2d.getText()); }
	|	a3d:ARRAY3D	{ tbd=theContext.getVariable(a3d.getText()); }
	|	tbd=expr
	;	

printAsciiStatement
	:
		#(PRINT_ASCII (printField)* { System.out.println();} )
	;

printField
	{DTDataType d=null;}
	:
		#(PRINT_NUMERIC	d=expr	{ System.out.print(d);})
	|	#(PRINT_STRING	d=expr	{ System.out.print(d);})
	|	#(PRINT_TAB	d=expr)
	|	PRINT_COMMA	{ System.out.print("\t");}
	|	PRINT_SEMI
	;

assign_statement returns [DTDataType d]
	{ DTDataType e=null;d=null; }
	:	#(
			EQ d=data_store e=expr
			{
				d.assign(e);
			}
		)
	;

expr	returns [DTDataType e]
	{ DTDataType c=null,d=null,e1=null,e2=null,e3=null;e=null;}
	:
		#(
			STAR e1=expr e2=expr
			{ e=e1.multiply(e2);}
		)
	|	#(
			PLUS e1=expr e2=expr
			{ e=e1.add(e2);}
		)
	|	#(
			SLASH e1=expr e2=expr
			{ e=e1.multiply(e2);}
		)
	|	#(
			"div" e1=expr e2=expr
			{ e=e1.divide(e2);}
		)
	|	#(
			"mod" e1=expr e2=expr
			{ e=e1.mod(e2);}
		)
	// comparison operators
	|	#(
			EQ_COMP e1=expr e2=expr
			{ e=new DTInteger(null, e1.compareTo(e2)==0 ? 1:0);}
		)
	|	#(
			NE_COMP e1=expr e2=expr
			{ e=new DTInteger(null, e1.compareTo(e2)==0 ? 0:1);}
		)
	|	#(
			LE e1=expr e2=expr
			{ e=new DTInteger(null, e1.compareTo(e2)<=0 ? 1:0);}
		)
	|	#(
			LT e1=expr e2=expr
			{ e=new DTInteger(null, e1.compareTo(e2) <0 ? 1:0);}
		)
	|	#(
			GE e1=expr e2=expr
			{ e=new DTInteger(null, e1.compareTo(e2)>=0 ? 1:0);}
		)
	|	#(
			GT e1=expr e2=expr
			{ e=new DTInteger(null, e1.compareTo(e2) >0 ? 1:0);}
		)
	// Boolean algebra
	|	#(
			"xor" e1=expr e2=expr
			{ e=new DTInteger(null,(e1.getInteger()!=e2.getInteger()) ? 1:0 );}
		)
	|	#(
			"and" e1=expr e2=expr
			{ e=new DTInteger(null,(e1.getInteger()==1&&e2.getInteger()==1) ? 1:0 );}
		)
	|	#(
			"or" e1=expr e2=expr
			{ e=new DTInteger(null,(e1.getInteger()==1||e2.getInteger()==1) ? 1:0 );}
		)
	// unary operators
	|	#(
			"not" e1=expr
			{ e=new DTInteger(null,e1.getInteger()==0 ? 1:0 );}
		)
	|	#(
			UNARY_PLUS e1=expr
			{ e=e1;}
		)
	|	#(
			UNARY_MINUS e1=expr
			{ e=e1.multiply(negOne);}
		)
	|	#(	SUBSTRING_OP	e1=expr	e2=expr	e3=expr
			{ e=e1.getDTDataType(e2,e3); }
		)	
	|	d=data_store	{ e=d;}
	|	c=con		{ e=c;}
	;

	
id	returns [DTDataType v]
	{ v=null; }
	:
	(
		s:STR_VAR	{v=theContext.ensureVariable(s.getText(),STR_VAR);}
	|	i:INT_VAR	{v=theContext.ensureVariable(i.getText(),INT_VAR);}
	|	f:FLT_VAR	{v=theContext.ensureVariable(f.getText(),FLT_VAR);}
	)
	;

con	returns [DTDataType c]
	{ c=null; }
	:
		s:STR_CONST	{c=new DTString	(theContext.getCurrentScope(),s.getText());}
	|	i:INT_CONST	{c=new DTInteger(theContext.getCurrentScope(),i.getText());}
	|	f:FLT_CONST	{c=new DTFloat	(theContext.getCurrentScope(),f.getText());}
	;

data_store returns [DTDataType tbd]
	{ DTDataType i1=null,i2=null,i3=null;tbd=null;}
	:
		#(INDEX_OP v:dimension_variable
				i1=expr
				(
					i2=expr
					(
						i3=expr
						{ tbd=theContext.getDTDataType(v.getText(),i1,i2,i3);}
					|
						{ tbd=theContext.getDTDataType(v.getText(),i1,i2);}
					)
				|
					{ tbd=theContext.getDTDataType(v.getText(),i1);}
				)
		)
		
	|	tbd=id
	;

// FOR NEXT BLOCK ---------------------------------------------------------------
forNextStatement
	{ DTDataType ff=null,ft=null,fb=null; }
	:
		#(FOR_LOOP ff=forFrom ft=forTo fb=forBy b:FOR_BODY
			{
				//DTDataType zero=new DTInteger(null,0);
				while( ff.compareTo(ft) != fb.compareTo(zero)  ) {
					try {
						forBody(b);
						ff.assign(ff.add(fb));
					}
					catch (ANTLRException ex) {
						//reportError(ex);
					}
				}
			
			
			}
		)
	;

forFrom	returns [DTDataType d]
	{ d=null; }
	:
		#(FOR_FROM	d=assign_statement)
	;
	

forTo	returns [DTDataType d]
	{ d=null; }
	:
		#(FOR_TO	d=expr)
	;
	
	
forBy	returns [DTDataType d]
	{ d=null; }
	:
		#(FOR_BY	d=expr)
	|	FOR_BY_ONE	{d=new DTInteger(theContext.getCurrentScope(),1);}
	;

// IF THEN BLOCK -------------------------------------------------------------------------
ifThenBlock
	{ int done=0;}
	:
		#( IF_THEN_BLOCK ( {done==0}? done=ifThenBody )+  )
	;

ifThenBody returns [int done]
	{ done=0;}
	:
		#(IF_BLOCK	done=conditional[1] )
	|	#(ELSE_IF_BLOCK	done=conditional[1] )
	|	#(ELSE_BLOCK	cb:CODE_BLOCK { codeBlock(cb);} )
	;

conditional[int forWhat] returns [int yes]
	{ yes=0; }
	:
		c:CONDITION	cb:CODE_BLOCK
		{
			yes= condition(c).getInteger();
			if(forWhat==yes){
				codeBlock(cb);
			}
		}
	;

doUntilLoop
	:
		#("until"  c:CONDITION
			{
				while(0==conditional(c,0)){};
			}
		)
			
	;

doLoopUntil
	:
		#("do"  cb:CODE_BLOCK c:CONDITION
			{
				do{
					codeBlock(cb);
				} while (0==condition(c).getInteger());
			}
		)
			
	;

when_error_call_statement
	:
		#(
			WHEN_ERROR_CALL "call" i:IDENT
			{System.out.println(" Attaching error:"+i.getText());}
		)
	;

dim_statement
	{ DTDataType i1=null,i2=null,i3=null; }
	:
		#("dim"	(
				#(
					ARRAY1D	dv1:dimension_variable i1=expr
					{ theContext.setDimension(dv1.getText(),i1);}
				)
			|	#(
					ARRAY2D	dv2:dimension_variable i1=expr i2=expr
					{ theContext.setDimension(dv2.getText(),i1,i2);}
				)
			|	#(
					ARRAY3D	dv3:dimension_variable i1=expr i2=expr i3=expr
					{ theContext.setDimension(dv3.getText(),i1,i2,i3);}
				)
			)
		)
	;

dimension_variable
	:
		STR_VAR
	|	FLT_VAR
	|	INT_VAR	
	;


// Numeric functions
doubleFunctions	returns	[DTDataType	tbd]
	{tbd=null;DTDataType n=null,i=null,s=null;}
	:
		#("abs"		n=expr
			{tbd=new	DTFloat(null,Math.abs(n.getFloat()));}
		)
	/*
	|	#("acos"	n=expr	
			{tbd=new	DTFloat(null,Math.acos(n.getFloat()));}	
		)	
	|	#("asc"		s=expr	
			{tbd=new	DTInteger(null,Math.asc(n.getFloat()));}	
		)	
	|	#("atn"		n=expr
			{tbd=new	DTFloat(null,Math.atn(n.getFloat()));}
		)	
	|	#("cos"		n=expr
			{tbd=new	DTFloat(null,Math.cos(n.getFloat()));}
		)	
	|	#("dround"	n=expr	i=expr	
			{tbd=new	DTFloat(null,Math.dround(n.getFloat()));}	
		)	
	|	#("errl"	
			{tbd=new	DTFloat(null,Math.errl(n.getFloat()));}
		)
	|	#("errn"	
			{tbd=new	DTInteger(null,Math.errn(n.getFloat()));}
		)
	|	#("exp"	n=expr
			{tbd=new	DTFloat(null,Math.exp(n.getFloat()));}
		)	
	|	#("fract"	n=expr
			{tbd=new	DTFloat(null,Math.fract(n.getFloat()));}
		)
	|	#("get_event"	n=expr
			{tbd=new	DTFloat(null,Math.get_event(n.getFloat()));}
		)
	|	#("in"	n=expr
			{tbd=new	DTFloat(null,Math.expr(n.getFloat()));}
		)
	|	#("instr"	s=expr	s=expr
			{tbd=new	DTFloat(null,Math.abs(n.getFloat()));}
		)
	|	#("int"	n=expr
			{tbd=new	DTFloat(null,Math.int(n.getFloat()));}
		)
	|	#("ival"	s=expr	
			{tbd=new	DTFloat(null,Math.ival(n.getFloat()));}
		)
	|	#("len"	s=expr
			{tbd=new	DTFloat(null,Math.len(n.getString()));}
		)
	|	#("lgt"	n=expr
			{tbd=new	DTFloat(null,Math.abs(n.getFloat()));}
		)
	|	#("log"	n=expr
			{tbd=new	DTFloat(null,Math.abs(n.getFloat()));}
		)
	|	#("max"	(
				n=expr
				{
					if(tbd==null){
						tbd=n;
					} else if((n.compare(tbd)>0){
						tbd=n;
					}
				}
			)+
		)	
	|	#("min"
			(
				n=expr
				{
					if(tbd==null){
						tbd=n;
					} else if((n.compare(tbd)<0){
						tbd=n;
					}
				}
			)+
		)	
	|	#("peek"	n=expr	i=expr
			{tbd=new	DTInteger(null,Math.peek(n.getFloat()));}
		)
	|	#("pi"	
			{tbd=new	DTFloat(null,Math.pi(n.getFloat()));}
		)
	|	#("rnd"	
			{tbd=new	DTInteger(null,Math.rnd(n.getFloat()));}
		)
	|	#("sgn"	n=expr
			{tbd=new	DTInteger(null,n.compare(zero)));}
		)
	|	#("signed"	i=expr
			{tbd=new	DTFloat(null,Math.abs(n.getFloat()));}
		)
	|	#("sin"	n=expr
			{tbd=new	DTFloat(null,Math.sin(n.getFloat()));}
		)
	|	#("sqr"	n=expr
			{tbd=new	DTFloat(null,Math.sqr(n.getFloat()));}
		)
	|	#("tan"	n=expr	
			{tbd=new	DTFloat(null,Math.tan(n.getFloat()));}
		)
	|	#("time"	
			{tbd=new	DTFloat(null,Math.time(n.getFloat()));}
		)
	|	#("ubound"	s=expr	i=expr
			{tbd=new	DTInteger(null,Math.ubound(n.getFloat()));}	
		)
	|	#("val"	s=expr
			{tbd=new	DTFloat(null,Math.val(n.getFloat()));}
		)
	//	BIT	Functions
	|	#("andb"	i=expr	i=expr
			{tbd=new	DTInteger(null,Math.andb(n.getFloat()));}
		)
	|	#("orb"	i=expr	i=expr
			{tbd=new	DTInteger(null,Math.orb(n.getInteger()));}
		)
	|	#("notb"	i=expr	
			{tbd=new	DTInteger(null,Math.abs(n.getInteger()));}
		)
	|	#("shiftb"	i=expr	i=expr	
			{tbd=new	DTInteger(null,Math.abs(i.getInteger()));}
		)
	|	#("xorb"	i=expr	i=expr	
			{tbd=new	DTInteger(null,Math.abs(i.getInteger()));}
		)
		*/
	;


//-------------Orphan helpers			
// Helper Orphan
condition returns [DTDataType e]
	{ e=null;}
	:
		#(CONDITION	e=expr)
	;

// Helper Orphan
forBody
	:
		#(FOR_BODY	codeBlock)
	;
	
// Helper Orphan
codeBlock
	:	#(CODE_BLOCK	(statement)*)
	;
