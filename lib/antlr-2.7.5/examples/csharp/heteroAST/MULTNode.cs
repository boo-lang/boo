using System;
using System.IO;
using antlr;
using antlr.collections;

/** A simple node to represent MULT operation */
public class MULTNode : BinaryOperatorAST {
	public MULTNode() {
	}

	public MULTNode(Token tok) {
	}

	/** Compute value of subtree; this is heterogeneous part :) */
	public override int Value() {
		return Left().Value() * Right().Value();
	}

	public override string ToString() {
		return " *";
	}

	public override void xmlSerializeRootOpen(TextWriter outWriter) {
		outWriter.Write("<MULT>");
	}

	public override void xmlSerializeRootClose(TextWriter outWriter) {
		outWriter.Write("</MULT>");
	}
}
