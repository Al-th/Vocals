using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Vocals.InternalClasses {
    [Serializable]
    public class Options {

        public bool ToggleListening;
        public Keys Key;
        public string Answer;
        public int Threshold;
        public string Language;

        public Options() {
            try {
                Load();
            }
            catch(Exception e){
                ToggleListening = false;
                Key = Keys.ShiftKey;
                Answer = "Toggle listening";
                Threshold = 0;
                Language = null;
            }
        }

        public Options(Options o) {
            this.ToggleListening = o.ToggleListening;
            this.Key = o.Key;
            this.Answer = o.Answer;
            this.Threshold = o.Threshold;
            this.Language = o.Language;
        }

        public void Save() {
            string dir = @"";
            string xmlSerializationFile = Path.Combine(dir, "options_xml.vc");
            try {
                Stream xmlStream = File.Open(xmlSerializationFile, FileMode.Create);
                XmlSerializer writer = new XmlSerializer(typeof(Options));
                writer.Serialize(xmlStream, this);
                xmlStream.Close();
            }
            catch (Exception e) {
                throw e;
            }
        }

        public void Load() {
            string dir = @"";
            string xmlSerializationFile = Path.Combine(dir, "options_xml.vc");
            try {
                Stream xmlStream = File.Open(xmlSerializationFile, FileMode.Open);
                XmlSerializer reader = new XmlSerializer(typeof(Options));
                Options opt = (Options)reader.Deserialize(xmlStream);
                this.ToggleListening = opt.ToggleListening;
                this.Answer = opt.Answer;
                this.Threshold = opt.Threshold;
                this.Key = opt.Key;
                this.Language = opt.Language;

                xmlStream.Close();
            }
            catch (Exception e) {
                throw e;
            }
        }
    }
}
