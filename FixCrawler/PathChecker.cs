using System.Text.RegularExpressions;

namespace DirectoryScraper;

class PathChecker
    {
        private const string CsvFileName = "relative_file_paths.csv";
        private const string ExtractedUrlsCsvFileName = "extracted_urls.csv";
        private static readonly string[] SupportedExtensions = { ".js", ".php" };

        private List<string> allRelativePaths = new List<string>();

        public void ToCSV()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string csvFilePath = Path.Combine(currentDirectory, CsvFileName);

            Console.WriteLine("Current directory: " + currentDirectory);

            allRelativePaths = Directory.EnumerateFiles(currentDirectory, "*", SearchOption.AllDirectories)
              .Select(filePath => Path.GetRelativePath(currentDirectory, filePath).Replace('\\', '/'))
              .ToList();

            using (StreamWriter writer = new StreamWriter(csvFilePath))
            {
                foreach (string relativeFilePath in allRelativePaths)
                {
                    writer.WriteLine(relativeFilePath);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("File Found : " + relativeFilePath);
                    Console.ResetColor();
                }
            }

            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("Relative file paths saved to: " + csvFilePath);
            Console.ResetColor();
        }

        public void ExtractAjaxUrls(string csvPath)
        {
            var relativePaths = File.ReadAllLines(csvPath);

            using (StreamWriter urlWriter = new StreamWriter(ExtractedUrlsCsvFileName))
            {
                urlWriter.WriteLine("File,Extracted URL,Status,Suggestion");

                foreach (var relativePath in relativePaths)
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

                    if (File.Exists(fullPath) && IsSupportedExtension(fullPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Process File " + fullPath);
                        Console.ResetColor();
                        ExtractUrlsFromFile(fullPath, urlWriter);
                    }
                }
            }
        }

        private void ExtractUrlsFromFile(string fullPath, StreamWriter urlWriter)
        {
            Console.WriteLine("Processing file: " + fullPath);

            string[] lines = File.ReadAllLines(fullPath);

            foreach (string line in lines)
            {
                var matches = Regex.Matches(line, @"url\s*:\s*['\']([^'\']+)['\']|require\(\s*['\']([^'\']+)['\']\)", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    string extractedPath = !string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[1].Value : match.Groups[2].Value;
                    string status = allRelativePaths.Contains(extractedPath) ? "Found" : "Not Found";
                    string suggestion = "";
                    if (status == "Not Found")
                    {
                        var similarPath = allRelativePaths.FirstOrDefault(p => p.Equals(extractedPath, StringComparison.OrdinalIgnoreCase));
                        if (similarPath != null && similarPath != extractedPath)
                        {
                            suggestion = $"{similarPath}";
                        }
                    }
                    urlWriter.WriteLine($"{fullPath},{extractedPath},{status},{suggestion}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Extracted URL: {extractedPath} from file {fullPath} - Status: {status}");
                    Console.ResetColor();
                }
            }
        }

        private bool IsSupportedExtension(string filePath)
        {
            return SupportedExtensions.Any(ext => ext.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase));
        }
    }