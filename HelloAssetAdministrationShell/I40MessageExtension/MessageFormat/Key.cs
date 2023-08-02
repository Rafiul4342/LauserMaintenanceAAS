using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.I40MessageExtension.MessageFormat
{
    public class Key
    {
        public string type { get; set; }
        public string idType { get; set; }
        public string value { get; set; }

        public static implicit operator List<object>(Key v)
        {
            throw new NotImplementedException();
        }
    }
}
