using System;

namespace urakawa.project
{
	public interface IProject
	{
		bool openXUK(Uri fileUri);
		bool saveXUK(Uri fileUri);
		urakawa.core.IPresentation getPresentation();
	}
}
