package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.media.AudioMedia;
import org.daisy.urakawa.media.ImageMedia;
import org.daisy.urakawa.media.TextMedia;
import org.daisy.urakawa.media.VideoMedia;

/**
 * Describes a multimedia, localized label.
 *
 * @depend - Aggregation 0..1 AudioMedia
 * @depend - Aggregation 0..1 ImageMedia
 * @depend - Aggregation 0..1 TextMedia
 * @depend - Aggregation 0..1 VideoMedia
 */
public interface MediaLabel {
    /**
     * Returns the localization identifier of this channel.
     *
     * @return the localization identifier of this channel.
     */
    String getLanguage();

    /**
     * @return The audio media object describing this label.
     */
    AudioMedia getAudio();

    /**
     * Sets the audio media object describing this label.
     *
     * @param audio the new audio media object describing this label.
     */
    void setAudio(AudioMedia audio);

    /**
     * @return The image media object describing this label.
     */
    ImageMedia getImage();

    /**
     * Sets the image  media object describing this label.
     *
     * @param  image the new image media object describing this label.
     */
    void setImage(ImageMedia image);

    /**
     * @return The text media object describing this label.
     */
    TextMedia getText();

    /**
     * Sets the text media object describing this label.
     *
     * @param text the new text media object describing this label.
     */
    void setText(TextMedia text);

    /**
     * @return The video media object describing this label.
     */
    VideoMedia getVideo();

    /**
     * Sets the video media object describing this label.
     *
     * @param video the new video media object describing this label.
     */
    void setVideo(VideoMedia video);
}
