using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace fft_demo_wpf.ViewModels
{
    //TimeDomainDataArray FrequencyDomainDataDoubleArray
    //UpdateDataAndGraphs() RenderTimeDomainWaveForm()
    public class GraphViewModel : BaseViewModel
    {
        private PlotModel _timeDomainModel = new PlotModel { Title = "Time-Domain" };
        private PlotModel _frequencyDomainModel = new PlotModel { Title = "Frequency-Domain" };

        public PlotModel TimeDomainModel
        {
            get => _timeDomainModel;
            set => SetProperty(ref _timeDomainModel, value);
        }

        public PlotModel FrequencyDomainModel
        {
            get => _frequencyDomainModel;
            set => SetProperty(ref _frequencyDomainModel, value);
        }

        public GraphViewModel()
        {
            // Time-Domain Graph
            PlotModel TimeDomainModel = new PlotModel { Title = "Time-Domain" };
            var timeXAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time (seconds)",
                Minimum = 0
            };
            var timeYAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Magnitude"
            };
            TimeDomainModel.Axes.Add(timeXAxis);
            TimeDomainModel.Axes.Add(timeYAxis);

            // Freq-Domain Graph
            PlotModel frequencyDomainModel = new PlotModel { Title = "Frequency-Domain" };
            var frequencyXAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Frequency (Hz)",
                Minimum = 0,
                Maximum = 1000
            };
            var frequencyYAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Magnitude",
                Minimum = 0,
                Maximum = 6
            };
            frequencyDomainModel.Axes.Add(frequencyXAxis);
            frequencyDomainModel.Axes.Add(frequencyYAxis);
        }
    }
}
