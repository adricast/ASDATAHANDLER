using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDATAHANDLER.Models
{
    public class CsvModel
    {
        public string[] Values { get; set; }

        public CsvModel(string[] values)
        {
            Values = values;
        }

        public override string ToString()
        {
            return string.Join(", ", Values);
        }
      
    }
}
