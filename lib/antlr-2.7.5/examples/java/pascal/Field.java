/** Represents a variable that is actually the field of a RECORD.
 *  We might not need the distinction, but it will make things
 *  more clear.
 */
import java.io.*;

public class Field extends Variable implements Serializable {
	public Field(String name, TypeSpecifier type) {
		super(name,type);
	}
}
