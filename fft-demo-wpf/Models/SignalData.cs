using fft_demo_wpf.ViewModels;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fft_demo_wpf.Models
{
    public class SignalData
    {
        private readonly Random random = new Random();
        public double[] TimeDomainDataArray { get; set; }
        public double[] FrequencyDomainDataDoubleArray { get; set; }
        public double[] SignalNoiseData { get; set; }
        public double SignalNoise { get; set; }
        public double SignalDuration { get; set; }
        public double SignalProportionVisible { get; set; }
        public int SampleRate { get; set; }

        public List<SineWaveComponent> SineWaveComponents { get; } = new List<SineWaveComponent>();

        public SignalData()
        {
            SignalNoise = 0;
            SignalDuration = 0.5;
            SignalProportionVisible = 0.1;
            SampleRate = 8000;
            int numSamples = (int)(SampleRate * SignalDuration);
            TimeDomainDataArray = new double[numSamples];
            FrequencyDomainDataDoubleArray = new double[numSamples];
            SignalNoiseData = new double[SampleRate];
        }

        public void GenerateNoise()
        {
            int numSamples = SampleRate;
            SignalNoiseData = new double[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                SignalNoiseData[i] = (random.NextDouble() - 0.5) * SignalNoise;
            }
        }

        public void ComputeSumOfSamples()
        {
            int numSamples = (int)(SignalDuration * SampleRate);
            TimeDomainDataArray = new double[numSamples];
            foreach (var component in SineWaveComponents)
            {
                for (int i = 0; i < numSamples; i++)
                {
                    TimeDomainDataArray[i] += component.WaveComponentSamples[i];
                }
            }
        }

        public void PerformFFTWithMathNet()
        {
            // Convert to MathNet Numerics data type
            var complexNumbers = TimeDomainDataArray.Select(t => new Complex32((float)t, 0)).ToArray();

            // Perform FFT using MathNet.Numerics
            Fourier.Forward(complexNumbers, FourierOptions.NoScaling);

            // Convert the results to a format suitable for your plotting library
            var frequencyDomainData = complexNumbers.Select(c => c.Magnitude).ToArray();

            // Convert float[] to double[]
            FrequencyDomainDataDoubleArray = Array.ConvertAll(frequencyDomainData, x => (double)x);
        }
    }
}
