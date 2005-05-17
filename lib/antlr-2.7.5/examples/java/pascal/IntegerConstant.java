/** The symbol table representation of CONST i=3; You would
 *  create new IntegerConstant("i", 3)
 */
public class IntegerConstant extends Constant {
	protected int value;

	public IntegerConstant(String name, int value) {
		super(name);
		setValue(value);
		setType(PascalParser.symbolTable.getPredefinedType("integer"));
	}

	public IntegerConstant(String svalue) {
		this(null,svalue);
	}

	public IntegerConstant(String name, String svalue) {
		super(name);
		setType(PascalParser.symbolTable.getPredefinedType("integer"));
		int v = 0;
		try {
			v = Integer.parseInt(svalue);
		}
		catch (NumberFormatException nfe) {
			;
		}
		setValue(v);
	}

 	public int getValue() {
		return value;
	}

	public void setValue(int value) {
		this.value = value;
	}

	public String toString() {
		return super.toString()+"="+value;
	}
}
