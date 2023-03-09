using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ShapeGrammar
{
    /// <summary>
    /// The base class for a rule in a shape grammar.
    /// </summary>
    /// <typeparam name="TState">
    /// The type of state that this rule works with.
    /// </typeparam>
    [Serializable]
    public abstract class SGRule<TState> : IWorkspaceItem
        where TState : ISGState
    {
        #region Constructors

        protected SGRule()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected SGRule(SerializationInfo info, StreamingContext context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            IsEnabled = reader.GetValue<bool>(nameof(IsEnabled));
        }

        #endregion

        #region Methods

        public abstract IReadOnlyList<ISGMarker> FindMarkers(TState state);

        public virtual ISGMarker FindMarker(TState state)
        {
            var markers = FindMarkers(state);

            if (markers == null)
            {
                return null;
            }
            if (markers.Count == 0)
            {
                return null;
            }

            return markers[0];
        }

        public abstract IReadOnlyList<ISGMarker> Rewrite(
            TState state, object marker
        );

        public virtual IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>();

        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(IsEnabled), IsEnabled);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public bool IsEnabled { get; set; } = DefaultIsEnabled;

        public static IReadOnlyList<ISGMarker> NoMarker =>
            new List<ISGMarker>();

        #endregion

        #region Constants

        public const bool DefaultIsEnabled = true;

        #endregion
    }

    /// <summary>
    /// The base class for a rule in a shape grammar that works with
    /// a given marker type.
    /// </summary>
    /// <typeparam name="TState">
    /// The type of state that this rule works with.
    /// </typeparam>
    /// <typeparam name="TMarker">
    /// The type of marker that this rule works with.
    /// </typeparam>
    [Serializable]
    public abstract class SGRule<TState, TMarker> : SGRule<TState>
        where TState : ISGState
        where TMarker : ISGMarker
    {
        #region Constructors

        protected SGRule() : base()
        { }

        protected SGRule(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }

        #endregion

        #region Methods

        public override IReadOnlyList<ISGMarker> FindMarkers(TState state)
        {
            return state.Markers.Where(marker => marker is TMarker).ToList();
        }

        public abstract IReadOnlyList<ISGMarker> Rewrite(
            TState state, TMarker marker
        );

        public override IReadOnlyList<ISGMarker> Rewrite(
            TState state, object marker
        )
        {
            return Rewrite(state, (TMarker)marker);
        }

        #endregion
    }
}
