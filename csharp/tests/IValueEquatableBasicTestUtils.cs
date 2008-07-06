using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa
{
    public static class IValueEquatableBasicTestUtils
    {
        /// <summary>
        /// Performs basic tests on a <see cref="IValueEquatable{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the object to test</typeparam>
        /// <param name="o1">An instance of <see cref="IValueEquatable{T}"/></param>
        /// <param name="o2">Another instance of <see cref="IValueEquatable{T}"/></param>
        /// <param name="o3">A third instance of <see cref="IValueEquatable{T}"/></param>
        public static void ValueEquals_BasicTests<T>(T o1, T o2, T o3) where T : class, IValueEquatable<T>
        {
            Assert.IsNotNull(o1, "The parameter o1 may not be null");
            Assert.IsNotNull(o2, "The parameter o2 may not be null");
            Assert.IsNotNull(o3, "The parameter o3 may not be null");
            T oNull = null;
            Assert.IsFalse(o1.ValueEquals(oNull), "An instance of IValueEquatable<T> must not be value equal to null");
            Assert.AreEqual(o1.ValueEquals(o2), o2.ValueEquals(o1), "ValueEquals must be symmetric");
            Assert.AreEqual(o1.ValueEquals(o3), o3.ValueEquals(o1), "ValueEquals must be symmetric");
            Assert.AreEqual(o2.ValueEquals(o3), o3.ValueEquals(o2), "ValueEquals must be symmetric");
            Assert.IsTrue(o1.ValueEquals(o1), "An instance of IValueEquatable<T> is value equal to itself");
            Assert.IsTrue(o2.ValueEquals(o2), "An instance of IValueEquatable<T> is value equal to itself");
            Assert.IsTrue(o3.ValueEquals(o3), "An instance of IValueEquatable<T> is value equal to itself");
            valueEquals_TransitiveTests<T>(o1, o2, o3);
            valueEquals_TransitiveTests<T>(o1, o3, o2);
            valueEquals_TransitiveTests<T>(o2, o1, o3);
            valueEquals_TransitiveTests<T>(o2, o3, o1);
            valueEquals_TransitiveTests<T>(o3, o1, o2);
            valueEquals_TransitiveTests<T>(o3, o2, o1);
        }

        private static void valueEquals_TransitiveTests<T>(T o1, T o2, T o3) where T : class, IValueEquatable<T>
        {
            bool t12 = o1.ValueEquals(o2);
            bool t23 = o2.ValueEquals(o3);
            bool t13 = o1.ValueEquals(o3);
            if (t12 && t23) Assert.IsTrue(t13, "ValueEquals must be transitive");
            if (!t13) Assert.IsFalse(t12 && t23, "ValueEquals must be transitive");
        }
    }
}