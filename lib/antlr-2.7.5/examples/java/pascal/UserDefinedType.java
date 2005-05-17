/** Just like a variable except it cannot hold a variable value--it
 *  holds a type like TYPE size=BIG;
 *
 *  You would create a UserDefinedType("size", ...lookup BIG...)
 *
 *  In essence, this is anything in the TYPE section.  This is how
 *  new types are introduced--they build upon TypeSpecifier objects
 *  that represent records, scalars etc...
 */
import java.io.*;

public class UserDefinedType extends Symbol implements Serializable {
	protected TypeSpecifier type;

	public UserDefinedType(String name, TypeSpecifier type) {
		super(name);
		setType(type);
	}

	public TypeSpecifier getType() {
		return type;
	}

	public void setType(TypeSpecifier type) {
		this.type = type;
	}
}
