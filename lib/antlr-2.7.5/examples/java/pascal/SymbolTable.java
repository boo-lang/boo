
import java.util.Hashtable;
import java.io.*;

/** Not sure what all we will want in this yet.  At minimum it can
 *  handle the predefined type objects like IntegerType.
 */
public class SymbolTable implements Serializable {
	protected Hashtable predefinedScalars = new Hashtable();

	public SymbolTable() {
		predefinedScalars.put("integer", new IntegerType());
		predefinedScalars.put("real", new RealType());
	}

	public TypeSpecifier getPredefinedType(String name) {
		TypeSpecifier ts = (TypeSpecifier)predefinedScalars.get(name);
		return ts;
	}
}
