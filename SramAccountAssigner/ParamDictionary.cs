using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SramAccountAssigner
{
    public class ParamDictionary
    {
        private string key;
        public string Value { get; set; }
        public string Key
        {
            get { return this.key; }
            set { this.key = "{"+value+"}"; }
        }

    }
}
