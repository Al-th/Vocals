using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Vocals.InternalClasses {
    [Serializable]
    public class Options {

        public bool toggleListening;
        public Keys key;
        public string answer;
        public int threshold;
        public string language;

        public Options() {
            try {
                load();
            }
            catch(Exception e){
                toggleListening = false;
                key = Keys.ShiftKey;
                answer = "Toggle listening";
                threshold = 0;
                language = null;
            }
        }

        public Options(Options o) {
            this.toggleListening = o.toggleListening;
            this.key = o.key;
            this.answer = o.answer;
            this.threshold = o.threshold;
            this.language = o.language;
        }

        public void save() {
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

        public void load() {
            string dir = @"";
            string xmlSerializationFile = Path.Combine(dir, "options_xml.vc");
            try {
                Stream xmlStream = File.Open(xmlSerializationFile, FileMode.Open);
                XmlSerializer reader = new XmlSerializer(typeof(Options));
                Options opt = (Options)reader.Deserialize(xmlStream);
                this.toggleListening = opt.toggleListening;
                this.answer = opt.answer;
                this.threshold = opt.threshold;
                this.key = opt.key;
                this.language = opt.language;

                xmlStream.Close();
            }
            catch (Exception e) {
                throw e;
            }
        }
    }
}
