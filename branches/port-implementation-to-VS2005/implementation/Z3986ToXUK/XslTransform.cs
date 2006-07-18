using System;
using System.Diagnostics;
using System.IO;
using MSXML2;

namespace Z3986ToXUK
{

	/// <summary>
	/// Summary description for XslTransform.
	/// </summary>
	public class XslTransform
	{

////    UINT GetTempFileName(
//      LPCTSTR lpPathName,
//      LPCTSTR lpPrefixString,
//      UINT uUnique,
//      LPTSTR lpTempFileName
//      );
    [System.Runtime.InteropServices.DllImport("kernel32", SetLastError=true)]
    public static extern bool GetTempFileName(
      string lpPathName,
      string lpPrefixString,
      uint  uUnique,
      System.Text.StringBuilder lpTempFileName);

		public XslTransform(string xslPath, string sourcePath)
		{
      _sourcePath = Path.GetFullPath(sourcePath);
      System.Text.StringBuilder sb = new System.Text.StringBuilder(255);
      string sourceDir = Path.GetDirectoryName(Path.GetFullPath(_sourcePath));
      if (!GetTempFileName(sourceDir, "temp", 0, sb))
      {
        throw new ApplicationException(String.Format(
          "Could not get temp file name for XSLT in directory {0}",
          sourceDir));
      }
      _xslTempPath = sb.ToString();
      File.Copy(xslPath, _xslTempPath, true);
		}

    ~ XslTransform() 
    {
      File.Delete(_xslTempPath);
    }


    private System.Collections.Specialized.StringDictionary _params 
      = new System.Collections.Specialized.StringDictionary();

    public void SetParameter(string name, string value)
    {
      _params.Add(name, value);
    }

    private string _xslTempPath;
    private string _sourcePath;

    public string Transform()
    {
      FreeThreadedDOMDocument40 xslDoc = new FreeThreadedDOMDocument40Class();
      xslDoc.async = false;
      xslDoc.resolveExternals = false;
      xslDoc.load(_xslTempPath);
      XSLTemplate40 xsl = new XSLTemplate40Class();
      xsl.stylesheet = xslDoc;
      IXSLProcessor proc = xsl.createProcessor();
      DOMDocument40 source = new DOMDocument40Class();
      source.async = false;
      source.load(_sourcePath);
      proc.input = source;
      DOMDocument40 output = new DOMDocument40Class();
      //output.resolveExternals = false;
      proc.output = output;
      if (!proc.transform())
      {
        throw new ApplicationException("Could not perform XSLT transform");
      }
      return output.xml;
    }
	}
}
