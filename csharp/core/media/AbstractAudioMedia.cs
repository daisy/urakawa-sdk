using urakawa.media.timing;

namespace urakawa.media
{
    /// <summary>
    /// Interface for audio <see cref="Media"/> which is <see cref="IContinuous"/>
    /// </summary>
    public abstract class AbstractAudioMedia : Media, IContinuous
    {

        /// <summary>
        /// This always returns true, because
        /// audio media is always considered continuous
        /// </summary>
        /// <returns></returns>
        public override bool IsContinuous
        {
            get { return true; }
        }

        /// <summary>
        /// This always returns false, because
        /// audio media is never considered discrete
        /// </summary>
        /// <returns></returns>
        public override bool IsDiscrete
        {
            get { return false; }
        }

        /// <summary>
        /// This always returns false, because
        /// a single media object is never considered to be a sequence
        /// </summary>
        /// <returns></returns>
        public override bool IsSequence
        {
            get { return false; }
        }
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AbstractAudioMedia otherz = other as AbstractAudioMedia;
            if (otherz == null)
            {
                return false;
            }

            if (!Duration.IsEqualTo(otherz.Duration))
            {
                return false;
            }

            return true;
        }

        #region Implementation of IContinuous

        /// <summary>
        /// Gets the duration of the media
        /// </summary>
        /// <returns>The duration</returns>
        public abstract Time Duration { get; }

        IContinuous IContinuous.Split(Time splitPoint)
        {
            return Split(splitPoint);
        }

        /// <summary>
        /// Splits <c>this</c> at a given <see cref="Time"/>
        /// </summary>
        /// <param name="splitPoint">The <see cref="Time"/> at which to split - 
        /// must be between clip begin and clip end <see cref="Time"/>s</param>
        /// <returns>
        /// A newly created <see cref="AbstractAudioMedia"/> containing the audio after <paramref localName="splitPoint"/>,
        /// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="splitPoint"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
        /// </exception>
        public AbstractAudioMedia Split(Time splitPoint)
        {
            return SplitProtected(splitPoint);
        }

        /// <summary>
        /// Splits <c>this</c> at a given <see cref="Time"/>
        /// </summary>
        /// <param name="splitPoint">The <see cref="Time"/> at which to split - 
        /// must be between clip begin and clip end <see cref="Time"/>s</param>
        /// <returns>
        /// A newly created <see cref="AbstractAudioMedia"/> containing the audio after <paramref localName="splitPoint"/>,
        /// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="splitPoint"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
        /// </exception>
        protected abstract AbstractAudioMedia SplitProtected(Time splitPoint);

        #endregion
    }
}