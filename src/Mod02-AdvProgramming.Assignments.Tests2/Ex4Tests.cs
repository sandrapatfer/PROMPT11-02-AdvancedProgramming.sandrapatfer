namespace Mod02_AdvProgramming.Assignments.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Ex4Tests {
        [TestMethod]
        public void CountRepeatedShouldReturnAPairSequenceWhenThereAreRepeatedElements() {
            // Arrange
            int[] v = { 0, 1, 1, 2, 3, 3, 3, 4, 5 };

            // Act
            IEnumerable<Ex4.Pair<int, int>> repeated = Ex4.CountRepeated(v);

            // Assert
            Assert.AreEqual(2, repeated.Count());
            Assert.AreEqual(1, repeated.ElementAt(0).t);
            Assert.AreEqual(2, repeated.ElementAt(0).u);
            Assert.AreEqual(3, repeated.ElementAt(1).t);
            Assert.AreEqual(3, repeated.ElementAt(1).u);
        }

        [TestMethod]
        public void CountRepeatedShouldReturnAnEmptySequenceWhenThereAreNoRepeatedElements()
        {
            // Arrange
            int[] v = { 0, 1, 2, 3, 4, 5, 2, 1 };

            // Act
            IEnumerable<Ex4.Pair<int, int>> repeated = Ex4.CountRepeated(v);


            // Assert
            Assert.AreEqual(0, repeated.Count());
        }

        [TestMethod]
        public void CountRepeatedShouldReturnAnEmptySequenceForANullSequence()
        {
            // Arrange

            // Act
            IEnumerable<Ex4.Pair<int, int>> repeated = Ex4.CountRepeated<int>(null);

            // Assert
            Assert.AreEqual(0, repeated.Count());
        }
    }
}