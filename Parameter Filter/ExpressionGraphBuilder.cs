using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Parameter_Filter
{
    class ExpressionGraphBuilder : ObservableObject
    {
        private RegulatoryContext regulatoryContext;
        private TimeSerie timeSerie;
        private IEnumerable<Witness> witnesses;

        private IEnumerable<string> _species;
        public IEnumerable<string> Species
        {
            get { return _species; }
            private set
            {
                if (value == _species)
                    return;

                _species = value;
                RaisePropertyChanged("Species");
            }
        }

        private int _selectedSpecie;
        public int SelectedSpecie
        {
            get { return _selectedSpecie; }
            set
            {
                if (value == _selectedSpecie)
                    return;

                _selectedSpecie = value;
                RaisePropertyChanged("SelectedSpecie");
            }
        }

        private bool _gridVisible;
        public bool GridVisible
        {
            get { return _gridVisible; }
            set
            {
                if (value == _gridVisible)
                    return;

                _gridVisible = value;
                RaisePropertyChanged("GridVisible");
            }
        }

        public ExpressionGraphBuilder(RegulatoryContext regulatoryContext, TimeSerie timeSerie, IEnumerable<Witness> witnesses)
        {
            this.regulatoryContext = regulatoryContext;
            this.timeSerie = timeSerie;
            this.witnesses = witnesses;

            Species = regulatoryContext.Species;
            SelectedSpecie = 0;

            GridVisible = true;
        }

        #region Commands

        private RelayCommand _exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                _exportCommand = _exportCommand ?? new RelayCommand(() => Export());
                return _exportCommand;
            }
        }

        private void Export()
        {
            using (System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog())
            {
                diag.Filter = "Scalable Vector Graphics|*.svg";
                diag.Title = string.Format("Save {0} expression graph as", Species.ElementAt(SelectedSpecie));

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SpecieExpressionGraph graph = new SpecieExpressionGraph(SelectedSpecie, regulatoryContext, timeSerie);

                    foreach (Witness w in witnesses)
                        graph.MapWitness(w);

                    graph.Export(diag.FileName, GridVisible);
                }
            }

            foreach (Window w in Application.Current.Windows)
                if (!w.Equals(Application.Current.MainWindow))
                    w.Close();
        }

        #endregion
    }
}
