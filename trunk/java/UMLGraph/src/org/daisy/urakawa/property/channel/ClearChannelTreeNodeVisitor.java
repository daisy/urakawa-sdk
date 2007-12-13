package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.visitor.TreeNodeVisitor;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.DoesNotAcceptMediaException;
import org.daisy.urakawa.media.Media;

/**
 * TreeNodeVisitor that clears all media content for a Channel
 */
public class ClearChannelTreeNodeVisitor implements TreeNodeVisitor {
	private Channel mChannelToClear;

	/**
	 * @param chToClear
	 */
	public ClearChannelTreeNodeVisitor(Channel chToClear) {
		mChannelToClear = chToClear;
	}

	/**
	 * @return Channel
	 */
	public Channel getChannelToClear() {
		return mChannelToClear;
	}

	public void postVisit(TreeNode node) throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		ChannelsProperty chProp = node
				.<ChannelsProperty> getProperty(ChannelsProperty.class);
		if (chProp != null) {
			Media m;
			try {
				m = chProp.getMedia(mChannelToClear);
			} catch (ChannelDoesNotExistException e) {
				// Ignore this.
				e.printStackTrace();
				m = null;
			}
			if (m != null) {
				try {
					chProp.setMedia(mChannelToClear, null);
				} catch (ChannelDoesNotExistException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (DoesNotAcceptMediaException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
	}

	@SuppressWarnings("unused")
	public void preVisit(@SuppressWarnings("unused")
	TreeNode node) throws MethodParameterIsNullException {
	}
}
