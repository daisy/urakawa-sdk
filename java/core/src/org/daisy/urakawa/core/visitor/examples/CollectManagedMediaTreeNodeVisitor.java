package org.daisy.urakawa.core.visitor.examples;

import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.core.visitor.ITreeNodeVisitor;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.IManagedMedia;
import org.daisy.urakawa.property.IProperty;
import org.daisy.urakawa.property.channel.IChannel;
import org.daisy.urakawa.property.channel.ChannelDoesNotExistException;
import org.daisy.urakawa.property.channel.IChannelsProperty;

/**
 * Visitor that collects all IMediaData used by the visited TreeNodes
 */
public class CollectManagedMediaTreeNodeVisitor implements ITreeNodeVisitor {
	private List<IManagedMedia> mCollectedMedia = new LinkedList<IManagedMedia>();

	/**
	 * @return list
	 */
	public List<IManagedMedia> getListOfCollectedMedia() {
		return mCollectedMedia;
	}

	public boolean preVisit(ITreeNode node) {
		for (IProperty prop : (List<IProperty>) node.getListOfProperties()) {
			if (prop instanceof IChannelsProperty) {
				IChannelsProperty chProp = (IChannelsProperty) prop;
				for (IChannel ch : chProp.getListOfUsedChannels()) {
					try {
						if (chProp.getMedia(ch) instanceof IManagedMedia) {
							IManagedMedia mm = (IManagedMedia) chProp
									.getMedia(ch);
							if (!mCollectedMedia.contains(mm))
								mCollectedMedia.add(mm);
						}
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (ChannelDoesNotExistException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
			}
		}
		return true;
	}

	public void postVisit(@SuppressWarnings("unused")
	ITreeNode node) {
		return;
	}
}
