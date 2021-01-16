﻿using System;
using System.Xml.Serialization;
using System.IO;
using System.Text;


namespace PloppableRICO
{
    /// <summary>
    /// RICO configuration file reader.
    /// </summary>
    public class RICOReader
    {

        /// <summary>
        /// Loads and parses the given RICO file.
        /// </summary>
        /// <param name="packageName">Package name</param>
        /// <param name="ricoDefPath">Definition file path</param>
        /// <param name="isLocal">True if this is a local settings file, false for author settings file</param>
        /// <returns>Parsed Ploppable RICO definition file</returns>
        public static PloppableRICODefinition ParseRICODefinition(string packageName, string ricoDefPath, bool isLocal = false)
        {
            // Note here we're using insanityOK as a local settings flag.
            string localOrAuthor = isLocal ? "local" : "author";

            try
            {
                // Open file.
                using (StreamReader reader = new StreamReader(ricoDefPath))
                {
                    // Create new XML (de)serializer
                    XmlAttributes attrs = new XmlAttributes();
                    XmlElementAttribute attr = new XmlElementAttribute();
                    attr.ElementName = "RICOBuilding";
                    attr.Type = typeof(RICOBuilding);
                    XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
                    attrOverrides.Add(typeof(RICOBuilding), "Building", attrs);

                    // Read XML.
                    var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition), attrOverrides);
                    var result = xmlSerializer.Deserialize(reader) as PloppableRICODefinition;

                    StringBuilder errorList;

                    if (result.Buildings.Count == 0)
                    {
                        Logging.Message("no parseable buildings in ", localOrAuthor, " XML settings file at ", ricoDefPath);
                    }
                    else
                    {
                        foreach (var building in result.Buildings)
                        {
                            // Check for fatal errors in each building.
                            errorList = building.fatalErrors;
                            if (errorList.Length == 0)
                            {
                                // No fatal errors; check for non-fatal errors.
                                errorList = building.nonFatalErrors;

                                if (errorList.Length != 0)
                                {
                                    if (isLocal && building.ricoEnabled)
                                    {
                                        // Errors in local settings need to be reported, except for buildings that aren't activated in RICO (e.g. for when the user has de-activated a RICO builidng with issues).
                                        Logging.Error("non-fatal errors for building '", building.name, "' in local settings:\r\n", errorList.ToString());
                                    }
                                    else
                                    {
                                        // Errors in other settings should be logged if verbose logging is enabled, but otherwise continue.
                                        Logging.Message("non-fatal errors for building '", building.name, "' in author settings:\r\n", errorList.ToString());
                                    }
                                }
                            }
                            else
                            {
                                // Fatal errors!  Need to be reported direct to user and the building ignored.
                                Logging.Error("fatal errors for building '", building.name, "' in ", localOrAuthor, " settings:\r\n", errorList.ToString());
                            }
                        }
                    }

                    reader.Close();

                    return result;
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "Unexpected Exception while deserializing ", localOrAuthor, " RICO file at ", ricoDefPath);
                return null;
            }
        }
    }
}
