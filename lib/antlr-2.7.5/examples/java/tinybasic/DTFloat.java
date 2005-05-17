package tinybasic;

//import javax.swing.*;

public class DTFloat extends DTDataType {
    protected double d;
    public DTFloat(Scope scope,DTDataType tbd){
	super(scope,FLT_VAR);
	setFloat(tbd);
    }
    
    public DTFloat(Scope scope,double d){
	super(scope,FLT_VAR);
	this.d=d;
    }
    
    public DTFloat(Scope scope,String s){
		super(scope,INT_VAR);
		if(false){  // 1.2
		    // note that parseDouble is not in 1.1 so we fake it.
		    // d = Double.parseDouble(s);
		} else {  //1.1
		    Double t=new Double(s);
		    d=t.doubleValue();
		}
	}
    
    public void setInteger(DTDataType tbd){
	setInteger(tbd.getInteger());
    }
    
    public void setFloat(DTDataType tbd){
	setFloat(tbd.getFloat());
    }
    
 
    public void setFloat(double d){
	this.d=d;
    }
    
    public double getFloat(){
	return d;
    }

    public void setInteger(int i){
	d=i;
    }
    public void assign(DTDataType tbd){
	setFloat(tbd);
    }
//
    public DTDataType multiply(DTDataType other){
	    return new DTFloat(null,getFloat()*other.getFloat());
	}
    public DTDataType divide(DTDataType other){
	    return new DTFloat(null,getFloat()/other.getFloat());
	}
    public DTDataType add(DTDataType other){ 
	    return new DTFloat(null,getFloat()+other.getFloat());
	}
    public DTDataType subtract(DTDataType other){
	    return new DTFloat(null,getFloat()-other.getFloat());
	}
    public DTDataType mod(DTDataType other){
	    return new DTFloat(null,getFloat() % other.getFloat());
	}
    public DTDataType round(){
	    return new DTInteger(null,new DTFloat(null,getFloat()+0.5));
	}
    public DTDataType truncate(){
	    return new DTInteger(null,getInteger());
	}

    public int compareTo(Object o){
	int d=0;
	if(getFloat() < ((DTDataType)o).getFloat()){
	    return -1;
	} else if ( getFloat() > ((DTDataType)o).getFloat()){
	    return 1;
	}
	return 0;
    }
    
    public String toString(){
	return new Double(getFloat()).toString();
    }

}
