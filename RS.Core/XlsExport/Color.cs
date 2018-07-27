using System;
using System.Collections.Generic;
using System.Text;
using RS.Xls.ByteUtil;

namespace RS.Xls
{
    /// <summary>
    /// Represents an RGB color value.  Use the values in Colors.  Custom colors are not yet supported.
    /// </summary>
    public class Color : ICloneable
    {
        internal byte Red;
        internal byte Green;
        internal byte Blue;
        internal ushort? Id;

        internal Color(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Id = null;
        }

        internal Color(ushort id)
        {
            Red = 0x00;
            Green = 0x00;
            Blue = 0x00;
            Id = id;
        }

        public Color(byte red, byte green, byte blue, ushort? id) : this(red, green, blue)
        {
            Id = id;
        }

        /// <summary>
        /// Determines whether this Color is equal to the specifed Color.
        /// </summary>
        /// <param name="obj">The Color to compare to this Color.</param>
        /// <returns>true if this Color is Equal to that Color, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            Color that = (Color) obj;
            if (this.Id != null || that.Id != null)
                return this.Id == that.Id;
            else
                return (this.Red == that.Red &&
                        this.Green == that.Green &&
                        this.Blue == that.Blue);
        }

        /// <summary>
        /// Returns a new Color instance Equal to this Color.
        /// </summary>
        /// <returns>A new Color instance Equal to this Color.</returns>
        public object Clone()
        {
            Color clone = new Color(Red, Green, Blue)
            {
                Id = Id
            };
            return clone;
        }

        public override int GetHashCode()
        {
            var hashCode = 153417133;
            hashCode = hashCode * -1521134295 + Red.GetHashCode();
            hashCode = hashCode * -1521134295 + Green.GetHashCode();
            hashCode = hashCode * -1521134295 + Blue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(Id);
            return hashCode;
        }
    }
}
