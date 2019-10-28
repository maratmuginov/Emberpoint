using Emberpoint.Core;
using Emberpoint.Core.Objects;
using Emberpoint.Core.Objects.Abstracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class EmberEntityTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            try
            {
                // Setup the engine and create the main window.
                SadConsole.Game.Create(Constants.GameWindowWidth, Constants.GameWindowHeight);

                // Run only one frame for tests
                SadConsole.Game.Instance.RunOneFrame();
            }
            catch (System.NullReferenceException)
            {

            }
            catch (NoAudioHardwareException)
            {
                // Ignore because appveyor is troubled by it
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            try
            {
                SadConsole.Game.Instance.Dispose();
            }
            catch (System.NullReferenceException)
            {

            }
            catch (NoAudioHardwareException)
            {
                // Ignore because appveyor is troubled by it
            }
        }

        [TearDown]
        public void Teardown()
        {
            EntityManager.Clear();
        }

        [Test]
        public void Entity_CannotCreateAtSamePosition()
        {
            var entities = new[]
            {
                EntityManager.Create<ExampleEntity>(new Point(0, 0)),
                EntityManager.Create<ExampleEntity>(new Point(0, 0))
            };

            Assert.IsNotNull(entities[0]);
            Assert.IsNull(entities[1]);
        }

        [Test]
        public void EntityObjectId_IncrementsCorrectly()
        {
            var entities = new[] 
            { 
                EntityManager.Create<ExampleEntity>(new Point(0, 0)),
                EntityManager.Create<ExampleEntity>(new Point(1, 0))
            };
            Assert.AreEqual(0, entities[0].ObjectId);
            Assert.AreEqual(1, entities[1].ObjectId);
        }

        [Test]
        public void Entity_CanMoveTowards()
        {
            // Initialize grid
            var grid = EmberGridTests.BuildCustomGrid(10, 10);
            GridManager.InitializeCustomGrid(grid);

            var entity1 = EntityManager.Create<ExampleEntity>(new Point(0, 0));
            var entity2 = EntityManager.Create<ExampleEntity>(new Point(1, 0));

            Assert.IsFalse(entity1.CanMoveTowards(new Point(0, 0))); // Cannot move to his own position
            Assert.IsFalse(entity2.CanMoveTowards(new Point(0, 0)));

            Assert.IsTrue(entity2.CanMoveTowards(new Point(2, 0)));
            entity2.MoveTowards(new Point(2, 0), true);
            Assert.IsTrue(entity2.Position == new Point(2, 0));
        }

        private class ExampleEntity : EmberEntity
        {
            public ExampleEntity() : base(Color.White, Color.Transparent, '%', 1, 1)
            { }
        }
    }
}
