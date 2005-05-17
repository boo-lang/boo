/** The symbol table representation of CONST PI = 3.141592; You would
 *  create new RealConstant("PI", 3.141592)
 */
import java.io.*;

public class RealConstant extends Constant implements Serializable {
	protected double value;

	public RealConstant(String name, double value) {
		super(name);
		setValue(value);
		setType(PascalParser.symbolTable.getPredefinedType("real"));
	}

	public RealConstant(String svalue) {
		this(null,svalue);
	}

	public RealConstant(String name, String svalue) {
		super(name);
		setType(PascalParser.symbolTable.getPredefinedType("real"));
		double v = 0;
		try {
			v = Double.parseDouble(svalue);
		}
		catch (NumberFormatException nfe) {
			;
		}
		setValue(v);
	}

 	public double getValue() {
		return value;
	}

	public void setValue(double value) {
		this.value = value;
	}

	public String toString() {
		return super.toString()+"="+value;
	}
}
