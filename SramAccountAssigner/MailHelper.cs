using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace SramAccountAssigner
{
    public class MailHelper
    {
        public string BuildMessage(string templatePath, List<ParamDictionary> _parameters)
        {
            string rawHtml = File.ReadAllText(templatePath);

            foreach (var prm in _parameters)
            {
                rawHtml = rawHtml.Replace(prm.Key, prm.Value);
            }

            return rawHtml;
        }
    }
}
