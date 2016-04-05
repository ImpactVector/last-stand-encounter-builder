using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;

namespace LastStand.Business
{
    public enum InvaderType
    {
        SimpleVermin = -1,
        Vermin = 0,
        Regular = 1,
        Giant = 2
    }

    [Serializable]
    [XmlRoot("Invaders")]
    public class InvaderList : BindingList<Invader>
    {
    }

    [Serializable]
    public class Invader : INotifyPropertyChanged
    {
        public static Invader NewInvader()
        {
            Invader i = new Invader();

            i._name = "New Invader";
            i._stat1 = new Stat() { Name = "Disruptor", Value = 1 };
            i._stat2 = new Stat() { Name = "Leader", Value = 1 };
            i._stat3 = new Stat() { Name = "Operative", Value = 1 };
            i._stat4 = new Stat() { Name = "Tactician", Value = 1 };
            i._power = 1;
            i.Powers = new BindingList<Power>();
            i._type = InvaderType.Regular;

            return i;
        }

        private Invader() { }

        private string _name;
        private InvaderType _type;
        private BindingList<Power> _powers;
        private string _flavorText;
        private string _rulesText;
        private int _vermHP;
        private int _vermGroupLow;
        private int _vermGroupHigh;

        private Stat _stat1;
        private Stat _stat2;
        private Stat _stat3;
        private Stat _stat4;
        private int _power;

        [XmlAttribute]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        [XmlAttribute]
        public InvaderType Type { get { return _type; } set { _type = value; OnPropertyChanged("Type"); OnPropertyChanged("VerminStatVisibility"); } }
        public BindingList<Power> Powers { get { return _powers; } set { _powers = value; OnPropertyChanged("Powers"); } }
        public Stat Stat1 { get { return _stat1; } set { _stat1 = value; OnPropertyChanged("Stat1"); } }
        public Stat Stat2 { get { return _stat2; } set { _stat2 = value; OnPropertyChanged("Stat2"); } }
        public Stat Stat3 { get { return _stat3; } set { _stat3 = value; OnPropertyChanged("Stat3"); } }
        public Stat Stat4 { get { return _stat4; } set { _stat4 = value; OnPropertyChanged("Stat4"); } }
        public int Power { get { return _power; } set { _power = value; OnPropertyChanged("Power"); } }
        [XmlElement, DefaultValue("")]
        public string FlavorText { get { return _flavorText; } set { _flavorText = value; OnPropertyChanged("FlavorText"); } }
        [XmlElement, DefaultValue("")]
        public string RulesText { get { return _rulesText; } set { _rulesText = value; OnPropertyChanged("RulesText"); } }

        [XmlAttribute, DefaultValue(0)]
        public int VermHP { get { return _vermHP; } set { _vermHP = value; OnPropertyChanged("VermHP"); } }
        [XmlAttribute, DefaultValue(0)]
        public int VermGroupLow { get { return _vermGroupLow; } set { _vermGroupLow = value; OnPropertyChanged("VermGroupLow"); } }
        [XmlAttribute, DefaultValue(0)]
        public int VermGroupHigh { get { return _vermGroupHigh; } set { _vermGroupHigh = value; OnPropertyChanged("VermGroupHigh"); } }

        [XmlIgnore]
        public List<Stat> Stats
        {
            get
            {
                List<Stat> s = new List<Stat>();
                s.Add(Stat1);
                s.Add(Stat2);
                s.Add(Stat3);
                s.Add(Stat4);
                return s;
            }
        }
        [XmlIgnore]
        public string VermGroupSize { get { return VermGroupLow != VermGroupHigh ? VermGroupLow.ToString() + " - " + VermGroupHigh.ToString() : VermGroupLow.ToString(); } }

        [XmlIgnore]
        public System.Windows.Visibility VerminStatVisibility { get { return Type == InvaderType.Vermin ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public PowerDataList GetRawPowerData()
        {
            PowerDataList data = new PowerDataList();

            foreach (Power p in Powers)
            {
                data.Add(new PowerData() { Name = p.Name, InvaderName = this.Name, Stat = p.StatName, Keywords = p.Ripped.Keywords, Type = "Ripped", Text = p.Ripped.Text });
                data.Add(new PowerData() { Name = p.Name, InvaderName = this.Name, Stat = p.StatName, Keywords = p.Slotted.Keywords, Type = "Slotted", Text = p.Slotted.Text });
            }

            return data;
        }

        public List<InvaderData> GetRawInvaderData(bool printFlavor)
        {
            List<InvaderData> data = new List<InvaderData>();

            Guid g = Guid.NewGuid();
            if (Powers.Count > 0)
                foreach (Power p in Powers)
                    data.Add(CreateDataLine(p, printFlavor, g, Stat1.Name, Stat1.Value, Stat2.Name, Stat2.Value, Stat3.Name, Stat3.Value, Stat4.Name, Stat4.Value, Power));
            else
                data.Add(CreateDataLine(null, printFlavor, g, Stat1.Name, Stat1.Value, Stat2.Name, Stat2.Value, Stat3.Name, Stat3.Value, Stat4.Name, Stat4.Value, Power));
            

            return data;
        }

        private InvaderData CreateDataLine(Power p, bool printFlavor, Guid id, string stat1Name, int stat1Val, string stat2Name, int stat2Val, string stat3Name, int stat3Val, string stat4Name, int stat4Val, int power)
        {
            InvaderData line;

            line = new InvaderData() { VerminGroupSize = VermGroupLow == VermGroupHigh ? VermGroupLow.ToString() : VermGroupLow.ToString() + "-" + VermGroupHigh.ToString(), VerminHP = VermHP, ID = id, Name = this.Name, Type = this.Type, RulesText = this.RulesText, Stat1Name = stat1Name, Stat1Value = stat1Val, Stat2Name = stat2Name, Stat2Value = stat2Val, Stat3Name = stat3Name, Stat3Value = stat3Val, Stat4Name = stat4Name, Stat4Value = stat4Val, Power = power };
            if (p != null)
            {
                line.PowerStatName = p.StatName;
                line.PowerName = p.Name;
                line.PowerKeywords = p.Invader.Keywords;
                line.PowerText = p.Invader.Text;
                if (printFlavor)
                    line.PowerFlavorText = p.FlavorText;
            }
            if (printFlavor)
                line.FlavorText = this.FlavorText;

            return line;
        }
    }

    [Serializable]
    public class Stat : INotifyPropertyChanged
    {
        private string _name;
        [XmlAttribute]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        private int _value;
        [XmlAttribute]
        public int Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }

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

    [Serializable]
    public class Power : INotifyPropertyChanged
    {
        public static Power NewPower(string statName)
        {
            Power p = new Power();

            p.Ripped = new PowerVersion();
            p.Slotted = new PowerVersion();
            p.Invader = new PowerVersion();
            p._statName = statName;
            p._name = "New Power";

            return p;
        }

        private string _name;
        private string _statName;
        private string _flavorText;

        [XmlAttribute]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        [XmlAttribute]
        public string StatName { get { return _statName; } set { _statName = value; OnPropertyChanged("StatName"); } }
        [XmlElement, DefaultValue("")]
        public string FlavorText { get { return _flavorText; } set { _flavorText = value; OnPropertyChanged("FlavorText"); } }
        [XmlIgnore]
        public string InvaderName { get; set; }

        public PowerVersion Invader { get; set; }
        public PowerVersion Ripped { get; set; }
        public PowerVersion Slotted { get; set; }

        public PowerDataList GetRawPowerData()
        {
            PowerDataList data = new PowerDataList();

            data.Add(new PowerData() { Name = Name, InvaderName = this.InvaderName, Stat = StatName, Keywords = Ripped.Keywords, Type = "Ripped", Text = Ripped.Text });
            data.Add(new PowerData() { Name = Name, InvaderName = this.InvaderName, Stat = StatName, Keywords = Slotted.Keywords, Type = "Slotted", Text = Slotted.Text });
            
            return data;
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

    [Serializable]
    public class PowerVersion : INotifyPropertyChanged
    {
        private string _keywords;
        private string _text;

        [XmlAttribute, DefaultValue("")]
        public string Keywords { get { return _keywords; } set { _keywords = value; OnPropertyChanged("Keywords"); } }
        [XmlText, DefaultValue("")]
        public string Text { get { return _text; } set { _text = value; OnPropertyChanged("Text"); } }

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

    public class PowerData
    {
        public string Name {get;set;}
        public string InvaderName {get;set;}
        public string Keywords {get;set;}
        public string Text {get;set;}
        public string Type { get; set; }
        public string Stat { get; set; }
    }

    public class PowerDataList : List<PowerData>
    {
    }

    public class InvaderData
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public InvaderType Type { get; set; }
        public string FlavorText { get; set; }
        public string RulesText { get; set; }

        public int VerminHP { get; set; }
        public string VerminGroupSize { get; set; }

        public string PowerStatName { get; set; }

        public string PowerName { get; set; }
        public string PowerKeywords { get; set; }
        public string PowerText { get; set; }
        public string PowerFlavorText { get; set; }

        public string Stat1Name { get; set; }
        public int Stat1Value { get; set; }
        public string Stat2Name { get; set; }
        public int Stat2Value { get; set; }
        public string Stat3Name { get; set; }
        public int Stat3Value { get; set; }
        public string Stat4Name { get; set; }
        public int Stat4Value { get; set; }
        public int Power { get; set; }
    }
}