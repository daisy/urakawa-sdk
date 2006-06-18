using System;

namespace urakawa.project
{
	/// <summary>
	/// this interface is to support basic project functions for the facade api
	/// </summary>
	public interface IProject
	{
		bool openXUK(Uri fileUri);
		bool saveXUK(Uri fileUri);
		urakawa.core.IPresentation getPresentation();
	}
}
