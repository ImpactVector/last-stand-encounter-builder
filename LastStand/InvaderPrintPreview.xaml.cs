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

namespace LastStand
{
    /// <summary>
    /// Interaction logic for InvaderPrintPreview.xaml
    /// </summary>
    public partial class InvaderPrintPreview : Window
    {
        private List<Business.InvaderData> _invaders;
        public InvaderPrintPreview(List<Business.InvaderData> invaders)
        {
            InitializeComponent();
            _invaders = invaders;
            _reportViewer.Load += ReportViewer_Load;
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.PowerDataListBindingSource.DataSource = _powers;
        //    this.reportViewer1.RefreshReport();
        //}

        private bool _isReportViewerLoaded;

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            if (!_isReportViewerLoaded)
            {
                Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

                reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
                reportDataSource1.Value = _invaders;
                this._reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                this._reportViewer.LocalReport.ReportEmbeddedResource = "LastStand.Resources.Invaders.rdlc";

                this._reportViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                _reportViewer.RefreshReport();

                _isReportViewerLoaded = true;
            }
        }
    }
}
