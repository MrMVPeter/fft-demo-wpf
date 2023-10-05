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
        public double SignalDuration  => ((MainViewModel)DataContext).SignalViewModel.SignalData.SignalDuration;
        public double SignalProportionVisible => ((MainViewModel)DataContext).SignalViewModel.SignalData.SignalProportionVisible;
        public double SignalNoise => ((MainViewModel)DataContext).SignalViewModel.SignalData.SignalNoise;
        // GraphViewModel properties
        public PlotModel TimeDomainModel => ((MainViewModel)DataContext).GraphViewModel.TimeDomainModel;
        public PlotModel FrequencyDomainModel => ((MainViewModel)DataContext).GraphViewModel.FrequencyDomainModel;

        private bool isNewComponentSelected = false;

        public static int sampleRate = 8000;
        public MainWindow()
        {
            InitializeComponent();

            // Initialize noise data (all 0s)
            ((MainViewModel)DataContext).SignalViewModel.SignalData.GenerateNoise();

            // Add initial wave component and select it
            ((MainViewModel)DataContext).SignalViewModel.AddWaveComponent();
            sineWaveComponentList.ItemsSource = WaveComponentViewModels;
            sineWaveComponentList.SelectedItem = WaveComponentViewModels.FirstOrDefault();

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

            // Time-Domain graph
            timeDomainView.Model = TimeDomainModel;
            
            // Freq-Domain graph
            frequencyDomainView.Model = FrequencyDomainModel;
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
            ((MainViewModel)DataContext).SignalViewModel.SignalData.SignalDuration = durationSlider.Value;
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }
        private void ProportionVisible_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((MainViewModel)DataContext).SignalViewModel.SignalData.SignalProportionVisible = proportionVisibleSlider.Value;
            UpdateAllWaveComponentSamples();
            UpdateDataAndGraphs();
        }
        private void NoiseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((MainViewModel)DataContext).SignalViewModel.SignalData.SignalNoise = noiseSlider.Value;
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
            //WaveComponentViewModel newComponent = new WaveComponentViewModel(new SineWaveComponent());

            //// Add the new component to the list
            //WaveComponentViewModels.Add(newComponent);

            //// Refresh the ListBox
            //sineWaveComponentList.Items.Refresh();

            //// Select the newly added component
            //sineWaveComponentList.SelectedItem = newComponent;
            ((MainViewModel)DataContext).SignalViewModel.AddWaveComponent();

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
            ((MainViewModel)DataContext).SignalViewModel.SignalData.GenerateNoise();
            UpdateDataAndGraphs();
        }

        public void UpdateDataAndGraphs()
        {
            ((MainViewModel)DataContext).SignalViewModel.ComputeSumOfSamples();
            ((MainViewModel)DataContext).GraphViewModel.RenderTimeDomainWaveForm();
            ((MainViewModel)DataContext).SignalViewModel.SignalData.PerformFFTWithMathNet();
            ((MainViewModel)DataContext).GraphViewModel.UpdateFrequencyDomainGraph();
            timeDomainView.InvalidatePlot();
            frequencyDomainView.InvalidatePlot();
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
    }
}
