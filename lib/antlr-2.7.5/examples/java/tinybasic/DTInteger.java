package tinybasic;

public class DTInteger extends DTDataType {

    protected int i;
	
    public DTInteger(Scope scope,DTDataType tbd){
		super(scope,INT_VAR);
		setInteger(tbd);
	}
    public DTInteger(Scope scope,int i){
		super(scope,INT_VAR);
		this.i=i;
	}
    
    public int getInteger(){
	    return i;
    }
    
    public DTInteger(Scope scope,String s){
		super(scope,INT_VAR);
		this.i=Integer.parseInt(s);
	}
    

    public void setInteger(DTDataType tbd){
	setInteger(tbd.getInteger());
    }
    
    public void setFloat(DTDataType tbd){
	setFloat(tbd.getFloat());
    }
    
    public void setInteger(int i){
	this.i=i;
    }

    public double getFloat(){
	    return i;
    }
    public void setFloat(double d){
	i=(int)d;
    }
    
    public void assign(DTDataType tbd){
	setInteger(tbd);
    }
//
    public DTDataType multiply(DTDataType other){
	    if(other instanceof DTFloat){
		DTFloat t=new DTFloat(null,this);
		return t.multiply(other);
	    }
	    return new DTInteger(null,getInteger()*other.getInteger());
	}
    public DTDataType divide(DTDataType other){
	    if(other instanceof DTFloat){
		DTFloat t=new DTFloat(null,this);
		return t.divide(other);
	    }
	    return new DTInteger(null,getInteger()/other.getInteger());
	}
    public DTDataType add(DTDataType other){ 
	    if(other instanceof DTFloat){
		DTFloat t=new DTFloat(null,this);
		return t.add(other);
	    }
	    return new DTInteger(null,getInteger()+other.getInteger());
	}
    public DTDataType subtract(DTDataType other){
	    if(other instanceof DTFloat){
		DTFloat t=new DTFloat(null,this);
		return t.subtract(other);
	    }
	    return new DTInteger(null,getInteger()-other.getInteger());
	}
    public DTDataType mod(DTDataType other){
	    if(other instanceof DTFloat){
		DTFloat t=new DTFloat(null,this);
		return t.mod(other);
	    }
	    return new DTInteger(null,getInteger() % other.getInteger());
	}
    public DTDataType round(){
	    return this;
	}
    public DTDataType truncate(){
	    return this;
	}

    public int compareTo(Object o){
	int d=0;
	if(getInteger() < ((DTDataType)o).getInteger()){
	    return -1;
	} else if ( getInteger() > ((DTDataType)o).getInteger()){
	    return 1;
	}
	return 0;
    }
    public String toString(){
	return new Integer(getInteger()).toString();
    }
    

}
