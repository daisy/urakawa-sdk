using System;

namespace urakawa.exception
{
    public class ObjectIsAlreadyManagedException : CheckedException
    {
        public ObjectIsAlreadyManagedException(string msg) : base(msg)
        {
        }

        public ObjectIsAlreadyManagedException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}
