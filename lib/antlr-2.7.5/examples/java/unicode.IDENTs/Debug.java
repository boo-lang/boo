import java.io.*;
import java.util.*;
import javax.swing.*;
import javax.swing.text.*;

import java.awt.*;              //for layout managers
import java.awt.event.*;        //for action and window events

import java.net.URL;
import java.io.IOException;
import java.io.OutputStreamWriter;

/**
 * Creates the debug log file (and window)
 *
 * @version	0.2 11th Sep 2000
 * @author	Dr. M.P. Ford (c)2000 Forward Computing and Control Pty. Ltd.
 *   NSW Australia  email fcc@forward.com.au
 * You may use this class without restriction.
 */

/**
 * This class displays all System.out and System.err text in an JTextPanel
 * and saves it to a log file using the specified encodeing
 */
 
public class Debug extends PrintStream {

	private static String logFile = "debug.log";
	private static String encoding = "UTF8";
  private static boolean DEBUG_WINDOW = true;
	
	// find new line chars for this OS
	private static final String newLine = System.getProperty("line.separator","\n");

  public static Debug log;
  private static JFrame frame;
  private static JTextPane textPane;
  private static Document doc;
  private static OutputStreamWriter writer;
    
  static {
		try {
				System.out.println("Redirecting all output to file " +logFile +" in "+ encoding +" format");
		    log = new Debug(new FileOutputStream(logFile));
		} catch(IOException ex) {
		    System.err.println("Could not open Debug file " + logFile);
		    System.exit(1);
		}
    }
	    

    private Debug(FileOutputStream logFile) {
	    super(logFile,true);  // autoflush
	    try {
	    		writer = new OutputStreamWriter(logFile,encoding);
	   	} catch (UnsupportedEncodingException ex) {
	   		System.out.println("Invalid encoding");
	   		System.exit(1);
	   	}
	    System.setErr(this);
	    System.setOut(this);
	   
	    if (DEBUG_WINDOW) {
		     frame = new JFrame("Debug Output");
					textPane = createTextPane();
					frame.getContentPane().add(new JScrollPane(textPane));
		     frame.addWindowListener(new WindowAdapter() {
		            public void windowClosing(WindowEvent e) {
		                System.exit(0);
		            }
		     });
			
		     frame.pack();
		     frame.setVisible(true);
	    }
    System.err.println("Debug Log file: " + new Date());
		System.err.println("Debug class by Matthew Ford  email:matthew.ford@forward.com.au");
		System.err.println("(c)2000 Forward Computing and Control Pty. Ltd.");
		System.err.println("   NSW, Australia,  www.forward.com.au");
		System.err.println(" You may use this class without restriction");
		System.err.println("---------------------------------------------------------------");
    }
    

    private static JTextPane createTextPane() {
        JTextPane textPane = new JTextPane();
        textPane.setEditable(false);
				 initStylesForTextPane(textPane);
        doc = textPane.getDocument();
        textPane.setPreferredSize(new Dimension(500,500));
        return textPane;
    }

    
    public void print(boolean b) {
    	this.print(new Boolean(b).toString());
    }
    public void print(double d) {
    	this.print(new Double(d).toString());
    }
    public void print(long l) {
    	this.print(new Long(l).toString());
    }

    public void println(boolean b) {
    	this.print(b);
    	this.println();
    }
    public void println(double d) {
    	this.print(d);
    	this.println();
    }
    public void println(long l) {
    	this.print(l);
    	this.println();
    }
    
    
    public void print(char c) {
    	char c_array[] = new char[1];
    	c_array[0] = c;
    	this.print(new String(c_array));
    }
    
    public void print(char c[]) {
   		this.print(new String(c));
    }

    public void println(char c) {
    	this.print(c);
    	this.println();
    }
    
    public void println(char c[]) {
    	this.print(c);
    	this.println();
    }

    public void println(String x) {
    	this.print(x);
			this.println();
    }

    public void print(String x) {
    	// add it the debug frame and also the log file
    	try {
    		writer.write(x,0,x.length());
    	} catch (IOException ex) {
    		x += ex.getMessage();
    	}
    	
	    if (DEBUG_WINDOW) {
				try {
				doc.insertString(doc.getLength(), x,
				 textPane.getStyle("regular"));
				} catch (BadLocationException ble) {
	  	  	System.exit(2);  // cannot write to System.err here
				}
	    }
    }
    

    
    public void println() {
    	// start newline in debug frame and also the log file
    	String x = "\n";
    	try {
	    	writer.write(newLine,0,newLine.length());
	    	writer.flush();
    	} catch (IOException ex) {
    		x += ex.getMessage();
    	}
	    if (DEBUG_WINDOW) {
				try {
				doc.insertString(doc.getLength(), "\n",
				 textPane.getStyle("regular"));
				} catch (BadLocationException ble) {
	  	  	System.exit(2); // cannot write to System.err here
				}
			}    	
    }	
    
    private static void initStylesForTextPane(JTextPane textPane) {
        //Initialize some styles.
        Style def = StyleContext.getDefaultStyleContext().
                                        getStyle(StyleContext.DEFAULT_STYLE);

        Style regular = textPane.addStyle("regular", def);
        StyleConstants.setFontFamily(def, "SansSerif");
    }    
    
}
