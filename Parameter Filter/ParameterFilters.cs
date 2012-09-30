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

        private int _minimumPossibleBehaviours;
        public int MinimumPossibleBehaviours
        {
            get { return _minimumPossibleBehaviours; }
            set
            {
                if (value == _minimumPossibleBehaviours)
                    return;

                _minimumPossibleBehaviours = value;

                if (LowerPossibleBehavioursBound < _minimumPossibleBehaviours)
                    LowerPossibleBehavioursBound = _minimumPossibleBehaviours;
                if (UpperPossibleBehavioursBound < _minimumPossibleBehaviours)
                    UpperPossibleBehavioursBound = _minimumPossibleBehaviours;

                RaisePropertyChanged("MinimumPossibleBehaviours");
            }
        }

        private int _maximumPossibleBehaviours;
        public int MaximumPossibleBehaviours
        {
            get { return _maximumPossibleBehaviours; }
            set
            {
                if (value == _maximumPossibleBehaviours)
                    return;

                _maximumPossibleBehaviours = value;

                if (LowerPossibleBehavioursBound > _maximumPossibleBehaviours)
                    LowerPossibleBehavioursBound = _maximumPossibleBehaviours;
                if (UpperPossibleBehavioursBound > _maximumPossibleBehaviours)
                    UpperPossibleBehavioursBound = _maximumPossibleBehaviours;

                RaisePropertyChanged("MaximumPossibleBehaviours");
            }
        }

        private bool _possibleBehavioursLowerBoundActive;
        public bool PossibleBehavioursLowerBoundActive
        {
            get { return _possibleBehavioursLowerBoundActive; }
            set
            {
                if (value == _possibleBehavioursLowerBoundActive)
                    return;

                _possibleBehavioursLowerBoundActive = value;
                RaisePropertyChanged("PossibleBehavioursLowerBoundActive");

                set.Refresh(_possibleBehavioursLowerBoundActive);
            }
        }

        private bool _possibleBehavioursUpperBoundActive;
        public bool PossibleBehavioursUpperBoundActive
        {
            get { return _possibleBehavioursUpperBoundActive; }
            set
            {
                if (value == _possibleBehavioursUpperBoundActive)
                    return;

                _possibleBehavioursUpperBoundActive = value;
                RaisePropertyChanged("PossibleBehavioursUpperBoundActive");

                set.Refresh(_possibleBehavioursUpperBoundActive);
            }
        }

        private int _lowerPossibleBehavioursBound;
        public int LowerPossibleBehavioursBound
        {
            get { return _lowerPossibleBehavioursBound; }
            set
            {
                if (value == _lowerPossibleBehavioursBound)
                    return;

                _lowerPossibleBehavioursBound = value;
                RaisePropertyChanged("LowerPossibleBehavioursBound");

                if (PossibleBehavioursLowerBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private int _upperPossibleBehavioursBound;
        public int UpperPossibleBehavioursBound
        {
            get { return _upperPossibleBehavioursBound; }
            set
            {
                if (value == _upperPossibleBehavioursBound)
                    return;

                _upperPossibleBehavioursBound = value;
                RaisePropertyChanged("UpperPossibleBehavioursBound");

                if (PossibleBehavioursUpperBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        /*private int _shortestWitness;
        public int ShortestWitness
        {
            get { return _shortestWitness; }
            set
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
            set
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
            set
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
            set
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
            set
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
            set
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
        }*/

        private bool _robustnessLowerBoundActive;
        public bool RobustnessLowerBoundActive
        {
            get { return _robustnessLowerBoundActive; }
            set
            {
                if (value == _robustnessLowerBoundActive)
                    return;

                _robustnessLowerBoundActive = value;
                RaisePropertyChanged("RobustnessLowerBoundActive");

                set.Refresh(_robustnessLowerBoundActive);
            }
        }

        private bool _robustnessUpperBoundActive;
        public bool RobustnessUpperBoundActive
        {
            get { return _robustnessUpperBoundActive; }
            set
            {
                if (value == _robustnessUpperBoundActive)
                    return;

                _robustnessUpperBoundActive = value;
                RaisePropertyChanged("RobustnessUpperBoundActive");

                set.Refresh(_robustnessUpperBoundActive);
            }
        }

        private double _lowestRobustness;
        public double LowestRobustness
        {
            get { return _lowestRobustness; }
            set
            {
                if (value == _lowestRobustness)
                    return;

                _lowestRobustness = value;

                if (LowerRobustnessBound < _lowestRobustness)
                    LowerRobustnessBound = _lowestRobustness;
                if (UpperRobustnessBound < _lowestRobustness)
                    UpperRobustnessBound = _lowestRobustness;

                RaisePropertyChanged("LowestRobustness");
            }
        }

        private double _highestRobustness;
        public double HighestRobustness
        {
            get { return _highestRobustness; }
            set
            {
                if (value == _highestRobustness)
                    return;

                _highestRobustness = value;

                if (LowerRobustnessBound > _highestRobustness)
                    LowerRobustnessBound = _highestRobustness;
                if (UpperRobustnessBound > _highestRobustness)
                    UpperRobustnessBound = _highestRobustness;

                RaisePropertyChanged("HighestRobustness");
            }
        }

        private double _lowerRobustnessBound;
        public double LowerRobustnessBound
        {
            get { return _lowerRobustnessBound; }
            set
            {
                if (value == _lowerRobustnessBound)
                    return;

                _lowerRobustnessBound = value;
                RaisePropertyChanged("LowerRobustnessBound");

                if (RobustnessLowerBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private double _upperRobustnessBound;
        public double UpperRobustnessBound
        {
            get { return _upperRobustnessBound; }
            set
            {
                if (value == _upperRobustnessBound)
                    return;

                _upperRobustnessBound = value;
                RaisePropertyChanged("UpperRobustnessBound");

                if (RobustnessUpperBoundActive)
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

                ContextMinimum = set.RegulatoryContext.MinimalValues[string.Join("",
                    set.RegulatoryContext.ContextMasks[_constrainedContextIndex].TakeWhile(c => (c != '{')).ToArray())];
                ContextMaximum = set.RegulatoryContext.MaximalValues[string.Join("",
                    set.RegulatoryContext.ContextMasks[_constrainedContextIndex].TakeWhile(c => (c != '{')).ToArray())];

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

            PossibleBehavioursLowerBoundActive = false;
            PossibleBehavioursUpperBoundActive = false;

            ContextConstraints = new ObservableCollection<ContextConstraint>();
        }

        public void SetBounds(IEnumerable<Parameter> parameters)
        {
            MinimumPossibleBehaviours = parameters.Select(p => p.PossibleBehaviours).Min();
            MaximumPossibleBehaviours = parameters.Select(p => p.PossibleBehaviours).Max();

            //IEnumerable<Witness> witnesses = parameters.SelectMany(p => p.Witnesses);

            /*if (witnesses.Any(x => (x != null)))
            {
                ShortestWitness = witnesses.Select(w => w.Length).Min();
                LongestWitness = witnesses.Select(w => w.Length).Max();
            }
            else
            {
                ShortestWitness = 0;
                LongestWitness = 0;
            }*/

            LowestRobustness = parameters.Select(p => p.Robustness).Min();
            HighestRobustness = parameters.Select(p => p.Robustness).Max();
        }

        public void RefreshRegulatoryContexts()
        {
            RaisePropertyChanged("RegulatoryContexts");
        }

        public IEnumerable<Predicate<Parameter>> GetActiveFilters()
        {
            if (PossibleBehavioursLowerBoundActive)
                yield return (p => p.PossibleBehaviours >= LowerPossibleBehavioursBound);
            if (PossibleBehavioursUpperBoundActive)
                yield return (p => p.PossibleBehaviours <= UpperPossibleBehavioursBound);
            /*
            if (WitnessLengthLowerBoundActive)
                yield return (p => p.Witnesses.Any(w => w.Length >= LowerWitnessLengthBound));
            if (WitnessLengthUpperBoundActive)
                yield return (p => p.Witnesses.Any(w => w.Length <= UpperWitnessLengthBound));

            if (AllWitnessLengthLowerBoundActive)
                yield return (p => p.Witnesses.All(w => w.Length >= LowerAllWitnessLengthBound));
            if (AllWitnessLengthUpperBoundActive)
                yield return (p => p.Witnesses.All(w => w.Length <= UpperAllWitnessLengthBound));
            */
            if (RobustnessLowerBoundActive)
                yield return (p => p.Robustness >= LowerRobustnessBound);
            if (RobustnessUpperBoundActive)
                yield return (p => p.Robustness <= UpperRobustnessBound);

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
