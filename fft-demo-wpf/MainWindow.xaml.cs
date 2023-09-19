using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using OxyPlot.Axes;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;

namespace fft_demo_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // List of Individual Wave Components
        public static List<SineWaveComponent> sineWaveComponents = new List<SineWaveComponent>();

        // Contains Samples of a waveform defined as the sum of each individual wave component
        public static Dictionary<double, double> sumOfWavesSamples = new Dictionary<double, double>();

        public static Double[] timeDomainDataArray;
        public static Double[] frequencyDomainDataDoubleArray;
        // Contains All noise data
        public static Dictionary<double, double> signalNoiseData = new Dictionary<double, double>();

        private Random random = new Random();
        public static double SignalDuration { get; set; } = 0.5;
        public static double SignalProportionVisible { get; set; } = 0.1;
        public static double SignalNoise { get; set; } = 0.0;

        private bool isNewComponentSelected = false;

        public static int sampleRate = 8000;
        public MainWindow()
        {
            InitializeComponent();
            GenerateNoise();

            // Time-Domain Graph
            PlotModel timeDomainModel = new PlotModel { Title = "Time-Domain" };
            var timeXAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time (seconds)",
                Minimum = 0
            };
            var timeYAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Magnitude"
            };
            timeDomainModel.Axes.Add(timeXAxis);
            timeDomainModel.Axes.Add(timeYAxis);
            timeDomainView.Model = timeDomainModel;

            //Freq-Domain-Graph
            PlotModel frequencyDomainModel = new PlotModel { Title = "Frequency-Domain" };
            var frequencyXAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Frequency (Hz)",
                Minimum = 0,
                Maximum = 1000
            };
            var frequencyYAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Magnitude",
                Minimum = 0,
                Maximum = 6
            };
            frequencyDomainModel.Axes.Add(frequencyXAxis);
            frequencyDomainModel.Axes.Add(frequencyYAxis);
            frequencyDomainView.Model = frequencyDomainModel;
            
            sineWaveComponents.Add(new SineWaveComponent { Frequency = 250, Magnitude = 2, Phase = 0 });
            sineWaveComponentList.ItemsSource = sineWaveComponents;
            sineWaveComponentList.SelectedItem = sineWaveComponents[0];

            frequencySlider.ValueChanged += FrequencySlider_ValueChanged;
            magnitudeSlider.ValueChanged += MagnitudeSlider_ValueChanged;
            phaseSlider.ValueChanged += PhaseSlider_ValueChanged;
            durationSlider.ValueChanged += DurationSlider_ValueChanged;
            proportionVisibleSlider.ValueChanged += ProportionVisible_ValueChanged;
            noiseSlider.ValueChanged += NoiseSlider_ValueChanged;
            UpdateSelectedWaveComponentSamples();
            UpdateDataAndGraphs();
        }

        private void FrequencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isNewComponentSelected)
            {
                UpdateSelectedComponent();
                UpdateSelectedWaveComponentSamples();
                UpdateDataAndGraphs();
            }
        }

        private void MagnitudeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isNewComponentSelected)
            {
                UpdateSelectedComponent();
                UpdateSelectedWaveComponentSamples();
                UpdateDataAndGraphs();
            }
        }

        private void PhaseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isNewComponentSelected)
            {
                UpdateSelectedComponent();
                UpdateSelectedWaveComponentSamples();
                UpdateDataAndGraphs();
            }
        }

        private void DurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateGlobalProperties();
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }
        private void ProportionVisible_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateGlobalProperties();
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }
        private void NoiseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateGlobalProperties();
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }

        private void SineWaveComponentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // flag to wave-component values changing during new selection
            isNewComponentSelected = true;

            var selectedComponent = sineWaveComponentList.SelectedItem as SineWaveComponent;
            if (selectedComponent != null)
            {
                frequencySlider.Value = selectedComponent.Frequency;
                magnitudeSlider.Value = selectedComponent.Magnitude;
                phaseSlider.Value = selectedComponent.Phase;
            }

            // Reset flag
            isNewComponentSelected = false;
        }

        private void AddComponent_Click(object sender, RoutedEventArgs e)
        {
            SineWaveComponent newComponent = new SineWaveComponent
            {
                Frequency = 250,
                Magnitude = 2,
                Phase = 0
            };

            // Add the new component to the list
            sineWaveComponents.Add(newComponent);

            // Refresh the ListBox
            sineWaveComponentList.Items.Refresh();

            // Select the newly added component
            sineWaveComponentList.SelectedItem = newComponent;

            // Create data samples for new component
            UpdateSelectedWaveComponentSamples();

            // Update the graphs to include the new component
            UpdateDataAndGraphs();
        }

        private void DeleteComponent_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected component
            SineWaveComponent selectedComponent = sineWaveComponentList.SelectedItem as SineWaveComponent;

            // Check if a component is selected
            if (selectedComponent != null)
            {
                // Remove the selected component from the list
                sineWaveComponents.Remove(selectedComponent);

                // Select the first compnent to ensure selected is not NULL
                if (sineWaveComponents.Count != 0)
                {
                    sineWaveComponentList.SelectedItem = sineWaveComponents[0];
                }

                // Refresh the ListBox
                sineWaveComponentList.Items.Refresh();

                // Update the graphs
                UpdateDataAndGraphs();
            }
            else
            {
                // Optionally, show a message if no component is selected
                MessageBox.Show("Please select a component to delete.");
            }
        }

        private void ApplyNoise_Click(object sender, RoutedEventArgs e)
        {
            GenerateNoise();
            UpdateDataAndGraphs();
        }


        private void UpdateGlobalProperties()
        {
            SignalDuration = durationSlider.Value;
            SignalProportionVisible = proportionVisibleSlider.Value;
            SignalNoise = noiseSlider.Value;
        }

        private void UpdateSelectedComponent()
        {
            var selectedComponent = sineWaveComponentList.SelectedItem as SineWaveComponent;
            if (selectedComponent != null)
            {
                selectedComponent.Frequency = frequencySlider.Value;
                selectedComponent.Magnitude = magnitudeSlider.Value;
                selectedComponent.Phase = phaseSlider.Value;
                sineWaveComponentList.Items.Refresh();
            }
        }

        public void UpdateDataAndGraphs()
        {
            ComputeSumOfSamples();
            RenderTimeDomainWaveForm();
            PerformFFTWithMathNet(timeDomainDataArray);
            UpdateFrequencyDomainGraph(frequencyDomainDataDoubleArray);
        }

        private void RenderTimeDomainWaveForm()
        {
            var plotModel = timeDomainView.Model;
            plotModel.Series.Clear();

            // Plot Individual Components
            foreach (var component in sineWaveComponents)
            {
                var series = new LineSeries { Title = $"Frequency: {component.Frequency:F1} Hz" };
                series.Color = OxyColor.FromArgb(10, 0, 0, 0);

                // For each key/value pair in the sample for that component
                foreach (var x in component.WaveComponentSamples)
                { 
                    // Stop plotting points past the proportion we want visible
                    if (x.Key > SignalDuration * SignalProportionVisible)
                    {
                        break;
                    }
                    series.Points.Add(new DataPoint(x.Key, x.Value));
                }
                plotModel.Series.Add(series);
            }

            // Plot Aggregate Waveform
            var sumSeries = new LineSeries { Title = "Sum of Components" };

            // For each key/value pair in the sample for that component
            foreach (var x in sumOfWavesSamples)
            {
                // Stop plotting points past the proportion we want visible
                if (x.Key > SignalDuration * SignalProportionVisible)
                {
                    break;
                }
                sumSeries.Points.Add(new DataPoint(x.Key, x.Value));
            }
            plotModel.Series.Add(sumSeries);

            // Update the plot
            timeDomainView.InvalidatePlot();
        }

        
        public void UpdateSelectedWaveComponentSamples()
        {
            SineWaveComponent selectedComponent = sineWaveComponentList.SelectedItem as SineWaveComponent;
            selectedComponent.WaveComponentSamples.Clear();
            double y;
            for (double x = 0; x < SignalDuration; x += 1.0 / sampleRate)
            {
                y = selectedComponent.Magnitude * Math.Sin((2 * Math.PI * selectedComponent.Frequency * x) + selectedComponent.Phase);
                selectedComponent.WaveComponentSamples[x] = y;
            }
        }

        public void UpdateAllWaveComponentSamples()
        {
            foreach (SineWaveComponent sineWaveComponent in sineWaveComponents)
            {
                sineWaveComponent.WaveComponentSamples.Clear();
                double y;
                for (double x = 0; x < SignalDuration; x += 1.0 / sampleRate)
                {
                    y = sineWaveComponent.Magnitude * Math.Sin((2 * Math.PI * sineWaveComponent.Frequency * x) + sineWaveComponent.Phase);
                    sineWaveComponent.WaveComponentSamples[x] = y;
                }
            }
        }

        public void ComputeSumOfSamples()
        {
            sumOfWavesSamples.Clear();
            foreach(var component in sineWaveComponents)
            {
                foreach (var x in component.WaveComponentSamples)
                {
                    if (sumOfWavesSamples.ContainsKey(x.Key))
                    {
                        sumOfWavesSamples[x.Key] += x.Value;
                        sumOfWavesSamples[x.Key] += signalNoiseData[x.Key];
                    }
                    else
                    {
                        sumOfWavesSamples[x.Key] = x.Value;
                        sumOfWavesSamples[x.Key] += signalNoiseData[x.Key];
                    }
                }
            }
            // Create time-domain data array
            timeDomainDataArray = sumOfWavesSamples.Values.ToArray();
        }

        public void GenerateNoise()
        {
            for (double x = 0; x < noiseSlider.Maximum; x += 1.0 / sampleRate)
            {
                signalNoiseData[x] = (random.NextDouble() -0.5) * SignalNoise;
            }
        }

        public void PerformFFTWithMathNet(double[] timeDomainData)
        {
            // Convert to MathNet Numerics data type
            var complexNumbers = timeDomainData.Select(t => new Complex32((float)t, 0)).ToArray();

            // Perform FFT using MathNet.Numerics
            Fourier.Forward(complexNumbers, FourierOptions.NoScaling);

            // Convert the results to a format suitable for your plotting library
            var frequencyDomainData = complexNumbers.Select(c => c.Magnitude).ToArray();

            // Convert float[] to double[]
            frequencyDomainDataDoubleArray = Array.ConvertAll(frequencyDomainData, x => (double)x);
        }

        private void UpdateFrequencyDomainGraph(double[] frequencyData)
        {
            var plotModel = frequencyDomainView.Model;
            var series = new LineSeries { Title = "FFT Result" };

            for (int i = 0; i < frequencyData.Length; i++)
            {
                if (i / SignalDuration > 1000)
                {
                    break;
                }
                series.Points.Add(new DataPoint(i / SignalDuration, frequencyData[i] * (2.0 / frequencyData.Length)));
            }

            plotModel.Series.Clear();
            plotModel.Series.Add(series);
            frequencyDomainView.InvalidatePlot();
        }
    }
    public class SineWaveComponent
    {
        public double Frequency { get; set; }
        public double Magnitude { get; set; }
        public double Phase { get; set; }
        public Dictionary<double, double> WaveComponentSamples { get; set; } = new Dictionary<double, double>();

        public override string ToString()
        {
            return $"F:{Frequency:F1}, M{Magnitude:F1}, P{Phase:F1}";
        }
    }
}
