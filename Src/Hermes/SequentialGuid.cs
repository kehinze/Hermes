﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes
{
    /// <summary>
    /// An identity generator that creates sequential <see cref="System.Guid"/> values 
    /// using a strategy suggested Jimmy Nilsson's 
    /// <a href="http://www.informit.com/articles/article.asp?p=25862">article</a>
    /// on <a href="http://www.informit.com">informit.com</a>. 
    /// </summary>
    /// <remarks>
    /// <p>
    /// The <c>comb</c> algorithm is designed to make the use of GUIDs as Primary Keys, Foreign Keys, 
    /// and Indexes nearly as efficient as ints.
    /// </p>
    /// </remarks>
    [DebuggerNonUserCode]
    public static class SequentialGuid
    {
        private static int counter;

        [DebuggerNonUserCode]
        private static long GetTicks()
        {
            int i = Interlocked.Increment(ref counter);
            // use UTC now to prevent conflicts w/ daylight savings
            return DateTime.UtcNow.Ticks + i;
        }

        /// <summary>
        /// Creates a new sequential guid (aka comb) <see cref="http://www.informit.com/articles/article.aspx?p=25862&seqNum=7"/>.
        /// </summary>
        /// <remarks>A comb provides the benefits of a standard Guid w/o the database performance problems.</remarks>
        /// <returns>A new sequential guid (comb).</returns>
        [DebuggerNonUserCode]
        public static Guid New()
        {
            byte[] uid = Guid.NewGuid().ToByteArray();
            byte[] binDate = BitConverter.GetBytes(GetTicks());

            // create comb in SQL Server sort order
            return new Guid(new[]
            {
                // the first 7 bytes are random - if two combs
                // are generated at the same point in time
                // they are not guaranteed to be sequential.
                // But for every DateTime.Tick there are
                // 72,057,594,037,927,935 unique possibilities so
                // there shouldn't be any collisions                
                uid[0],
                uid[1],
                uid[2],
                uid[3],
                uid[4],
                uid[5],
                uid[6],

                // set the first 'nibble of the 7th byte to '1100' so 
                // later we can validate it was generated by us
                (byte)(0xc0 | (0xf & uid[7])),

                // the last 8 bytes are sequential,
                // these will reduce index fragmentation
                // to a degree as long as there are not a large
                // number of Combs generated per millisecond                
                binDate[1],
                binDate[0],
                binDate[7],
                binDate[6],
                binDate[5],
                binDate[4],
                binDate[3],
                binDate[2]
            });
        }

        /// <summary>
        /// Validates if comb was generated by this class
        /// </summary>
        /// <remarks>
        /// Guids generated by Guid.NewGuid() have a value of 
        /// 0100 for the first 4 bits of the 7th byte. Ours will
        /// have a value of 1100 for the 6th byte. We're checking that here.
        /// 
        /// We could do additional validation by verifying that
        /// the value of a new Guid is greater than the
        /// one being validated (or that the last 6 bytes
        /// resolve to a valid DateTime), but this should
        /// be enough for now.
        /// </remarks>
        [DebuggerNonUserCode]
        public static bool IsSequentialGuid(Guid value)
        {
            // get the 7th byte
            byte b = value.ToByteArray()[7];

            // make sure the first 'nibble' == 1100
            return (0xc0 & b) == 0xc0;
        }

        /// <summary>
        /// Validates Guid to determine the supplied
        /// value was generated by CombGuid.NewCombGuid. If
        /// invalid an ArgumentException is thrown.
        /// </summary>
        /// <param name="value"></param>
        [DebuggerNonUserCode]
        public static void ValidateSequentialGuid(Guid value)
        {
            if (!IsSequentialGuid(value))
                throw new ArgumentException("The supplied Id value was not generated by IdentityFactory.NewComb()");
        }
    }
}
