using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.Departament
{
    public class DepartamentFilterModel
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string Name { get; set; }
    }
}
