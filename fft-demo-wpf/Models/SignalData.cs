using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fft_demo_wpf.Models
{
    internal class SignalData
    {
        public double[] TimeDomainDataArray { get; private set; }
        public double[] FrequencyDomainDataDoubleArray { get; private set; }
        public double[] SignalNoiseData { get; private set; }
        public double SignalDuration { get; private set; }
        public readonly int sampleRate = 8000;

        public SignalData()
        {

        }
    }
}
