using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Parameter_Filter
{
    public class WitnessFilter : ObservableObject
    {
        private ParameterSet set;
        private Subject<Unit> filterChange;

        private bool _parameterCountLowerBoundActive;
        public bool ParameterCountLowerBoundActive
        {
            get { return _parameterCountLowerBoundActive; }
            set
            {
                if (value == _parameterCountLowerBoundActive)
                    return;

                _parameterCountLowerBoundActive = value;
                RaisePropertyChanged("ParameterCountLowerBoundActive");

                set.RefreshWitnesses(_parameterCountLowerBoundActive);
            }
        }

        private bool _parameterCountUpperBoundActive;
        public bool ParameterCountUpperBoundActive
        {
            get { return _parameterCountUpperBoundActive; }
            set
            {
                if (value == _parameterCountUpperBoundActive)
                    return;

                _parameterCountUpperBoundActive = value;
                RaisePropertyChanged("ParameterCountUpperBoundActive");

                set.RefreshWitnesses(_parameterCountUpperBoundActive);
            }
        }

        private int _minimumParameterCount;
        public int MinimumParameterCount
        {
            get { return _minimumParameterCount; }
            set
            {
                if (value == _minimumParameterCount)
                    return;

                _minimumParameterCount = value;

                if (LowerParameterCountBound < _minimumParameterCount)
                    LowerParameterCountBound = _minimumParameterCount;
                if (UpperParameterCountBound < _minimumParameterCount)
                    UpperParameterCountBound = _minimumParameterCount;

                RaisePropertyChanged("MinimumParameterCount");
            }
        }

        private int _maximumParameterCount;
        public int MaximumParameterCount
        {
            get { return _maximumParameterCount; }
            set
            {
                if (value == _maximumParameterCount)
                    return;

                _maximumParameterCount = value;

                if (LowerParameterCountBound > _maximumParameterCount)
                    LowerParameterCountBound = _maximumParameterCount;
                if (UpperParameterCountBound > _maximumParameterCount)
                    UpperParameterCountBound = _maximumParameterCount;

                RaisePropertyChanged("MaximumParameterCount");
            }
        }

        private int _lowerParameterCountBound;
        public int LowerParameterCountBound
        {
            get { return _lowerParameterCountBound; }
            set
            {
                if (value == _lowerParameterCountBound)
                    return;

                _lowerParameterCountBound = value;
                RaisePropertyChanged("LowerParameterBound");

                if (ParameterCountLowerBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private int _upperParameterCountBound;
        public int UpperParameterCountBound
        {
            get { return _upperParameterCountBound; }
            set
            {
                if (value == _upperParameterCountBound)
                    return;

                _upperParameterCountBound = value;
                RaisePropertyChanged("UpperParameterBound");

                if (ParameterCountUpperBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private bool _lengthLowerBoundActive;
        public bool LengthLowerBoundActive
        {
            get { return _lengthLowerBoundActive; }
            set
            {
                if (value == _lengthLowerBoundActive)
                    return;

                _lengthLowerBoundActive = value;
                RaisePropertyChanged("LengthLowerBoundActive");

                set.RefreshWitnesses(_lengthLowerBoundActive);
            }
        }

        private bool _lengthUpperBoundActive;
        public bool LengthUpperBoundActive
        {
            get { return _lengthUpperBoundActive; }
            set
            {
                if (value == _lengthUpperBoundActive)
                    return;

                _lengthUpperBoundActive = value;
                RaisePropertyChanged("LengthUpperBoundActive");

                set.RefreshWitnesses(_lengthUpperBoundActive);
            }
        }

        private int _shortestWitness;
        public int ShortestWitness
        {
            get { return _shortestWitness; }
            set
            {
                if (value == _shortestWitness)
                    return;

                _shortestWitness = value;

                if (LowerLengthBound < _shortestWitness)
                    LowerLengthBound = _shortestWitness;
                if (UpperLengthBound < _shortestWitness)
                    UpperLengthBound = _shortestWitness;

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

                if (LowerLengthBound > _shortestWitness)
                    LowerLengthBound = _shortestWitness;
                if (UpperLengthBound > _shortestWitness)
                    UpperLengthBound = _shortestWitness;

                _longestWitness = value;
                RaisePropertyChanged("LongestWitness");
            }
        }

        private int _lowerLengthBound;
        public int LowerLengthBound
        {
            get { return _lowerLengthBound; }
            set
            {
                if (value == _lowerLengthBound)
                    return;

                _lowerLengthBound = value;
                RaisePropertyChanged("LowerLengthBound");

                if (LengthLowerBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }

        private int _upperLengthBound;
        public int UpperLengthBound
        {
            get { return _upperLengthBound; }
            set
            {
                if (value == _upperLengthBound)
                    return;

                _upperLengthBound = value;
                RaisePropertyChanged("UpperLengthBound");

                if (LengthUpperBoundActive)
                    filterChange.OnNext(Unit.Default);
            }
        }
        
        public WitnessFilter(ParameterSet set)
        {
            this.set = set;
            filterChange = new Subject<Unit>();

            filterChange
                .Throttle(TimeSpan.FromSeconds(0.5))
                .Subscribe(_ => set.RefreshWitnesses(false));
        }

        public void SetBounds(WitnessSet witnessSet)
        {
            MinimumParameterCount = witnessSet.Witnesses.Select(w => w.Parameters.Count()).Min();
            MaximumParameterCount = witnessSet.Witnesses.Select(w => w.Parameters.Count()).Max();

            ShortestWitness = witnessSet.Witnesses.Select(w => w.Length).Min();
            LongestWitness = witnessSet.Witnesses.Select(w => w.Length).Max();
        }

        public IEnumerable<Predicate<Witness>> GetActiveFilters()
        {
            if (ParameterCountLowerBoundActive)
                yield return (w => (w.Parameters.Count() >= LowerParameterCountBound));
            if (ParameterCountUpperBoundActive)
                yield return (w => (w.Parameters.Count() <= UpperParameterCountBound));

            if (LengthLowerBoundActive)
                yield return (w => (w.Length >= LowerLengthBound));
            if (LengthUpperBoundActive)
                yield return (w => (w.Length <= UpperLengthBound));
        }
    }
}
