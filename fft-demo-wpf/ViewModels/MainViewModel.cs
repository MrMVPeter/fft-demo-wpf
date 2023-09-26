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
    public class MainViewModel : BaseViewModel
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

        public MainViewModel()
        {
            //Initialize Sub-ViewModels
            SignalViewModel = new SignalViewModel();
            GraphViewModel = new GraphViewModel();
            WaveComponentViewModels = new ObservableCollection<WaveComponentViewModel>();
        }
    }
}
