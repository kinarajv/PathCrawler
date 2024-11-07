namespace DirectoryScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("========================================");
                Console.WriteLine("          FILE CRAWLER PHP, JS & HTML   ");
                Console.WriteLine("========================================");
                Console.ResetColor();
                Console.WriteLine("Crafted with care by Kinara");
                Thread.Sleep(1000);
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Crawl all files only");
                Console.WriteLine("2. Crawl all files and fix it");
                Console.WriteLine("3. Check for Dangerous Connections");
                Console.WriteLine("4. Quit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                PathChecker program = new PathChecker();

                if (choice == "1")
                {
                    Console.WriteLine("Extracting PHP / JS / HTML URLs: relative_file_paths.csv");
                    Thread.Sleep(1000);
                    program.ToCSV();
                    Console.WriteLine("\t Enter to continue");
                    Console.ReadLine();
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Extracting and fixing PHP / JS / HTML URLs: relative_file_paths.csv and extracted_urls.csv");
                    Thread.Sleep(1000);
                    program.ToCSV();
                    Thread.Sleep(1000);
                    program.ExtractAjaxUrls("relative_file_paths.csv");
                    program.UpdateJavaScriptFiles();
                    Console.WriteLine("\t Enter to continue");
                    Console.ReadLine();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Checking for Dangerous Connections...");
                    program.ToCSV();
                    Thread.Sleep(1000);
                    ConnectionDangerChecker checker = new ConnectionDangerChecker();
                    checker.CheckForDangerousConnections("relative_file_paths.csv");
                    Console.WriteLine("\t Enter to continue");
                    Console.ReadLine();
                }
                else if (choice == "4")
                {
                    Console.WriteLine("Quitting... Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
    }
}