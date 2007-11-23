using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace urakawa
{
	/// <summary>
	/// Factory creating the data model objects, includes creator methods for:
	/// <list type="ul">
	/// <item><see cref="Presentation"/></item>
	/// <item><see cref="metadata.MetadataFactory"/></item>
	/// <item><see cref="undo.CommandFactory"/></item>
	/// <item><see cref="undo.UndoRedoManager"/></item>
	/// <item><see cref="core.TreeNodeFactory"/></item>
	/// <item><see cref="property.PropertyFactory"/></item>
	/// <item><see cref="property.channel.ChannelFactory"/></item>
	/// <item><see cref="property.channel.ChannelsManager"/></item>
	/// <item><see cref="media.MediaFactory"/></item>
	/// <item><see cref="media.data.MediaDataFactory"/></item>
	/// <item><see cref="media.data.MediaDataManager"/></item>
	/// <item><see cref="media.data.FileDataProviderFactory"/></item>
	/// <item><see cref="media.data.FileDataProviderManager"/></item>
	/// </list>
	/// </summary>
	public class DataModelFactory
	{
		/// <summary>
		/// Creates a default <typeparamref name="T"/> object by calling <see cref="create{T}(string,string)"/> 
		/// using <c><see cref="ToolkitSettings.XUK_NS"/>:typeof(<typeparamref name="T"/>).Name</c> as Xuk QName
		/// </summary>
		/// <typeparam name="T">The object type to create</typeparam>
		/// <returns>The created <typeparamref name="T"/> instance</returns>
		private T create<T>() where T : class
		{
			return create<T>(typeof(T).Name, ToolkitSettings.XUK_NS);
		}

		/// <summary>
		/// Creates a <typeparamref name="T"/> matching a given Xuk QName
		/// </summary>
		/// <typeparam name="T">The object type to create</typeparam>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <typeparamref name="T"/> instance</returns>
		private T create<T>(string localName, string namespaceUri) where T : class
		{
			T res = null;
			if (namespaceUri==ToolkitSettings.XUK_NS && localName==typeof(T).Name)
			{
				foreach (ConstructorInfo ci in typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
				{
					if (ci.GetParameters().Length == 0)
					{
						res = (T)ci.Invoke(new object[0]);
					}
				}
			}
			return res;
		}

		/// <summary>
		/// Creates a <see cref="Presentation"/> of default type (that is <see cref="Presentation"/>
		/// </summary>
		/// <returns>The created <see cref="Presentation"/></returns>
		public virtual Presentation createPresentation()
		{
			return create<Presentation>();
		}

		/// <summary>
		/// Creates a <see cref="Presentation"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="Presentation"/></returns>
		public virtual Presentation createPresentation(string localName, string namespaceUri)
		{
			return create<Presentation>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="metadata.MetadataFactory"/> of default type (that is <see cref="metadata.MetadataFactory"/>
		/// </summary>
		/// <returns>The created <see cref="metadata.MetadataFactory"/></returns>
		public virtual metadata.MetadataFactory createMetadataFactory()
		{
			return create<metadata.MetadataFactory>();
		}

		/// <summary>
		/// Creates a <see cref="metadata.MetadataFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="metadata.MetadataFactory"/></returns>
		public virtual metadata.MetadataFactory createMetadataFactory(string localName, string namespaceUri)
		{
			return create<metadata.MetadataFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="undo.CommandFactory"/> of default type (that is <see cref="undo.CommandFactory"/>
		/// </summary>
		/// <returns>The created <see cref="undo.CommandFactory"/></returns>
		public virtual undo.CommandFactory createCommandFactory()
		{
			return create<undo.CommandFactory>();
		}

		/// <summary>
		/// Creates a <see cref="undo.CommandFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="undo.CommandFactory"/></returns>
		public virtual undo.CommandFactory createCommandFactory(string localName, string namespaceUri)
		{
			return create<undo.CommandFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="undo.UndoRedoManager"/> of default type (that is <see cref="undo.UndoRedoManager"/>
		/// </summary>
		/// <returns>The created <see cref="undo.UndoRedoManager"/></returns>
		public virtual undo.UndoRedoManager createUndoRedoManager()
		{
			return create<undo.UndoRedoManager>();
		}

		/// <summary>
		/// Creates a <see cref="undo.UndoRedoManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="undo.UndoRedoManager"/></returns>
		public virtual undo.UndoRedoManager createUndoRedoManager(string localName, string namespaceUri)
		{
			return create<undo.UndoRedoManager>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="core.TreeNodeFactory"/> of default type (that is <see cref="core.TreeNodeFactory"/>
		/// </summary>
		/// <returns>The created <see cref="core.TreeNodeFactory"/></returns>
		public virtual core.TreeNodeFactory createTreeNodeFactory()
		{
			return create<core.TreeNodeFactory>();
		}

		/// <summary>
		/// Creates a <see cref="core.TreeNodeFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="core.TreeNodeFactory"/></returns>
		public virtual core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
		{
			return create<core.TreeNodeFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="property.PropertyFactory"/> of default type (that is <see cref="property.PropertyFactory"/>
		/// </summary>
		/// <returns>The created <see cref="property.PropertyFactory"/></returns>
		public virtual property.PropertyFactory createPropertyFactory()
		{
			return create<property.PropertyFactory>();
		}

		/// <summary>
		/// Creates a <see cref="property.PropertyFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="property.PropertyFactory"/></returns>
		public virtual property.PropertyFactory createPropertyFactory(string localName, string namespaceUri)
		{
			return create<property.PropertyFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelFactory"/> of default type (that is <see cref="property.channel.ChannelFactory"/>
		/// </summary>
		/// <returns>The created <see cref="property.channel.ChannelFactory"/></returns>
		public virtual property.channel.ChannelFactory createChannelFactory()
		{
			return create<property.channel.ChannelFactory>();
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="property.channel.ChannelFactory"/></returns>
		public virtual property.channel.ChannelFactory createChannelFactory(string localName, string namespaceUri)
		{
			return create<property.channel.ChannelFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelsManager"/> of default type (that is <see cref="property.channel.ChannelsManager"/>
		/// </summary>
		/// <returns>The created <see cref="property.channel.ChannelsManager"/></returns>
		public virtual property.channel.ChannelsManager createChannelsManager()
		{
			return create<property.channel.ChannelsManager>();
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelsManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="property.channel.ChannelsManager"/></returns>
		public virtual property.channel.ChannelsManager createChannelsManager(string localName, string namespaceUri)
		{
			return create<property.channel.ChannelsManager>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.IMediaFactory"/> of default type (that is <see cref="media.MediaFactory"/>
		/// </summary>
		/// <returns>The created <see cref="media.MediaFactory"/></returns>
		public virtual media.IMediaFactory createMediaFactory()
		{
			return create<media.MediaFactory>();
		}

		/// <summary>
		/// Creates a <see cref="media.IMediaFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.IMediaFactory"/></returns>
		public virtual media.IMediaFactory createMediaFactory(string localName, string namespaceUri)
		{
			return create<media.MediaFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataFactory"/> of default type (that is <see cref="media.data.MediaDataFactory"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.MediaDataFactory"/></returns>
		public virtual media.data.MediaDataFactory createMediaDataFactory()
		{
			return create<media.data.MediaDataFactory>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.MediaDataFactory"/></returns>
		public virtual media.data.MediaDataFactory createMediaDataFactory(string localName, string namespaceUri)
		{
			return create<media.data.MediaDataFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataManager"/> of default type (that is <see cref="media.data.MediaDataManager"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.MediaDataManager"/></returns>
		public virtual media.data.MediaDataManager createMediaDataManager()
		{
			return create<media.data.MediaDataManager>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.MediaDataManager"/></returns>
		public virtual media.data.MediaDataManager createMediaDataManager(string localName, string namespaceUri)
		{
			return create<media.data.MediaDataManager>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderFactory"/> of default type (that is <see cref="media.data.FileDataProviderFactory"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.IDataProviderFactory"/></returns>
		public virtual media.data.IDataProviderFactory createDataProviderFactory()
		{
			return create<media.data.FileDataProviderFactory>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.IDataProviderFactory"/></returns>
		public virtual media.data.IDataProviderFactory createDataProviderFactory(string localName, string namespaceUri)
		{
			return create<media.data.FileDataProviderFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderManager"/> of default type (that is <see cref="media.data.FileDataProviderManager"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.IDataProviderManager"/></returns>
		public virtual media.data.IDataProviderManager createDataProviderManager()
		{
			return create<media.data.FileDataProviderManager>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.IDataProviderManager"/></returns>
		public virtual media.data.IDataProviderManager createDataProviderManager(string localName, string namespaceUri)
		{
			return create<media.data.FileDataProviderManager>(localName, namespaceUri);
		}
	}
}
