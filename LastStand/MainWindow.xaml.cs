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
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace LastStand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Business.InvaderList Invaders { get; set; }
        private Business.Invader _currentInvader;
        public Business.Invader CurrentInvader { get { return _currentInvader; } set { _currentInvader = value; powerEditor1.CurrentInvader = value; OnPropertyChanged("CurrentInvader"); } }
        private InvaderWrapper _currentEncInvader;
        public InvaderWrapper CurrentEncInvader { get { return _currentEncInvader; } set { _currentEncInvader = value; OnPropertyChanged("CurrentEncInvader"); } }

        public ObservableCollection<InvaderWrapper> Encounter { get; set; }
        public bool PrintDuplicates { get; set; }
        public bool PrintFlavor { get; set; }

        private string _dataPath;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //DataObject.AddPastingHandler(tbFlavorText, new DataObjectPastingEventHandler(OnPaste));
            //DataObject.AddPastingHandler(tbRulesText, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(tbName, new DataObjectPastingEventHandler(OnPaste));
            _dataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LastStand\\Data\\");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);
            if (!File.Exists(System.IO.Path.Combine(_dataPath,"powers.xml")))
            {
                using (StreamWriter ResourceFile = new StreamWriter(new FileStream(System.IO.Path.Combine(_dataPath,"powers.xml"), FileMode.Create)))
                    using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LastStand.Data.powers.xml"))
                        using (var reader = new StreamReader(stream))
                        {
                            ResourceFile.Write(reader.ReadToEnd());
                            ResourceFile.Flush();
                            ResourceFile.Close();
                        }
            }
            try
            {
                TextReader reader = new StreamReader(System.IO.Path.Combine(_dataPath,"powers.xml"));
                XmlSerializer serializer = new XmlSerializer(typeof(Business.InvaderList));
                Invaders = (Business.InvaderList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception)
            { }
            if (Invaders == null)
                Invaders = new Business.InvaderList();

            Encounter = new ObservableCollection<InvaderWrapper>();

            OnPropertyChanged("Invaders");
            OnPropertyChanged("Encounter");
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            switch ((string)((FrameworkElement)sender).Tag)
            {
                case "Exit":
                    Application.Current.Shutdown();
                    break;
                case "Save":
                    TextWriter reader = new StreamWriter(System.IO.Path.Combine(_dataPath, "powers.xml"));
                    XmlSerializer serializer = new XmlSerializer(typeof(Business.InvaderList));
                    serializer.Serialize(reader, Invaders);
                    reader.Close();
                    break;
                case "NewInvader":
                    Invaders.Add(Business.Invader.NewInvader());
                    OnPropertyChanged("Invaders");
                    break;
                case "DelInvader":
                    Invaders.Remove(CurrentInvader);
                    powerEditor1.CurrentPower = null;
                    CurrentInvader = null;
                    OnPropertyChanged("Invaders");
                    break;
                case "PrintEnc":
                    {
                        var pl = new Business.PowerDataList();
                        var il = new List<Business.InvaderData>();

                        foreach (var i in Encounter)
                        {
                            pl.AddRange(i.Invader.GetRawPowerData());
                            if (PrintDuplicates || !il.Exists(invader => invader.Name == i.Invader.Name))
                                il.AddRange(i.Invader.GetRawInvaderData(PrintFlavor));
                        }

                        PowersPrintPreview prev = new PowersPrintPreview(pl);
                        prev.Show();
                        InvaderPrintPreview iprev = new InvaderPrintPreview(il);
                        iprev.Show();
                    }
                    break;
                case "RemEnc":
                    if (CurrentEncInvader != null)
                    {
                        Encounter.Remove(CurrentEncInvader);
                        CurrentEncInvader = null;
                        OnPropertyChanged("Encounter");
                    }
                    break;
                case "AddEnc":
                    if (CurrentInvader != null)
                    {
                        Encounter.Add(new InvaderWrapper(CurrentInvader));
                        OnPropertyChanged("Encounter");
                    }
                    break;
                case "ClearEnc":
                    if (Encounter.Count > 0)
                    {
                        Encounter.Clear();
                        OnPropertyChanged("Encounter");
                    }
                    break;
                case "FullPowerList":
                    {
                        var pl = new List<Business.Power>();
                        foreach (var i in Invaders)
                        {
                            foreach (var p in i.Powers)
                            {
                                p.InvaderName = i.Name;
                                pl.Add(p);
                            }
                        }

                        FullPowerList fpl = new FullPowerList(pl);
                        fpl.Show();
                    }
                    break;
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
            if (!isText) return;

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            TextBox tb = (TextBox)sender;
            tb.SelectedText = text.Replace("\r\n", " ").Replace("\n", " ");
            tb.CaretIndex += tb.SelectedText.Length;
            tb.SelectionLength = 0;

            int lineIndex = tb.GetLineIndexFromCharacterIndex(tb.CaretIndex);
            tb.ScrollToLine(lineIndex);
            e.CancelCommand();
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
    }
    public class InvaderWrapper
    {
        public Business.Invader Invader { get; set; }
        public InvaderWrapper(Business.Invader i)
        {
            Invader = i;
        }
    }
}
