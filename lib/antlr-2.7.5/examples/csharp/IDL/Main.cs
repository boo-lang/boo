using System;
using antlr;

public class AppMain {
  public static void Main(string[] args) {
    try {
      IDLLexer lexer = new IDLLexer(new ByteBuffer(Console.OpenStandardInput()));
      IDLParser parser = new IDLParser(lexer);
      parser.specification();
    } catch(Exception e) {
      Console.Error.WriteLine("exception: "+e);
    }
  }
}

