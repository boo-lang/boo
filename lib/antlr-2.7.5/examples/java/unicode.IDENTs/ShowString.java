
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

import java.awt.*;

class ShowString extends Frame {

    FontMetrics fontM;
    String outString;
    
    ShowString (String target, String title) {

        setTitle(title);
        outString = target;

        Font font = new Font("Monospaced", Font.PLAIN, 36);
        fontM = getFontMetrics(font);
        setFont(font);

        int size = 0;
        for (int i = 0; i < outString.length(); i++) {
           size += fontM.charWidth(outString.charAt(i));
        }
        size += 24;

        setSize(size, fontM.getHeight() + 60);
        setLocation(getSize().width/2, getSize().height/2);
        show();
    }

    public void paint(Graphics g) {
        Insets insets = getInsets();
        int x = insets.left; 
        int y = insets.top;
        g.drawString(outString, x + 6, y + fontM.getAscent() + 14);
    }
}
