package org.daisy.urakawa.event.project;

import org.daisy.urakawa.Project;
import org.daisy.urakawa.event.DataModelChangedEvent;

/**
 *
 */
public class ProjectEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public ProjectEvent(Project source) {
		super(source);
		mSourceProject = source;
	}

	private Project mSourceProject;

	/**
	 * @return project
	 */
	public Project getSourceProject() {
		return mSourceProject;
	}
}
