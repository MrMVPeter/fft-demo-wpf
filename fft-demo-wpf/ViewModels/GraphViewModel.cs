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
using System.Collections.ObjectModel;
using fft_demo_wpf.Models;

namespace fft_demo_wpf.ViewModels
{
    //TimeDomainDataArray FrequencyDomainDataDoubleArray
    //UpdateDataAndGraphs() RenderTimeDomainWaveForm()
    public class GraphViewModel : BaseViewModel
    {
        private PlotModel _timeDomainModel = new PlotModel { Title = "Time-Domain" };
        private PlotModel _frequencyDomainModel = new PlotModel { Title = "Frequency-Domain" };
        private ObservableCollection<WaveComponentViewModel> _waveComponentViewModels;
        private SignalViewModel _signalViewModel;

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

        public GraphViewModel(SignalViewModel signalViewModel, ObservableCollection<WaveComponentViewModel> waveComponentViewModels)
        {
            InitializeTimeDomainGraph();

            InitializeFrequencyDomainGraph();

            _signalViewModel = signalViewModel;
            
            _waveComponentViewModels = waveComponentViewModels;
        }

        private void InitializeTimeDomainGraph()
        {
            // Time-Domain Graph
            TimeDomainModel = new PlotModel { Title = "Time-Domain" };
            var timeXAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time (seconds)",
                Minimum = 0,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            };
            var timeYAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Magnitude",
                IsPanEnabled = false,
                IsZoomEnabled = false,
            };
            TimeDomainModel.Axes.Add(timeXAxis);
            TimeDomainModel.Axes.Add(timeYAxis);
        }

        private void InitializeFrequencyDomainGraph()
        {
            // Freq-Domain Graph
            FrequencyDomainModel = new PlotModel { Title = "Frequency-Domain" };
            var frequencyXAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Frequency (Hz)",
                Minimum = 0,
                Maximum = 1000,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            };
            var frequencyYAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Magnitude",
                Minimum = 0,
                Maximum = 6,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            };
            FrequencyDomainModel.Axes.Add(frequencyXAxis);
            FrequencyDomainModel.Axes.Add(frequencyYAxis);
        }

        public void RenderTimeDomainWaveForm()
        {
            int sampleRate = _signalViewModel.SignalData.SampleRate;
            double signalDuration = _signalViewModel.SignalData.SignalDuration;
            double signalProportionVisible = _signalViewModel.SignalData.SignalProportionVisible;
            double[] timeDomainDataArray = _signalViewModel.SignalData.TimeDomainDataArray;

            TimeDomainModel.Series.Clear();
            int numSamplesToPlot = (int)(signalDuration * signalProportionVisible * sampleRate);

            // Plot Individual Components
            foreach (var component in _waveComponentViewModels)
            {
                var series = new LineSeries { Title = $"Frequency: {component.Frequency:F1} Hz" };
                series.Color = OxyColor.FromArgb(20, 0, 0, 0);

                // Add each sample to the series
                for (int i = 0; i < numSamplesToPlot; i++)
                {
                    double x = (double)i / sampleRate;
                    double y = component.WaveComponentSamples[i];
                    series.Points.Add(new DataPoint(x, y));
                }
                // Add series to graph
                TimeDomainModel.Series.Add(series);
            }

            // Plot Aggregate Waveform
            var sumSeries = new LineSeries { Title = "Sum of Components" };

            for (int i = 0; i < numSamplesToPlot; i++)
            {
                double x = (double)i / sampleRate;
                double y = timeDomainDataArray[i];
                sumSeries.Points.Add(new DataPoint(x, y));
            }
            TimeDomainModel.Series.Add(sumSeries);
        }

        public void UpdateFrequencyDomainGraph()
        {
            double signalDuration = _signalViewModel.SignalData.SignalDuration;
            double[] frequencyData = _signalViewModel.SignalData.FrequencyDomainDataDoubleArray;
            var series = new LineSeries { Title = "FFT Result" };

            for (int i = 0; i < frequencyData.Length; i++)
            {
                // Stop plotting past the max frequency of 1000
                if (i / signalDuration > 1000)
                {
                    break;
                }
                series.Points.Add(new DataPoint(i / signalDuration, frequencyData[i] * (2.0 / frequencyData.Length)));
            }

            FrequencyDomainModel.Series.Clear();
            FrequencyDomainModel.Series.Add(series);
        }

    }
}
