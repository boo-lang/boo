// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

package antlr;

public class PythonCharFormatter implements antlr.CharFormatter {

  public String escapeChar(int c, boolean forCharLiteral) { 
    //System.out.println("escapeChar("+c+","+forCharLiteral+") called");
    String s = _escapeChar(c,forCharLiteral);
    //System.out.println("=>[" + s + "]");
    return s;
  }
 
    
  public String _escapeChar(int c, boolean forCharLiteral) {
    switch (c) {
      //		case GrammarAnalyzer.EPSILON_TYPE : return "<end-of-token>";
      case '\n':
        return "\\n";
      case '\t':
        return "\\t";
      case '\r':
        return "\\r";
      case '\\':
        return "\\\\";
      case '\'':
        return forCharLiteral ? "\\'" : "'";
      case '"':
        return forCharLiteral ? "\"" : "\\\"";
      default :
        if (c < ' ' || c > 126) {
          if ((0x0000 <= c) && (c <= 0x000F)) {
            return "\\u000" + Integer.toString(c, 16);
          }
          else if ((0x0010 <= c) && (c <= 0x00FF)) {
            return "\\u00" + Integer.toString(c, 16);
          }
          else if ((0x0100 <= c) && (c <= 0x0FFF)) {
            return "\\u0" + Integer.toString(c, 16);
          }
          else {
            return "\\u" + Integer.toString(c, 16);
          }
        }
        else {
          return String.valueOf((char)c);
        }
    }
  }

  public String escapeString(String s) {
    String retval = new String();
    for (int i = 0; i < s.length(); i++) {
      retval += escapeChar(s.charAt(i), false);
    }
    return retval;
  }

  public String literalChar(int c) {
    return "" + escapeChar(c, true) + "";
  }

  public String literalString(String s) {
    return "\"" + escapeString(s) + "\"";
  }
}




