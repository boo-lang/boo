package tinybasic;

public class DTString extends DTDataType{
    protected String s=null;

    public DTString(Scope scope,DTDataType tbd){
	super(scope,STR_VAR);
	this.s=tbd.getString();
    }

    public DTString(Scope scope,String s){
	super(scope,STR_VAR);
	this.s=s;
    }

    public void setString(String s){
	this.s=s;
    }
    public String getString(){
	return s;
    }
    
    public int compareTo(Object o){
	return s.compareTo(((DTDataType)o).getString());
    }
    
    public void assign(DTDataType tbd){
	setString(tbd);
    }
    
    public String toString(){
	return s.toString();
    }


}
