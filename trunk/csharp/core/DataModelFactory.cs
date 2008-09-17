using System.Reflection;
using urakawa.xuk;

namespace urakawa
{
    /// <summary>
    /// Factory creating <see cref="Presentation"/>s
    /// </summary>
    public class DataModelFactory
    {
        /// <summary>
        /// Creates a default <typeparamref name="T"/> object by calling <see cref="Create{T}(string,string)"/> 
        /// using <c><see cref="XukAble.XUK_NS"/>:typeof(<typeparamref name="T"/>).Name</c> as Xuk QName
        /// </summary>
        /// <typeparam name="T">The object type to create</typeparam>
        /// <returns>The created <typeparamref name="T"/> instance</returns>
        private static T Create<T>() where T : class
        {
            return Create<T>(typeof (T).Name, XukAble.XUK_NS);
        }

        /// <summary>
        /// Creates a <typeparamref name="T"/> matching a given Xuk QName
        /// </summary>
        /// <typeparam name="T">The object type to create</typeparam>
        /// <param name="localName">The local name part of the given Xuk QName</param>
        /// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
        /// <returns>The created <typeparamref name="T"/> instance</returns>
        private static T Create<T>(string localName, string namespaceUri) where T : class
        {
            T res = null;
            if (namespaceUri == XukAble.XUK_NS && localName == typeof (T).Name)
            {
                foreach (
                    ConstructorInfo ci in
                        typeof (T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    )
                {
                    if (ci.GetParameters().Length == 0)
                    {
                        res = (T) ci.Invoke(new object[0]);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Creates a <see cref="Presentation"/> of default type (that is <see cref="Presentation"/>
        /// </summary>
        /// <returns>The created <see cref="Presentation"/></returns>
        public virtual Presentation Create()
        {
            return Create<Presentation>();
        }

        /// <summary>
        /// Creates a <see cref="Presentation"/> of type matching a given Xuk QName
        /// </summary>
        /// <param name="localName">The local name part of the given Xuk QName</param>
        /// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
        /// <returns>The created <see cref="Presentation"/></returns>
        public virtual Presentation Create(string localName, string namespaceUri)
        {
            return Create<Presentation>(localName, namespaceUri);
        }

    }
}