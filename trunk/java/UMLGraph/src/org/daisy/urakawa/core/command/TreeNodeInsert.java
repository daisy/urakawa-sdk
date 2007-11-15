package org.daisy.urakawa.core.command;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.undo.Command;
import org.daisy.urakawa.undo.CommandCannotExecuteException;
import org.daisy.urakawa.undo.CommandCannotUnExecuteException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * @see org.daisy.urakawa.core.TreeNodeWriteOnlyMethods#insert(TreeNode, int)
 */
public class TreeNodeInsert implements Command {
	/**
	 * @param nodeAnchor
	 * @param nodeToAdd
	 * @param indexInsert
	 */
	public TreeNodeInsert(TreeNode nodeAnchor, TreeNode nodeToAdd,
			int indexInsert) {
	}

	/*
	 * @hidden
	 */
	public boolean canExecute() {
		return false;
	}

	/*
	 * @hidden
	 */
	public boolean canUnExecute() {
		return false;
	}

	/*
	 * @hidden
	 */
	public void execute() throws CommandCannotExecuteException {
	}

	/*
	 * @hidden
	 */
	public List<MediaData> getListOfUsedMediaData() {
		return null;
	}

	/*
	 * @hidden
	 */
	public String getLongDescription() {
		return null;
	}

	/*
	 * @hidden
	 */
	public String getShortDescription() {
		return null;
	}

	/*
	 * @hidden
	 */
	public void unExecute() throws CommandCannotUnExecuteException {
	}

	/*
	 * @hidden
	 */
	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	/*
	 * @hidden
	 */
	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	/*
	 * @hidden
	 */
	public String getXukLocalName() {
		return null;
	}

	/*
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return null;
	}

	/*
	 * @hidden
	 */
	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
	}

	/*
	 * @hidden
	 */
	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}
}
