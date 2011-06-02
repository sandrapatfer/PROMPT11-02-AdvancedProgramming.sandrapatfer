using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mod02_AdvProgramming.Assignments.Tests2
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Ex3Tests
    {
        public Ex3Tests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Private utility methods

        private int CheckFibonacciValues(IEnumerator<int> fsEnum, int max)
        {
            int prev = 1, beforePrev = 0, newValue;

            int i;
            for (i = 0; i < max && fsEnum.MoveNext(); i++)
            {
                newValue = beforePrev + prev;
                Assert.AreEqual(beforePrev, fsEnum.Current);

                beforePrev = prev;
                prev = newValue;
            }

            return i;
        }
        #endregion

        [TestMethod]
        public void FibonacciSequenceWithoutLimitShouldReturnInfiniteAndCorrectValues()
        {
            // Arrange
            Ex3.FibonacciSequence fs = new Ex3.FibonacciSequence();

            // Act
            IEnumerator<int> fsEnum = fs.GetEnumerator();

            // Assert
            // Infinite is a lot. Lets check the first 100. Should be enough.
            const int LIMIT = 100;
            Assert.AreEqual(LIMIT, CheckFibonacciValues(fsEnum, LIMIT));
        }

        [TestMethod]
        public void FibonacciSequenceWithLimitShouldReturnFiniteAndCorrectValues()
        {
            const int LIMIT = 20;
            // Arrange
            Ex3.FibonacciSequence fs = new Ex3.FibonacciSequence(LIMIT);

            // Act
            IEnumerator<int> fsEnum = fs.GetEnumerator();

            // Assert
            Assert.AreEqual(LIMIT, CheckFibonacciValues(fsEnum, LIMIT));
        }

        [TestMethod]
        public void FibonacciSequenceWithLimitShouldReturnAnEmptySequenceWith0Limit()
        {
            // Arrange
            Ex3.FibonacciSequence fs = new Ex3.FibonacciSequence(0);

            // Act
            IEnumerator<int> fsEnum = fs.GetEnumerator();

            // Assert
            Assert.IsFalse(fsEnum.MoveNext());
        }

    }
}
