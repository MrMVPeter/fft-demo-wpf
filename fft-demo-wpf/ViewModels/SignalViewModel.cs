using fft_demo_wpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fft_demo_wpf.ViewModels
{
    public class SignalViewModel : BaseViewModel
    {
        private readonly Random random = new Random();

        public double[] SignalNoiseData { get; set; }

        private double _signalDuration;
        private double _signalProportionVisible;
        private double _signalNoise;

        public double SignalDuration
        {
            get { return _signalDuration; }
            set { SetProperty(ref _signalDuration, value); }
        }

        public double SignalProportionVisible
        {
            get { return _signalProportionVisible; }
            set { SetProperty(ref _signalProportionVisible, value); }
        }

        public double SignalNoise
        {
            get { return _signalNoise; }
            set { SetProperty(ref _signalNoise, value); }
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

        

        public SignalViewModel()
        {
            // Initialize with default values
            _signalDuration = 0.5;
            _signalProportionVisible = 0.1;
            _signalNoise = 0.0;

            SineWaveComponents = new ObservableCollection<SineWaveComponent>();
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

        public void UpdateGlobalProperties()
        {

        }
    }
}
