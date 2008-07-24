package org.daisy.urakawa.events.project;

import org.daisy.urakawa.Project;
import org.daisy.urakawa.events.DataModelChangedEvent;

/**
 *
 */
public class ProjectEvent extends DataModelChangedEvent
{
    /**
     * @param source
     */
    public ProjectEvent(Project source)
    {
        super(source);
        mSourceProject = source;
    }

    private Project mSourceProject;

    /**
     * @return project
     */
    public Project getSourceProject()
    {
        return mSourceProject;
    }
}
