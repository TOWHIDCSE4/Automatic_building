using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Physics
{
    /// <summary>
    /// Resolves <see cref="Contact2D"/>s
    /// generated from a set of 2D rigid bodies
    /// (<see cref="SceneObject"/>s with valid
    /// <see cref="RigidBody2DComponent"/>s).
    /// </summary>
    [Serializable]
    public class Contact2DResolver : IWorkspaceItem
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Contact2DResolver"/>.
        /// </summary>
        public Contact2DResolver()
        {
            ItemInfo = new WorkspaceItemInfo();

            RigidBodies = _rigidBodies.AsReadOnly();
            Contacts = _contacts.AsReadOnly();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">
        /// Contains data of a serialized <see cref="Contact2DResolver"/>
        /// instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected Contact2DResolver(
            SerializationInfo info, StreamingContext context
        )
        {
            RigidBodies = _rigidBodies.AsReadOnly();
            Contacts = _contacts.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _maxNumOfIterations =
                reader.GetValue<int>(nameof(MaxNumOfIterations));

            _epsilon = reader.GetValue<double>(nameof(Epsilon));

            _rigidBodies.AddRange(
                reader.GetValues<SceneObject>(nameof(RigidBodies))
            );

            _contacts.AddRange(reader.GetValues<Contact2D>(nameof(Contacts)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a rigid body <see cref="SceneObject"/> to the end of
        /// <see cref="RigidBodies"/>.
        /// </summary>
        /// 
        /// <param name="rigidBody">
        /// The <see cref="SceneObject"/> to add as a rigid body.
        /// </param>
        /// 
        /// <remarks>
        /// Please see <see cref="InsertRigidBody"/> for possible exceptions.
        /// </remarks>
        public void AddRigidBody(SceneObject rigidBody)
        {
            InsertRigidBody(_rigidBodies.Count, rigidBody);
        }

        /// <summary>
        /// Inserts a rigid body <see cref="SceneObject"/> at
        /// a specified index in <see cref="RigidBodies"/>.
        /// </summary>
        /// 
        /// <param name="rigidBodyIndex">
        /// The index in <see cref="RigidBodies"/> at which to insert
        /// <paramref name="rigidBody"/>.
        /// </param>
        /// <param name="rigidBody">
        /// The <see cref="SceneObject"/> to insert as a rigid body.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="rigidBody"/> is <see langword="null"/>.
        /// </exception>
        public void InsertRigidBody(int rigidBodyIndex, SceneObject rigidBody)
        {
            if (rigidBody == null)
            {
                throw new ArgumentNullException(nameof(rigidBody));
            }

            _rigidBodies.Insert(rigidBodyIndex, rigidBody);
        }

        /// <summary>
        /// Removes a rigid body at a specified index in
        /// <see cref="RigidBodies"/>.
        /// </summary>
        /// 
        /// <param name="rigidBodyIndex">
        /// The index of the rigid body to remove.
        /// </param>
        public void RemoveRigidBody(int rigidBodyIndex)
        {
            _rigidBodies.RemoveAt(rigidBodyIndex);
        }

        /// <summary>
        /// Removes all rigid bodies in <see cref="RigidBodies"/>.
        /// </summary>
        public void ClearRigidBodies()
        {
            _rigidBodies.Clear();
        }

        private IReadOnlyList<Contact2D> GetContacts()
        {
            var contacts = new List<Contact2D>();

            for (int rbIndex0 = 0; rbIndex0 < RigidBodies.Count; ++rbIndex0)
            {
                SceneObject rb0 = RigidBodies[rbIndex0];

                for (int rbIndex1 = 0; rbIndex1 < rbIndex0; ++rbIndex1)
                {
                    SceneObject rb1 = RigidBodies[rbIndex1];

                    var rb0Rb1Contacts =
                        Contact2D.GetContacts(rb0, rb1, Epsilon);

                    contacts.AddRange(rb0Rb1Contacts);
                }
            }

            return contacts;
        }

        public bool HasContact => GetContacts().Any();

        /// <summary>
        /// Detects contacts between <see cref="RigidBodies"/> and
        /// updates <see cref="Contacts"/>.
        /// </summary>
        private void UpdateContacts()
        {
            var contacts = GetContacts();

            _contacts.Clear();

            _contacts.AddRange(contacts);
        }

        /// <summary>
        /// Gets a <see cref="Contact2D"/> from <see cref="Contacts"/>
        /// to resolve at the current iteration in
        /// <see cref="ResolveContacts"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="Contact2D"/> from <see cref="Contacts"/>
        /// to resolve at the current iteration in
        /// <see cref="ResolveContacts"/>.
        /// </returns>
        public Contact2D GetPriorityContact()
        {
            Contact2D priorityContact = null;
            double maxResolutionDistance = double.NegativeInfinity;

            foreach (Contact2D contact in Contacts)
            {
                double resolutionDistance =
                    contact.PenetrationPoint
                    .GetDistance(contact.TargetPoint);

                if (resolutionDistance > maxResolutionDistance)
                {
                    maxResolutionDistance = resolutionDistance;
                    priorityContact = contact;
                }
            }

            return priorityContact;
        }

        /// <summary>
        /// Resolves <see cref="Contact2D"/>s in <see cref="Contacts"/>.
        /// Returns when the number of iterations (i.e. calls to
        /// <see cref="Contact2D.Resolve"/> on <see cref="Contact2D"/>s
        /// in <see cref="Contacts"/>) reaches
        /// <see cref="MaxNumOfIterations"/> , or all contacts are
        /// resolved (i.e. <see cref="Contacts"/> becomes empty).
        /// </summary>
        public void ResolveContacts()
        {
            for (int iter = 0; iter < MaxNumOfIterations; ++iter)
            {
                UpdateContacts();

                if (Contacts.Count == 0)
                {
                    break;
                }

                Contact2D priorityContact = GetPriorityContact();
                priorityContact.Resolve();
            }

            if (Contacts.Count != 0)
            {
                UpdateContacts();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Concat(_rigidBodies)
            .Concat(_contacts);

        /// <inheritdoc/>
        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(MaxNumOfIterations), _maxNumOfIterations);
            writer.AddValue(nameof(Epsilon), _epsilon);

            writer.AddValues(nameof(RigidBodies), _rigidBodies);
            writer.AddValues(nameof(Contacts), _contacts);
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public WorkspaceItemInfo ItemInfo { get; }

        /// <summary>
        /// The maximum number of attempts to resolve
        /// <see cref="Contact2D"/>s in <see cref="Contacts"/>. This is to
        /// avoid infinite loops due to irreconcilable contacts
        /// (e.g. a rigid body stuck between multiple static rigid bodies).
        /// Each call to <see cref="Contact2D.Resolve"/> on
        /// a <see cref="Contact2D"/> in <see cref="Contacts"/> counts as
        /// one iteration.
        /// </summary>
        public int MaxNumOfIterations
        {
            get => _maxNumOfIterations;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxNumOfIterations)} cannot be negative"
                    );
                }

                _maxNumOfIterations = value;
            }
        }

        /// <summary>
        /// Tolerance used to deal with precision errors in
        /// the collision detection and resolution process.
        /// </summary>
        public double Epsilon
        {
            get => _epsilon;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Epsilon)} cannot be negative"
                    );
                }

                _epsilon = value;
            }
        }

        /// <summary>
        /// The list of <see cref="SceneObject"/>s representing rigid bodies
        /// that are involved in the contact detection and resolution
        /// process performed by this <see cref="Contact2DResolver"/>.
        /// <see cref="SceneObject"/>s without a
        /// <see cref="RigidBody2DComponent"/> are simply ignored
        /// during this process.
        /// </summary>
        public IReadOnlyList<SceneObject> RigidBodies { get; }

        /// <summary>
        /// The list of <see cref="Contact2D"/>s detected between
        /// rigid bodies in <see cref="RigidBodies"/> to be resolved.
        /// </summary>
        public IReadOnlyList<Contact2D> Contacts { get; }

        #endregion

        #region Member variables

        private double _epsilon = Contact2D.DefaultEpsilon;

        private int _maxNumOfIterations = DefaultMaxNumOfIterations;

        private readonly List<SceneObject> _rigidBodies =
            new List<SceneObject>();

        private readonly List<Contact2D> _contacts =
            new List<Contact2D>();

        #endregion

        #region Constants

        /// <summary>
        /// Default value for <see cref="MaxNumOfIterations"/>.
        /// </summary>
        public const int DefaultMaxNumOfIterations = 20;

        #endregion
    }
}
