namespace Mod02_AdvProgramming.Assignments {
    using System;
    using System.Collections.Generic;

    public class Ex4
	{
        public struct Pair<T, U>
		{
            public T t;
            public U u;
            public static Pair<T, U> MakePair(T t, U u)
			{
                return new Pair<T, U>() { t = t, u = u };
            }
        }

        public static IEnumerable<Pair<T, int>> CountRepeated<T>(IEnumerable<T> seq)
		{
            if (seq == null)
                yield break;
            else
            {
                var seqEnum = seq.GetEnumerator();
                if (seqEnum.MoveNext())
                {
                    var currElem = seqEnum.Current;
                    int nRepeated = 1;
                    while (seqEnum.MoveNext())
                    {
                        if (currElem.Equals(seqEnum.Current))
                        {
                            nRepeated++;
                        }
                        else
                        {
                            if (nRepeated > 1)
                                yield return Pair<T, int>.MakePair(currElem, nRepeated);
                            currElem = seqEnum.Current;
                            nRepeated = 1;
                        }
                    }
                }
            }
		}
    }
}