﻿using ChartTools.Exceptions;

using System;
using System.Reflection;

namespace ChartTools
{
    /// <summary>
    /// Stores the estimated difficulties for instruments
    /// </summary>
    public class InstrumentDifficultySet
    {
        /// <summary>
        /// Difficulty of <see cref="InstrumentIdentity.LeadGuitar"/>, <see cref="InstrumentIdentity.CoopGuitar"/> and <see cref="InstrumentIdentity.RhythmGuitar"/>
        /// </summary>
        public sbyte? Guitar { get; set; }
        /// <summary>
        /// Difficulty of <see cref="InstrumentIdentity.Bass"/>
        /// </summary>
        public sbyte? Bass { get; set; }
        /// <summary>
        /// Difficulty of <see cref="InstrumentIdentity.Drums"/>
        /// </summary>
        public sbyte? Drums { get; set; }
        /// <summary>
        /// Difficulty of <see cref="InstrumentIdentity.Keys"/>
        /// </summary>
        public sbyte? Keys { get; set; }
        /// <summary>
        /// Difficulty of <see cref="InstrumentIdentity.GHLGuitar"/>
        /// </summary>
        public sbyte? GHLGuitar { get; set; }
        /// <summary>
        /// Difficulty of <see cref="InstrumentIdentity.GHLBass"/>
        /// </summary>
        public sbyte? GHLBass { get; set; }

        /// <summary>
        /// Gets the difficulty for an <see cref="InstrumentIdentity"/>.
        /// </summary>
        public sbyte? GetDifficulty(InstrumentIdentity identity) => GetDifficultyProperty(identity, out var info) ? (sbyte?)info!.GetValue(this) : null;
        /// <summary>
        /// Sets the difficulty for an <see cref="InstrumentIdentity"/>.
        /// </summary>
        public void SetDifficulty(InstrumentIdentity identity, sbyte? difficulty)
        {
            if (GetDifficultyProperty(identity, out var info))
                info!.SetValue(this, difficulty);
        }

        private bool GetDifficultyProperty(InstrumentIdentity identity, out PropertyInfo? info)
        {
            Validator.ValidateEnum(identity);
            var propName = identity switch
            {
                InstrumentIdentity.LeadGuitar or InstrumentIdentity.CoopGuitar or InstrumentIdentity.RhythmGuitar => nameof(Guitar),
                InstrumentIdentity.Bass => nameof(Bass),
                InstrumentIdentity.Drums => nameof(Drums),
                InstrumentIdentity.Keys => nameof(Keys),
                InstrumentIdentity.GHLGuitar => nameof(GHLGuitar),
                InstrumentIdentity.GHLBass => nameof(GHLBass),
                _ => null
            };

            if (propName is null)
            {
                info = null;
                return false;
            }

            info = typeof(InstrumentDifficultySet).GetProperty(propName);

            return true;
        }
    }
}
