package tinybasic;
import java.lang.*;
import antlr.collections.AST;

public class DTArray1D extends DTDataType{

private Object data[];

protected int dim1;
protected int base=0;

public DTArray1D(int _ttype,Scope scope)
	{
		super(scope,_ttype);
		dim1=0;
	}
	
	protected void init(){
		data =new Object[dim1];
	}

	public DTDataType getDTDataType(DTDataType i1){
		int idx1=i1.getInteger()-base;
		if(dim1==0){
		    dim1=10;
		    init();
		}
		
		if(idx1>dim1){
		    return null;
		} else {
		    DTDataType t=(DTDataType)data[idx1];
		    if(t==null){
			data[idx1]=t=getOne();
		    }
		    return t;
		}
	    }

	public void setDTDataType(DTDataType i1,DTDataType s){
		int idx1=i1.getInteger()-base;
		if(dim1==0){
		    dim1=10;
		    init();
		}
		
		if(idx1>dim1){
		    //return null;
		} else {
		    DTDataType t=(DTDataType)data[idx1];
		    if(t==null){
			data[idx1]=getOne(s);
		    } else {
			t.assign(s);
		    }
		}
	}

	public	int getDimension(){return 1;}
	public	int getDimensioned(int i){
	    if(i==1){
		return dim1;
	    }
	    return 0;
	}
    public void setDimension(int i1){
	dim1=i1;
	init();
    }

    public int compareTo(Object o){
	return 0;
    }

}
