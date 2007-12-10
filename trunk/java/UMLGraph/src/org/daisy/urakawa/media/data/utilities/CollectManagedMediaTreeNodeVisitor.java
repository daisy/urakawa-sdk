package org.daisy.urakawa.media.data.utilities;

import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.visitor.TreeNodeVisitor;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.ManagedMedia;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.channel.Channel;
import org.daisy.urakawa.property.channel.ChannelDoesNotExistException;
import org.daisy.urakawa.property.channel.ChannelsProperty;

/**
 * Visitor that collects all MediaData used by the visited TreeNodes
 */
public class CollectManagedMediaTreeNodeVisitor implements TreeNodeVisitor {
	private List<ManagedMedia> mCollectedMedia = new LinkedList<ManagedMedia>();

	/**
	 * @return
	 */
	public List<ManagedMedia> getListOfCollectedMedia() {
		return mCollectedMedia;
	}

	public void preVisit(TreeNode node) {
		// TODO: Why do we need a cast here to compile ?? (List<Property>)  
		for (Property prop : (List<Property>) node.getListOfProperties()) {
			if (prop instanceof ChannelsProperty) {
				ChannelsProperty chProp = (ChannelsProperty) prop;
				for (Channel ch : chProp.getListOfUsedChannels()) {
					try {
						if (chProp.getMedia(ch) instanceof ManagedMedia) {
							ManagedMedia mm = (ManagedMedia) chProp
									.getMedia(ch);
							if (!mCollectedMedia.contains(mm))
								mCollectedMedia.add(mm);
						}
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!");
					} catch (ChannelDoesNotExistException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!");
					}
				}
			}
		}
	}

	public void postVisit(@SuppressWarnings("unused")
	TreeNode node) {
		return;
	}
}
