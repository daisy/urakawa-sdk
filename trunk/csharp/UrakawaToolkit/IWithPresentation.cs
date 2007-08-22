using System;
namespace urakawa
{
	public interface IWithPresentation
	{
		Presentation getPresentation();
		void setPresentation(Presentation newPres);
	}
}
