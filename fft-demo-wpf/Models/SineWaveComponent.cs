using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fft_demo_wpf.Models
{
    public class SineWaveComponent
    {
        public double Frequency { get; set; }
        public double Magnitude { get; set; }
        public double Phase { get; set; }
        public double[] WaveComponentSamples { get; set; }

        public SineWaveComponent()
        {
            Frequency = 250;
            Magnitude = 2;
            Phase = 0;
        }

        public void GenerateSamples(double signalDuration, int sampleRate)
        {
            int size = (int)(signalDuration * sampleRate);
            WaveComponentSamples = new double[size];
            for (int i = 0; i < size; i++)
            {
                double x = i * (1.0 / sampleRate);
                double y = Magnitude * Math.Sin((2 * Math.PI * Frequency * x) + Phase);
                WaveComponentSamples[i] = y;
            }
        }

        public override string ToString()
        {
            return $"F:{Frequency:F1}, M{Magnitude:F1}, P{Phase:F1}";
        }
    }
}
