using System;
using System.Collections.Generic;
using System.Text;
using urakawa.xuk;

namespace urakawa.exception
{
    
    /// <summary>
    /// Exception thrown when a factory encounteres two <see cref="XukAble"/> object <see cref="Type"/>s
    /// reporting the same Xuk QName
    /// </summary>
    public class XukQNameNotUniqueWithinFactoryException : CheckedException
    {
        /// <summary>
        /// Constructor setting the QName and <see cref="XukAble"/> object <see cref="Type"/>s of the exception
        /// </summary>
        /// <param name="qn">The Xuk QName reported by both <see cref="XukAble"/> object <see cref="Type"/>s</param>
        /// <param name="fstTp">The first <see cref="XukAble"/> object <see cref="Type"/>s</param>
        /// <param name="secTp">The second <see cref="XukAble"/> object <see cref="Type"/>s</param>
        public XukQNameNotUniqueWithinFactoryException(string qn, Type fstTp, Type secTp)
            : this(String.Format("XukAble Types {1} and {2} both report QName {0}", qn, fstTp.FullName, secTp.FullName), qn, fstTp, secTp)
        {
            
        }


        /// <summary>
        /// Constructor setting the message, QName and <see cref="XukAble"/> object <see cref="Type"/>s of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="qn">The Xuk QName reported by both <see cref="XukAble"/> object <see cref="Type"/>s</param>
        /// <param name="fstTp">The first <see cref="XukAble"/> object <see cref="Type"/>s</param>
        /// <param name="secTp">The second <see cref="XukAble"/> object <see cref="Type"/>s</param>
        public XukQNameNotUniqueWithinFactoryException(string msg, string qn, Type fstTp, Type secTp)
            : base(msg)
        {
            QName = qn;
            FirstType = fstTp;
            SecondType = secTp;
        }

        /// <summary>
        /// The Xuk QName reported by both <see cref="XukAble"/> object <see cref="Type"/>s
        /// </summary>
        public readonly string QName;
        /// <summary>
        /// The first <see cref="XukAble"/> object <see cref="Type"/>s
        /// </summary>
        public readonly Type FirstType;
        /// <summary>
        /// The second <see cref="XukAble"/> object <see cref="Type"/>s
        /// </summary>
        public readonly Type SecondType;
    }

}
