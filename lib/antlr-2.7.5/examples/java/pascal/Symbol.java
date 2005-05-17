/** Any kind of symbol in a pascal program; variable, type, etc... */
import java.io.*;

public class Symbol implements Serializable {
	protected String name;

	public Symbol() {
	}

	public Symbol(String name) {
		setName(name);
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String toString() {
		return getClass().getName()+"<"+getName()+">";
	}

}
