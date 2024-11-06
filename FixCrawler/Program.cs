using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace DirectoryScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("          FILE CRAWLER PHP & JS        ️  ");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine("Crafted with care by Kinara");
            Thread.Sleep(1000);
            Console.WriteLine("Extracted PHP / JS url : relative_file_paths.csv");
            Console.WriteLine("Extracted PHP / JS url : extracted_urls.csv");
            Thread.Sleep(1000);
            Console.WriteLine("Hold tight! The crawler is about to start... ");
            Thread.Sleep(3000);
            PathChecker program = new PathChecker();
            program.ToCSV();
            program.ExtractAjaxUrls("relative_file_paths.csv");
        }
    }
}
