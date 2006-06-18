using System;

namespace urakawa.project
{
	/// <summary>
	/// this interface is to represent metadata access for the facade api
	/// </summary>
	public interface IMetadata
	{
		void appendMetadata(object metadata);
		System.Collections.IList getMetadataList();
		System.Collections.IList getMetadataList(string name);
		void deleteMetadata(string name);
		void deleteMetadata(object metadata);
	}
}
