package tinybasic;
import antlr.collections.AST;

public  class DTDataTypeProxy  extends DTDataType {

    protected DTDataType theBoss;
    protected int dims;

    public DTDataTypeProxy(int theType,Scope scope,int dims){
	super(scope,theType);
	this.dims=dims;
    }

    public int getType(){   return theBoss.getType();}

    public int	    getInteger	()	{ return theBoss.getInteger(); }
    public double   getFloat	()	{ return theBoss.getFloat();} 
    public String   getString	()	{ return theBoss.getString(); }

    public void setInteger  (DTDataType tbd){theBoss.setInteger	(tbd);}
    public void setFloat    (DTDataType tbd){theBoss.setFloat	(tbd);}
    public void setString   (DTDataType tbd){theBoss.setString	(tbd);}

    public void setInteger  (int    i){theBoss.setInteger(i);}
    public void setFloat    (double d){theBoss.setFloat(d);}
    public void setString   (String s){theBoss.setString(s);}
        
    public DTDataType getDTDataType(DTDataType i1)
	    {return theBoss.getDTDataType(i1);}
    public DTDataType getDTDataType(DTDataType i1,DTDataType i2)
	    {return theBoss.getDTDataType(i1,i2);}
    public DTDataType getDTDataType(DTDataType i1,DTDataType i2,DTDataType i3)
	    {return theBoss.getDTDataType(i1,i2,i3);}


    public void setDTDataType(DTDataType i1,DTDataType s)
	    {theBoss.setDTDataType(i1,s);}
    public void setDTDataType(DTDataType i1,DTDataType i2,DTDataType s)
	    {theBoss.setDTDataType(i1,i2,s);}
    public void setDTDataType(DTDataType i1,DTDataType i2,DTDataType i3,DTDataType s)
	    {theBoss.setDTDataType(i1,i2,i3,s);}

    public void assign(DTDataType tbd){theBoss.assign(tbd);}

    public int getDimension()			{return dims /*theBoss.getDimension()*/ ;}
    
    public int getDimensioned(int i)		{return theBoss.getDimensioned(i);}

    public DTDataType multiply	(DTDataType other) { return theBoss.multiply	(other); }
    public DTDataType divide	(DTDataType other) { return theBoss.divide	(other); }
    public DTDataType add	(DTDataType other) { return theBoss.add		(other); }
    public DTDataType subtract	(DTDataType other) { return theBoss.subtract	(other); }
    public DTDataType mod	(DTDataType other) { return theBoss.mod		(other); }
    public DTDataType round	(DTDataType other) { return theBoss.round	(other); }
    public DTDataType truncate	(DTDataType other) { return theBoss.truncate	(other); }
    
    protected DTDataType getOne(){
	return theBoss.getOne();
    }
    
    public int compareTo(Object o){
	return theBoss.compareTo(o);
    }

    public void attach(DTDataType theBoss){
	this.theBoss=theBoss;
    }
    
    public DTDataType cloneDTDataType()
	{
	    return new DTDataTypeProxy(theType,scope,dims);
	}

    public String toString(){
	return theBoss.toString();
    }
    

}
