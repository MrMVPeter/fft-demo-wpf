using fft_demo_wpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fft_demo_wpf.ViewModels
{
    public class SignalViewModel : BaseViewModel
    {
        // Expose SignalData Model and it's attributes
        private SignalData _signalData;
        public SignalData SignalData
        {
            get { return _signalData; }
            set { SetProperty(ref _signalData, value); }
        }

        // Expose collection of SineWaveComponents and their attributes
        private ObservableCollection<WaveComponentViewModel> _sineWaveComponents;
        public ObservableCollection<WaveComponentViewModel> SineWaveComponents
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

        public SignalViewModel(ObservableCollection<WaveComponentViewModel> waveComponentViewModels)
        {
            // Initialize with default values
            SineWaveComponents = waveComponentViewModels;
            SignalData = new SignalData();
        }

        public void AddWaveComponent()
        {
            // Create a new wave component for the Model
            var newComponent = new SineWaveComponent();
            SignalData.SineWaveComponents.Add(newComponent);
            int count = SignalData.SineWaveComponents.Count;
            SignalData.SineWaveComponents[count - 1].GenerateSamples(SignalData.SignalDuration, SignalData.SampleRate);

            // Create a new ViewModel for this component
            WaveComponentViewModel newComponentViewModel = new WaveComponentViewModel(newComponent);
            SineWaveComponents.Add(newComponentViewModel);
        }


        public void GenerateNoise()
        {
            _signalData.GenerateNoise();
            OnPropertyChanged(nameof(SignalData));
        }

        public void ComputeSumOfSamples()
        {
            _signalData.ComputeSumOfSamples();
            OnPropertyChanged(nameof(SignalData));
        }

        public void PerformFFTWithMathNet()
        {
            _signalData.PerformFFTWithMathNet();
            OnPropertyChanged(nameof(SignalData));
        }
    }
}
