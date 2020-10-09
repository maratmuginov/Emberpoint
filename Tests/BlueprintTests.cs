using Emberpoint.Core.GameObjects.Managers;
using NUnit.Framework;
using Tests.TestObjects.Blueprints;
using Tests.TestObjects.Grids;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;
using NuGet.Frameworks;

namespace Tests
{
    [TestFixture]
    public class BlueprintTests : EmberGridTests
    {
        [SetUp]
        protected override void Setup()
        {
            // Setup a grid based on a blueprint
            _grid = BaseGrid.Create(new BaseBlueprint());
            GridManager.InitializeCustomGrid(_grid);
        }

        [Test]
        public void ConvertBlueprintToCells_DoesNotFail()
        {
            Assert.IsNotNull(_grid.Blueprint);

            var cells = _grid.Blueprint.GetCells();
            Assert.IsNotNull(cells);
            Assert.IsTrue(cells.Length > 0);
        }

        [Test]
        public void RefactoredMethodIdentical()
        {
            var cells = _grid.Blueprint.GetCells();
            var cellsRefactored = _grid.Blueprint.GetCellsRefactored();

            Assert.IsTrue(cells.Length == cellsRefactored.Length);
            Assert.IsTrue(GetComparisons(cells, cellsRefactored).All(b=>b));
        }

        private IEnumerable<bool> GetComparisons(IEnumerable<EmberCell> cells1, IEnumerable<EmberCell> cells2)
        {
            var enumerator1 = cells1.GetEnumerator();
            var enumerator2 = cells2.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                enumerator2.MoveNext();
                yield return enumerator1.Current != null && 
                             enumerator1.Current.Equals(enumerator2.Current);
            }

            enumerator1.Dispose();
            enumerator2.Dispose();
        }

        [Test]
        public void BlueprintToCells_ReturnsCorrectCells_BasedOnTxtFile()
        {
            var blueprintPath = Path.Combine(Constants.Blueprint.TestBlueprintsPath, _grid.Blueprint.GetType().Name + ".txt");
            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            var nullTile = BlueprintTile.Null();
            for (int y = 0; y < _grid.GridSizeY; y++)
            {
                for (int x = 0; x < _grid.GridSizeX; x++)
                {
                    char charValue;
                    if (y >= blueprint.Length || x >= blueprint[y].Length)
                    {
                        charValue = nullTile.Glyph;
                    }
                    else
                    {
                        charValue = blueprint[y][x];
                    }

                    Assert.IsTrue(_grid.GetCell(x, y).Glyph == charValue);
                }
            }
        }
    }
}
