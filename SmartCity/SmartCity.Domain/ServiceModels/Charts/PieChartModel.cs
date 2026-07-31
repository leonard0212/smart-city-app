using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.Charts
{
    public class PieChartModel : BaseChartModel
    {

        public PieChartModel()
        {
            plotOptions = new PlotOptions()
            {
                pie = new Pie()
                {
                    donut = new Donut()
                    {
                        size = "65%",
                        labels = new Labels()
                        {
                            show = true,
                            total = new Total()
                            {
                                show = true,
                                label = "Total"
                            }
                        }
                    }
                }
            };
        }

        public PlotOptions plotOptions { get; set; }



    }


    public class Donut
    {
        public string size { get; set; }
        public Labels labels { get; set; }
    }
    public class Labels
    {
        public bool show { get; set; }
        public Total total { get; set; }
    }
    public class Pie
    {
        public Donut donut { get; set; }
    }

    public class PlotOptions
    {
        public Pie pie { get; set; }
    }

    public class Total
    {
        public bool show { get; set; }
        public string label { get; set; }
    }

}
