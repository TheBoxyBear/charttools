using System;

namespace ChartTools
{
    /// <summary>
    /// Marker that alters the tempo
    /// </summary>
    public class Tempo : TrackObjectBase
    {
        /// <inheritdoc cref="TrackObjectBase.Position" path="/summary"/>
        /// <remarks>If <see cref="Anchor"/> is not <see langword="null"/>, only refer to the position if <see cref="PositionSynced"/> is <see langword="true"/>.</remarks>
        public override uint Position
        {
            get => _position;
            set
            {
                _position = value;

                if (Anchor is not null)
                    PositionSynced = false;
            }
        }
        private uint _position;

        /// <summary>
        /// New tempo
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Locks the tempo to a specific time in the song independent to the sync track.
        /// </summary>
        public TimeSpan? Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;

                if (value is not null)
                    PositionSynced = false;
            }
        }
        private TimeSpan? _anchor;

        /// <summary>
        /// Indicates if the tick position is up to date with <see cref="Anchor"/>.
        /// </summary>
        /// <remarks><see langword="true"/> if the marker has no anchor.</remarks>
        public bool PositionSynced { get; private set; } = true;

        /// <summary>
        /// Creates an instance of <see cref="Tempo"/>.
        /// </summary>
        public Tempo(uint position, float value) : base(position) => Value = value;
        public Tempo(TimeSpan anchor, float value) : this(0, value) => Anchor = anchor;

        internal void SyncPosition(uint position)
        {
            _position = position;
            PositionSynced = true;
        }
    }
}
