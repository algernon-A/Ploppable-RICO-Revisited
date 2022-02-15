using System;
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
        /// <param name="ricoDefPath">Definition file path</param>
        /// <param name="isLocal">True if this is a local settings file, false for author settings file</param>
        /// <returns>Parsed Ploppable RICO definition file</returns>
        public static PloppableRICODefinition ParseRICODefinition(string ricoDefPath, bool isLocal = false)
        {
            // Local settings flag.
            string localOrAuthor = isLocal ? "local" : "author";

            try
            {
                // Open file.
                using (StreamReader reader = new StreamReader(ricoDefPath))
                {
                    // Create new XML (de)serializer
                    XmlAttributes attrs = new XmlAttributes();
                    XmlElementAttribute attr = new XmlElementAttribute
                    {
                        ElementName = "RICOBuilding",
                        Type = typeof(RICOBuilding)
                    };
                    XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
                    attrOverrides.Add(typeof(RICOBuilding), "Building", attrs);

                    // Read XML.
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition), attrOverrides);
                    PloppableRICODefinition result = xmlSerializer.Deserialize(reader) as PloppableRICODefinition;

                    StringBuilder errorList;

                    if (result.Buildings.Count == 0)
                    {
                        Logging.Message("no parseable buildings in ", localOrAuthor, " XML settings file at ", ricoDefPath);
                    }
                    else
                    {
                        foreach (RICOBuilding building in result.Buildings)
                        {
                            // Check for fatal errors in each building.
                            errorList = building.FatalErrors;
                            if (errorList.Length == 0)
                            {
                                // No fatal errors; check for non-fatal errors.
                                errorList = building.NonFatalErrors;

                                if (errorList.Length != 0)
                                {
                                    if (isLocal && building.ricoEnabled)
                                    {
                                        // Errors in local settings need to be reported, except for buildings that aren't activated in RICO (e.g. for when the user has de-activated a RICO builidng with issues).
                                        Logging.Error("non-fatal errors for building '", building.Name, "' in local settings:\r\n", errorList);
                                    }
                                    else
                                    {
                                        // Errors in other settings should be logged if verbose logging is enabled, but otherwise continue.
                                        Logging.Message("non-fatal errors for building '", building.Name, "' in author settings:\r\n", errorList);
                                    }
                                }
                            }
                            else
                            {
                                // Fatal errors!  Need to be reported direct to user and the building ignored.
                                Logging.Error("fatal errors for building '", building.Name, "' in ", localOrAuthor, " settings:\r\n", errorList);
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
