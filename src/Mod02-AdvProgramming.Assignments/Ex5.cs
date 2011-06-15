using System;
using System.Linq;
using Mod02_AdvProgramming.Data;

namespace Mod02_AdvProgramming.Assignments
{
    using System.Collections.Generic;

    public class Ex5
    {
        public class CountryWithCities
        {
            public string Country { get; set; }
            public IEnumerable<string> Cities { get; set; }
        }

        public class CustomerOrders
        {
            public string Customer { get; set; }
            public int NumOrders { get; set; }
            public decimal TotalSales { get; set; }
        }

        public class TotalsByCountry
        {
            public string Country { get; set; }
            public int NumCustomers { get; set; }
            public decimal TotalSales { get; set; }
        }

        public enum PeriodRange
        {
            Year = 12,
            Semester = 6,
            Trimester = 3,
            Month = 1
        }

        public class TotalsByCountryByPeriod : TotalsByCountry
        {
            public string PeriodRange { get; set; }
        }

        public static IEnumerable<string> CustomerCountriesSorted()
        {
            var list = SampleData.LoadCustomersFromXML();

            //return list.Select(c => c.Country).GroupBy(c => c).Select(g => g.Key).OrderBy(c => c);
            return list.Select(c => c.Country).Distinct().OrderBy(c => c);

        }

        public static IEnumerable<CountryWithCities> CustomerCountriesWithCitiesSortedByCountry()
        {
            var list = SampleData.LoadCustomersFromXML();
            return list.GroupBy(c => c.Country).Select(g => new CountryWithCities { Country = g.Key, Cities = g.Select(c => c.City).Distinct() });
        }


        public static IEnumerable<CustomerOrders> CustomerWithNumOrdersSortedByNumOrdersDescending()
        {
            var list = SampleData.LoadCustomersFromXML();
            return list.Select(c => new CustomerOrders { Customer = c.Name, NumOrders = c.Orders.Count(), TotalSales = c.Orders.Sum(o => o.Total) });
        }

        public static IEnumerable<TotalsByCountry> TotalsByCountrySortedByCountry()
        {
            var list = SampleData.LoadCustomersFromXML();
            return list.GroupBy(c => c.Country).
                Select(g => new TotalsByCountry { Country = g.Key, NumCustomers = g.Count(), TotalSales = g.SelectMany(c => c.Orders).Sum(o => o.Total) }).
                OrderBy(c => c.Country);
        }

        static string ConvertDateToPeriod(DateTime dt, PeriodRange p)
        {
            switch (p)
            {
                case PeriodRange.Year:
                    return Convert.ToString(dt.Year);
                case PeriodRange.Semester:
                    return string.Format("{0}_S{1}", dt.Year, dt.Month <= 6 ? "1" : "2");
                case PeriodRange.Trimester:
                    return string.Format("{0}_Q{1}", dt.Year, dt.Month <= 3 ? "1" : dt.Month <= 6 ? "2" : dt.Month <= 9 ? "3" : "4");
                case PeriodRange.Month:
                    return string.Format("{0}_M{1}", dt.Year, dt.Month);
                default:
                    throw new ArgumentException();
            }
        }

        public static IEnumerable<TotalsByCountryByPeriod> TotalsByCountryByPeriodSortedByCountry(PeriodRange periodRange)
        {
            var list = SampleData.LoadCustomersFromXML();

            return list.SelectMany(c => c.Orders, (c, o) => new { Country = c.Country, CustName = c.Name, Order = o }).
                GroupBy(co => co.Country).SelectMany(gc => gc.
                GroupBy(o => ConvertDateToPeriod(o.Order.OrderDate, periodRange)).
                Select(g => new TotalsByCountryByPeriod
                {
                    NumCustomers = g.Select(co => co.CustName).Distinct().Count(),
                    TotalSales = g.Sum(o => o.Order.Total),
                    Country = gc.Key,
                    PeriodRange = g.Key
                }));
        }

    }

}
