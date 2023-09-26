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
using System.Collections.ObjectModel;

namespace fft_demo_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // List of Individual Wave Components
        public ObservableCollection<WaveComponentViewModel> WaveComponentViewModels => ((MainViewModel)DataContext).WaveComponentViewModels;

        // Double Arrays for each Time and Frequency Domains
        public static Double[] timeDomainDataArray;
        public static Double[] frequencyDomainDataDoubleArray;

        // SignalViewModel properties
        public double SignalDuration  => ((MainViewModel)DataContext).SignalViewModel.SignalDuration;
        public double SignalProportionVisible => ((MainViewModel)DataContext).SignalViewModel.SignalProportionVisible;
        public double SignalNoise => ((MainViewModel)DataContext).SignalViewModel.SignalNoise;
        // GraphViewModel properties
        public PlotModel TimeDomainModel => ((MainViewModel)DataContext).GraphViewModel.TimeDomainModel;
        public PlotModel FrequencyDomainModel => ((MainViewModel)DataContext).GraphViewModel.FrequencyDomainModel;

        private bool isNewComponentSelected = false;

        public static int sampleRate = 8000;
        public MainWindow()
        {
            InitializeComponent();

            // Initialize noise data (all 0s)
            ((MainViewModel)DataContext).SignalViewModel.GenerateNoise(1, 8000);

            // Add initial wave component and select it
            var initialWaveComponent = new SineWaveComponent();
            var initialWaveComponentViewModel = new WaveComponentViewModel(initialWaveComponent);
            WaveComponentViewModels.Add(initialWaveComponentViewModel);
            sineWaveComponentList.ItemsSource = WaveComponentViewModels;
            sineWaveComponentList.SelectedItem = initialWaveComponentViewModel;

            // Time-Domain graph
            timeDomainView.Model = TimeDomainModel;

            // Freq-Domain graph
            frequencyDomainView.Model = FrequencyDomainModel;

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
            var selectedComponent = sineWaveComponentList.SelectedItem as WaveComponentViewModel;
            if (!isNewComponentSelected && selectedComponent != null) 
            {
                selectedComponent.Frequency = frequencySlider.Value;
                sineWaveComponentList.Items.Refresh();
                selectedComponent.GenerateSamples(SignalDuration, sampleRate);
                UpdateDataAndGraphs();
            }
        }

        private void MagnitudeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var selectedComponent = sineWaveComponentList.SelectedItem as WaveComponentViewModel;
            if (!isNewComponentSelected && selectedComponent != null)
            {
                selectedComponent.Magnitude = magnitudeSlider.Value;
                sineWaveComponentList.Items.Refresh();
                selectedComponent.GenerateSamples(SignalDuration, sampleRate);
                UpdateDataAndGraphs();
            }
        }

        private void PhaseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var selectedComponent = sineWaveComponentList.SelectedItem as WaveComponentViewModel;
            if (!isNewComponentSelected && selectedComponent != null)
            {
                selectedComponent.Phase = phaseSlider.Value;
                sineWaveComponentList.Items.Refresh();
                selectedComponent.GenerateSamples(SignalDuration, sampleRate);
                UpdateDataAndGraphs();
            }
        }

        private void DurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((MainViewModel)DataContext).SignalViewModel.SignalDuration = durationSlider.Value;
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }
        private void ProportionVisible_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((MainViewModel)DataContext).SignalViewModel.SignalProportionVisible = proportionVisibleSlider.Value;
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }
        private void NoiseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((MainViewModel)DataContext).SignalViewModel.SignalNoise = noiseSlider.Value;
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }

        private void SineWaveComponentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // flag to ensure component data is preseved during selection
            isNewComponentSelected = true;

            var selectedComponent = sineWaveComponentList.SelectedItem as WaveComponentViewModel;
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
            WaveComponentViewModel newComponent = new WaveComponentViewModel(new SineWaveComponent());

            // Add the new component to the list
            WaveComponentViewModels.Add(newComponent);

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
            var selectedComponent = sineWaveComponentList.SelectedItem as WaveComponentViewModel;

            // Check if a component is selected
            if (selectedComponent != null)
            {
                // Remove the selected component from the list
                WaveComponentViewModels.Remove(selectedComponent);

                // Select the first compnent to ensure selected is not NULL
                if (WaveComponentViewModels.Count != 0)
                {
                    sineWaveComponentList.SelectedItem = WaveComponentViewModels[0];
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
            ((MainViewModel)DataContext).SignalViewModel.GenerateNoise(1, 8000);
            UpdateDataAndGraphs();
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
            TimeDomainModel.Series.Clear();
            int numSamplesToPlot = (int)(SignalDuration * SignalProportionVisible * sampleRate);

            // Plot Individual Components
            foreach (var component in WaveComponentViewModels)
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
                TimeDomainModel.Series.Add(series);
            }

            // Plot Aggregate Waveform
            var sumSeries = new LineSeries { Title = "Sum of Components" };

            for (int i = 0; i < numSamplesToPlot; i++)
            {
                double x = (double)i / sampleRate;
                double y = timeDomainDataArray[i];
                sumSeries.Points.Add(new DataPoint(x, y));
            }
            TimeDomainModel.Series.Add(sumSeries);

            // Update the plot
            timeDomainView.InvalidatePlot();
        }

        
        public void UpdateSelectedWaveComponentSamples()
        {
            WaveComponentViewModel selectedComponent = sineWaveComponentList.SelectedItem as WaveComponentViewModel;
            selectedComponent.GenerateSamples(SignalDuration, sampleRate);
        }

        public void UpdateAllWaveComponentSamples()
        {
            foreach (WaveComponentViewModel sineWaveComponent in WaveComponentViewModels)
            {
                sineWaveComponent.GenerateSamples(SignalDuration, sampleRate);
            }
        }

        public void ComputeSumOfSamples()
        {
            int numSamples = (int)(SignalDuration * sampleRate);
            timeDomainDataArray = new double[numSamples];
            foreach(var component in WaveComponentViewModels)
            {
                for (int i = 0; i < numSamples; i++)
                {
                    timeDomainDataArray[i] += component.WaveComponentSamples[i] + ((MainViewModel)DataContext).SignalViewModel.SignalNoiseData[i];
                }
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

            FrequencyDomainModel.Series.Clear();
            FrequencyDomainModel.Series.Add(series);
            frequencyDomainView.InvalidatePlot();
        }
    }
}
