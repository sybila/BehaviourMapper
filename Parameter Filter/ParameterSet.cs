using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace Parameter_Filter
{
    public class ParameterSet : ObservableObject
    {
        public int ParameterCount { get { return ((Parameters == null) ? 0 : Parameters.Count()); } }
        public int WitnessCount { get { return ((Parameters == null) ? 0 : Parameters.SelectMany(p => p.Witnesses).Distinct().Count()); } }

        private IEnumerable<Parameter> parameters;
        public IEnumerable<Parameter> Parameters
        {
            get
            {
                return parameters.Where(p => Filters.GetActiveFilters().All(f => f(p))).OrderBy(p => p.WitnessCount).OrderBy(p => p.ShortestWitness);
            }
        }

        public ParameterFilters Filters { get; private set; }
        public ParameterStatistic Statistic { get; private set; }

        public ParameterSet()
        {
            Filters = new ParameterFilters(this);
            Statistic = new ParameterStatistic(this);
        }

        public void Refresh()
        {
            RaisePropertyChanged("Parameters");
            RaisePropertyChanged("ParameterCount");
            RaisePropertyChanged("WitnessCount");
            Statistic.Refresh();
        }

        #region Commands

        private RelayCommand _importCommand;
        public ICommand ImportCommand
        {
            get
            {
                _importCommand = _importCommand ?? new RelayCommand(() => ImportData());
                return _importCommand;
            }
        }

        private void ImportData()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Filter = "Parameter Set File|*.PSF|Any File|*.*";
                diag.Title = "Select Parameter Set to import.";
                diag.CheckFileExists = true;

                if (diag.ShowDialog() == DialogResult.OK)
                {
                    parameters = ParameterParser.Parse(diag.FileName);
                    RaisePropertyChanged("Parameters");
                    RaisePropertyChanged("ParameterCount");
                    RaisePropertyChanged("WitnessCount");

                    Filters.SetBounds(parameters);
                    Statistic.Refresh();
                }
            }
        }

        private RelayCommand _exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                _exportCommand = _exportCommand ?? new RelayCommand(() => ExportData());
                return _exportCommand;
            }
        }

        private void ExportData()
        {
            using (SaveFileDialog diag = new SaveFileDialog())
            {
                diag.Filter = "Witness Plotter File|*.WPF";
                diag.Title = "Save as";

                if (diag.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(diag.FileName))
                    {
                        foreach (Witness w in Parameters.SelectMany(p => p.Witnesses))
                            sw.WriteLine(w.ToString());
                    }
                }
            }
        }

        #endregion
    }
}
