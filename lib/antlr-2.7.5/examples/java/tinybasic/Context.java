package tinybasic;

import java.util.*;

public class Context {

 protected Scope theGlobalScope=null;
 protected Scope theProgramScope=null;
 protected Stack theScopeStack;

    	Hashtable subroutineTable;
    	Hashtable functionTable;
 
 	public Context(){
 		theGlobalScope=new GlobalScope(null);
 		theScopeStack=new Stack();
		theScopeStack.push(theGlobalScope);
		subroutineTable = new Hashtable();
		functionTable	= new Hashtable();
 	}
    
	void insertSubroutine(String v,DTCodeType t){
	    subroutineTable.put(v.toLowerCase(),t);
	}

	public DTCodeType  getSubroutine(String v){
		DTCodeType t=(DTCodeType)subroutineTable.get(v.toLowerCase());
		return t;
	}

	void insertFunction(String v,DTCodeType t){
	    functionTable.put(v.toLowerCase(),t);
	}

	public DTCodeType  getFunction(String v){
		DTCodeType t=(DTCodeType)functionTable.get(v.toLowerCase());
		return t;
	}


 	void insertGlobalVariable(String v,DTDataType t){
	    theGlobalScope.insertVariable(v,t);
	}
 	
 	void insertVariable(String v,DTDataType t){
	   getCurrentScope().insertVariable(v,t);
	}
	
	public DTDataType  getVariable(String var){
		DTDataType t=getCurrentScope().getVariable(var);
		if(t==null){
			t=theGlobalScope.getVariable(var);
		}
		return t;
	}
	
	public int getVariableDimension(String var){
		int dim=0;
		dim=getCurrentScope().getVariableDimension(var);
		if(dim==0){
			dim=theGlobalScope.getVariableDimension(var);
		}
		return dim;
	}
	
	public int getVariableType(String var){
		DTDataType t=null;
		t=getCurrentScope().getVariable(var);
		if(t==null){
			t=theGlobalScope.getVariable(var);
		}
	    
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
		return getCurrentScope().getPrev();
	}
	
	public void pushScope(Scope scope){
		theScopeStack.push(scope);
	}
	
	public Scope popScope(){
		if( getCurrentScope() == theGlobalScope ){
			return theGlobalScope;
		} else {
			return (Scope)theScopeStack.pop();
		}
	}
	
	public Scope getCurrentScope(){
		return ((Scope)theScopeStack.peek());
	}
	
	public Scope getGlobalScope(){
		return theGlobalScope;
	}
	
	public void setProgramScope(Scope scope){
	    theProgramScope=scope;
	    while((Scope)theScopeStack.peek()!=theGlobalScope){
		theScopeStack.pop();
	    }
	    theScopeStack.push(theProgramScope);
	}

	protected Scope getProgramScope(){
	    return theProgramScope;
	}

	public void setDimension(String s,DTDataType i1){
	    DTDataType v=getVariable(s);
	    v.setDimension(i1.getInteger());
	}
	
	public void initialize(){
	    setProgramScope(getProgramScope());
	}
	public void setDimension(String s,DTDataType i1,DTDataType i2){
	    DTDataType v=getVariable(s);
	    v.setDimension(i1.getInteger(),i2.getInteger());
	}
	public void setDimension(String s,DTDataType i1,DTDataType i2,DTDataType i3){
	    DTDataType v=getVariable(s);
	    v.setDimension(i1.getInteger(),i2.getInteger(),i3.getInteger());
	}


	public DTDataType getDTDataType(String s,DTDataType i1){
	    DTDataType t=getVariable(s);
	    return t.getDTDataType(i1);
	}

	public DTDataType getDTDataType(String s,DTDataType i1,DTDataType i2){
	    DTDataType t=getVariable(s);
	    return t.getDTDataType(i1,i2);
	}

	public DTDataType getDTDataType(String s,DTDataType i1,DTDataType i2,DTDataType i3){
	    DTDataType t=getVariable(s);
	    return t.getDTDataType(i1,i2,i3);
	}
	
	public DTDataType ensureVariable(String s,int t){
	    DTDataType v = getVariable(s);
	    if(v==null){
		v=DTDataType.getOne(t,getCurrentScope());
		insertVariable(s,v);
	    }
	    return v;
	}
	    
		

	//public boolean isArrayVariable(String s){
	//    if(getCurrentScope().isArrayVariable(s){
	//	return true;
	//    } else {
	//	return theGlobalScope.isArrayVariable(s);
	//}
	//}

}
