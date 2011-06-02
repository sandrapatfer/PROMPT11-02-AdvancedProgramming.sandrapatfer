using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mod02_AdvProgramming.Assignments.Tests2
{
    [TestClass]
    public class Ex6Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            int[] list = { 1, 2, 3, 1, 5 };
            IEnumerable<int> result = list.MyWhere(i => i == 1);
            Assert.AreEqual(2, result.Count());
        }
        [TestMethod]
        public void TestMethod2()
        {
            int[] list = { 1, 2 };
            IEnumerable<int> result = list.MySelect(i => i * 2);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, result.ElementAt(0));
            Assert.AreEqual(4, result.ElementAt(1));
        }

        [TestMethod]
        public void TestMethod3()
        {
            int[] list1 = { 1, 2 };
            int[] list2 = { 1, 2 };
            IEnumerable<int> result = list1.MyConcat(list2);
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void TestMethod4()
        {
            int[] list = { 1, 2, 3, 1, 5 };
            int result = list.MyFirst(i => i > 2);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestMethod5()
        {
            int[] list = { 1, 2, 3, 1, 5 };
            int result = list.MyLast(i => i < 2);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TotalsByCountrySortedByCountryFirstElementTest()
        {
            var totalsByCountry = Ex5.TotalsByCountrySortedByCountry();
            var firstElem = totalsByCountry.First();
            Assert.AreEqual("Argentina", firstElem.Country);
            Assert.AreEqual(3, firstElem.NumCustomers);
            Assert.AreEqual((decimal)8119.10, firstElem.TotalSales);
        }
    }
}
