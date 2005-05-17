package tinybasic;
import antlr.collections.AST;

public class DTArray2D extends DTDataType{

private Object data[][];
	
	protected int dim1,dim2;
	protected int base =0;

	public DTArray2D(int _ttype,Scope scope){
		super(scope,_ttype);
		dim1=dim2=0;
	}
	protected void init(){
		data =new Object[dim1][dim2];
	}

	public DTDataType getDTDataType(DTDataType i1,DTDataType i2){
		int idx1=i1.getInteger()-base;
		int idx2=i2.getInteger()-base;
		if(dim1==0){
		    dim1=10; dim2=10;
		    init();
		}
		
		if(idx1>dim1){
		    return null;
		} else if(idx2>dim2){
		    return null;
		} else {
		    DTDataType t=(DTDataType)data[idx1][idx2];
		    if(t==null){
			data[idx1][idx2]=t=getOne();
		    }
		    return t;
		}
	    }

	public void setDTDataType(DTDataType i1,DTDataType i2,DTDataType s){
		int idx1=i1.getInteger()-base;
		int idx2=i2.getInteger()-base;
		if(dim1==0){
		    dim1=10;dim2=10;
		    init();
		}
		
		if(idx1<=dim1 && idx2<=dim2){
		    DTDataType t=(DTDataType)data[idx1][idx2];
		    if(t==null){
			data[idx1][idx2]=getOne(s);
		    } else {
			t.assign(s);
		    }
		}
	}

	public	int getDimension(){return 2;}
	public	int getDimensioned(int i){
	    if(i==1){
		return dim1;
	    } else if (i==2){
		return dim2;
	    }
	    return 0;
	}
	public void setDimension(int i1,int i2){
	    dim1=i1;dim2=i2;
	    init();
    }

    public int compareTo(Object o){
	return 0;
    }

}
