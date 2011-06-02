using System.Collections;

namespace Mod02_AdvProgramming.Assignments
{
    using System;
    using System.Collections.Generic;

    public class Ex3
    {
        public class FibonacciEnumerator : IEnumerator<int>
        {
            int? m_limit;
            int m_prev;
            int m_current;

            public FibonacciEnumerator(int? limit)
            {
                m_limit = limit;
                m_prev = 1;
                m_current = -1;
            }

            public int Current
            {
                get
                {
                    return m_current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public bool MoveNext()
            {
                if (m_current == -1)
                {
                    if (m_limit.HasValue && m_limit == 0)
                        return false;
                    else
                    {
                        m_current = 0;
                        return true;
                    }
                }
                else if (!m_limit.HasValue || m_limit > 0)
                {
                    m_limit--;
                    int tmp = m_current + m_prev;
                    m_prev = m_current;
                    m_current = tmp;
                    return true;
                }
                else
                    return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            #region IDisposable Members

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class FibonacciSequence1 : IEnumerable<int>
        {
            int? m_limit;

            public FibonacciSequence1()
            {
                m_limit = null;
            }

            public FibonacciSequence1(int limit)
            {
                m_limit = limit;
            }

            #region Implementation of IEnumerable

            public IEnumerator<int> GetEnumerator()
            {
                return new FibonacciEnumerator(m_limit);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        public class FibonacciSequence : IEnumerable<int>
        {
            int? m_limit;

            public FibonacciSequence()
            {
                m_limit = null;
            }

            public FibonacciSequence(int limit)
            {
                m_limit = limit;
            }

            public IEnumerator<int> GetEnumerator()
            {
                int prev = 1;
                int current = -1;

                while (true)
                {
                    if (current == -1)
                    {
                        if (m_limit != null && m_limit == 0)
                            yield break;
                        else
                        {
                            current = 0;
                            yield return current;
                        }
                    }
                    else if (m_limit == null || m_limit > 0)
                    {
                        m_limit--;
                        int tmp = current + prev;
                        prev = current;
                        current = tmp;
                        yield return current;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
