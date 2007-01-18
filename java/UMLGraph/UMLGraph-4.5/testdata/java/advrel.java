/*
 * Advanced relationships
 * UML User Guide p. 137
 * $Id: advrel.java,v 1.1 2005/11/23 22:18:45 dds Exp $
 */

/**
 * @opt attributes
 * @opt operations
 * @hidden
 */
class UMLOptions {}

class Controller {}
class EmbeddedAgent {}
class PowerManager {}

/**
 * @extends Controller
 * @extends EmbeddedAgent
 * @navassoc - - - PowerManager
 */
class  SetTopController implements URLStreamHandler {
	int authorizationLevel;
	void startUp() {}
	void shutDown() {}
	void connect() {}
}

/** @depend - <friend> - SetTopController */
class ChannelIterator {}

interface URLStreamHandler {
	void OpenConnection();
	void parseURL();
	void setURL();
	void toExternalForm();
}
