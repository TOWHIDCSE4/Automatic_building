using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ShapeGrammar
{
    /// <summary>
    /// The base class for a shape grammar.
    /// </summary>
    /// <typeparam name="TState">
    /// The type of state that the shape grammar manipulates.
    /// </typeparam>
    [Serializable]
    public abstract class ShapeGrammar<TState> : IWorkspaceItem
        where TState : ISGState
    {
        #region Constructors

        protected ShapeGrammar()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected ShapeGrammar(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _rules.AddRange(reader.GetValues<SGRule<TState>>(nameof(Rules)));
        }

        #endregion

        #region Methods

        public void AddRule(SGRule<TState> rule)
        {
            InsertRule(_rules.Count, rule);
        }

        public void InsertRule(int ruleIndex, SGRule<TState> rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            _rules.Insert(ruleIndex, rule);
        }

        public bool ReplaceRule(
            SGRule<TState> oldRule, SGRule<TState> newRule
        )
        {
            if (newRule == null)
            {
                throw new ArgumentNullException(nameof(newRule));
            }

            int oldRuleIndex = _rules.IndexOf(oldRule);

            if (oldRuleIndex == -1) { return false; }

            _rules.RemoveAt(oldRuleIndex);
            _rules.Insert(oldRuleIndex, newRule);

            return true;
        }

        public bool RemoveRule(SGRule<TState> rule)
        {
            return _rules.Remove(rule);
        }

        public int GetRuleIndex(SGRule<TState> rule)
        {
            return _rules.IndexOf(rule);
        }

        public void ClearRules()
        {
            _rules.Clear();
        }

        public abstract bool IsTerminated(TState state);

        public void Run(TState state)
        {
            while (!IsTerminated(state))
            {
                bool didAnyRewrite = false;

                foreach (SGRule<TState> rule in Rules)
                {
                    if (!rule.IsEnabled) { continue; }

                    var marker = rule.FindMarker(state);

                    if (marker == null) { continue; }

                    var rewrittenMarkers = rule.Rewrite(state, marker);

                    state.Markers.Remove(marker);

                    foreach (var rewrittenMarker in rewrittenMarkers)
                    {
                        state.Markers.Add(rewrittenMarker);
                    }

                    didAnyRewrite = true;
                    break;
                }

                if (!didAnyRewrite)
                {
                    break;
                }
            }
        }

        public virtual IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Concat(Rules);

        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValues(nameof(Rules), _rules);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public IReadOnlyList<SGRule<TState>> Rules => _rules;

        #endregion

        #region Member variables

        private readonly List<SGRule<TState>> _rules =
            new List<SGRule<TState>>();

        #endregion
    }
}
