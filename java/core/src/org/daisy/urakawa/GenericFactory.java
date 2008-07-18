package org.daisy.urakawa;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.AbstractXukAble;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.QualifiedName;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * A generic factory based on the given type, that loads types lazily (on
 * demand) and manages a /cache registry of queried types for later
 * Xuk-serialization
 * 
 * @param <T>
 */
public class GenericFactory<T extends WithPresentation> extends
		WithPresentation {
	private class TypeAndQNames {
		public QualifiedName mQName;
		public QualifiedName mBaseQName;

		public String mFullName;
		public Class<?> mKlass;

		public void readFromXmlReader(IXmlDataReader rd) {
			try {
				mQName = new QualifiedName(rd.getAttribute("XukLocalName"), rd
						.getAttribute("XukNamespaceUri"));
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (rd.getAttribute("BaseXukLocalName") != null) {
				try {
					mBaseQName = new QualifiedName(rd
							.getAttribute("BaseXukLocalName"), rd
							.getAttribute("BaseXukNamespaceUri"));
				} catch (MethodParameterIsNullException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (MethodParameterIsEmptyStringException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			} else {
				mBaseQName = null;
			}
			/*
			 * AssemblyName = new AssemblyName(rd.GetAttribute("AssemblyName"));
			 * if (rd.GetAttribute("AssemblyVersion")!=null) {
			 * AssemblyName.Version = new
			 * Version(rd.GetAttribute("AssemblyVersion")); }
			 */
			mFullName = rd.getAttribute("FullName");
			if (mFullName != null) // AssemblyName != null &&
			{
				// ClassLoader.getSystemClassLoader()
				try {
					mKlass = this.getClass().getClassLoader().loadClass(
							mFullName);
				} catch (ClassNotFoundException e) {
					e.printStackTrace();
					mKlass = null;
				}
				/*
				 * try { Assembly a = Assembly.Load(AssemblyName); try { Type =
				 * a.GetType(FullName); } catch (ArgumentException) { Type =
				 * null; } } catch (FileLoadException) { Type = null; } catch
				 * (FileNotFoundException) { Type = null; }
				 */
			} else {
				mKlass = null;
			}
		}
	}

	private List<TypeAndQNames> mRegisteredTypeAndQNames = new LinkedList<TypeAndQNames>();
	private Map<String, TypeAndQNames> mRegisteredTypeAndQNamesByQualifiedName = new HashMap<String, TypeAndQNames>();
	private Map<Class<?>, TypeAndQNames> mRegisteredTypeAndQNamesByType = new HashMap<Class<?>, TypeAndQNames>();

	/**
	 * Clears the factory of all its registered types
	 */
	@Override
	protected void clear() {
		mRegisteredTypeAndQNames.clear();
		mRegisteredTypeAndQNamesByQualifiedName.clear();
		mRegisteredTypeAndQNamesByType.clear();
		// super.clear();
	}

	private void registerType_(TypeAndQNames tq) {
		mRegisteredTypeAndQNames.add(tq);
		mRegisteredTypeAndQNamesByQualifiedName.put(tq.mQName.getQName(), tq);
		if (tq.getClass() != null)
			mRegisteredTypeAndQNamesByType.put(tq.getClass(), tq);
	}

	@SuppressWarnings( { "unchecked", "synthetic-access" })
	private TypeAndQNames registerType(Class<? extends T> t) {
		TypeAndQNames tq = new TypeAndQNames();
		try {
			tq.mQName = AbstractXukAble.getXukQualifiedName(t);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		tq.mKlass = t;
		// TODO: check that the canonical name is the full class name including
		// namespaces
		tq.mFullName = t.getCanonicalName();
		// tq.AssemblyName = t.Assembly.GetName();
		if (WithPresentation.class.isAssignableFrom(t.getSuperclass())
				&& !mRegisteredTypeAndQNamesByType.containsKey(t
						.getSuperclass())) {
			tq.mBaseQName = registerType((Class<? extends T>) t.getSuperclass()).mQName;
		}
		registerType_(tq);
		return tq;
	}

	private boolean isRegistered(Class<?> t) {
		return mRegisteredTypeAndQNamesByType.containsKey(t);
	}

	private Class<?> lookupType(String qname) {
		if (mRegisteredTypeAndQNamesByQualifiedName.containsKey(qname)) {
			TypeAndQNames t = mRegisteredTypeAndQNamesByQualifiedName
					.get(qname);
			if (t != null) {
				if (t.mKlass != null) {
					return t.mKlass;
				}

				return lookupType(t.mBaseQName);
			}
		}
		return null;
	}

	private Class<?> lookupType(QualifiedName qname) {
		if (qname == null)
			return null;
		return lookupType(qname.getQName());
	}

	/**
	 * Initializes a created instance by assigning it an Presentation owner. In
	 * derived classes of this factory, this method can be overridden in order
	 * to perform additional initialization, in which case
	 * super.InitializeInstance(instance) must be called.
	 */
	protected void initializeInstance(T instance) {
		try {
			instance.setPresentation(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * TODO: Check that this instantiation mechanism actually works in Java 1.5
	 * 
	 * @param <T>
	 * @param klass
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return the created instance
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	@SuppressWarnings("unused")
	private <Z> Z create(Class<Z> klass, String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		String str = klass.getSimpleName();

		if (str != xukLocalName || xukNamespaceUri != IXukAble.XUK_NS) {
			return null;
		}
		try {
			return klass.newInstance();
		} catch (InstantiationException e) {
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			e.printStackTrace();
		}
		return null;
	}

	/**
	 * creates an instance of the given class type.
	 */
	public <U extends T> U create(Class<U> klass)
			throws MethodParameterIsNullException {
		if (klass == null) {
			throw new MethodParameterIsNullException();
		}
		U res;
		try {
			res = klass.newInstance();
		} catch (InstantiationException e) {

			e.printStackTrace();
			return null;
		} catch (IllegalAccessException e) {

			e.printStackTrace();
			return null;
		}
		initializeInstance(res);

		if (!isRegistered(klass)) {
			registerType(klass);
		}
		return res;
	}

	/**
	 * creates an instance based on the given qualified name, or returns null if
	 * there is no corresponding match in the currently registered types of the
	 * factory
	 */
	@SuppressWarnings("unchecked")
	public T create(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		String qname = xukLocalName + ":" + xukNamespaceUri;
		Class<?> t = lookupType(qname);
		if (t == null)
			return null;
		return create((Class<T>) t);
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			ProgressCancelledException, XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeStartElement("mRegisteredTypes", IXukAble.XUK_NS);
		for (TypeAndQNames tp : mRegisteredTypeAndQNames) {
			destination.writeStartElement("Type", IXukAble.XUK_NS);
			destination.writeAttributeString("XukLocalName", tp.mQName
					.getLocalName());
			destination.writeAttributeString("XukNamespaceUri", tp.mQName
					.getNamespaceUri());
			if (tp.mBaseQName != null) {
				destination.writeAttributeString("BaseXukLocalName",
						tp.mBaseQName.getLocalName());
				destination.writeAttributeString("BaseXukNamespaceUri",
						tp.mBaseQName.getNamespaceUri());
			}
			if (tp.mKlass != null) {
				// tp.AssemblyName = tp.Type.Assembly.GetName();
				tp.mFullName = tp.mKlass.getCanonicalName();
			}
			/*
			 * if (tp.AssemblyName!=null) {
			 * destination.WriteAttributeString("AssemblyName",
			 * tp.AssemblyName.Name);
			 * destination.WriteAttributeString("AssemblyVersion",
			 * tp.AssemblyName.Version.ToString()); }
			 */
			if (tp.mFullName != null)
				destination.writeAttributeString("FullName", tp.mFullName);
			destination.writeEndElement();
		}
		destination.writeEndElement();
		// super.xukOutChildren(destination, baseUri, ph);
	}

	@Override
	protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (source.getLocalName() == "mRegisteredTypes"
				&& source.getNamespaceURI() == IXukAble.XUK_NS) {
			xukInRegisteredTypes(source);
			return;
		}
		super.xukInChild(source, ph);
	}

	@SuppressWarnings("synthetic-access")
	private void xukInRegisteredTypes(IXmlDataReader source)
			throws XukDeserializationFailedException {
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					if (source.getLocalName() == "Type"
							&& source.getNamespaceURI() == IXukAble.XUK_NS) {
						TypeAndQNames tq = new TypeAndQNames();
						tq.readFromXmlReader(source);
						registerType_(tq);
					}
					if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		;
	}

}
