namespace Mod02_AdvProgramming.Assignments {
    using System;
    using System.Collections.Generic;

    public class Ex1 {
        public static List<Prm> LessThan<Prm>(ICollection<Prm?> col, Prm r)
            where Prm : struct, IComparable<Prm>
        {
            if (col == null)
                return new List<Prm>();
            else
            {
                List<Prm> newList = new List<Prm>();
                foreach (var p in col)
                {
                    if (p.HasValue && p.Value.CompareTo(r) < 0)
                        newList.Add(p.Value);
                }
                return newList;
            }
        }


    }
}