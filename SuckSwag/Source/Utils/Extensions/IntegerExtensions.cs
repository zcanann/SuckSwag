namespace SuckSwag.Source.Utils.Extensions
{
    using System;

    /// <summary>
    /// Contains extension methods for integers.
    /// </summary>
    internal static class IntegerExtensions
    {
        /// <summary>
        /// Converts the given integer to a <see cref="Int32"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="Int32"/>.</returns>
        public static Int32 ToInt32(this Int64 integer)
        {
            return unchecked((Int32)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="Int32"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="Int32"/>.</returns>
        public static Int32 ToInt32(this UInt64 integer)
        {
            return unchecked((Int32)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="UInt32"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="UInt32"/>.</returns>
        public static UInt32 ToUInt32(this Int64 integer)
        {
            return unchecked((UInt32)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="UInt32"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="UInt32"/>.</returns>
        public static UInt32 ToUInt32(this UInt64 integer)
        {
            return unchecked((UInt32)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="UInt64"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="UInt64"/>.</returns>
        public static UInt64 ToUInt64(this Int64 integer)
        {
            return unchecked((UInt64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="UInt64"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="UInt64"/>.</returns>
        public static UInt64 ToUInt64(this Int32 integer)
        {
            return unchecked((UInt64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="UInt64"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="UInt64"/>.</returns>
        public static UInt64 ToUInt64(this UInt32 integer)
        {
            return unchecked((UInt64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="Int64"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="Int64"/>.</returns>
        public static Int64 ToInt64(this UInt64 integer)
        {
            return unchecked((Int64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="Int64"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="Int64"/>.</returns>
        public static Int64 ToInt64(this Int32 integer)
        {
            return unchecked((Int64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="Int64"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="Int64"/>.</returns>
        public static Int64 ToInt64(this UInt32 integer)
        {
            return unchecked((Int64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static IntPtr ToIntPtr(this Int64 integer)
        {
            return unchecked((IntPtr)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static UIntPtr ToUIntPtr(this Int64 integer)
        {
            return unchecked((UIntPtr)(UInt64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static IntPtr ToIntPtr(this UInt64 integer)
        {
            return unchecked((IntPtr)(Int64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static UIntPtr ToUIntPtr(this UInt64 integer)
        {
            return unchecked((UIntPtr)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static IntPtr ToIntPtr(this Int32 integer)
        {
            return unchecked((IntPtr)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static UIntPtr ToUIntPtr(this Int32 integer)
        {
            return unchecked((UIntPtr)(UInt64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static IntPtr ToIntPtr(this UInt32 integer)
        {
            return unchecked((IntPtr)(Int64)integer);
        }

        /// <summary>
        /// Converts the given integer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <returns>The integer converted to a <see cref="IntPtr"/>.</returns>
        public static UIntPtr ToUIntPtr(this UInt32 integer)
        {
            return unchecked((UIntPtr)integer);
        }
    }
    //// End class
}
//// End namespace