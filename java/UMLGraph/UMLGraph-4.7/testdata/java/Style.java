// $Id$
package net.sf.whatever.test;

/**
 * @hidden
 * @opt postfixpackage
 * @opt edgefontname arialbd
 * @opt nodefontname arial
 * @opt nodefontsize 9
 * @opt nodefontabstract ariali
 * @opt nodefontclassname arialbd
 * @opt nodefontclassabstractname arialbi
 * @opt nodefonttagsize 6
 * @opt nodefonttagname ariali
 * @opt nodefontpackagesize 8
 * @opt operations
 * @opt attributes
 * @opt qualify
 * @opt types
 */
class UMLOptions{}

/**
 * @stereotype base
 * @tagvalue since 1.0
 */
abstract class AbstractBase {
    /** @tagvalue since 1.5 */
    private int field;
    public abstract void abstractMethod();
    public int concreteMethod() { return 1; }
}

/**
 * @composed 1 has * from.Outer.Space.AlienClass
 */
class Composite extends AbstractBase {}

public class Style extends AbstractBase {}
