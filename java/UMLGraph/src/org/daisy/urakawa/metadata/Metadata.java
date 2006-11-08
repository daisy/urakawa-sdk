package org.daisy.urakawa.metadata;

import org.daisy.urakawa.xuk.XukAble;

public interface Metadata extends XukAble {
    public String getName();

    public void setName(String name);

    public String getContent();

    public void setContent(String content);

    public String getScheme();

    public void setScheme(String scheme);
}
