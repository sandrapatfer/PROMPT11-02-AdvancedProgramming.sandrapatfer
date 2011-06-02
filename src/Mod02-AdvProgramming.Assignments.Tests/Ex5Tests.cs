namespace Mod02_AdvProgramming.Assignments.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Mod02_AdvProgramming.Data;
    using System.Linq;

    [TestFixture]
    public class Ex5Tests
    {
        #region Setup AndTearDown methods

        [TestFixtureSetUp]
        void SetUpFixture()
        {
            
        }

        #endregion Setup AndTearDown methods

        #region Test methods

        [Test]
        public void CustomerCountriesSortedShouldReturn21Countries()
        {
            // Arrange

            // Act
            var countries = Ex5.CustomerCountriesSorted();

            // Assert
            Assert.AreEqual(21, countries.Count());
        }



        #endregion
    }
}
