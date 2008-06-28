using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using urakawa.command;

namespace urakawa
{
	/// <summary>
	/// Factory creating the data model objects, includes creator methods for:
	/// <list type="ul">
	/// <item><see cref="Presentation"/></item>
	/// <item><see cref="metadata.MetadataFactory"/></item>
	/// <item><see cref="CommandFactory"/></item>
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
		public virtual Presentation CreatePresentation()
		{
			return create<Presentation>();
		}

		/// <summary>
		/// Creates a <see cref="Presentation"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="Presentation"/></returns>
		public virtual Presentation CreatePresentation(string localName, string namespaceUri)
		{
			return create<Presentation>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="metadata.MetadataFactory"/> of default type (that is <see cref="metadata.MetadataFactory"/>
		/// </summary>
		/// <returns>The created <see cref="metadata.MetadataFactory"/></returns>
		public virtual metadata.MetadataFactory CreateMetadataFactory()
		{
			return create<metadata.MetadataFactory>();
		}

		/// <summary>
		/// Creates a <see cref="metadata.MetadataFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="metadata.MetadataFactory"/></returns>
		public virtual metadata.MetadataFactory CreateMetadataFactory(string localName, string namespaceUri)
		{
			return create<metadata.MetadataFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="CommandFactory"/> of default type (that is <see cref="CommandFactory"/>
		/// </summary>
		/// <returns>The created <see cref="CommandFactory"/></returns>
		public virtual CommandFactory CreateCommandFactory()
		{
			return create<CommandFactory>();
		}

		/// <summary>
		/// Creates a <see cref="CommandFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="CommandFactory"/></returns>
		public virtual CommandFactory CreateCommandFactory(string localName, string namespaceUri)
		{
			return create<CommandFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="undo.UndoRedoManager"/> of default type (that is <see cref="undo.UndoRedoManager"/>
		/// </summary>
		/// <returns>The created <see cref="undo.UndoRedoManager"/></returns>
		public virtual undo.UndoRedoManager CreateUndoRedoManager()
		{
			return create<undo.UndoRedoManager>();
		}

		/// <summary>
		/// Creates a <see cref="undo.UndoRedoManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="undo.UndoRedoManager"/></returns>
		public virtual undo.UndoRedoManager CreateUndoRedoManager(string localName, string namespaceUri)
		{
			return create<undo.UndoRedoManager>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="core.TreeNodeFactory"/> of default type (that is <see cref="core.TreeNodeFactory"/>
		/// </summary>
		/// <returns>The created <see cref="core.TreeNodeFactory"/></returns>
		public virtual core.TreeNodeFactory CreateTreeNodeFactory()
		{
			return create<core.TreeNodeFactory>();
		}

		/// <summary>
		/// Creates a <see cref="core.TreeNodeFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="core.TreeNodeFactory"/></returns>
		public virtual core.TreeNodeFactory CreateTreeNodeFactory(string localName, string namespaceUri)
		{
			return create<core.TreeNodeFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="property.PropertyFactory"/> of default type (that is <see cref="property.PropertyFactory"/>
		/// </summary>
		/// <returns>The created <see cref="property.PropertyFactory"/></returns>
		public virtual property.PropertyFactory CreatePropertyFactory()
		{
			return create<property.PropertyFactory>();
		}

		/// <summary>
		/// Creates a <see cref="property.PropertyFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="property.PropertyFactory"/></returns>
		public virtual property.PropertyFactory CreatePropertyFactory(string localName, string namespaceUri)
		{
			return create<property.PropertyFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelFactory"/> of default type (that is <see cref="property.channel.ChannelFactory"/>
		/// </summary>
		/// <returns>The created <see cref="property.channel.ChannelFactory"/></returns>
		public virtual property.channel.ChannelFactory CreateChannelFactory()
		{
			return create<property.channel.ChannelFactory>();
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="property.channel.ChannelFactory"/></returns>
		public virtual property.channel.ChannelFactory CreateChannelFactory(string localName, string namespaceUri)
		{
			return create<property.channel.ChannelFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelsManager"/> of default type (that is <see cref="property.channel.ChannelsManager"/>
		/// </summary>
		/// <returns>The created <see cref="property.channel.ChannelsManager"/></returns>
		public virtual property.channel.ChannelsManager CreateChannelsManager()
		{
			return create<property.channel.ChannelsManager>();
		}

		/// <summary>
		/// Creates a <see cref="property.channel.ChannelsManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="property.channel.ChannelsManager"/></returns>
		public virtual property.channel.ChannelsManager CreateChannelsManager(string localName, string namespaceUri)
		{
			return create<property.channel.ChannelsManager>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.IMediaFactory"/> of default type (that is <see cref="media.MediaFactory"/>
		/// </summary>
		/// <returns>The created <see cref="media.MediaFactory"/></returns>
		public virtual media.IMediaFactory CreateMediaFactory()
		{
			return create<media.MediaFactory>();
		}

		/// <summary>
		/// Creates a <see cref="media.IMediaFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.IMediaFactory"/></returns>
		public virtual media.IMediaFactory CreateMediaFactory(string localName, string namespaceUri)
		{
			return create<media.MediaFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataFactory"/> of default type (that is <see cref="media.data.MediaDataFactory"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.MediaDataFactory"/></returns>
		public virtual media.data.MediaDataFactory CreateMediaDataFactory()
		{
			return create<media.data.MediaDataFactory>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.MediaDataFactory"/></returns>
		public virtual media.data.MediaDataFactory CreateMediaDataFactory(string localName, string namespaceUri)
		{
			return create<media.data.MediaDataFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataManager"/> of default type (that is <see cref="media.data.MediaDataManager"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.MediaDataManager"/></returns>
		public virtual media.data.MediaDataManager CreateMediaDataManager()
		{
			return create<media.data.MediaDataManager>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.MediaDataManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.MediaDataManager"/></returns>
		public virtual media.data.MediaDataManager CreateMediaDataManager(string localName, string namespaceUri)
		{
			return create<media.data.MediaDataManager>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderFactory"/> of default type (that is <see cref="media.data.FileDataProviderFactory"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.IDataProviderFactory"/></returns>
		public virtual media.data.IDataProviderFactory CreateDataProviderFactory()
		{
			return create<media.data.FileDataProviderFactory>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.IDataProviderFactory"/></returns>
		public virtual media.data.IDataProviderFactory CreateDataProviderFactory(string localName, string namespaceUri)
		{
			return create<media.data.FileDataProviderFactory>(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderManager"/> of default type (that is <see cref="media.data.FileDataProviderManager"/>
		/// </summary>
		/// <returns>The created <see cref="media.data.IDataProviderManager"/></returns>
		public virtual media.data.IDataProviderManager CreateDataProviderManager()
		{
			return create<media.data.FileDataProviderManager>();
		}

		/// <summary>
		/// Creates a <see cref="media.data.IDataProviderManager"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="media.data.IDataProviderManager"/></returns>
		public virtual media.data.IDataProviderManager CreateDataProviderManager(string localName, string namespaceUri)
		{
			return create<media.data.FileDataProviderManager>(localName, namespaceUri);
		}
	}
}
