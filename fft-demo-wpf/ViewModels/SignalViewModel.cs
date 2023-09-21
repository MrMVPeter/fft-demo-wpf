using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fft_demo_wpf.ViewModels
{
    internal class SignalViewModel : BaseViewModel
    {
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

        public SignalViewModel()
        {
            // Initialize with default values
            _signalNoise = 0.5;
            _signalProportionVisible = 0.1;
            _signalNoise = 0.0;
        }

        public void GenerateNoise()
        {
            
        }

        public void UpdateGlobalProperties()
        {

        }
    }
}
