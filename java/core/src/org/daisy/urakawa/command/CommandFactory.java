package org.daisy.urakawa.command;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 */
public final class CommandFactory extends GenericFactory<AbstractCommand> {

	/**
	 * @return
	 */
	public CompositeCommand createCompositeCommand() {

		try {
			return create(CompositeCommand.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}
}
