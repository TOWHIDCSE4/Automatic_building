using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Physics
{
    /// <summary>
    /// Represents a contact between a pair of 2D rigid bodies.
    /// </summary>
    [Serializable]
    public class Contact2D : IWorkspaceItem
    {
        #region Constructors

        private Contact2D()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">
        /// Contains data of a serialized <see cref="Contact2D"/> instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected Contact2D(SerializationInfo info, StreamingContext context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            RigidBody0 = reader.GetValue<SceneObject>(nameof(RigidBody0));
            RigidBody1 = reader.GetValue<SceneObject>(nameof(RigidBody1));

            PenetrationPoint = new Point(
                reader.GetValue<List<double>>(nameof(PenetrationPoint))
            );
            TargetPoint = new Point(
                reader.GetValue<List<double>>(nameof(TargetPoint))
            );

            IsResolved = reader.GetValue<bool>(nameof(IsResolved));
            Epsilon = reader.GetValue<double>(nameof(Epsilon));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a list of <see cref="Contact2D"/>s between
        /// a pair of 2D rigid bodies.
        /// </summary>
        /// 
        /// <param name="rb0">
        /// The first 2D rigid body represented by
        /// a <see cref="SceneObject"/>.
        /// </param>
        /// <param name="rb1">
        /// The second 2D rigid body represented by
        /// a <see cref="SceneObject"/>.
        /// </param>
        /// <param name="epsilon">
        /// Tolerance used to deal with precision errors during
        /// the calculation.
        /// </param>
        /// 
        /// <returns>
        /// A list of <see cref="Contact2D"/>s specifying the contacts
        /// between <paramref name="rb0"/> and <paramref name="rb1"/>.
        /// If <paramref name="rb0"/> or <paramref name="rb1"/> does not have
        /// a <see cref="RigidBody2DComponent"/> attached, no contact will be
        /// detected and an empty list will be returned.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="rb0"/> or <paramref name="rb1"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="rb0"/> and <paramref name="rb1"/>
        /// do not have the same <see cref="SceneObject.Parent"/>.
        /// </exception>
        public static IReadOnlyList<Contact2D> GetContacts(
            SceneObject rb0, SceneObject rb1,
            double epsilon = DefaultEpsilon
        )
        {
            if (rb0 == null)
            {
                throw new ArgumentNullException(nameof(rb0));
            }

            if (rb1 == null)
            {
                throw new ArgumentNullException(nameof(rb1));
            }

            if (rb1.Parent != rb0.Parent)
            {
                throw new ArgumentException(
                    $"{nameof(rb1.Parent)} and {nameof(rb0.Parent)} " +
                    "must be the same",
                    nameof(rb1)
                );
            }

            if (epsilon < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(epsilon),
                    $"{nameof(epsilon)} cannot be negative"
                );
            }

            List<Contact2D> contacts = new List<Contact2D>();

            var pointContacts01 = GetPointContacts01(rb0, rb1, epsilon);
            var pointContacts10 = GetPointContacts01(rb1, rb0, epsilon);

            contacts.AddRange(pointContacts01);
            contacts.AddRange(pointContacts10);

            return contacts;
        }

        private static IReadOnlyList<Contact2D> GetPointContacts01(
            SceneObject rb0, SceneObject rb1, double epsilon
        )
        {
            var rbc0 = rb0.GetComponent<RigidBody2DComponent>();

            if (rbc0 == null || !rbc0.IsValid || rbc0.IsSpace)
            {
                return new List<Contact2D>();
            }

            var rbc1 = rb1.GetComponent<RigidBody2DComponent>();

            if (rbc1 == null || !rbc1.IsValid)
            {
                return new List<Contact2D>();
            }

            var contacts = new List<Contact2D>();

            var cps0 =
                rbc0.CollisionBody2DComponent
                .GetTransformedCollisionPolygons();

            var cps1 =
                rbc1.CollisionBody2DComponent
                .GetTransformedCollisionPolygons();

            foreach (var cp0 in cps0)
            {
                foreach (var point0 in cp0.Points)
                {
                    foreach (var cp1 in cps1)
                    {
                        Contact2D contact = GetPointContact(
                            rb0, rb1, point0, cp1, epsilon
                        );

                        if (contact != null)
                        {
                            contacts.Add(contact);
                        }
                    }
                }
            }

            return contacts;
        }

        private static Contact2D GetPointContact(
            SceneObject rb0, SceneObject rb1,
            Point point0, Polygon cp1,
            double epsilon
        )
        {
            var closestPointInfo = cp1.GetClosestPointOnEdge(point0);

            Point closestPoint = closestPointInfo.Item3;

            bool isPoint0InCp1 = cp1.IsPointInside2D(point0);

            var rbc1 = rb1.GetComponent<RigidBody2DComponent>();

            if (rbc1.IsSpace && isPoint0InCp1)
            {
                return null;
            }

            if (!rbc1.IsSpace && !isPoint0InCp1)
            {
                return null;
            }

            if (point0.GetDistance(closestPoint) < epsilon)
            {
                return null;
            }

            Contact2D contact = new Contact2D
            {
                RigidBody0 = rb0,
                RigidBody1 = rb1,
                PenetrationPoint = point0,
                TargetPoint = closestPoint,
                IsResolved = false,
                Epsilon = epsilon
            };

            return contact;
        }

        /// <summary>
        /// Resolves this <see cref="Contact2D"/>. <see cref="IsResolved"/>
        /// will be marked <see langword="true"/> after the resolution.
        /// </summary>
        /// 
        /// <remarks>
        /// This <see cref="Contact2D"/> will be resolved if:
        /// 
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     Both <see cref="RigidBody0"/> and <see cref="RigidBody0"/>
        ///     have an <see cref="RigidBody2DComponent.EffectiveMass"/> of
        ///     infinity as a result of
        ///     being static (<see cref="RigidBody2DComponent.IsStatic"/>) or
        ///     a space (<see cref="RigidBody2DComponent.IsSpace"/>)
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     The non-static rigid body (bodies) is (are) moved so that
        ///     <see cref="PenetrationPoint"/> is no longer inside
        ///     <see cref="RigidBody1"/>`
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        public void Resolve()
        {
            var rbc0 = RigidBody0.GetComponent<RigidBody2DComponent>();

            var rbc1 = RigidBody1.GetComponent<RigidBody2DComponent>();

            double mass0 = rbc0.EffectiveMass;
            double mass1 = rbc1.EffectiveMass;

            if (mass0 == double.PositiveInfinity &&
                mass1 == double.PositiveInfinity)
            {
                IsResolved = true;
                return;
            }

            double scale0;

            if (mass0 == 0.0 && mass1 == 0.0)
            {
                scale0 = 0.5;
            }
            else if (mass1 == double.PositiveInfinity)
            {
                scale0 = 1.0;
            }
            else if (mass0 == double.PositiveInfinity)
            {
                scale0 = 0.0;
            }
            else
            {
                scale0 = mass1 / (mass0 + mass1);
            }

            double scale1 = 1.0 - scale0;

            var resolutionVector =
                TargetPoint.Vector - PenetrationPoint.Vector;

            var resolutionVector0 =
                resolutionVector * (scale0 + Epsilon);
            var resolutionVector1 =
                -resolutionVector * (scale1 + Epsilon);

            var tc0 = rbc0.TransformComponent;

            tc0.Transform.Tx += resolutionVector0[0];
            tc0.Transform.Ty += resolutionVector0[1];

            var tc1 = rbc1.TransformComponent;

            tc1.Transform.Tx += resolutionVector1[0];
            tc1.Transform.Ty += resolutionVector1[1];

            IsResolved = true;
        }

        /// <inheritdoc/>
        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new[] { RigidBody0, RigidBody1 };

        /// <inheritdoc/>
        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(RigidBody0), RigidBody0);
            writer.AddValue(nameof(RigidBody1), RigidBody1);

            writer.AddValue(
                nameof(PenetrationPoint), PenetrationPoint.AsList()
            );
            writer.AddValue(nameof(TargetPoint), TargetPoint.AsList());

            writer.AddValue(nameof(IsResolved), IsResolved);
            writer.AddValue(nameof(Epsilon), Epsilon);
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public WorkspaceItemInfo ItemInfo { get; }

        /// <summary>
        /// The first 2D rigid body involved in the contact.
        /// </summary>
        public SceneObject RigidBody0 { get; private set; }

        /// <summary>
        /// The second 2D rigid body involved in the contact.
        /// </summary>
        public SceneObject RigidBody1 { get; private set; }

        /// <summary>
        /// The <see cref="Point"/> on <see cref="RigidBody0"/> that
        /// penetrates <see cref="RigidBody1"/>.
        /// </summary>
        public Point PenetrationPoint { get; private set; }

        /// <summary>
        /// The <see cref="Point"/> that <see cref="PenetrationPoint"/>
        /// should be moved to in order to resolve
        /// this <see cref="Contact2D"/>.
        /// </summary>
        public Point TargetPoint { get; private set; }

        /// <summary>
        /// Whether this <see cref="Contact2D"/> is resolved.
        /// </summary>
        public bool IsResolved { get; private set; }

        /// <summary>
        /// Tolerance used to deal with precision errors in
        /// collision detection (<see cref="GetContacts"/>) and
        /// collision response (<see cref="Resolve"/>).
        /// </summary>
        public double Epsilon { get; private set; }

        #endregion

        #region Constants

        /// <summary>
        /// Default value for <see cref="Epsilon"/> and passed to
        /// <see cref="GetContacts"/>.
        /// </summary>
        public const double DefaultEpsilon = 1e-6;

        #endregion
    }
}
