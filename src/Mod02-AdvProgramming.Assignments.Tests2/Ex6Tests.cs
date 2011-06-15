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

        [TestMethod]
        public void TestMyTake()
        {
            int[] list = { 1, 2, 3, 1, 5 };
            IEnumerable<int> result = list.MyTake(2);
            Assert.AreEqual(1, result.ElementAt(0));
            Assert.AreEqual(2, result.ElementAt(1));
        }

        [TestMethod]
        public void TestMyZip()
        {
            int[] list1 = { 1, 2, 3, };
            char[] list2 = { '1', '2' };
            var result = list1.MyZip(list2, (t, u) => new { T = t, U = u });
            Assert.AreEqual(1, result.ElementAt(0).T);
            Assert.AreEqual('1', result.ElementAt(0).U);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void TestMyAggregate()
        {
            int[] list = { 1, 2, 3, };
            var result = list.MyAggregate((t1, t2) => t1 + t2);
            Assert.AreEqual(6, result);
        }

        class Person
        {
            public string Name { get; set; }
        }

        class Pet
        {
            public string Name { get; set; }
            public Person Owner { get; set; }
        }

        [TestMethod]
        public void TestMyJoin()
        {
            Person magnus = new Person { Name = "Hedlund, Magnus" };
            Person terry = new Person { Name = "Adams, Terry" };
            Person charlotte = new Person { Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            List<Person> people = new List<Person> { magnus, terry, charlotte };
            List<Pet> pets = new List<Pet> { barley, boots, whiskers, daisy };

            var result = people.Join(pets,
                                            person => person,
                                            pet => pet.Owner,
                                            (person, pet) =>
                                                new { OwnerName = person.Name, Pet = pet.Name });

            Assert.AreEqual(4, result.Count());
            Assert.AreEqual("Hedlund, Magnus", result.ElementAt(0).OwnerName);
            Assert.AreEqual("Adams, Terry", result.ElementAt(1).OwnerName);
            Assert.AreEqual("Adams, Terry", result.ElementAt(2).OwnerName);
            Assert.AreEqual("Weiss, Charlotte", result.ElementAt(3).OwnerName);
        }
    }
}
