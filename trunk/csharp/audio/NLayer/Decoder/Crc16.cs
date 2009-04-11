/*
 * Jan 2009     Ported to C# Mark Heath
 * 11/19/04 : 1.0 moved to LGPL.
 * 
 * 02/12/99 : Java Conversion by E.B , javalayer@javazoom.net
 *
 *  @(#) crc.h 1.5, last edit: 6/15/94 16:55:32
 *  @(#) Copyright (C) 1993, 1994 Tobias Bading (bading@cs.tu-berlin.de)
 *  @(#) Berlin University of Technology
 *-----------------------------------------------------------------------
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU Library General Public License as published
 *   by the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Library General Public License for more details.
 *
 *   You should have received a copy of the GNU Library General Public
 *   License along with this program; if not, write to the Free Software
 *   Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 *----------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLayer.Decoder
{
    /// <summary>
    /// 16 bit CRC checksum
    /// </summary>
    public sealed class Crc16
    {
        private static ushort polynomial = (ushort)0x8005;
        private ushort crc;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Crc16()
        {
            crc = (ushort)0xFFFF;
        }

        /// <summary>
        /// Feed a bitstring to the crc calculation (0 &lt; length &lt;= 32).
        /// </summary>
        public void add_bits(int bitstring, int length)
        {
            uint bitmask = (uint)(1 << (length - 1));
            do
                if (((crc & 0x8000) == 0) ^ ((bitstring & bitmask) == 0))
                {
                    crc <<= 1;
                    crc ^= polynomial;
                }
                else
                    crc <<= 1;
            while ((bitmask >>= 1) != 0);
        }

        /// <summary>
        /// Return the calculated checksum.
        /// Erase it for next calls to add_bits().
        /// </summary>
        public short checksum()
        {
            ushort sum = crc;
            crc = (ushort)0xFFFF;
            return (short)sum;
        }
    }
}
