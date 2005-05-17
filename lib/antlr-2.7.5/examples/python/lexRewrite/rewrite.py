### main - create and run lexer from stdin
if __name__ == '__main__' :
  import sys
  import antlr
  import rewrite_l
  
  
  ### create lexer - shall read from stdin
  L = rewrite_l.Lexer()
  try:
     L.mSTART(1);
     token = L.getTokenObject()
  except antlr.TokenStreamException, e:
    print "error: exception caught while lexing: "
### end of main
