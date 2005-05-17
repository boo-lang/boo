header{
    import basic
}
options {
    language=Python;
}

{
    def println(*args):
        if not args:
            print ""
            return
        // make empty line here to test for E0009
        for x in args[0:-1]: print x,
        print args[-1]

    def printx(*args):
        if not args:
            return
        for x in args:
            print x,
}

class basic_w extends TreeParser;

options {
    importVocab = TinyBasic;
}

{
}

compilationUnit[context] returns [self.theContext = context]
{
    self.theContext.initialize();
    self.zero   = basic.DTInteger(self.theContext.getCurrentScope(),0);
    self.posOne = basic.DTInteger(self.theContext.getCurrentScope(),1);
    self.negOne = basic.DTInteger(self.theContext.getCurrentScope(),-1);
}
    :   
        pd:PROGRAM_DEF
        {
            try:
                self.programDefinition(pd)
            except basic.DTExecException, didit:
                print "Yes it works!", didit
        }
    ;

programDefinition
    :   #( PROGRAM_DEF moduleBody )
    ;


subroutineDefinition
    :   #( SUBROUTINE_DEF IDENT moduleBody )
    ;

moduleBody
    :   #(PARAMETERS parameters )
#(CODE_BLOCK ( statement )+ )
        EXIT_MODULE
        {
            raise basic.DTExitModuleException("Done folks")
        }
    ;


parameters
    :
        (parameter)*
    ;


parameter
{ argNum=0;}
    :
#(VAR_PROXY arg[argNum]{ argNum += 1} )
    ;

arg[r]
    :
        (
            s:STR_VAR   //{v=theContext.ensureVariable(s.getText(),STR_VAR);}
        |   i:INT_VAR   //{v=theContext.ensureVariable(i.getText(),INT_VAR);}
        |   f:FLT_VAR   //{v=theContext.ensureVariable(f.getText(),FLT_VAR);}
        )
    ;


statement
{ expr = None }
    :
        when_error_call_statement
    |   dim_statement           // done
    |   forNextStatement        // done
    |   printAsciiStatement     // done
    |   ifThenBlock             // done
    |   expr=assign_statement        // done
    |   doUntilLoop             // done
    |   doLoopUntil             // done
    |   subExecuteStatement     // done
    |   exitModuleStatement     // done
    ;

exitModuleStatement
    :
        EXIT_MODULE
        {
            raise basic.DTExitModuleException("Asynchronous return")
        }
    ;
subExecuteStatement
{ sub=None; argNum=0;tbd=None; }
    :
#(SUB_EXECUTE   i:IDENT
            {
                sub=self.theContext.getSubroutine(i.getText())
            }
#(ARGLIST
                (
                    tbd=argExpr
                    { 
                        argNum +=1 
                        sub.attachArg(argNum,tbd)
                    }
                )*
                
            )
            {
                try:
                  sub.newCall(self.theContext);
                  try:
                    self.subroutineDefinition(sub.getAST())
                  except basic.DTExitModuleException, didit:
                    print "Yes it works!", didit
                except antlr.ANTLRException, ex:
                  pass
            }
        )
    ;


argExpr returns [exprValue]
    :
        a1d:ARRAY1D { exprValue=self.theContext.getVariable(a1d.getText()); }
    |   a2d:ARRAY2D { exprValue=self.theContext.getVariable(a2d.getText()); }
    |   a3d:ARRAY3D { exprValue=self.theContext.getVariable(a3d.getText()); }
    |   exprValue=expr
    ;   

printAsciiStatement
    :
#(PRINT_ASCII (printField)* { print } )
    ;

printField
{d=None}
    :
#(PRINT_NUMERIC d=expr  { printx(d)})
    |   #(PRINT_STRING  d=expr  { printx(d)})
    |   #(PRINT_TAB d=expr)
    |   PRINT_COMMA { printx("\t")}
    |   PRINT_SEMI
    ;

assign_statement returns [exprValue]
{ e=None }
    :   #(
            EQ exprValue=data_store e=expr
            {
                exprValue.assign(e);
            }
        )
    ;

expr    returns [exprValue]
{ c=None;d=None;e1=None;e2=None;e3=None }
    :
#(
            STAR e1=expr e2=expr
            { exprValue=e1.multiply(e2) }
        )
    |   #(
            PLUS e1=expr e2=expr
            { exprValue=e1.add(e2) }
        )
    |   #(
            SLASH e1=expr e2=expr
            { exprValue=e1.multiply(e2) }
        )
    |   #(
            "div" e1=expr e2=expr
            { exprValue=e1.divide(e2) }
        )
    |   #(
            "mod" e1=expr e2=expr
            { exprValue=e1.mod(e2) }
        )
        // comparison operators
    |   #(
            EQ_COMP e1=expr e2=expr
            { exprValue=basic.DTInteger(None, antlr.ifelse(e1.compareTo(e2)==0,1,0)) }
        )
    |   #(
            NE_COMP e1=expr e2=expr
            { exprValue=basic.DTInteger(None, antlr.ifelse(e1.compareTo(e2)==0,0,1)) }
        )
    |   #(
            LE e1=expr e2=expr
            { exprValue=basic.DTInteger(None, antlr.ifelse(e1.compareTo(e2)<=0,1,0)) }
        )
    |   #(
            LT e1=expr e2=expr
            { exprValue=basic.DTInteger(None, antlr.ifelse(e1.compareTo(e2) <0,1,0)) }
        )
    |   #(
            GE e1=expr e2=expr
            { exprValue=basic.DTInteger(None, antlr.ifelse(e1.compareTo(e2)>=0,1,0)) }
        )
    |   #(
            GT e1=expr e2=expr
            { exprValue=basic.DTInteger(None, antlr.ifelse(e1.compareTo(e2) >0,1,0)) }
        )
        // Boolean algebra
    |   #(
            "xor" e1=expr e2=expr
            { exprValue=basic.DTInteger(None,antlr.ifelse(e1.getInteger()!=e2.getInteger(),1,0)) }
        )
    |   #(
            "and" e1=expr e2=expr
            { exprValue=basic.DTInteger(None,antlr.ifelse(e1.getInteger()==1 and e2.getInteger()==1,1,0)) }
        )
    |   #(
            "or" e1=expr e2=expr
            { exprValue=basic.DTInteger(None,antlr.ifelse(e1.getInteger()==1 or e2.getInteger()==1,1,0)) }
        )
        // unary operators
    |   #(
            "not" e1=expr
            { exprValue=basic.DTInteger(None,antlr.ifelse(e1.getInteger()==0,1,0)) }
        )
    |   #(
            UNARY_PLUS e1=expr
            { exprValue=e1 }
        )
    |   #(
            UNARY_MINUS e1=expr
            { exprValue=e1.multiply(self.negOne) }
        )
    |   #(  SUBSTRING_OP    e1=expr e2=expr e3=expr
            { exprValue=e1.getDTDataType(e2,e3) }
        )   
    |   d=data_store    { exprValue=d }
    |   c=con       { exprValue=c }
    ;


id  returns [value]
    :
        (
            s:STR_VAR   {value=self.theContext.ensureVariable(s.getText(),STR_VAR)}
        |   i:INT_VAR   {value=self.theContext.ensureVariable(i.getText(),INT_VAR)}
        |   f:FLT_VAR   {value=self.theContext.ensureVariable(f.getText(),FLT_VAR)}
        )
    ;

con returns [value]
    :
        s:STR_CONST {value=basic.DTString   (self.theContext.getCurrentScope(),s.getText())}
    |   i:INT_CONST {value=basic.DTInteger  (self.theContext.getCurrentScope(),i.getText())}
    |   f:FLT_CONST {value=basic.DTFloat    (self.theContext.getCurrentScope(),f.getText())}
    ;

data_store returns [value]
{ i1=None;i2=None;i3=None;tbd=None;}
    : #(INDEX_OP v:dimension_variable i1=expr
            (
                i2=expr
                (
                    i3=expr
                    {
                        value=self.theContext.getDTDataType(
                            v.getText(),i1,i2,i3)
                    }
                |
                    { 
                        value=self.theContext.getDTDataType(
                            v.getText(),i1,i2)
                    }
                )
            |
                { 
                    value=self.theContext.getDTDataType(
                        v.getText(),i1)
                }
            )
        )
        
    |   value=id
    ;

// FOR NEXT BLOCK ---------------------------------------
forNextStatement
{ ff=None;ft=None;fb=None; }
    :
#(FOR_LOOP ff=forFrom ft=forTo fb=forBy b:FOR_BODY
            {
                while ff.compareTo(ft) != fb.compareTo(self.zero):
                  try:
                    self.forBody(b)
                    ff.assign(ff.add(fb))
                  except antlr.ANTLRException,ex:
                    pass
            }
        )
    ;

forFrom returns [forValue]
    :
#(FOR_FROM  forValue=assign_statement)
    ;


forTo   returns [forValue]
    :
#(FOR_TO    forValue=expr)
    ;


forBy   returns [forValue]
    :
#(FOR_BY    forValue=expr)
    |   FOR_BY_ONE  {forValue=basic.DTInteger(self.theContext.getCurrentScope(),1)}
    ;

// IF THEN BLOCK ---------------------------------------------
ifThenBlock
{ done=0 }
    : #( IF_THEN_BLOCK ( {done==0}? done=ifThenBody )+  )
    ;

ifThenBody returns [ifValue]
{ r=0 }
    : #(IF_BLOCK      ifValue=conditional[1] )
    | #(ELSE_IF_BLOCK ifValue=conditional[1] )
    | #(ELSE_BLOCK    cb:CODE_BLOCK { 
                self.codeBlock(cb)
            } 
        )
    ;


conditional[forWhat] returns [condValue]
    : c:CONDITION cb:CODE_BLOCK {
            condValue = self.condition(c).getInteger()
            if forWhat==condValue:
                self.codeBlock(cb)
        }
    ;

doUntilLoop
    :
#("until"  c:CONDITION
            {
                while 0 == self.conditional(c,0):
                   pass
            }
        )
        
    ;

doLoopUntil
    : #("do"  cb:CODE_BLOCK c:CONDITION {
                self.codeBlock(cb); 
                while 0 == self.condition(c).getInteger():
                   self.codeBlock(cb)
            }
        )
        
    ;

when_error_call_statement
    :
#(
            WHEN_ERROR_CALL "call" i:IDENT
            {println(" Attaching error:",i.getText())}
        )
    ;

dim_statement
{ i1=None;i2=None;i3=None; }
    :
#("dim" (
#(
                    ARRAY1D dv1:dimension_variable i1=expr
                    { self.theContext.setDimension(dv1.getText(),i1)}
                )
            |   #(
                    ARRAY2D dv2:dimension_variable i1=expr i2=expr
                    { self.theContext.setDimension(dv2.getText(),i1,i2)}
                )
            |   #(
                    ARRAY3D dv3:dimension_variable i1=expr i2=expr i3=expr
                    { self.theContext.setDimension(dv3.getText(),i1,i2,i3)}
                )
            )
        )
    ;

dimension_variable
    :
        STR_VAR
    |   FLT_VAR
    |   INT_VAR 
    ;


// Numeric functions
doubleFunctions returns [funcValue]
{n=None;i=None;s=None;}
    :
#("abs"     n=expr
            {funcValue=basic.DTFloat(None,Math.abs(n.getFloat()))}
        )
        /*
    |   #("acos"    n=expr  
            {tbd=new    DTFloat(None,Math.acos(n.getFloat()));} 
        )   
    |   #("asc"     s=expr  
            {tbd=new    DTInteger(None,Math.asc(n.getFloat()));}    
        )   
    |   #("atn"     n=expr
            {tbd=new    DTFloat(None,Math.atn(n.getFloat()));}
        )   
    |   #("cos"     n=expr
            {tbd=new    DTFloat(None,Math.cos(n.getFloat()));}
        )   
    |   #("dround"  n=expr  i=expr  
            {tbd=new    DTFloat(None,Math.dround(n.getFloat()));}   
        )   
    |   #("errl"    
            {tbd=new    DTFloat(None,Math.errl(n.getFloat()));}
        )
    |   #("errn"    
            {tbd=new    DTInteger(None,Math.errn(n.getFloat()));}
        )
    |   #("exp" n=expr
            {tbd=new    DTFloat(None,Math.exp(n.getFloat()));}
        )   
    |   #("fract"   n=expr
            {tbd=new    DTFloat(None,Math.fract(n.getFloat()));}
        )
    |   #("get_event"   n=expr
            {tbd=new    DTFloat(None,Math.get_event(n.getFloat()));}
        )
    |   #("in"  n=expr
            {tbd=new    DTFloat(None,Math.expr(n.getFloat()));}
        )
    |   #("instr"   s=expr  s=expr
            {tbd=new    DTFloat(None,Math.abs(n.getFloat()));}
        )
    |   #("int" n=expr
            {tbd=new    DTFloat(None,Math.int(n.getFloat()));}
        )
    |   #("ival"    s=expr  
            {tbd=new    DTFloat(None,Math.ival(n.getFloat()));}
        )
    |   #("len" s=expr
            {tbd=new    DTFloat(None,Math.len(n.getString()));}
        )
    |   #("lgt" n=expr
            {tbd=new    DTFloat(None,Math.abs(n.getFloat()));}
        )
    |   #("log" n=expr
            {tbd=new    DTFloat(None,Math.abs(n.getFloat()));}
        )
    |   #("max" (
                n=expr
                {
                    if tbd:
                        tbd=n
                    elif((n.compare(tbd)>0):
                        tbd=n
                }
            )+
        )   
    |   #("min"
            (
                n=expr
                {
                    if tbd:
                        tbd=n
                    elif((n.compare(tbd)<0):
                        tbd=n
                }
            )+
        )   
    |   #("peek"    n=expr  i=expr
            {tbd=new    DTInteger(None,Math.peek(n.getFloat()));}
        )
    |   #("pi"  
            {tbd=new    DTFloat(None,Math.pi(n.getFloat()));}
        )
    |   #("rnd" 
            {tbd=new    DTInteger(None,Math.rnd(n.getFloat()));}
        )
    |   #("sgn" n=expr
            {tbd=new    DTInteger(None,n.compare(zero)));}
        )
    |   #("signed"  i=expr
            {tbd=new    DTFloat(None,Math.abs(n.getFloat()));}
        )
    |   #("sin" n=expr
            {tbd=new    DTFloat(None,Math.sin(n.getFloat()));}
        )
    |   #("sqr" n=expr
            {tbd=new    DTFloat(None,Math.sqr(n.getFloat()));}
        )
    |   #("tan" n=expr  
            {tbd=new    DTFloat(None,Math.tan(n.getFloat()));}
        )
    |   #("time"    
            {tbd=new    DTFloat(None,Math.time(n.getFloat()));}
        )
    |   #("ubound"  s=expr  i=expr
            {tbd=new    DTInteger(None,Math.ubound(n.getFloat()));} 
        )
    |   #("val" s=expr
            {tbd=new    DTFloat(None,Math.val(n.getFloat()));}
        )
    //  BIT Functions
    |   #("andb"    i=expr  i=expr
            {tbd=new    DTInteger(None,Math.andb(n.getFloat()));}
        )
    |   #("orb" i=expr  i=expr
            {tbd=new    DTInteger(None,Math.orb(n.getInteger()));}
        )
    |   #("notb"    i=expr  
            {tbd=new    DTInteger(None,Math.abs(n.getInteger()));}
        )
    |   #("shiftb"  i=expr  i=expr  
            {tbd=new    DTInteger(None,Math.abs(i.getInteger()));}
        )
    |   #("xorb"    i=expr  i=expr  
            {tbd=new    DTInteger(None,Math.abs(i.getInteger()));}
        )
        */
    ;


//-------------Orphan helpers           
// Helper Orphan
condition returns [condValue]
    :
    #(CONDITION condValue=expr)
    ;

// Helper Orphan
forBody
    :
    #(FOR_BODY  codeBlock)
    ;

// Helper Orphan
codeBlock
    :   #(CODE_BLOCK    (statement)*)
    ;
