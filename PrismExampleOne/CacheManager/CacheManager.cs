using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Live;
using eeDevices = Microsoft.Expression.Encoder.Devices;


namespace OGV.Cache
{
    public class CacheManager
    {
        private string presetFolder = "Presets";

        #region Fields

        
        #endregion

        #region Construction

        public CacheManager()
        {
        }

        #endregion

        #region Presets
        /// <summary>
        /// Gets the preset data from the web service.
        /// </summary>
        /// <param name="subClientID">The sub client who is running the application.</param>
        /// <param name="userName">The username the current user has singed on with or read from cache.</param>
        /// <returns></returns>
        public PresetService.PresetContainer[] GetPresetsFromWeb(string subClientID, string userName)
        {
            OGV.Cache.PresetService.PresetServiceClient pc = new PresetService.PresetServiceClient("BasicHttpBinding_IPresetService");
            PresetService.PresetContainer[] presetList = pc.GetPresetContainers(subClientID, userName);
            return presetList;
        }

        /// <summary>
        /// Reads presets in the form of xml and write them to
        /// the application directory as xml files.
        /// </summary>
        /// <param name="subClientID">The sub client who is running the application.</param>
        /// <param name="userName">The username the current user has singed on with or read from cache.</param>
        public void CachePresets(string subClientID, string userName)
        {
            XmlWriter xr = null;
            StreamWriter sr = null;

            PresetService.PresetContainer[] presetList = GetPresetsFromWeb(subClientID, userName);

            if(presetList.Length <1)
                throw new Exception ("No presets have been configured.");

            if(!Directory.Exists("Presets"))
                Directory.CreateDirectory("Presets");

            foreach (PresetService.PresetContainer p in presetList)
            {
                StringBuilder output = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                xr = XmlWriter.Create(output, settings);
                xr.WriteRaw(p.Xml);
                xr.Flush();

                //string builder variable has the xml
                sr = new StreamWriter(Path.Combine(presetFolder,p.Name + ".xml"));
                sr.Write(output.ToString());
                sr.Flush();
            }
            
        }

        /// <summary>
        /// Reads preset files from the application directory
        /// and converts them to EE 4 Live Presets
        /// </summary>
        /// <returns>List of Live Presets</returns>
        public List<Preset> ReadPresetsFromFile()
        {
            List<Preset> eePresets = new List<Preset>();

            string[] fileList = Directory.GetFiles("presets");
            foreach (string fileName in fileList)
            {
                Preset x = Preset.FromFile(fileName);
                eePresets.Add(x);
            }

            return eePresets;

        }

        #endregion

        #region Settings
        /// <summary>
        /// write previously used settings
        /// </summary>
        public void WriteCacheSettings(IList<string> names, IList<string> values,string fileName)
        {
            //build the xml fragment
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            XmlWriter wr = XmlTextWriter.Create(sb, settings);
            wr.WriteStartElement("Settings");
            for(int i = 0; i<names.Count;i++)
            {

                wr.WriteElementString(names[i], values[i]);
             
            }
            wr.WriteEndElement();
            wr.Flush();
            wr.Close();
            string content = sb.ToString();
            WriteCacheFile(fileName, content);
        }

        private void WriteCacheFile(string fileName, string fileText)
        {
 
            //if the file exists copy to a .tmp file
            if (File.Exists(fileName))
            {
                File.Copy(fileName, fileName + ".tmp");
            }

            try
            {


                File.WriteAllText(fileName, fileText);
                File.Delete(fileName + ".tmp");
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to write settings file", ex);
            }


        }

        public string ReadCacheFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    string contents = File.ReadAllText(fileName);
                    return contents;
                }

                return string.Empty;
            }
            catch ( Exception ex )
            {

                throw new Exception(string.Format("Unable to read cached settings file: {0}", fileName), ex);
            }
            
        }
        #endregion

    }
}
