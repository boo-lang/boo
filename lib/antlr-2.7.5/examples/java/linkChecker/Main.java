import java.io.*;

public class Main {
  public static void main(String[] args) {
	if ( args.length==0 ) {
	  System.out.println("java Main document.html");
	}
	else {
	  LinkChecker chk = new LinkChecker(args[0]);
	  try {
	chk.doCheck();
	  }
	  catch (IOException io) {
	System.err.println("IOException: "+io.getMessage());
	io.printStackTrace();
	  }
	}
  }  
}