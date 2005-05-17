/** A symbol that can hold a run-time value
 *  (has a type associated with it too).
 *
 *  In essence, this is anything in the global VAR section or is a parameter or
 *  a local variable.  Subclass Constant extends to include anything in CONST
 *  section also.  Subclass Field extends to include anything in a RECORD.
 */
import java.io.*;

public class Variable extends Symbol implements Serializable {
	protected TypeSpecifier type;

	public Variable() {
	}

	public Variable(String name) {
		super(name);
	}

	public Variable(String name, TypeSpecifier type) {
		super(name);
		setType(type);
	}

	public TypeSpecifier getType() {
		return type;
	}

	public void setType(TypeSpecifier type) {
		this.type = type;
	}

	public String toString() {
		return getClass().getName()+"<"+getName()+","+type+">";
	}
}
