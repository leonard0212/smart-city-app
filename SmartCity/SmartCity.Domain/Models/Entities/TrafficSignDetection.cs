using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class TrafficSignDetection : Detection
    {

        public string Subclass { get; set; }
        public string Category { get; set; }
    }
}
