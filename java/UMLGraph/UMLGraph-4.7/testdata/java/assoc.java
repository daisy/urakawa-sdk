// $Id: assoc.java,v 1.1 2005/11/23 22:18:45 dds Exp $

/**
 * Associations with visibility
 * UML User Guide p. 145
 *
 * @opt horizontal
 * @hidden
 */
class UMLOptions {}

/** @assoc * - "*\n\n+user " User */
class UserGroup {}

/** @navassoc "1\n\n+owner\r" - "*\n\n+key" Password */
class User{}

class Password{}
