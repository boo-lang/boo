import antlr.BaseAST;
import antlr.Token;
import antlr.collections.AST;
import java.io.*;

/** A simple node to represent PLUS operation */
public class PLUSNode extends BinaryOperatorAST {
	public PLUSNode(Token tok) {
	}

	/** Compute value of subtree; this is heterogeneous part :) */
	public int value() {
		return left().value() + right().value();
	}

	public String toString() {
		return " +";
	}

	public void xmlSerializeRootOpen(Writer out) throws IOException {
		out.write("<PLUS>");
	}

	public void xmlSerializeRootClose(Writer out) throws IOException {
		out.write("</PLUS>");
	}
}
