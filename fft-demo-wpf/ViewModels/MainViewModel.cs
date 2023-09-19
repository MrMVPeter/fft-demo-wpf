using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fft_demo_wpf.Models;

namespace fft_demo_wpf.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly Random random = new Random();
        public double[] SignalNoiseData { get; set; }
        public MainViewModel()
        {
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
