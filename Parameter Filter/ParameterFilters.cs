using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Input;

namespace Parameter_Filter
{
    public class ParameterFilters : ObservableObject
    {
        private ParameterSet set;
        private Subject<Unit> filterChange;

        private int _minimumWitnessCount;
        public int MinimumWitnessCount
        {
            get { return _minimumWitnessCount; }
            private set
            {
                if (value == _minimumWitnessCount)
                    return;

                _minimumWitnessCount = value;

                if (LowerWitnessCountBound < _minimumWitnessCount)
                    LowerWitnessCountBound = _minimumWitnessCount;
                if (UpperWitnessCountBound < _minimumWitnessCount)
                    UpperWitnessCountBound = _minimumWitnessCount;

                RaisePropertyChanged("MinimumWitnessCount");
            }
        }

        private int _maximumWitnessCount;
        public int MaximumWitnessCount
        {
            get { return _maximumWitnessCount; }
            private set
            {
                if (value == _maximumWitnessCount)
                    return;

                _maximumWitnessCount = value;

                if (LowerWitnessCountBound > _maximumWitnessCount)
                    LowerWitnessCountBound = _maximumWitnessCount;
                if (UpperWitnessCountBound > _maximumWitnessCount)
                    UpperWitnessCountBound = _maximumWitnessCount;

                RaisePropertyChanged("MaximumWitnessCount");
            }
        }

        private bool _witnessCountLowerBoundActive;
        public bool WitnessCountLowerBoundActive
        {
            get { return _witnessCountLowerBoundActive; }
            private set
            {
                if (value == _witnessCountLowerBoundActive)
                    return;

                _witnessCountLowerBoundActive = value;
                RaisePropertyChanged("WitnessCountLowerBoundActive");

                set.Refresh(_witnessCountLowerBoundActive);
            }
        }

        private bool _witnessCountUpperBoundActive;
        public bool WitnessCountUpperBoundActive
        {
            get { return _witnessCountUpperBoundActive; }
            private set
            {
                if (value == _witnessCountUpperBoundActive)
                    return;

                _witnessCountUpperBoundActive = value;
                RaisePropertyChanged("WitnessCountUpperBoundActive");

                set.Refresh(_witnessCountUpperBoundActive);
            }
        }

        private int _lowerWitnessCountBound;
        public int LowerWitnessCountBound
        {
            get { return _lowerWitnessCountBound; }
            set
            {
                if (value == _lowerWitnessCountBound)
                    return;

                _lowerWitnessCountBound = value;
                RaisePropertyChanged("LowerWitnessCountBound");

                if (WitnessCountLowerBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private int _upperWitnessCountBound;
        public int UpperWitnessCountBound
        {
            get { return _upperWitnessCountBound; }
            set
            {
                if (value == _upperWitnessCountBound)
                    return;

                _upperWitnessCountBound = value;
                RaisePropertyChanged("UpperWitnessCountBound");

                if (WitnessCountUpperBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private int _shortestWitness;
        public int ShortestWitness
        {
            get { return _shortestWitness; }
            private set
            {
                if (value == _shortestWitness)
                    return;

                _shortestWitness = value;

                if (LowerWitnessLengthBound < _shortestWitness)
                    LowerWitnessLengthBound = _shortestWitness;
                if (UpperWitnessLengthBound < _shortestWitness)
                    UpperWitnessLengthBound = _shortestWitness;

                if (LowerAllWitnessLengthBound < _shortestWitness)
                    LowerAllWitnessLengthBound = _shortestWitness;
                if (UpperAllWitnessLengthBound < _shortestWitness)
                    UpperAllWitnessLengthBound = _shortestWitness;

                RaisePropertyChanged("ShortestWitness");
            }
        }

        private int _longestWitness;
        public int LongestWitness
        {
            get { return _longestWitness; }
            private set
            {
                if (value == _longestWitness)
                    return;

                _longestWitness = value;

                if (LowerWitnessLengthBound > _longestWitness)
                    LowerWitnessLengthBound = _longestWitness;
                if (UpperWitnessLengthBound > _longestWitness)
                    UpperWitnessLengthBound = _longestWitness;

                if (LowerAllWitnessLengthBound > _longestWitness)
                    LowerAllWitnessLengthBound = _longestWitness;
                if (UpperAllWitnessLengthBound > _longestWitness)
                    UpperAllWitnessLengthBound = _longestWitness;

                RaisePropertyChanged("LongestWitness");
            }
        }

        private bool _witnessLengthLowerBoundActive;
        public bool WitnessLengthLowerBoundActive
        {
            get { return _witnessLengthLowerBoundActive; }
            private set
            {
                if (value == _witnessLengthLowerBoundActive)
                    return;

                _witnessLengthLowerBoundActive = value;
                RaisePropertyChanged("WitnessLengthLowerBoundActive");

                set.Refresh(_witnessLengthLowerBoundActive);
            }
        }

        private bool _witnessLengthUpperBoundActive;
        public bool WitnessLengthUpperBoundActive
        {
            get { return _witnessLengthUpperBoundActive; }
            private set
            {
                if (value == _witnessLengthUpperBoundActive)
                    return;

                _witnessLengthUpperBoundActive = value;
                RaisePropertyChanged("WitnessLengthUpperBoundActive");

                set.Refresh(_witnessLengthUpperBoundActive);
            }
        }

        private int _lowerWitnessLengthBound;
        public int LowerWitnessLengthBound
        {
            get { return _lowerWitnessLengthBound; }
            set
            {
                if (value == _lowerWitnessLengthBound)
                    return;

                _lowerWitnessLengthBound = value;
                RaisePropertyChanged("LowerWitnessLengthBound");

                if (WitnessLengthLowerBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private int _upperWitnessLengthBound;
        public int UpperWitnessLengthBound
        {
            get { return _upperWitnessLengthBound; }
            set
            {
                if (value == _upperWitnessLengthBound)
                    return;

                _upperWitnessLengthBound = value;
                RaisePropertyChanged("UpperWitnessLengthBound");

                if (WitnessLengthUpperBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private bool _allWitnessLengthLowerBoundActive;
        public bool AllWitnessLengthLowerBoundActive
        {
            get { return _allWitnessLengthLowerBoundActive; }
            private set
            {
                if (value == _allWitnessLengthLowerBoundActive)
                    return;

                _allWitnessLengthLowerBoundActive = value;
                RaisePropertyChanged("AllWitnessLengthLowerBoundActive");

                set.Refresh(_allWitnessLengthLowerBoundActive);
            }
        }

        private bool _allWitnessLengthUpperBoundActive;
        public bool AllWitnessLengthUpperBoundActive
        {
            get { return _allWitnessLengthUpperBoundActive; }
            private set
            {
                if (value == _allWitnessLengthUpperBoundActive)
                    return;

                _allWitnessLengthUpperBoundActive = value;
                RaisePropertyChanged("AllWitnessLengthUpperBoundActive");

                set.Refresh(_allWitnessLengthUpperBoundActive);
            }
        }

        private int _lowerAllWitnessLengthBound;
        public int LowerAllWitnessLengthBound
        {
            get { return _lowerAllWitnessLengthBound; }
            set
            {
                if (value == _lowerAllWitnessLengthBound)
                    return;

                _lowerAllWitnessLengthBound = value;
                RaisePropertyChanged("LowerAllWitnessLengthBound");

                if (AllWitnessLengthLowerBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private int _upperAllWitnessLengthBound;
        public int UpperAllWitnessLengthBound
        {
            get { return _upperAllWitnessLengthBound; }
            set
            {
                if (value == _upperAllWitnessLengthBound)
                    return;

                _upperAllWitnessLengthBound = value;
                RaisePropertyChanged("UpperAllWitnessLengthBound");

                if (AllWitnessLengthUpperBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        public IEnumerable<string> ContextConstraintTypes { get { return ContextConstraint.contextConstraintTypeConversion.Keys; } }

        public IEnumerable<string> RegulatoryContexts { get { return ((set.RegulatoryContext == null) ? new string[] { } : set.RegulatoryContext.ContextMasks); } }

        private int _constrainedContextIndex;
        public int ConstrainedContextIndex
        {
            get { return _constrainedContextIndex; }
            set
            {
                if (value == _constrainedContextIndex)
                    return;

                _constrainedContextIndex = value;

                ContextMinimum = set.RegulatoryContext.MinimalValues[set.RegulatoryContext.ContextMasks[_constrainedContextIndex]];
                ContextMaximum = set.RegulatoryContext.MaximalValues[set.RegulatoryContext.ContextMasks[_constrainedContextIndex]];

                RaisePropertyChanged("ConstrainedContextIndex");
            }
        }

        private int _contextMinimum;
        public int ContextMinimum
        {
            get { return _contextMinimum; }
            set
            {
                if (_contextMinimum == value)
                    return;

                _contextMinimum = value;

                if (CurrentContextConstraint < _contextMinimum)
                    CurrentContextConstraint = _contextMinimum;

                RaisePropertyChanged("ContextMinimum");
            }
        }

        private int _contextMaximum;
        public int ContextMaximum
        {
            get { return _contextMaximum; }
            set
            {
                if (_contextMaximum == value)
                    return;

                _contextMaximum = value;

                if (CurrentContextConstraint > _contextMaximum)
                    CurrentContextConstraint = _contextMaximum;

                RaisePropertyChanged("ContextMaximum");
            }
        }

        private string _contextConstraintType;
        public string ContextConstraintType
        {
            get { return _contextConstraintType; }
            set
            {
                if (value == _contextConstraintType)
                    return;

                _contextConstraintType = value;
                RaisePropertyChanged("ContextConstraintType");
            }
        }

        private int _currentContextConstraint;
        public int CurrentContextConstraint
        {
            get { return _currentContextConstraint; }
            set
            {
                if (value == _currentContextConstraint)
                    return;

                _currentContextConstraint = value;
                RaisePropertyChanged("CurrentContextConstraint");
            }
        }

        public ObservableCollection<ContextConstraint> ContextConstraints { get; private set; }

        public ParameterFilters(ParameterSet set)
        {
            this.set = set;
            filterChange = new Subject<Unit>();

            filterChange
                .Throttle(TimeSpan.FromSeconds(0.5))
                .Subscribe(_ => set.Refresh(false));

            WitnessCountLowerBoundActive = false;
            WitnessCountUpperBoundActive = false;

            ContextConstraints = new ObservableCollection<ContextConstraint>();
        }

        public void SetBounds(IEnumerable<Parameter> parameters)
        {
            MinimumWitnessCount = parameters.Select(p => p.WitnessCount).Min();
            MaximumWitnessCount = parameters.Select(p => p.WitnessCount).Max();

            ShortestWitness = parameters.SelectMany(p => p.Witnesses).Select(w => w.Length).Min();
            LongestWitness = parameters.SelectMany(p => p.Witnesses).Select(w => w.Length).Max();
        }

        public void RefreshRegulatoryContexts()
        {
            RaisePropertyChanged("RegulatoryContexts");
        }

        public IEnumerable<Predicate<Parameter>> GetActiveFilters()
        {
            if (WitnessCountLowerBoundActive)
                yield return (p => p.WitnessCount >= LowerWitnessCountBound);
            if (WitnessCountUpperBoundActive)
                yield return (p => p.WitnessCount <= UpperWitnessCountBound);

            if (WitnessLengthLowerBoundActive)
                yield return (p => p.Witnesses.Any(w => w.Length >= LowerWitnessLengthBound));
            if (WitnessLengthUpperBoundActive)
                yield return (p => p.Witnesses.Any(w => w.Length <= UpperWitnessLengthBound));

            if (AllWitnessLengthLowerBoundActive)
                yield return (p => p.Witnesses.All(w => w.Length >= LowerAllWitnessLengthBound));
            if (AllWitnessLengthUpperBoundActive)
                yield return (p => p.Witnesses.All(w => w.Length <= UpperAllWitnessLengthBound));

            foreach (var filter in ContextConstraints.Select(c => c.GetFilter()))
                yield return filter;
        }

        #region Commands

        RelayCommand _addContextConstraintCommand;
        public ICommand AddContextConstraintCommand
        {
            get
            {
                _addContextConstraintCommand = _addContextConstraintCommand ?? new RelayCommand(() => AddContextCommand());
                return _addContextConstraintCommand;
            }
        }

        private void AddContextCommand()
        {
            ContextConstraints.Add(new ContextConstraint(ConstrainedContextIndex, set.RegulatoryContext.ContextMasks[ConstrainedContextIndex],
                ContextConstraintType, CurrentContextConstraint, this));
            set.Refresh(true);
        }

        public void RemoveContextConstraint(ContextConstraint constraint)
        {
            ContextConstraints.Remove(constraint);
            set.Refresh(false);
        }

        #endregion
    }
}
