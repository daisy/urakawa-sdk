package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.IProject;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.event.CancellableEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataReaderImpl;
import org.daisy.urakawa.progress.ProgressAction;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressInformation;

/**
 *
 */
public class OpenXukAction extends ProgressAction implements
		IEventListener<CancellableEvent> {
	protected IEventHandler<Event> mEventNotifier = new EventHandlerImpl();
	private URI mUri;
	private IProject mProject;
	private IStream mStream;

	/**
	 * @param proj
	 * @param uri
	 * @param iStream
	 * @throws MethodParameterIsNullException
	 */
	public OpenXukAction(URI uri, IProject proj, IStream iStream)
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
	 */
	public OpenXukAction(URI uri, IProject proj)
			throws MethodParameterIsNullException {
		this(uri, proj, getStreamFromUri(uri));
	}

	private static IStream getStreamFromUri(URI uri)
			throws MethodParameterIsNullException {
		if (uri == null)
			throw new MethodParameterIsNullException();
		return new FileStream(uri.getPath());
	}

	public boolean canExecute() {
		return true;
	}

	@Override
	public ProgressInformation getProgressInfo() {
		if (mStream == null) {
			return null;
		}
		ProgressInformation pi = new ProgressInformation();
		pi.setCurrent(mStream.getPosition());
		pi.setTotal(mStream.getLength());
		return pi;
	}

	@SuppressWarnings("unused")
	public void execute() throws CommandCannotExecuteException {
		mCancelHasBeenRequested = false;
		IXmlDataReader mReader = new XmlDataReaderImpl(mStream);
		if (!mReader.readToFollowing("Xuk", IXukAble.XUK_NS)) {
			mReader.close();
			throw new RuntimeException(new XukDeserializationFailedException());
		}
		boolean foundProject = false;
		if (!mReader.isEmptyElement()) {
			while (mReader.read()) {
				if (mReader.getNodeType() == IXmlDataReader.ELEMENT) {
					if (mReader.getLocalName() == mProject.getXukLocalName()
							&& mReader.getNamespaceURI() == mProject
									.getXukNamespaceURI()) {
						foundProject = true;
						try {
							registerListener(this, CancellableEvent.class);
						} catch (MethodParameterIsNullException e1) {
							System.out
									.println("WTF ?! This should never happen !");
							e1.printStackTrace();
						}
						try {
							mProject.xukIn(mReader, this);
							notifyFinished();
						} catch (MethodParameterIsNullException e) {
							System.out
									.println("WTF ?! This should never happen !");
							e.printStackTrace();
						} catch (XukDeserializationFailedException e) {
							throw new RuntimeException(e);
						} catch (ProgressCancelledException e) {
							notifyCancelled();
						} finally {
							try {
								unregisterListener(this, CancellableEvent.class);
							} catch (MethodParameterIsNullException e) {
								System.out
										.println("WTF ?! This should never happen !");
								e.printStackTrace();
							}
						}
					} else if (!mReader.isEmptyElement()) {
						mReader.readSubtree().close();
					}
				} else if (mReader.getNodeType() == IXmlDataReader.ELEMENT) {
					break;
				}
				if (mReader.isEOF()) {
					mReader.close();
					throw new RuntimeException(
							new XukDeserializationFailedException());
				}
			}
		}
		if (!foundProject) {
			mReader.close();
			throw new RuntimeException(new XukDeserializationFailedException());
		}
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

	public <K extends Event> void unregisterListener(IEventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
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
