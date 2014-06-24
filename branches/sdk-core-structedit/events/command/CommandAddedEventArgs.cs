using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;

namespace urakawa.events.command
{
    ///// <summary>
    ///// Arguments of the <see cref="CompositeCommand.CommandAdded"/> event
    ///// </summary>
    //public class CommandAddedEventArgs : CommandEventArgs
    //{
    //    /// <summary>
    //    /// Constructor setting the source <see cref="CompositeCommand"/>, the addee <see cref="Command"/> 
    //    /// and the index at which the addee was added
    //    /// </summary>
    //    /// <param name="source">The source <see cref="CompositeCommand"/> of the evnent</param>
    //    /// <param name="addee">The <see cref="Command"/> that was added</param>
    //    /// <param name="indx">The index at which the <see cref="Command"/> was added</param>
    //    public CommandAddedEventArgs(CompositeCommand source, Command addee, int indx)
    //        : base(source)
    //    {
    //        SourceCompositeCommand = source;
    //        AddedCommand = addee;
    //        Index = indx;
    //    }

    //    /// <summary>
    //    /// The source <see cref="CompositeCommand"/> of the evnent
    //    /// </summary>
    //    public readonly CompositeCommand SourceCompositeCommand;

    //    /// <summary>
    //    /// The <see cref="Command"/> that was added
    //    /// </summary>
    //    public readonly Command AddedCommand;

    //    /// <summary>
    //    /// The index at which the <see cref="Command"/> was added
    //    /// </summary>
    //    public readonly int Index;
    //}
}