using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for Presentation.
	/// </summary>
	public class Presentation : ICoreNodeFactory
	{
    /// <summary>
    /// Default constructor
    /// </summary>
		public Presentation()
		{
			// Test change
			// TODO: Add constructor logic here
			//
    }
    #region ICoreNodeFactory Members

    /// <summary>
    /// Creates a new <see cref="CoreNode"/> instance, that is owner by the <see cref="Presentation"/>,
    /// but not connected to the core tree
    /// </summary>
    /// <returns>The new <see cref="CoreNode"/></returns>
    public ICoreNode createNode()
    {
      return null;//new CoreNode(this);
    }

    #endregion
  }
}
