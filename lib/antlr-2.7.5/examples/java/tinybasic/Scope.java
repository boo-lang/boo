package tinybasic;

import java.util.*;
public class Scope {

	protected Scope prev;
	protected Scope global;
	
	Hashtable symbolTable;

	protected Scope(Scope prev){
		this.prev=prev;
		symbolTable = new Hashtable();
	}
	
	public Scope cloneScope(Scope prev){
		Scope newScope = new Scope(prev);
		return newScope;
	}
	
	void insertVariable(String v,DTDataType t){
	    symbolTable.put(v.toLowerCase(),t);
	}
	
	public DTDataType  getVariable(String v){
		DTDataType t=(DTDataType)symbolTable.get(v.toLowerCase());
		return t;
	}
	
	public int getVariableDimension(String v){
	    DTDataType t=getVariable(v);
	    
	    if(t!=null){
		return t.getDimension();
	    } else {
		return 0;
	    }
	}
	
	public int getVariableType(String v){
	    DTDataType t=getVariable(v);
	    
	    if(t!=null){
		return t.getType();
	    } else {
		return 0;
	    }
	}
	
	public boolean isArrayVariable(String s){
	    return (getVariableDimension(s) > 0);
	}
	
	public Scope getPrev(){
		return prev;
	}
		
	
}
	
