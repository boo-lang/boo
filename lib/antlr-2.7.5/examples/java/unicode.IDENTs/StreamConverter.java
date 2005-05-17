
/*
 * Copyright (c) 1995-1998 Sun Microsystems, Inc. All Rights Reserved.
 *
 * Permission to use, copy, modify, and distribute this software
 * and its documentation for NON-COMMERCIAL purposes and without
 * fee is hereby granted provided that this copyright notice
 * appears in all copies. Please refer to the file "copyright.html"
 * for further important copyright and licensing information.
 *
 * SUN MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF
 * THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 * PARTICULAR PURPOSE, OR NON-INFRINGEMENT. SUN SHALL NOT BE LIABLE FOR
 * ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR
 * DISTRIBUTING THIS SOFTWARE OR ITS DERIVATIVES.
 */

import java.io.*;
import java.util.*;

public class StreamConverter {

   static void writeOutput(String str) {

       try {
           FileOutputStream fos = new FileOutputStream("test.txt");
           Writer out = new OutputStreamWriter(fos, "UTF8");
           out.write(str);
           out.close();
       } catch (IOException e) {
           e.printStackTrace();
       }
   }

   static String readInput() {

      StringBuffer buffer = new StringBuffer();
      try {
          FileInputStream fis = new FileInputStream("test.txt");
          InputStreamReader isr = new InputStreamReader(fis, "UTF8");
          Reader in = new BufferedReader(isr);
          int ch;
          while ((ch = in.read()) > -1) {
             buffer.append((char)ch);
          }
          in.close();
          return buffer.toString();
      } catch (IOException e) {
          e.printStackTrace();
          return null;
      }
   }

   public static void main(String[] args) {

      String jaString  = 
         new String("\u65e5\u672c\u8a9e\u6587\u5b57\u5217");

      writeOutput(jaString);
      String inputString = readInput();
      String displayString = jaString + " " + inputString;
      new ShowString(displayString, "Conversion Demo");
   }

}

