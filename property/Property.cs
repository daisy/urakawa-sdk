using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using urakawa.core;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.property
{
    [XukNameUglyPrettyAttribute("prp", "Property")]
    public abstract class Property : WithPresentation, IXukAble, urakawa.events.IChangeNotifier
    {
        #region IChangeNotifier Members

        /// <summary>
        /// Event fired after the <see cref="Property"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<urakawa.events.DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(urakawa.events.DataModelChangedEventArgs args)
        {
            EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        #endregion

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="Property"/>s should only be created via. the <see cref="PropertyFactory"/>
        /// </summary>
        public Property()
        {
        }

        protected TreeNode mOwner = null;


        /// <summary>
        /// Tests if a the <see cref="Property"/> can be validly added to a given potential owning <see cref="TreeNode"/>
        /// </summary>
        /// <param name="potentialOwner">The potential new owner</param>
        /// <returns>A <see cref="bool"/> indicating if the property can be added</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="potentialOwner"/> is <c>null</c>
        /// </exception>
        public virtual bool CanBeAddedTo(TreeNode potentialOwner)
        {
            if (potentialOwner == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not test if the Property can be added to a null TreeNode");
            }
            return true;
        }

        /// <summary>
        /// Creates a copy of the property
        /// </summary>
        /// <returns>The copy</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown if the property has not been initialized with an owning <see cref="TreeNode"/>
        /// </exception>
        /// <exception cref="exception.FactoryCannotCreateTypeException">
        /// Thrown if the <see cref="PropertyFactory"/> associated with the property via. it's owning <see cref="TreeNode"/>
        /// can not create an <see cref="Property"/> mathcing the Xuk QName of <c>this</c>
        /// </exception>
        /// <remarks>
        /// In subclasses of <see cref="Property"/> the implementor should override <see cref="CopyProtected"/> and if the impelemntor
        /// wants the copy method of his subclass to have "correct" type he should create a new version of <see cref="Copy"/> 
        /// that delegates the copy operation to <see cref="CopyProtected"/> followed by type casting. 
        /// See <see cref="urakawa.property.xml.XmlProperty.Copy"/>
        /// and <see cref="CopyProtected"/> for an example of this.
        /// </remarks>
        public Property Copy()
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            return CopyProtected();
        }

        /// <summary>
        /// Protected version of <see cref="Copy"/>. Override this method in subclasses to copy additional data
        /// </summary>
        /// <returns>A copy of <c>this</c></returns>
        protected virtual Property CopyProtected()
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            Property theCopy = TreeNodeOwner.Presentation.PropertyFactory.Create(GetType());
            if (theCopy == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The PropertyFactory can not create a Property of type matching QName {1}:{0}",
                                                                         GetXukName(), GetXukNamespace()));
            }
            return theCopy;
        }

        /// <summary>
        /// Gets a property with identical content to this but compatible with a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported property</returns>
        public Property Export(Presentation destPres)
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            return ExportProtected(destPres);
        }

        /// <summary>
        /// Gets a property with identical content to this but compatible with a given destination <see cref="Presentation"/>.
        /// Override this method in subclasses to export additional data
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported property</returns>
        protected virtual Property ExportProtected(Presentation destPres)
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            Property exportedProp = destPres.PropertyFactory.Create(GetType());
            if (exportedProp == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The PropertyFactory of the export destination Presentation can not create a Property of type matching QName {1}:{0}",
                                                                         GetXukName(), GetXukNamespace()));
            }
            return exportedProp;
        }

        public void XukIn(XmlReader source, IProgressHandler handler, TreeNode node)
        {
            mOwner = node;
            XukIn(source, handler);
            mOwner = null;
        }

        /// <summary>
        /// Gets the owner <see cref="TreeNode"/> of the property
        /// </summary>
        /// <returns>The owner</returns>
        public virtual TreeNode TreeNodeOwner
        {
            get
            {
                if (mOwner == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The Property has not been initialized with an owning TreeNode");
                }
                return mOwner;
            }
            set
            {
                if (value != null)
                {
                    if (mOwner != null && value != mOwner)
                    {
                        throw new exception.PropertyAlreadyHasOwnerException(
                            "The Property is already owner by a different TreeNode");
                    }
                    if (value.Presentation != Presentation)
                    {
                        throw new exception.NodeInDifferentPresentationException(
                            "The property can not have a owning TreeNode from a different Presentation");
                    }
                }
                mOwner = value;
            }
        }

        #region IValueEquatable<Property> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }
            
            return true;
        }

        #endregion
    }
}