/** A predefined type specifier.  They may have a name like
 *  "integer" sometimes for quick lookup, but are not user type
 *  names like for TYPE foo = RECORD i:integer END;
 *
 *  That would be a UserDefinedType object pointing to a RecordType
 *  object.  All variables refer
 *  to a TypeSpecifier to indicate the type of their values.
 *
 *  You should view this class and subclasses as templates for
 *  building up more complicated types.  These are the predefined
 *  types used by UserDefinedType to build new ones.
 */
import java.io.*;

public class TypeSpecifier extends Symbol implements Serializable {

	public TypeSpecifier() {
	}

	public TypeSpecifier(String name) {
		super(name);
	}
}
