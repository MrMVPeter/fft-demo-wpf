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
using fft_demo_wpf.Models;
using fft_demo_wpf.ViewModels;

namespace fft_demo_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // List of Individual Wave Components
        public static List<SineWaveComponent> sineWaveComponents = new List<SineWaveComponent>();

        // Double Arrays for each Time and Frequency Domains
        public static Double[] timeDomainDataArray;
        public static Double[] frequencyDomainDataDoubleArray;

        private readonly Random random = new Random();

        // Global properties
        public static double SignalDuration { get; set; } = 0.5;
        public static double SignalProportionVisible { get; set; } = 0.1;
        public static double SignalNoise { get; set; } = 0.0;

        private bool isNewComponentSelected = false;

        public static int sampleRate = 8000;
        public MainWindow()
        {
            InitializeComponent();

            // Initialize Noise Data (all 0s)
            ((MainViewModel)DataContext).GenerateNoise(1, 8000);

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
            
            // Add initial wave component and select it
            sineWaveComponents.Add(new SineWaveComponent());
            sineWaveComponentList.ItemsSource = sineWaveComponents;
            sineWaveComponentList.SelectedItem = sineWaveComponents[0];

            // setup slider event handlers
            frequencySlider.ValueChanged += FrequencySlider_ValueChanged;
            magnitudeSlider.ValueChanged += MagnitudeSlider_ValueChanged;
            phaseSlider.ValueChanged += PhaseSlider_ValueChanged;
            durationSlider.ValueChanged += DurationSlider_ValueChanged;
            proportionVisibleSlider.ValueChanged += ProportionVisible_ValueChanged;
            noiseSlider.ValueChanged += NoiseSlider_ValueChanged;

            // Generate data and paint data
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
            // flag to ensure component data is preseved during selection
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
            SineWaveComponent newComponent = new SineWaveComponent();

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
                // Show a message if no component is selected
                MessageBox.Show("Please select a component to delete.");
            }
        }

        private void ApplyNoise_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).GenerateNoise(1, 8000);
            UpdateDataAndGraphs();
        }

        private void UpdateGlobalProperties()
        {
            SignalDuration = durationSlider.Value;
            SignalProportionVisible = proportionVisibleSlider.Value;
            ((MainViewModel)DataContext).SignalNoise = noiseSlider.Value;
        }

        private void UpdateSelectedComponent()
        {
            SineWaveComponent selectedComponent = sineWaveComponentList.SelectedItem as SineWaveComponent;
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
            int numSamplesToPlot = (int)(SignalDuration * SignalProportionVisible * sampleRate);

            // Plot Individual Components
            foreach (var component in sineWaveComponents)
            {
                var series = new LineSeries { Title = $"Frequency: {component.Frequency:F1} Hz" };
                series.Color = OxyColor.FromArgb(10, 0, 0, 0);

                // Add each sample to the series
                for (int i = 0; i < numSamplesToPlot; i++)
                {
                    double x = (double)i / sampleRate;
                    double y = component.WaveComponentSamples[i];
                    series.Points.Add(new DataPoint(x, y));
                }
                // Add series to graph
                plotModel.Series.Add(series);
            }

            // Plot Aggregate Waveform
            var sumSeries = new LineSeries { Title = "Sum of Components" };

            for (int i = 0; i < numSamplesToPlot; i++)
            {
                double x = (double)i / sampleRate;
                double y = timeDomainDataArray[i];
                sumSeries.Points.Add(new DataPoint(x, y));
            }
            plotModel.Series.Add(sumSeries);

            // Update the plot
            timeDomainView.InvalidatePlot();
        }

        
        public void UpdateSelectedWaveComponentSamples()
        {
            SineWaveComponent selectedComponent = sineWaveComponentList.SelectedItem as SineWaveComponent;
            selectedComponent.GenerateSamples(SignalDuration, sampleRate);
        }

        public void UpdateAllWaveComponentSamples()
        {
            foreach (SineWaveComponent sineWaveComponent in sineWaveComponents)
            {
                sineWaveComponent.GenerateSamples(SignalDuration, sampleRate);
            }
        }

        public void ComputeSumOfSamples()
        {
            int numSamples = (int)(SignalDuration * sampleRate);
            timeDomainDataArray = new double[numSamples];
            foreach(var component in sineWaveComponents)
            {
                for (int i = 0; i < numSamples; i++)
                {
                    timeDomainDataArray[i] += component.WaveComponentSamples[i] + ((MainViewModel)DataContext).SignalNoiseData[i];
                }
            }
        }

        //public void GenerateNoise()
        //{
        //    int numSamples = (int)(noiseSlider.Maximum * sampleRate);
        //    signalNoiseData = new double[numSamples];
        //    for (int i = 0; i < numSamples; i++)
        //    {
        //        signalNoiseData[i] = (random.NextDouble() -0.5) * SignalNoise;
        //    }
        //}

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
                // Stop plotting past the max frequency of 1000
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
}
