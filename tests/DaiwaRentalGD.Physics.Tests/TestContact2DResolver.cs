using System.Linq;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Physics.Tests
{
    public class TestContact2DResolver
    {
        #region Constructors

        public TestContact2DResolver(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestSerialization()
        {
            var so0 = new SceneObject(
                new Component[]
                {
                    new TransformComponent(),
                    new CollisionBody2DComponent(
                        new[]
                        {
                            new Polygon(
                                new[]
                                {
                                    new Point(0.0, 0.0, 0.0),
                                    new Point(1.0, 0.0, 0.0),
                                    new Point(0.0, 1.0, 0.0)
                                }
                            )
                        }
                    ),
                    new RigidBody2DComponent(),
                }
            )
            {
                Name = "Scene Object 0"
            };

            var so1 = new SceneObject(
                new Component[]
                {
                    new TransformComponent(),
                    new CollisionBody2DComponent(
                        new[]
                        {
                            new Polygon(
                                new[]
                                {
                                    new Point(0.2, 0.2, 0.0),
                                    new Point(1.2, 0.2, 0.0),
                                    new Point(1.2, 1.2, 0.0)
                                }
                            )
                        }
                    ),
                    new RigidBody2DComponent(),
                }
            )
            {
                Name = "Scene Object 1"
            };

            var resolver = new Contact2DResolver();

            resolver.AddRigidBody(so0);
            resolver.AddRigidBody(so1);

            resolver.ResolveContacts();

            var workspace = new JsonWorkspace();
            workspace.Save(resolver);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedResolver =
                workspaceCopy.Load<Contact2DResolver>(resolver.ItemInfo.Uid);

            Assert.Equal(
                resolver.MaxNumOfIterations,
                deserializedResolver.MaxNumOfIterations
            );
            Assert.Equal(
                resolver.Epsilon,
                deserializedResolver.Epsilon
            );

            Assert.Equal(
                resolver.RigidBodies.Select(rb => rb.Name),
                deserializedResolver.RigidBodies.Select(rb => rb.Name)
            );

            foreach (var (contact, deserializedContact) in
                resolver.Contacts.Zip(deserializedResolver.Contacts))
            {
                Assert.Equal(
                    contact.RigidBody0.Name,
                    deserializedContact.RigidBody0.Name
                );
                Assert.Equal(
                    contact.RigidBody1.Name,
                    deserializedContact.RigidBody1.Name
                );

                Assert.Equal(
                    contact.PenetrationPoint,
                    deserializedContact.PenetrationPoint
                );
                Assert.Equal(
                    contact.TargetPoint,
                    deserializedContact.TargetPoint
                );

                Assert.Equal(
                    contact.IsResolved,
                    deserializedContact.IsResolved
                );
                Assert.Equal(
                    contact.Epsilon,
                    deserializedContact.Epsilon
                );
            }
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
