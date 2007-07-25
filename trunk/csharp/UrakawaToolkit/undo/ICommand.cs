using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.undo
{
	/// <summary>
	/// Classes realizing this interface must store the state of the object(s) affected by the command
	/// execution (including exception/redo). Implementations may choose various techniques suitable in terms
	/// of performance and memory usage (storage of the transition or the full object snapshot.)
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Execute the reverse command.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotUndoException">Thrown when the command cannot be reversed.</exception>
		void unExecute();

		/// <summary>
		/// Get a long uman-readable description of the command
		/// </summary>
		/// <exception cref="urakawa.exception.CannotUndoException">Thrown when the command cannot be reversed.</exception>
		string getLongDescription();

		/// <summary>
		/// Execute the command.
		/// </summary>
		void execute();

		/// <summary>
		/// Gets a short humanly readable description of the command
		/// </summary>
		string getShortDescription();

		/// <summary>
		/// True if the command is reversible.
		/// </summary>
		bool canUnExecute();

		/// <summary>
		/// Gets a list of the <see cref="media.data.MediaData"/> used by the Command
		/// </summary>
		/// <returns></returns>
		List<media.data.MediaData> getListOfUsedMediaData();
	}
}
