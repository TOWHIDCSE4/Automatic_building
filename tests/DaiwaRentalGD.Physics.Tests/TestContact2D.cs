using System.Linq;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Physics.Tests
{
    public class TestContact2D
    {
        #region Constructors

        public TestContact2D(ITestOutputHelper outputHelper)
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

            var contacts = Contact2D.GetContacts(so0, so1);

            var workspace = new JsonWorkspace();

            foreach (var contact in contacts)
            {
                workspace.Save(contact);
            }

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedContacts = contacts.Select(
                contact => workspaceCopy.Load<Contact2D>(contact.ItemInfo.Uid)
            );

            foreach (var (contact, deserializedContact) in
                contacts.Zip(deserializedContacts))
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
