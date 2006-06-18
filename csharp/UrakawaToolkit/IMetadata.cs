using System;

namespace urakawa.project
{
	public interface IMetadata
	{
		void appendMetadata(object metadata);
		System.Collections.IList getMetadataList();
		System.Collections.IList getMetadataList(string name);
		void deleteMetadata(string name);
		void deleteMetadata(object metadata);
	}
}
