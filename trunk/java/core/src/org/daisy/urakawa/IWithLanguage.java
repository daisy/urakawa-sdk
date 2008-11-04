package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;

/**
 * <p>
 * Getting and Setting a language.
 * </p>
 */
public interface IWithLanguage
{
    /**
     * The language (e.g. "en-US")
     * 
     * @param lang
     *        can be null, but cannot be empty String
     * @tagvalue Events "LanguageChanged"
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     */
    public void setLanguage(String lang)
            throws MethodParameterIsEmptyStringException;

    /**
     * The language (e.g. "en-US")
     * 
     * @return can return null but not empty string, by contract.
     */
    public String getLanguage();
}
