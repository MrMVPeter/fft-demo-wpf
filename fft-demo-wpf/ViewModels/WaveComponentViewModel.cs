using fft_demo_wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace fft_demo_wpf.ViewModels
{
    //Frequency Magnitude Magnitude
    //GenerateSamples() UpdateSelectedComponent()
    public class WaveComponentViewModel : BaseViewModel
    {
        private SineWaveComponent _sineWaveComponent;

        public double Frequency
        {
            get => _sineWaveComponent.Frequency;
            set
            {
                if (_sineWaveComponent.Frequency != value)
                {
                    _sineWaveComponent.Frequency = value;
                    OnPropertyChanged(nameof(Frequency));
                }
            }
        }

        public double Magnitude
        {
            get => _sineWaveComponent.Magnitude;
            set
            {
                if ( _sineWaveComponent.Magnitude != value)
                {  
                    _sineWaveComponent.Magnitude = value;
                    OnPropertyChanged(nameof(Magnitude));
                }
            }
        }

        public double Phase
        {
            get => _sineWaveComponent.Phase;
            set
            {
                if (_sineWaveComponent.Phase != value)
                {
                    _sineWaveComponent.Phase = value;
                    OnPropertyChanged(nameof(Phase));
                }
            }
        }

        public double[] WaveComponentSamples
        {
            get => _sineWaveComponent.WaveComponentSamples;
            set
            {
                if (_sineWaveComponent.WaveComponentSamples != value)
                {
                    _sineWaveComponent.WaveComponentSamples = value;
                    OnPropertyChanged(nameof(WaveComponentSamples));
                }
            }
        }

        // Returns the underlying Model, important for synchronization
        public SineWaveComponent UnderlyingModel
        {
            get => _sineWaveComponent;
        }

        public WaveComponentViewModel(SineWaveComponent sineWaveComponent)
        {
            _sineWaveComponent = sineWaveComponent ?? throw new ArgumentNullException(nameof(sineWaveComponent));
        }

        public void GenerateSamples(double signalDuration, int sampleRate)
        {
            _sineWaveComponent.GenerateSamples(signalDuration, sampleRate);
            OnPropertyChanged(nameof(WaveComponentSamples));
        }

        public override string ToString()
        {
            return $"F:{Frequency:F1}, M{Magnitude:F1}, P{Phase:F1}";
        }
    }
}
