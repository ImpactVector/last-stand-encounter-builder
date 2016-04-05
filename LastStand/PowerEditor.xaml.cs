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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;


namespace LastStand
{
    /// <summary>
    /// Interaction logic for PowerEditor.xaml
    /// </summary>
    public partial class PowerEditor : UserControl, INotifyPropertyChanged
    {
        private Business.Invader _currentInvader;
        public Business.Invader CurrentInvader { get { return _currentInvader; } set { _currentInvader = value; OnPropertyChanged("CurrentInvader"); } }

        public PowerEditor()
        {
            InitializeComponent();
            this.DataContext = this;
            DataObject.AddPastingHandler(tbName, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbFlavor, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbInvaderText, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbInvaderKey, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbRippedText, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbRippedKey, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbSlottedText, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbSlottedKey, new DataObjectPastingEventHandler(OnPaste));
        }

        private Business.Power _currentPower;
        public Business.Power CurrentPower { get { return _currentPower; } set { _currentPower = value; OnPropertyChanged("CurrentPower"); } }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            switch ((string)((FrameworkElement)sender).Tag)
            {
                case "NewPower":
                    CurrentInvader.Powers.Add(Business.Power.NewPower(CurrentInvader.Stat1.Name));
                    OnPropertyChanged("Powers");
                    break;
                case "DelPower":
                    CurrentInvader.Powers.Remove(CurrentPower);
                    CurrentPower = null;
                    OnPropertyChanged("Powers");
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

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
            if (!isText) return;

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            TextBox tb = (TextBox)sender;
            tb.SelectedText = text.Replace("\r\n", " ");
            tb.CaretIndex += tb.SelectedText.Length;
            tb.SelectionLength = 0;

            int lineIndex = tb.GetLineIndexFromCharacterIndex(tb.CaretIndex);
            tb.ScrollToLine(lineIndex);
            e.CancelCommand();
        }
    }
}
