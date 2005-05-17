package tinybasic;
import antlr.collections.AST;


public class DTArray3D extends DTDataType{

private Object data[][][];

	protected int dim1,dim2,dim3;
	protected int base=0;
	
	public DTArray3D(int _ttype,Scope scope){
		super(scope,_ttype);
		dim1=dim2=dim3=0;
	}

	protected void init(){
		data =new Object[dim1][dim2][dim3];
	}

	public DTDataType getDTDataType(DTDataType i1,DTDataType i2,DTDataType i3){
		int idx1=i1.getInteger()-base;
		int idx2=i2.getInteger()-base;
		int idx3=i3.getInteger()-base;
		if(dim1==0){
		    dim1=10;dim2=10;dim3=10;
		    init();
		}
		
		if(idx1>dim1){
		    return null;
		} else if(idx2>dim2){
		    return null;
		} else if(idx3>dim3){
		    return null;
		} else {
		    DTDataType t=(DTDataType)data[idx1][idx2][idx3];
		    if(t==null){
			data[idx1][idx2][idx3]=t=getOne();
		    }
		    return t;
		}
	    }

	public void setDTDataType(DTDataType i1,DTDataType i2,DTDataType i3,DTDataType s){
		int idx1=i1.getInteger()-base;
		int idx2=i2.getInteger()-base;
		int idx3=i3.getInteger()-base;
		if(dim1==0){
		    dim1=10;dim2=10;dim3=10;
		    init();
		}
		
		if(idx1<=dim1 && idx2<=dim2 && idx3<=dim3){
		    DTDataType t=(DTDataType)data[idx1][idx2][idx3];
		    if(t==null){
			data[idx1][idx2][idx3]=getOne(s);
		    } else {
			t.assign(s);
		    }
		}
	}

	public	int getDimension(){return 3;}
	public	int getDimensioned(int i){
	    if(i==1){
		return dim1;
	    } else if (i==2){
		return dim2;
	    } else if (i==3){
		return dim3;
	    }
	    return 0;
	}
    public void setDimension(int i1,int i2,int i3){
	dim1=i1;dim2=i2;dim3=i3;
	init();
    }
    
    public int compareTo(Object o){
	return 0;
    }


}
