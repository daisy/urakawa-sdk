package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.media.AudioMedia;
import org.daisy.urakawa.media.ImageMedia;
import org.daisy.urakawa.media.TextMedia;
import org.daisy.urakawa.media.VideoMedia;

/**
 * @depend - Aggregation 0..1 AudioMedia
 * @depend - Aggregation 0..1 ImageMedia
 * @depend - Aggregation 0..1 TextMedia
 * @depend - Aggregation 0..1 VideoMedia
 */
public interface MediaLabel {
    String getLanguage();
    AudioMedia getAudio();

    void setAudio(AudioMedia audio);

    ImageMedia getImage();

    void setImage(ImageMedia image);

    TextMedia getText();

    void setText(TextMedia text);

    VideoMedia getVideo();

    void setVideo(VideoMedia video);
}
