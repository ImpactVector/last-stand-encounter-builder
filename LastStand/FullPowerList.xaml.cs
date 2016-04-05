using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace LastStand
{
    /// <summary>
    /// Interaction logic for EncounterBuilder.xaml
    /// </summary>
    public partial class FullPowerList : Window, INotifyPropertyChanged
    {
        public FullPowerList(List<Business.Power> powers)
        {
            InitializeComponent();
            Powers = powers;
            PrintPowers = new ObservableCollection<PowerWrapper>();
            this.DataContext = this;
        }

        public List<Business.Power> Powers { get; set; }
        private Business.Power _currentPower;
        public Business.Power CurrentPower { get { return _currentPower; } set { _currentPower = value; OnPropertyChanged("CurrentPower"); } }
        public ObservableCollection<PowerWrapper> PrintPowers { get; set; }
        private PowerWrapper _currentPrintPower;
        public PowerWrapper CurrentPrintPower { get { return _currentPrintPower; } set { _currentPrintPower = value; OnPropertyChanged("CurrentPrintPower"); } }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            switch ((string)((FrameworkElement)sender).Tag)
            {
                case "PrintEnc":
                    {
                        var pl = new Business.PowerDataList();

                        foreach (var p in PrintPowers)
                        {
                            pl.AddRange(p.Power.GetRawPowerData());
                        }

                        PowersPrintPreview prev = new PowersPrintPreview(pl);
                        prev.Show();
                    }
                    break;
                case "RemEnc":
                    if (CurrentPrintPower != null)
                    {
                        PrintPowers.Remove(CurrentPrintPower);
                        CurrentPrintPower = null;
                        OnPropertyChanged("PrintPowers");
                    }
                    break;
                case "AddEnc":
                    if (CurrentPower != null)
                    {
                        PrintPowers.Add(new PowerWrapper(CurrentPower));
                        OnPropertyChanged("PrintPowers");
                    }
                    break;
                case "ClearEnc":
                    if (PrintPowers.Count > 0)
                    {
                        PrintPowers.Clear();
                        OnPropertyChanged("PrintPowers");
                    }
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    public class PowerWrapper
    {
        public Business.Power Power { get; set; }
        public PowerWrapper(Business.Power p)
        {
            Power = p;
        }
    }
}
