package org.daisy.urakawa.validation.node;

import org.daisy.urakawa.core.TreeNodeFactory;

/**
 * This factory should take care of building particular Validator for certain types of Nodes.
 *
 * @depend - Create 1 TreeNodeValidator
 * @see TreeNodeFactory
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface TreeNodeValidatorFactory {
    /**
     * Creates a new Validator.
     *
     * @return cannot return null.
     */
    public TreeNodeValidator createNodeValidator();
}