package org.daisy.urakawa.core;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.core.TreeNode
 */
public final class TreeNodeFactory extends GenericFactory<TreeNode>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public TreeNodeFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }

    /**
     * @return
     */
    public TreeNode createTreeNode()
    {
        try
        {
            return create(TreeNode.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
