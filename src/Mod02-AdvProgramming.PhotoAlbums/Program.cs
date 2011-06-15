using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Resources;
using System.Linq.Expressions;

namespace Mod02_AdvProgramming.PhotoAlbums
{
    class Program
    {
        static void Main(string[] args)
        {

            // Listing c:\windows desdendant files in as eager way
/*            foreach (FileInfo fileInfo in DirectoryEnumerator.GetDirectoryEnumeratorEager(new DirectoryInfo("c:\\windows")))
            {
                Console.WriteLine(fileInfo.FullName);
            }*/

            // Listing c:\windows desdendant files in as lazy way
            //foreach (FileInfo fileInfo in DirectoryEnumerator.GetDirectoryEnumeratorLazy(new DirectoryInfo("c:\\windows")))
            //{
            //    Console.WriteLine(fileInfo.FullName);
            //}

            // Listing c:\windows desdendant files with an extension method for DirectoryInfo
            //foreach (FileInfo fileInfo in new DirectoryInfo("c:\\windows").GetDirectoryEnumerator())
            //{
            //    Console.WriteLine(fileInfo.FullName);
            //}

            // Listing c:\SPF descendent images
            //foreach (var img in new DirectoryInfo("c:\\SPF").GetDirectoryImages())
            //{
            //    Console.WriteLine(img);
            //}

            string imageBase;
            if (args.Length == 0)
            {
                imageBase = Directory.GetCurrentDirectory();
            }
            else
            {
                imageBase = args[0];
            }

            var seq = new ImagesSequence(@"c:\SPF");
            foreach (var img in seq.Where("Length>10000"))
            {
                Console.WriteLine(string.Format("Name: {0} Size: {1}", img.Name, img.Length));
            }

            //var images = (new DirectoryInfo(imageBase)).GetDirectoryImages().Select(img => new XElement("a", 
            //        new XAttribute("href", img),
            //        new XAttribute("rel", "lightbox-photos"), 
            //        new XAttribute("title", Path.GetFileNameWithoutExtension(img)),
            //        new XElement("img",
            //            new XAttribute("src", img)))).Aggregate(new StringBuilder(""), (str, img)=> str.Append(img));
            //string html = string.Format(Properties.Resources.Template, "The images", images.ToString());
            
            //string htmlFile = imageBase + "\\photos.html";
            //StreamWriter stream = new StreamWriter(htmlFile);
            //stream.Write(html);
            //stream.Close();

            //Console.WriteLine(string.Format("File {0} created!", htmlFile));
        }
    }
}


            //XElement root = new XElement("html",
            //    new XElement("head",
            //        new XElement("Title", "Page")),
            //    new XElement("body",
            //        new XElement("h1", "XXX")));

            //Console.WriteLine(root.ToString());

            //var nodes = XElement.Parse(root.ToString()).Descendants("root").Select(nxml => new { name = nxml.Name, value = nxml.Value });
            //nodes.Aggregate((_, x) => {Console.WriteLine(x.name); return _;});
