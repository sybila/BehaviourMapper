using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public int WitnessCount { get { return ((Parameters == null) ? 0 : Parameters.SelectMany(p => p.Witnesses).Count()); } }
        public int DistinctWitnessCount { get { return ((Parameters == null) ? 0 : Parameters.SelectMany(p => p.Witnesses).Distinct().Count()); } }

        private IEnumerable<Parameter> parameters;
        private IEnumerable<Parameter> _filteredParameters;
        public IEnumerable<Parameter> Parameters
        {
            get { return _filteredParameters; }
            set
            {
                _filteredParameters = value
                    .Where(p => Filters.GetActiveFilters().All(f => f(p)))
                    .OrderBy(p => p, new ParameterByWitnessesComparer())
                    .ToArray();
                RaisePropertyChanged("Parameters");
            }
        }

        private WitnessSet witnesses;

        public RegulatoryContext RegulatoryContext { get; private set; }
        public TimeSerie TimeSerie { get; private set; }

        public ParameterFilters Filters { get; private set; }
        public ParameterStatistic Statistic { get; private set; }

        public ParameterSet()
        {
            witnesses = new WitnessSet();

            Filters = new ParameterFilters(this);
            Statistic = new ParameterStatistic(this);
        }

        public void Refresh(bool tighten)
        {
            if (tighten)
                Parameters = Parameters;
            else
                Parameters = parameters;

            RaisePropertyChanged("ParameterCount");
            RaisePropertyChanged("WitnessCount");
            RaisePropertyChanged("DistinctWitnessCount");
            Statistic.Refresh();
        }

        private class ParameterByWitnessesComparer : IComparer<Parameter>
        {
            public int Compare(Parameter x, Parameter y)
            {
                if (x == null)
                {
                    if (y == null)
                        return 0;

                    return (-1);
                }

                if (y == null)
                    return 1;

                if (x.WitnessCount > y.WitnessCount)
                    return 1;
                else if (x.WitnessCount < y.WitnessCount)
                    return (-1);

                return x.ShortestWitness.CompareTo(y.ShortestWitness);
            }
        }

        public bool HasRegulatoryContext { get { return (RegulatoryContext != null); } }
        public bool HasTimeSerie { get { return (TimeSerie != null); } }

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
                    parameters = ParameterParser.Parse(diag.FileName, witnesses).ToArray();
                    Parameters = parameters;

                    if (RegulatoryContext == null)
                        RegulatoryContext = new RegulatoryContext(parameters.First().Values.Count(),
                            parameters.First().Witnesses.First().Sequence.First().ActivityLevels.Count());

                    foreach (Parameter p in parameters)
                        p.CreateMask(RegulatoryContext);

                    Filters.SetBounds(parameters);
                    Filters.RefreshRegulatoryContexts();

                    Refresh(false);
                }
            }
        }

        private RelayCommand _exportWitnessesCommand;
        public ICommand ExportWitnessesCommand
        {
            get
            {
                _exportWitnessesCommand = _exportWitnessesCommand ?? new RelayCommand(() => ExportWitnesses());
                return _exportWitnessesCommand;
            }
        }

        private void ExportWitnesses()
        {
            using (System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog())
            {
                diag.Filter = "Witness Plotter File|*.WPF";
                diag.Title = "Save as";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    ExportWitnesses(diag.FileName);
            }
        }

        private void ExportWitnesses(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if (TimeSerie != null)
                    sw.WriteLine(TimeSerie);

                foreach (Witness w in Parameters.SelectMany(p => p.Witnesses))
                    sw.WriteLine(w.ToString());
            }
        }

        private RelayCommand _exportGraphCommand;
        public ICommand ExportGraphCommand
        {
            get
            {
                _exportGraphCommand = _exportGraphCommand ?? new RelayCommand(() => ExportGraph());
                return _exportGraphCommand;
            }
        }

        private void ExportGraph()
        {
            using (System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog())
            {
                //diag.Filter = "Cytoscape Graph|*.XGMML";
                diag.AddExtension = false;
                diag.Title = "Save graph as";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Random rnd = new Random();
                    string tmp = string.Format("{0}{1}.tmp", diag.FileName, rnd.Next());

                    ExportWitnesses(tmp);

                    ProcessStartInfo witnessPlotterStarter = new ProcessStartInfo("\"C:\\Program Files\\Java\\jdk1.7.0_01\\bin\\java.exe\"",
                        string.Format("-jar WitnessPlotter.jar \"{0}\" \"{1}\" --all", tmp, diag.FileName));
                    witnessPlotterStarter.WorkingDirectory = Environment.CurrentDirectory;
                    witnessPlotterStarter.CreateNoWindow = true;
                    witnessPlotterStarter.UseShellExecute = false;
                    witnessPlotterStarter.WindowStyle = ProcessWindowStyle.Hidden;

                    Process witnessPlotter = Process.Start(witnessPlotterStarter);
                    witnessPlotter.WaitForExit();
                    witnessPlotter.Close();

                    File.Delete(tmp);
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
                    RaisePropertyChanged("HasRegulatoryContext");
                    if (parameters != null)
                    {
                        foreach (Parameter p in parameters)
                            p.CreateMask(RegulatoryContext);

                        Filters.RefreshRegulatoryContexts();
                        Statistic.Refresh();
                    }
                }
            }
        }

        private RelayCommand _loadTimeSerieCommand;
        public ICommand LoadTimeSerieCommand
        {
            get
            {
                _loadTimeSerieCommand = _loadTimeSerieCommand ?? new RelayCommand(() => LoadTimeSerie());
                return _loadTimeSerieCommand;
            }
        }

        private void LoadTimeSerie()
        {
            using (System.Windows.Forms.OpenFileDialog diag = new System.Windows.Forms.OpenFileDialog())
            {
                diag.Filter = "Parsybone Model File|*.dbm|Time Serie File|*.tms|Any File|*.*";
                diag.Title = "Select time serie to load.";
                diag.CheckFileExists = true;

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (diag.FileName.Split('.').Last() == "dbm")
                        TimeSerie = new TimeSerie(diag.FileName, RegulatoryContext);
                    else
                        TimeSerie = new TimeSerie(diag.FileName, RegulatoryContext.Species.Count());

                    RaisePropertyChanged("HasTimeSerie");
                }
            }
        }

        #endregion
    }
}
