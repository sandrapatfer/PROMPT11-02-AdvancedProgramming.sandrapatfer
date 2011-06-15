using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Mod02_AdvProgramming.PhotoAlbums
{
    public class ImagesSequence : IEnumerable<FileInfo>
    {
        string m_path;

        public ImagesSequence(string path)
        {
            m_path = path;
        }

        #region IEnumerable<FileInfo> Members

        public IEnumerator<FileInfo> GetEnumerator()
        {
            return new DirectoryInfo(m_path).GetDirectoryImagesFileInfo().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public IEnumerable<FileInfo> Where(string whereClause)
        {
            string[] parts = whereClause.Split('>');

            if (parts.Length == 2)
            {
                var parameter = ParameterExpression.Parameter(typeof(FileInfo));
                
                var expr = Expression.Lambda(Expression.GreaterThan(
                    Expression.MakeMemberAccess(parameter,
                    typeof(FileInfo).GetProperty(parts[0])),
                    Expression.Constant(System.Convert.ToInt64(parts[1]))), parameter);
                Delegate del = expr.Compile();
                return new DirectoryInfo(m_path).GetDirectoryImagesFileInfo().Where((Func<FileInfo, bool>)del);
            }

            return null;
        }
    }
}
