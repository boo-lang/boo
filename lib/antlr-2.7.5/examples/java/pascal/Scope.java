import java.util.Hashtable;
import java.util.Enumeration;
import java.io.*;

/** A set of symbols (type defs and/or variables) */
public class Scope implements Serializable {
	protected Scope parent;

	protected Hashtable symbols = new Hashtable();

	public Scope(Scope parent) {
		this.parent = parent;
	}

	public void addSymbol(Symbol s) {
		if ( s.getName() != null ) {
			symbols.put(s.getName(), s);
		}
	}

	/** Look up name in this scope or parent */
	public Symbol resolve(String name) {
		Symbol s = (Symbol)symbols.get(name);
		if ( s==null ) {
			return parent.resolve(name);
		}
		return s;
	}

	public String toString() {
		return "Scope=["+symbols+"]"+(parent==null?"<root>":"");
	}
}
