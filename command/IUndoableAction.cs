namespace urakawa.command
{
    public interface IUndoableAction : IAction
    {
        /// <summary>
        /// Execute the reverse action.
        /// </summary>
        void UnExecute();

        /// <summary>
        /// True if the action is reversible.
        /// </summary>
        bool CanUnExecute { get; }
    }
}
