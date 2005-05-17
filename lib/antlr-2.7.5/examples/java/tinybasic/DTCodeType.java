package tinybasic;
import java.util.Stack;
import java.util.Vector;
import antlr.collections.AST;

public class DTCodeType {

    protected Stack callDepthStack;
    class SaveEnv{
	protected Scope scope;
	protected Vector args;
	
	SaveEnv(Scope scope,Vector args){
	    this.scope=scope;
	    this.args=args;
	}
	
	Scope getScope(){return scope;}
	Vector getArgs() { return args;}
    }
	
    
    protected AST entry,cb;
    protected Context theContext;
    protected Scope scope;
    protected Vector args;
    
    String name;
    
    class CodeContext {
	protected Context context;
	protected Scope scope;
	protected Vector args;
	CodeContext (Context context,Scope scope,Vector args){
	    this.context=context;
	    this.scope=scope;
	    this.args=args;
	}
    }
    
    public DTCodeType(AST entry,AST cb,Scope scope,Vector args,String name){
	this.entry	=	entry	;
	this.cb		=	cb	;
	this.scope	=	scope;
	this.args	=	args	;
	this.name	=	name	;
	
	callDepthStack=new Stack();
    }

    public void newCall(Context context){

	CodeContext codeContext=new CodeContext(context,scope,args);
	callDepthStack.push(codeContext);
	context.pushScope(scope);
    }
    
    public void attachArg(int argnum,DTDataType arg){
	DTDataType proxy=(DTDataType)args.elementAt(argnum);
	proxy.attach(arg);
    }
    
    public AST getAST(){
	return this.entry;
    }


}
