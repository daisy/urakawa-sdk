using System;

namespace urakawa
{
	/// <summary>
	/// Provides a place holder for toolkit wide settings - class is not instanciable
	/// </summary>
	public class ToolkitSettings
	{
		/// <summary>
		/// Default namespace of XUK files
		/// </summary>
		public static string XUK_NS = "http://www.daisy.org/urakawa/xuk/0.5";

		/// <summary>
		/// Path of XUK Xml-Schema - leaving this member empty will produce XUK files with no schema location
		/// </summary>
		public static string XUK_XSD_PATH = "xuk.xsd";
	}
}
