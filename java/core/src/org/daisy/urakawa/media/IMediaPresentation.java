package org.daisy.urakawa.media;

import java.util.List;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * This interface represents a basic "media presentation" with:
 * <ul>
 * <li>a factory for creating media objects.</li>
 * <li>a URI pointing at the directory containing the media objects</li>
 * </ul>
 * This is convenience interface for the design only, in order to organize the
 * data model in smaller modules.
 * 
 * @depend - Aggregation 0..n IMedia
 */
public interface IMediaPresentation
{
    /**
     * Convenience method to get the list of IMedia used by the given ITreeNode,
     * that is to say the IMedia objects that are associated to a ITreeNode via
     * a IChannelsProperty.
     * 
     * @param node
     *        cannot be null
     * @return a non-null list, which can be empty.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public List<IMedia> getListOfMediaUsedByTreeNode(ITreeNode node)
            throws MethodParameterIsNullException;

    /**
     * Gets the list of all used IMedia objects in the Presentation.
     * 
     * @see #getListOfMediaUsedByTreeNode(ITreeNode)
     * @return a non-null list, which can be empty.
     */
    public List<IMedia> getListOfUsedMedia();
}
