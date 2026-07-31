using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.Charts
{
    public class BaseChartModel
    {

        public BaseChartModel()
        {
            Id = Guid.NewGuid().ToString();
            series = new List<int>();
            chart = new Chart();
            dataLabels = new DataLabels();
            responsive = new List<Responsive>();
            legend = new Legend();
            title = new Title();
            labels = new List<string>();
            colors = new List<string>();
        }

        public string Id { get; set; }


        public List<int> series { get; set; }
        public Chart chart { get; set; }
        public DataLabels dataLabels { get; set; }
        public List<Responsive> responsive { get; set; }
        public Legend legend { get; set; }
        public Title title { get; set; }
        public List<string> labels { get; set; }
        public List<string> colors { get; set; }

    }

    public class Title
    {
        public string text { get; set; }
    }

    public class Chart
    {
        public int width { get; set; }
        public string type { get; set; }
    }

    public class DataLabels
    {
        public bool enabled { get; set; }
    }

    public class Legend
    {
        public bool show { get; set; }
        public string position { get; set; }
        public string horizontalAlign { get; set; }
        public int offsetY { get; set; }
        public int height { get; set; }
    }

    public class Options
    {
        public Chart chart { get; set; }
        public Legend legend { get; set; }
    }

    public class Responsive
    {
        public int breakpoint { get; set; }
        public Options options { get; set; }
    }
}
