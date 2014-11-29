using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ncvVJoyInputer
{
    [Serializable]
    public class VJoyInputerConfigData
    {
        public int span = 20;
        public string[] buttons = new string[32] 
        { 
            "^B$", "^A$", "^Y$", "^X$", "^L1$", "^L2$", "^R1$", "^R2$", 
            "^SELECT$", "^START$", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", ""
        };
        public string[] axis = new string[8]
        {
            "^LSF$", "^LSR$", "^LSB$", "^LSL$", "^RSF$", "^RSR$", "^RSB$", "^RSL$" 
        };
        public string[] pov = new string[4]
        {
            "^UP$", "^RIGHT$", "^DOWN$", "^LEFT$"
        };
    }

    class VJoyInputerConfig
    {
        private string path;
        public VJoyInputerConfigData current;
        public VJoyInputerConfigData temporary;

        public VJoyInputerConfig(string path)
        {
            this.path = Path.Combine(path, "VJoyInputerConfig.dat");
        }

        public void Save()
        {
            Copy(this.temporary, this.current);

            using (var stream = new FileStream(this.path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this.current);
            }
        }

        public void Load()
        {
            if (File.Exists(this.path))
            {
                using (var stream = new FileStream(this.path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var formatter = new BinaryFormatter();
                    this.current = (VJoyInputerConfigData)formatter.Deserialize(stream);
                }
            }
            else
            {
                this.current = new VJoyInputerConfigData();
            }

            this.temporary = new VJoyInputerConfigData();
            Copy(this.current, this.temporary);
            
        }

        private void Copy(VJoyInputerConfigData source, VJoyInputerConfigData destination)
        {
            destination.span = source.span;
            source.buttons.Select((c, i) => { destination.buttons[i] = c; return c; }).ToArray();
            source.axis.Select((c, i) => { destination.axis[i] = c; return c; }).ToArray();
            source.pov.Select((c, i) => { destination.pov[i] = c; return c; }).ToArray();
        }

        public void SetDefaultToTemporary()
        {
            this.temporary = new VJoyInputerConfigData();
        }
    }
}
