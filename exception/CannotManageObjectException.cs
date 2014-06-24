using System;

namespace urakawa.exception
{
    public class CannotManageObjectException : CheckedException
    {
        public CannotManageObjectException(string msg) : base(msg)
        {
        }

        public CannotManageObjectException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}
