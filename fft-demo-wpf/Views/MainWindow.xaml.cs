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

            var viewModel = new MainViewModel();
            DataContext = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Initialize noise data (all 0s)
            ((MainViewModel)DataContext).SignalViewModel.SignalData.GenerateNoise();

            // Add initial wave component and select it
            ((MainViewModel)DataContext).SignalViewModel.AddWaveComponent();
            //sineWaveComponentList.ItemsSource = WaveComponentViewModels;
            sineWaveComponentList.SelectedItem = WaveComponentViewModels.FirstOrDefault();

            // setup slider event handlers
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

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as MainViewModel;
            if (viewModel == null) return;

            if (e.PropertyName == nameof(MainViewModel.ShouldInvalidatePlot))
            {
                if (viewModel.ShouldInvalidatePlot)
                {
                    timeDomainView.InvalidatePlot();
                    frequencyDomainView.InvalidatePlot();
                    viewModel.ShouldInvalidatePlot = false;
                }
            }
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
            // Select the newly added component
            ((MainViewModel)DataContext).SignalViewModel.AddWaveComponent();
            var count = ((MainViewModel)DataContext).SignalViewModel.SineWaveComponentViewModels.Count;
            sineWaveComponentList.SelectedItem = ((MainViewModel)DataContext).SignalViewModel.SineWaveComponentViewModels[count - 1];

            // Create data samples for new component
            UpdateSelectedWaveComponentSamples();

            // Update the graphs to include the new component
            UpdateDataAndGraphs();
        }

        private void DeleteComponent_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected component
            var selectedComponent = sineWaveComponentList.SelectedItem as WaveComponentViewModel;

            ((MainViewModel)DataContext).SignalViewModel.RemoveWaveComponent(selectedComponent);
            var count = ((MainViewModel)DataContext).SignalViewModel.SineWaveComponentViewModels.Count;
            if (count != 0)
            {
                sineWaveComponentList.SelectedItem = ((MainViewModel)DataContext).SignalViewModel.SineWaveComponentViewModels[count - 1];
            }

            UpdateDataAndGraphs();
        }

        private void ApplyNoise_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).SignalViewModel.SignalData.GenerateNoise();
            UpdateDataAndGraphs();
        }

        public void UpdateDataAndGraphs()
        {
            ((MainViewModel)DataContext).UpdateDataAndGraphs();
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
