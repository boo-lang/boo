using System;
using System.IO;
using antlr;
using antlr.collections;

/** A simple node to represent PLUS operation */
public class PLUSNode : BinaryOperatorAST {
	public PLUSNode() {
	}

	public PLUSNode(Token tok) {
	}

	/** Compute value of subtree; this is heterogeneous part :) */
	public override int Value() {
		return Left().Value() + Right().Value();
	}

	public override string ToString() {
		return " +";
	}

	public override void xmlSerializeRootOpen(TextWriter outWriter) {
		outWriter.Write("<PLUS>");
	}

	public override void xmlSerializeRootClose(TextWriter outWriter) {
		outWriter.Write("</PLUS>");
	}
}
