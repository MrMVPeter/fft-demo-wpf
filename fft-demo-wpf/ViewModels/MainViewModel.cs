using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fft_demo_wpf.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using fft_demo_wpf.Utils;
using System.Windows.Media.Animation;

namespace fft_demo_wpf.ViewModels
{
    //SignalViewModel WaveComponentViewModel GraphViewModel
    public class MainViewModel : BaseViewModel
    {
        //Sub-ViewModels
        private SignalViewModel _signalViewModel;
        private ObservableCollection<WaveComponentViewModel> _waveComponentViewModels;
        private GraphViewModel _graphViewModel;

        public ICommand UpdateFrequencyCommand { get; private set; }
        public ICommand UpdateMagnitudeCommand { get; private set; }
        public ICommand UpdatePhaseCommand { get; private set; }


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

        private bool _shouldInvalidatePlot;

        public bool ShouldInvalidatePlot
        {
            get { return _shouldInvalidatePlot; }
            set
            {
                if (_shouldInvalidatePlot != value)
                {
                    _shouldInvalidatePlot = value;
                    OnPropertyChanged(nameof(ShouldInvalidatePlot));
                }
            }
        }

        private WaveComponentViewModel _selectedComponent;
        public WaveComponentViewModel SelectedComponent
        {
            get { return _selectedComponent; }
            set
            {
                _selectedComponent = value;
                OnPropertyChanged("SelectedComponent");
            }
        }

        public MainViewModel()
        {
            //Initialize Sub-ViewModels
            WaveComponentViewModels = new ObservableCollection<WaveComponentViewModel>();
            SignalViewModel = new SignalViewModel(WaveComponentViewModels);
            GraphViewModel = new GraphViewModel(SignalViewModel, WaveComponentViewModels);

            // Initialize Commands
            
        }

        public void UpdateDataAndGraphs()
        {
            SignalViewModel.ComputeSumOfSamples();
            GraphViewModel.RenderTimeDomainWaveForm();
            SignalViewModel.SignalData.PerformFFTWithMathNet();
            GraphViewModel.UpdateFrequencyDomainGraph();
        }

        public void SetFrequency(double frequency)
        {
            if (SelectedComponent == null) return;
            SelectedComponent.Frequency = frequency;
            SelectedComponent.GenerateSamples(_signalViewModel.SignalData.SignalDuration, _signalViewModel.SignalData.SampleRate);
            ShouldInvalidatePlot = true;
        }
    }
}
