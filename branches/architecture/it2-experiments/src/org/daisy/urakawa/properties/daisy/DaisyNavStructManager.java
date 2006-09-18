package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.NavStructAlreadyExistsException;
import org.daisy.urakawa.properties.core.NavStruct;
import org.daisy.urakawa.properties.core.NavStructManager;

/**
 * A NavStructManager aware of Daisy requirements.
 *
 * @depends - Composition 1 NavMap
 * @depends - Composition 0..1 PageList
 * @depends - Composition 0..n NavList
 */
public interface DaisyNavStructManager extends NavStructManager {

    /**
     * At most one PageList per presentation, exactly one NavMap per presentation.
     */
    void addNavStruct(NavStruct navStruct) throws MethodParameterIsNullException, NavStructAlreadyExistsException;
}
