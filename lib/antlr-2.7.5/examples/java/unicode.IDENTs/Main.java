import java.io.*;
import antlr.RecognitionException;
import antlr.TokenStreamException;
import antlr.TokenStreamRecognitionException;
import antlr.TokenStreamRetryException;
import antlr.CharBuffer;

class Main {
	// find new line chars for this OS
	private static final String nl = System.getProperty("line.separator","\n");

	public static void main(String[] args) {
		
   InputStreamReader reader = null;
   FileInputStream inputStream = null;
   
		// this one command redirects all System.out and System.err to debug file
		Debug.log.println(" Output for Unicode example");

   System.out.println(" The input file is:"+args[0]);
   try {
   		inputStream = new FileInputStream(args[0]);
   } catch (FileNotFoundException ex) {
   		System.out.println("Could not find file.");
   		System.exit(1);
   }
   	
   try {
   		reader = new InputStreamReader(inputStream,"SJIS");
   } catch (UnsupportedEncodingException ex) {
   		System.out.println("Invalid encoding");
   		System.exit(1);
   }
   BufferedReader bufferedReader = new BufferedReader(reader);
   try {
	   String line = bufferedReader.readLine();
	   while (line != null) {
	   	 System.out.println(line);
	     line = bufferedReader.readLine();
	   }
	   inputStream.close();
   } catch (IOException ex) {
   		System.out.println(ex.getMessage());
   		System.exit(1);
   }
   	
   
   System.out.println("\nThe parse output is:");
   
   try {
   		inputStream = new FileInputStream(args[0]);
   } catch (FileNotFoundException ex) {
   		System.out.println("Could not find file.");
   		System.exit(1);
   }
   try {
   		reader = new InputStreamReader(inputStream,"SJIS");
   } catch (UnsupportedEncodingException ex) {
   		System.out.println("Invalid encoding");
   		System.exit(1);
   }
   CharBuffer cb = new CharBuffer(reader);
		try {
			UnicodeLexer lexer = new UnicodeLexer(cb);
			UnicodeParser parser = new UnicodeParser(lexer);
			parser.program();
			
		} catch(Exception e) {
			// catch all mainly IOExceptions
			System.err.println("exception: "+e);
		}
	}
}

