using SmartCity.Domain.ServiceModels.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Utils
{
    public static class ChartsUtils
    {
        public static PieChartModel CreatePieChartModel(Dictionary<string, int> data, List<string> colors, string title)
        {
            var model = new PieChartModel();
            model.chart.type = "donut";
            model.chart.width = 380;
            model.series = data.Select(x => x.Value).ToList();
            model.labels = data.Select(x => x.Key).ToList();
            model.colors = colors;

            model.dataLabels.enabled = false;
            model.responsive.Add(new Responsive()
            {
                breakpoint = 480,
                options = new Options()
                {
                    chart = new Chart()
                    {
                        width = 300
                    },
                    legend = new Legend()
                    {
                        show = false
                    }
                }
            });
            model.legend.show = true;
            model.legend.position = "right";
            model.legend.offsetY = 0;
            model.legend.height = 200;
            model.legend.horizontalAlign = "right";
            model.title.text = title;


            return model;

        }
    }
}
