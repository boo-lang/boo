package antlr.collections;

/* ANTLR Translator Generator
 * Project led by Terence Parr at http://www.jGuru.com
 * Software rights: http://www.antlr.org/license.html
 *
 * $Id: //depot/code/org.antlr/release/antlr-2.7.5/antlr/collections/Enumerator.java#1 $
 */

public interface Enumerator {
    /**Return the element under the cursor; return null if !valid() or
     * if called before first next() call.
     */
    public Object cursor();

    /**Return the next element in the enumeration; first call to next()
     * returns the first element.
     */
    public Object next();

    /**Any more elements in the enumeration? */
    public boolean valid();
}
