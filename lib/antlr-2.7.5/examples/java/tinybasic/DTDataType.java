package tinybasic;
import antlr.collections.AST;

public abstract class DTDataType  {

    protected Scope scope;
    protected int theType;
    protected static int INT_VAR	=   TinyBasicTokenTypes.INT_VAR;
    protected static int FLT_VAR	=   TinyBasicTokenTypes.FLT_VAR;
    protected static int STR_VAR	=   TinyBasicTokenTypes.STR_VAR;
    protected static int INT_CONST	=   TinyBasicTokenTypes.INT_CONST;
    protected static int FLT_CONST	=   TinyBasicTokenTypes.FLT_CONST;
    protected static int STR_CONST	=   TinyBasicTokenTypes.STR_CONST;

    public DTDataType(Scope scope,int _ttype){
	this.scope=scope;
	theType=_ttype;
    }

    public int getType(){
	return theType;
    }

    public int	    getInteger	()	{ return 12345; }
    public double   getFloat	()	{ return 12345.0;} 
    public String   getString	()	{ return null; }

    public void setInteger  (DTDataType tbd){setInteger	(tbd.getInteger	());}
    public void setFloat    (DTDataType tbd){setFloat	(tbd.getFloat	());}
    public void setString   (DTDataType tbd){setString	(tbd.getString	());}

    public void setInteger  (int    i){}
    public void setFloat    (double d){}
    public void setString   (String s){}
        
    public DTDataType getDTDataType(DTDataType i1){return null;}
    public DTDataType getDTDataType(DTDataType i1,DTDataType i2){return null;}
    public DTDataType getDTDataType(DTDataType i1,DTDataType i2,DTDataType i3){return null;}


    public void setDTDataType(DTDataType i1,DTDataType s){}
    public void setDTDataType(DTDataType i1,DTDataType i2,DTDataType s){}
    public void setDTDataType(DTDataType i1,DTDataType i2,DTDataType i3,DTDataType s){}

    public void assign(DTDataType tbd){}

    public int getDimension(){return 0;}
    
    public int getDimensioned(int i){return 0;}

    public DTDataType multiply(DTDataType other) { return null;}
    public DTDataType divide(DTDataType other) { return null;}
    public DTDataType add(DTDataType other) { return null;}
    public DTDataType subtract(DTDataType other) { return null;}
    public DTDataType mod(DTDataType other) { return null;}
    public DTDataType round(DTDataType other) { return null;}
    public DTDataType truncate(DTDataType other) { return null;}
    
    protected DTDataType getOne(){
	return getOne(theType,scope);
    }

    public static DTDataType getOne(int aType,Scope scope){
	if	    ( aType==INT_CONST    ){
		    return new DTInteger(scope,0);
	} else if   ( aType==INT_VAR	    ){
		    return new DTInteger(scope,0);
	} else if   ( aType==FLT_CONST    ){
		    return new DTFloat	(scope,0.0);
	} else if   ( aType==FLT_VAR	    ){
		    return new DTFloat	(scope,0.0);
	} else if   ( aType==STR_CONST    ){
		    return new DTString	(scope,"");
	} else if   ( aType==STR_VAR	    ){
		    return new DTString	(scope,"");
	}
	return null;
    
    
    }
    
    protected DTDataType getOne(DTDataType s){
	DTDataType t=getOne();
	t.assign(s);
	return t;
    }

    public DTDataType cloneDTDataType()
	{
	    return getOne(this);
	}

    public void setDimension(int i1){
    }
    public void setDimension(int i1,int i2){
    }
    public void setDimension(int i1,int i2,int i3){
    }
    
    public abstract int compareTo(Object o);

    public void attach(DTDataType theBoss){}

}
