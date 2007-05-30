package org.daisy.urakawa.validation.node;

import org.daisy.urakawa.core.CoreNodeFactory;

/**
 * This factory should take care of building particular Validator for certain types of Nodes.
 *
 * @depend - Create 1 CoreNodeValidator
 * @see CoreNodeFactory
 */
public interface CoreNodeValidatorFactory {
    /**
     * Creates a new Validator.
     *
     * @return cannot return null.
     */
    public CoreNodeValidator createNodeValidator();
}