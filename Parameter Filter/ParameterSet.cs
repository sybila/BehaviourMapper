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
    using Windows;

    enum SortOption { Cost, /*Parameter_count,*/ Possible_behaviours, Robustness }

    public class ParameterSet : ObservableObject
    {
        private static readonly Dictionary<SortOption, string> sortOptionHeaders = new Dictionary<SortOption, string>()
        {
            { SortOption.Cost, "By cost" },
            //{ SortOption.Parameter_count, "By count of parameters" },
            { SortOption.Possible_behaviours, "By count of possible behaviours" },
            { SortOption.Robustness, "By robustness" }
        };

        private static readonly ISet<SortOption> parameterSortOptions = new HashSet<SortOption>() { SortOption.Cost, SortOption.Possible_behaviours, SortOption.Robustness };

        public int ParameterCount { get { return ((Parameters == null) ? 0 : Parameters.Count()); } }
        //public int WitnessCount { get { return ((Parameters == null) ? 0 : Witnesses.Count()); } }
        //public int DistinctWitnessCount { get { return ((Parameters == null) ? 0 : DistinctWitnesses.Count()); } }

        private IEnumerable<Parameter> parameters;
        private IEnumerable<Parameter> _filteredParameters;
        public IEnumerable<Parameter> Parameters
        {
            get { return _filteredParameters; }
            set
            {
                _filteredParameters = value
                    .Where(p => Filters.GetActiveFilters().All(f => f(p)))
                    .OrderByDescending(p => p, parameterComparer)
                    .ToArray();
                RaisePropertyChanged("Parameters");
            }
        }

        /*private WitnessSet witnesses;
        private IEnumerable<Witness> _filteredWitnesses;
        private IEnumerable<Witness> _distinctWitnesses;
        public IEnumerable<Witness> Witnesses
        {
            get { return _filteredWitnesses; }
            set
            {
                if (value == _filteredWitnesses)
                    return;

                _filteredWitnesses = value;

                _distinctWitnesses = _filteredWitnesses.Distinct().OrderByDescending(w => w, witnessComparer).ToArray();

                RaisePropertyChanged("DisplayedItems");
            }
        }
        public IEnumerable<Witness> DistinctWitnesses { get { return _distinctWitnesses; } }*/

        public RegulatoryContext RegulatoryContext { get; private set; }
        public TimeSerie TimeSerie { get; private set; }

        public ParameterFilters Filters { get; private set; }
        //public WitnessFilter WitnessFilters { get; private set; }

        public ParameterStatistic Statistic { get; private set; }

        #region Display Properties

        private bool _parametersDisplayed;
        public bool ParametersDisplayed
        {
            get { return _parametersDisplayed; }
            private set
            {
                if (value == _parametersDisplayed)
                    return;

                _parametersDisplayed = value;

                if (_parametersDisplayed)
                {
                    parameterComparer.ResetPriorities();
                    lastSelectedSortOption = SortOption.Cost;
                }
                else
                {
                    //witnessComparer.ResetPriorities();
                    //lastSelectedSortOption = SortOption.Parameter_count;
                }

                RaisePropertyChanged("ParametersDisplayed");
                //RaisePropertyChanged("WitnessesDisplayed");
                RaisePropertyChanged("DisplayedItems");
                RaisePropertyChanged("SortOptions");
            }
        }
        //public bool WitnessesDisplayed { get { return !_parametersDisplayed; } }

        /*public IEnumerable<object> DisplayedItems
        {
            get
            {
                if (ParametersDisplayed)
                    return Parameters;
                else
                    return DistinctWitnesses;
            }
        }*/

        #endregion

        #region Sort Properties

        private class ParameterComparer : IComparer<Parameter>
        {
            private List<SortOption> sortOptionPriorities;

            public ParameterComparer()
            {
                sortOptionPriorities = new List<SortOption>();
                ResetPriorities();
            }

            public void ResetPriorities()
            {
                sortOptionPriorities.Clear();
                sortOptionPriorities.Add(SortOption.Cost);
                sortOptionPriorities.Add(SortOption.Possible_behaviours);
                sortOptionPriorities.Add(SortOption.Robustness);
            }

            public void MoveOptionToTop(SortOption option)
            {
                sortOptionPriorities.Remove(option);
                sortOptionPriorities.Insert(0, option);
            }

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

                foreach (SortOption o in sortOptionPriorities)
                {
                    switch (o)
                    {
                        case SortOption.Cost:
                            {
                                if (x.Cost != y.Cost)
                                    return (y.Cost - x.Cost);
                                break;
                            }
                        case SortOption.Possible_behaviours:
                            {
                                if (x.PossibleBehaviours != y.PossibleBehaviours)
                                    return (x.PossibleBehaviours - y.PossibleBehaviours);
                                break;
                            }
                        case SortOption.Robustness:
                            {
                                if (x.Robustness != y.Robustness)
                                    return Math.Sign(x.Robustness - y.Robustness);
                                break;
                            }
                        default: break;
                    }
                }

                return 0;
            }
        }

        /*private class WitnessComparer : IComparer<Witness>
        {
            private List<SortOption> sortOptionPriorities;

            public WitnessComparer()
            {
                sortOptionPriorities = new List<SortOption>();
                ResetPriorities();
            }

            public void ResetPriorities()
            {
                sortOptionPriorities.Clear();
                sortOptionPriorities.Add(SortOption.Length);
                sortOptionPriorities.Add(SortOption.Parameter_Count);
            }

            public void MoveOptionToTop(SortOption option)
            {
                sortOptionPriorities.Remove(option);
                sortOptionPriorities.Insert(0, option);
            }

            public int Compare(Witness x, Witness y)
            {
                if (x == null)
                {
                    if (y == null)
                        return 0;

                    return (-1);
                }

                if (y == null)
                    return 1;

                foreach (SortOption o in sortOptionPriorities)
                {
                    switch (o)
                    {
                        case SortOption.Length:
                            {
                                if (x.Length != y.Length)
                                    return (y.Length - x.Length);
                                break;
                            }
                        case SortOption.Parameter_Count:
                            {
                                if (x.ParameterCount != y.ParameterCount)
                                    return (x.ParameterCount - y.ParameterCount);
                                break;
                            }
                        default: break;
                    }
                }

                return 0;
            }
        }*/

        private ParameterComparer parameterComparer = new ParameterComparer();
        //private WitnessComparer witnessComparer = new WitnessComparer();

        public IEnumerable<object> SortOptions
        {
            get
            {
                return sortOptionHeaders.Keys
                    .Where(o => ((parameterSortOptions.Contains(o) == ParametersDisplayed) && (o != lastSelectedSortOption)))
                    .Select(o => new
                    {
                        Label = sortOptionHeaders[o],
                        SortCommand = new RelayCommand(() =>
                        {
                            if (ParametersDisplayed)
                                parameterComparer.MoveOptionToTop(o);
                            else
                                //witnessComparer.MoveOptionToTop(o);

                            lastSelectedSortOption = o;

                            Refresh(true);
                            RaisePropertyChanged("SortOptions");
                        })
                    });
            }
        }

        private SortOption lastSelectedSortOption;

        #endregion

        public ParameterSet()
        {
            //witnesses = new WitnessSet();

            Filters = new ParameterFilters(this);
            //WitnessFilters = new WitnessFilter(this);

            Statistic = new ParameterStatistic(this);

            ParametersDisplayed = true;
        }

        public void Refresh(bool tighten)
        {
            if (tighten)
                Parameters = Parameters;
            else
                Parameters = parameters;

            //Witnesses = Parameters.SelectMany(p => p.Witnesses);

            RaisePropertyChanged("ParameterCount");
            //RaisePropertyChanged("WitnessCount");
            //RaisePropertyChanged("DistinctWitnessCount");
            Statistic.Refresh();
        }

        /*public void RefreshWitnesses(bool tighten)
        {
            foreach(Parameter p in parameters)
                p.FilterWitnesses(WitnessFilters.GetActiveFilters().ToArray(), tighten);

            Witnesses = Parameters.SelectMany(p => p.Witnesses).ToArray();

            Filters.SetBounds(parameters);

            RaisePropertyChanged("WitnessCount");
            RaisePropertyChanged("DistinctWitnessCount");
        }*/

        public bool HasRegulatoryContext { get { return (RegulatoryContext != null); } }
        public bool HasTimeSerie { get { return (TimeSerie != null); } }

        #region Commands

        #region Import/Export Commands

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
                    Parameters = parameters;
                    //Witnesses = Parameters.SelectMany(p => p.Witnesses).ToArray();

                    if (RegulatoryContext == null)
                        RegulatoryContext = new RegulatoryContext(parameters.First().Values.Count(),
                            parameters.First().Transitions.First().InitialState.ActivityLevels.Count());

                    foreach (Parameter p in parameters)
                        p.CreateMask(RegulatoryContext);

                    Filters.SetBounds(parameters);
                    //WitnessFilters.SetBounds(witnesses);

                    Filters.RefreshRegulatoryContexts();

                    Refresh(false);
                }
            }
        }

        /*private RelayCommand _exportWitnessesCommand;
        public ICommand ExportWitnessesCommand
        {
            get
            {
                _exportWitnessesCommand = _exportWitnessesCommand ?? new RelayCommand(() => ExportWitnesses());
                return _exportWitnessesCommand;
            }
        }*/

        /*private void ExportWitnesses()
        {
            using (System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog())
            {
                diag.Filter = "Witness Plotter File|*.WPF";
                diag.Title = "Save as";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    ExportWitnesses(diag.FileName);
            }
        }*/

        /*private void ExportWitnesses(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if (TimeSerie != null)
                    sw.WriteLine(TimeSerie);

                foreach (Witness w in Parameters.SelectMany(p => p.Witnesses))
                    sw.WriteLine(w.ToString());
            }
        }*/

        /*private RelayCommand _exportGraphCommand;
        public ICommand ExportGraphCommand
        {
            get
            {
                _exportGraphCommand = _exportGraphCommand ?? new RelayCommand(() => ExportGraph());
                return _exportGraphCommand;
            }
        }*/

        /*private void ExportGraph()
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
        }*/

        private RelayCommand _expressionGraphCommand;
        public ICommand ExpressionGraphCommand
        {
            get
            {
                _expressionGraphCommand = _expressionGraphCommand ?? new RelayCommand(() => ExportExpressionGraph());
                return _expressionGraphCommand;
            }
        }

        private void ExportExpressionGraph()
        {
            new ExpressionGraphPropertiesWindow()
            {
                DataContext = new ExpressionGraphBuilder(RegulatoryContext, TimeSerie, Parameters)
            }.Show();
        }

        #endregion

        private RelayCommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                _exitCommand = _exitCommand ?? new RelayCommand(() => Application.Current.Shutdown());
                return _exitCommand;
            }
        }

        #region Model & Time Serie Commands

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

        #region Data Commands

        private RelayCommand _displayParametersCommand;
        public ICommand DisplayParametersCommand
        {
            get
            {
                _displayParametersCommand = _displayParametersCommand ?? new RelayCommand(() => ParametersDisplayed = true);
                return _displayParametersCommand;
            }
        }

        private RelayCommand _displayWitnessesCommand;
        public ICommand DisplayWitnessesCommand
        {
            get
            {
                _displayWitnessesCommand = _displayWitnessesCommand ?? new RelayCommand(() => ParametersDisplayed = false);
                return _displayWitnessesCommand;
            }
        }

        #endregion

        #endregion
    }
}
