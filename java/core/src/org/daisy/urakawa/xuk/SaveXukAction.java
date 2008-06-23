package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.IProject;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.event.CancellableEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressAction;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressInformation;

/**
 *
 */
public class SaveXukAction extends ProgressAction implements
		IEventListener<CancellableEvent> {
	protected IEventHandler<Event> mEventNotifier = new EventHandler();
	private URI mUri;
	private IProject mProject;
	private IStream mStream;

	/**
	 * @param proj
	 * @param uri
	 * @param iStream
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public SaveXukAction(URI uri, IProject proj, IStream iStream)
			throws MethodParameterIsNullException {
		if (mUri == null || mProject == null || iStream == null) {
			throw new MethodParameterIsNullException();
		}
		mUri = uri;
		mProject = proj;
		mStream = iStream;
	}

	/**
	 * @param proj
	 * @param uri
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public SaveXukAction(URI uri, IProject proj)
			throws MethodParameterIsNullException {
		this(uri, proj, getStreamFromUri(uri));
	}

	private static IStream getStreamFromUri(URI uri)
			throws MethodParameterIsNullException {
		if (uri == null)
			throw new MethodParameterIsNullException();
		return new FileStream(uri.getPath());
	}

	@Override
	public ProgressInformation getProgressInfo() {
		if (mStream == null) {
			return null;
		}
		ProgressInformation pi = null;
		try {
			pi = new ProgressInformation(mStream.getLength(), mStream
					.getPosition());
		} catch (MethodParameterIsOutOfBoundsException e) {
			e.printStackTrace();
			return null;
		}
		return pi;
	}

	public boolean canExecute() {
		return true;
	}

	/**
	 * @tagvalue Events "CancelledEvent-FinishedEvent"
	 */
	@SuppressWarnings("unused")
	public void execute() throws CommandCannotExecuteException {
		mCancelHasBeenRequested = false;
		mStream = new FileStream(mUri.getPath());
		IXmlDataWriter mWriter = new XmlDataWriter(mStream);
		mWriter.writeStartDocument();
		mWriter.writeStartElement("Xuk", IXukAble.XUK_NS);
		if (IXukAble.XUK_XSD_PATH != "") {
			if (IXukAble.XUK_NS == "") {
				mWriter.writeAttributeString("xsi",
						"noNamespaceSchemaLocation",
						"http://www.w3.org/2001/XMLSchema-instance",
						IXukAble.XUK_XSD_PATH);
			} else {
				mWriter.writeAttributeString("xsi",
						"noNamespaceSchemaLocation",
						"http://www.w3.org/2001/XMLSchema-instance",
						IXukAble.XUK_NS + " " + IXukAble.XUK_XSD_PATH);
			}
		}
		try {
			registerListener(this, CancellableEvent.class);
		} catch (MethodParameterIsNullException e1) {
			System.out.println("WTF ?! This should never happen !");
			e1.printStackTrace();
		}
		try {
			mProject.xukOut(mWriter, mUri, this);
			notifyFinished();
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
		} catch (XukSerializationFailedException e) {
			mWriter.close();
			throw new RuntimeException(e);
		} catch (ProgressCancelledException e) {
			notifyCancelled();
		} finally {
			try {
				unregisterListener(this, CancellableEvent.class);
			} catch (MethodParameterIsNullException e) {
				System.out.println("WTF ?! This should never happen !");
				e.printStackTrace();
			}
		}
		mWriter.writeEndElement();
		mWriter.writeEndDocument();
		mWriter.close();
	}

	public String getLongDescription() {
		return null;
	}

	public String getShortDescription() {
		return null;
	}

	@SuppressWarnings("unused")
	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
	}

	@SuppressWarnings("unused")
	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public <K extends Event> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.notifyListeners(event);
	}

	public <K extends Event> void registerListener(IEventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.registerListener(listener, klass);
	}

	public <K extends Event> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.unregisterListener(listener, klass);
	}

	public <K extends CancellableEvent> void eventCallback(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (event.isCancelled()) {
			requestCancel();
		}
	}
}
