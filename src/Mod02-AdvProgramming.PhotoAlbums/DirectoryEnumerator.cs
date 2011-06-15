// -----------------------------------------------------------------------
// <copyright file="DirectoryEnumerator.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Linq;

namespace Mod02_AdvProgramming.PhotoAlbums
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DirectoryEnumerator
    {
        public static IEnumerable<FileInfo> GetDirectoryEnumeratorEager(DirectoryInfo di)
        {
            var list = new List<FileInfo>();
            try
            {
                list.AddRange(di.GetFiles());
            }
            catch
            {}
            foreach(var dir in di.GetDirectories())
            {
                try
                {
                    list.AddRange(GetDirectoryEnumeratorEager(dir));
                }
                catch
                { }
            }
            return list;
        }

        public static IEnumerable<FileInfo> GetDirectoryEnumeratorLazy(DirectoryInfo di)
        {
            return di.EnumerateFiles().Where(fi=> fi.CanAccess()).Concat(di.EnumerateDirectories().Where(dir=>dir.CanAccess()).SelectMany(dir => GetDirectoryEnumeratorLazy(dir)));

/*            foreach (var file in di.EnumerateFiles())
            {
                yield return file;
            }
            foreach (var dir in di.EnumerateDirectories())
            {
                foreach (var file in GetDirectoryEnumeratorLazy(dir))
                {
                    yield return file;
                }
            }*/
        }

        public static IEnumerable<FileInfo> GetDirectoryEnumerator(this DirectoryInfo di)
        {
            return GetDirectoryEnumeratorLazy(di);
        }

        public static IEnumerable<string> GetDirectoryImages(this DirectoryInfo di)
        {
            return di.GetDirectoryEnumerator().Where(fi => string.Compare(fi.Extension, ".jpg", true) == 0 || 
                string.Compare(fi.Extension, ".png", true) == 0
                || string.Compare(fi.Extension, ".gif", true) == 0).Select(fi => fi.FullName);
        }

        public static IEnumerable<FileInfo> GetDirectoryImagesFileInfo(this DirectoryInfo di)
        {
            return di.GetDirectoryEnumerator().Where(fi => string.Compare(fi.Extension, ".jpg", true) == 0 ||
                string.Compare(fi.Extension, ".png", true) == 0
                || string.Compare(fi.Extension, ".gif", true) == 0);
        }

        public static bool CanAccess(this FileInfo fi)
        {
            return !fi.Attributes.HasFlag(FileAttributes.System);
        }

        public static bool CanAccess(this DirectoryInfo di)
        {
            try
            {
                di.GetDirectoryEnumerator();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
