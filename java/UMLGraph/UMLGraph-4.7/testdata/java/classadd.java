/*
 * Class stereotypes and tagged values
 * UML User Guide p. 439
 * $Id: classadd.java,v 1.1 2005/11/23 22:18:45 dds Exp $
 */

/** 
 * @opt attributes 
 * @opt operations 
 * @opt types 
 * @hidden
 */
class UMLOptions {}

/** @hidden */
class Action {}

/** 
 * @stereotype container 
 * @tagvalue version 3.2
 */
class ActionQueue {
	void add(Action a) {};
	/** @tagvalue version 1.0 */
	void add(Action a, int n) {};
	void remove(int n) {};
	/** @stereotype query */
	int length() {};
	/** @stereotype "helper functions" */
	void reorder() {};
}
