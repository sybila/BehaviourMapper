using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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

        public RegulatoryContext RegulatoryContext { get; private set; }
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
            using (System.Windows.Forms.OpenFileDialog diag = new System.Windows.Forms.OpenFileDialog())
            {
                diag.Filter = "Parameter Set File|*.PSF|Any File|*.*";
                diag.Title = "Select Parameter Set to import.";
                diag.CheckFileExists = true;

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    parameters = ParameterParser.Parse(diag.FileName).ToArray();
                    RaisePropertyChanged("Parameters");
                    RaisePropertyChanged("ParameterCount");
                    RaisePropertyChanged("WitnessCount");

                    if (RegulatoryContext == null)
                        RegulatoryContext = new RegulatoryContext(parameters.First().Values.Count());

                    foreach (Parameter p in parameters)
                        p.CreateMask(RegulatoryContext);

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
            using (System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog())
            {
                diag.Filter = "Witness Plotter File|*.WPF";
                diag.Title = "Save as";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(diag.FileName))
                    {
                        foreach (Witness w in Parameters.SelectMany(p => p.Witnesses))
                            sw.WriteLine(w.ToString());
                    }
                }
            }
        }

        private RelayCommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                _exitCommand = _exitCommand ?? new RelayCommand(() => Application.Current.Shutdown());
                return _exitCommand;
            }
        }

        private RelayCommand _loadModelCommand;
        public ICommand LoadModelCommand
        {
            get
            {
                _loadModelCommand = _loadModelCommand ?? new RelayCommand(() => LoadModel());
                return _loadModelCommand;
            }
        }

        private void LoadModel()
        {
            using (System.Windows.Forms.OpenFileDialog diag = new System.Windows.Forms.OpenFileDialog())
            {
                diag.Filter = "Parsybone Model File|*.dbm|Any File|*.*";
                diag.Title = "Select model to load.";
                diag.CheckFileExists = true;

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RegulatoryContext = new RegulatoryContext(diag.FileName);
                    if (parameters != null)
                        foreach (Parameter p in parameters)
                            p.CreateMask(RegulatoryContext);
                }
            }
        }

        #endregion
    }
}
