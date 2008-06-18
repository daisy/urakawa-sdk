package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.Project;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.event.CancellableEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataReaderImpl;
import org.daisy.urakawa.progress.ProgressAction;
import org.daisy.urakawa.progress.ProgressCancelledException;

/**
 *
 */
public class OpenXukAction extends ProgressAction implements
		EventListener<CancellableEvent> {
	protected EventHandler<Event> mEventNotifier = new EventHandlerImpl();
	private URI mUri;
	private Project mProject;

	/**
	 * @param proj
	 * @param uri
	 * @throws MethodParameterIsNullException
	 */
	public OpenXukAction(Project proj, URI uri)
			throws MethodParameterIsNullException {
		mUri = uri;
		mProject = proj;
		//
		if (mUri == null || mProject == null) {
			throw new MethodParameterIsNullException();
		}
	}

	public void notifyProgress() {
		// TODO Auto-generated method stub
	}

	public boolean canExecute() {
		return true;
	}

	@SuppressWarnings("unused")
	public void execute() throws CommandCannotExecuteException {
		mCancel = false;
		XmlDataReader mReader = new XmlDataReaderImpl(mUri);
		if (!mReader.readToFollowing("Xuk", XukAble.XUK_NS)) {
			mReader.close();
			throw new RuntimeException(new XukDeserializationFailedException());
		}
		boolean foundProject = false;
		if (!mReader.isEmptyElement()) {
			while (mReader.read()) {
				if (mReader.getNodeType() == XmlDataReader.ELEMENT) {
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
				} else if (mReader.getNodeType() == XmlDataReader.ELEMENT) {
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

	public <K extends Event> void registerListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.registerListener(listener, klass);
	}

	public <K extends Event> void unregisterListener(EventListener<K> listener,
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
