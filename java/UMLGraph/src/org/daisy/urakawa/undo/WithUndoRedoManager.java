package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the UndoRedoManager.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithUndoRedoManager {
	/**
	 * @return the manager object. Can be null.
	 */
	public UndoRedoManager getUndoRedoManager();

	/**
	 * This method should only be used internally. Instead, API users should
	 * call {@link org.daisy.urakawa.Presentation#enableUndoRedo()} and
	 * {@link org.daisy.urakawa.Presentation#disableUndoRedo()}
	 * 
	 * @param factory
	 *            can be null
	 * @stereotype Initialize
	 */
	public void setUndoRedoManager(UndoRedoManager manager)
			throws MethodParameterIsNullException;
}
