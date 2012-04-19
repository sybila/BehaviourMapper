using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Parameter_Filter
{
    public class ContextConstraint
    {
        public static readonly Dictionary<string, string> contextConstraintTypeConversion = new Dictionary<string, string>()
        {
            { "exactly", "=" },
            { "at least", ">=" },
            { "at most", "<="}
        };

        private ParameterFilters parent;

        private int index;

        public string Mask { get; private set; }
        public string ConstraintType { get; private set; }
        public int Constraint { get; private set; }

        public ContextConstraint(int index, string type, int constraint, ParameterFilters parent)
        {
            this.parent = parent;

            this.index = index;

            Mask = RegulatoryContext.regulatoryContext.ContextMasks[index];
            ConstraintType = contextConstraintTypeConversion[type];
            Constraint = constraint;
        }

        public Predicate<Parameter> GetFilter()
        {
            return (p => Opp(p.Values.ElementAt(index)));
        }

        private bool Opp(int value)
        {
            switch (ConstraintType)
            {
                case "=": { return (value == Constraint); }
                case ">=": { return (value >= Constraint); }
                case "<=": { return (value <= Constraint); }
                default: return false;
            }
        }

        #region Commands

        private RelayCommand _removeConstraintCommand;
        public ICommand RemoveConstraintCommand
        {
            get
            {
                _removeConstraintCommand = _removeConstraintCommand ?? new RelayCommand(() => RemoveConstraint());
                return _removeConstraintCommand;
            }
        }

        private void RemoveConstraint()
        {
            parent.RemoveContextConstraint(this);
        }

        #endregion
    }
}
