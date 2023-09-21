using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fft_demo_wpf.Models;
using System.Collections.ObjectModel;

namespace fft_demo_wpf.ViewModels
{
    //SignalViewModel WaveComponentViewModel GraphViewModel
    internal class MainViewModel : BaseViewModel
    {
        //Sub-ViewModels
        private SignalViewModel _signalViewModel;
        private ObservableCollection<WaveComponentViewModel> _waveComponentViewModels;
        private GraphViewModel _graphViewModel;

        //Properties
        public SignalViewModel SignalViewModel
        {
            get => _signalViewModel;
            set => SetProperty(ref _signalViewModel, value);
        }

        public ObservableCollection<WaveComponentViewModel> WaveComponentViewModels
        {
            get => _waveComponentViewModels;
            set => SetProperty(ref _waveComponentViewModels, value);
        }

        public GraphViewModel GraphViewModel
        {
            get => _graphViewModel;
            set => SetProperty(ref _graphViewModel, value);
        }

        // OLD
        private readonly Random random = new Random();
        public double[] SignalNoiseData { get; set; }
        public MainViewModel()
        {
            //Initialize Sub-ViewModels
            SignalViewModel = new SignalViewModel();
            GraphViewModel = new GraphViewModel();
            WaveComponentViewModels = new ObservableCollection<WaveComponentViewModel>();

            // OLD
            SineWaveComponents = new ObservableCollection<SineWaveComponent>();
        }

        private ObservableCollection<SineWaveComponent> _sineWaveComponents;
        public ObservableCollection<SineWaveComponent> SineWaveComponents
        {
            get => _sineWaveComponents;
            set
            {
                if (_sineWaveComponents != value)
                {
                    _sineWaveComponents = value;
                    OnPropertyChanged(nameof(SineWaveComponents));
                }
            }
        }

        private double _signalNoise;
        public double SignalNoise
        {
            get => _signalNoise;
            set
            {
                if (_signalNoise != value)
                {
                    _signalNoise = value;
                    OnPropertyChanged(nameof(SignalNoise));
                }
            }
        }

        public void GenerateNoise(int sampleRate, double maxNoise)
        {
            int numSamples = (int)(maxNoise * sampleRate);
            SignalNoiseData = new double[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                SignalNoiseData[i] = (random.NextDouble() - 0.5) * SignalNoise;
            }
        }

    }
}
