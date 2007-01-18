// $Id: foo.java,v 1.1 2005/11/23 22:18:45 dds Exp $

/**
 * Associations with visibility
 * UML User Guide p. 145
 *
 * @opt horizontal
 */
package a.b.c;

class UMLOptions {}

/** @assoc * - "*\n\n+user " User */
class UserGroup {}

/** @navassoc "1\n\n+owner\r" - "*\n\n+key" Password */
class User{}

class Password{}

class Hidden{}
