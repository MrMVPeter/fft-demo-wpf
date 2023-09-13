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
using System.Runtime.InteropServices;

namespace fft_demo_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<SineWaveComponent> sineWaveComponents = new List<SineWaveComponent>();
        public double SignalDuration { get; set; } = 0.5;
        public MainWindow()
        {
            InitializeComponent();

            // Time-Domain Graph
            PlotModel timeDomainModel = new PlotModel { Title = "Time-Domain" };
            timeDomainView.Model = timeDomainModel;

            //Freq-Domain-Graph
            PlotModel frequencyDomainModel = new PlotModel { Title = "Frequency-Domain" };
            frequencyDomainModel.Series.Add(new LineSeries { Title = "Series 2", Points = { new DataPoint(0, 10), new DataPoint(10, 0) } });
            frequencyDomainView.Model = frequencyDomainModel;

            
            sineWaveComponents.Add(new SineWaveComponent { Frequency = 1, Magnitude = 1, Phase = 0 });
            sineWaveComponents.Add(new SineWaveComponent { Frequency = 2, Magnitude = 1, Phase = 0 });
            sineWaveComponentList.ItemsSource = sineWaveComponents;

            frequencySlider.ValueChanged += FrequencySlider_ValueChanged;
            magnitudeSlider.ValueChanged += MagnitudeSlider_ValueChanged;
            phaseSlider.ValueChanged += PhaseSlider_ValueChanged;
            durationSlider.ValueChanged += DurationSlider_ValueChanged;
            UpdateTimeDomainWaveForm();
        }

        private void FrequencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateSelectedComponent();
            UpdateTimeDomainWaveForm();
        }

        private void MagnitudeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateSelectedComponent();
            UpdateTimeDomainWaveForm();
        }

        private void PhaseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateSelectedComponent();
            UpdateTimeDomainWaveForm();
        }

        private void DurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateGlobalProperties();
            UpdateTimeDomainWaveForm();
        }

        private void SineWaveComponentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedComponent = sineWaveComponentList.SelectedItem as SineWaveComponent;
            if (selectedComponent != null)
            {
                frequencySlider.Value = selectedComponent.Frequency;
                magnitudeSlider.Value = selectedComponent.Magnitude;
                phaseSlider.Value = selectedComponent.Phase;
            }
        }

        private void AddComponent_Click(object sender, RoutedEventArgs e)
        {
            SineWaveComponent newComponent = new SineWaveComponent
            {
                Frequency = 1,
                Magnitude = 1,
                Phase = 0
            };

            // Add the new component to the list
            sineWaveComponents.Add(newComponent);

            // Refresh the ListBox
            sineWaveComponentList.Items.Refresh();

            // Optionally, select the newly added component
            sineWaveComponentList.SelectedItem = newComponent;

            // Update the graph to include the new component
            UpdateTimeDomainWaveForm();
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

                // Refresh the ListBox
                sineWaveComponentList.Items.Refresh();

                // Update the graph to remove the component
                UpdateTimeDomainWaveForm();
            }
            else
            {
                // Optionally, show a message if no component is selected
                MessageBox.Show("Please select a component to delete.");
            }
        }


        private void UpdateGlobalProperties()
        {
            SignalDuration = durationSlider.Value;
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

        private void UpdateTimeDomainWaveForm()
        {
            var plotModel = timeDomainView.Model;

            // Step 1: Clear existing Series
            plotModel.Series.Clear();

            // Placeholder for sum of all components at each point
            Dictionary<double, double> sumOfAllComponents = new Dictionary<double, double>();

            // Step 2: Plot Individual Components
            foreach (var component in sineWaveComponents)
            {
                var series = new LineSeries { Title = $"Frequency: {component.Frequency:F1} Hz" };
                series.Color = OxyColor.FromArgb(10, 0, 0 ,0);

                for (double x = 0; x <= SignalDuration; x += 1.0 / 4000.0)
                {
                    double y = component.Magnitude * Math.Sin((2 * Math.PI * component.Frequency * x) + component.Phase);
                    series.Points.Add(new DataPoint(x, y));

                    // Add this component's value to the sum
                    if (sumOfAllComponents.ContainsKey(x))
                    {
                        sumOfAllComponents[x] += y;
                    }
                    else
                    {
                        sumOfAllComponents[x] = y;
                    }
                }

                plotModel.Series.Add(series);
            }

            // Step 3: Plot Sum of Components
            var sumSeries = new LineSeries { Title = "Sum of Components", LineStyle = LineStyle.Solid };

            foreach (var point in sumOfAllComponents)
            {
                sumSeries.Points.Add(new DataPoint(point.Key, point.Value));
            }

            plotModel.Series.Add(sumSeries);

            // Update the plot
            timeDomainView.InvalidatePlot();
        }

    }
    public class SineWaveComponent
    {
        public double Frequency { get; set; }
        public double Magnitude { get; set; }
        public double Phase { get; set; }

        public override string ToString()
        {
            return $"F:{Frequency:F1}, M{Magnitude:F1}, P{Phase:F1}";
        }
    }

    public static class FFTWInterop
    {
        [DllImport("libfftw3-3-x64.ddl", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr fftw_plan_dft_1d(int n, IntPtr input, IntPtr output, int sign, uint flags);
    }
}
