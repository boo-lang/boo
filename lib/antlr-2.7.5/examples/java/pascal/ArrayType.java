
import java.util.Vector;
import java.io.*;

/** Represents an array of form ARRAY [...] OF type.
 *  If named like foo=array [1..5] of integer; then this Symbol
 *  can be looked up in the symbol table.  Otherwise, this object
 *  will be exclusively pointed to from a variable or field def.
 */
public class ArrayType extends TypeSpecifier implements Serializable {
	protected Vector/*<TypeSpecifier>*/ indexTypes;
	protected TypeSpecifier elementType;

}
