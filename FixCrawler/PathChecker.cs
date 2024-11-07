using System.Text.RegularExpressions;

namespace DirectoryScraper;

class PathChecker
{
    private const string CsvFileName = "relative_file_paths.csv";
    private const string ExtractedUrlsCsvFileName = "extracted_urls.csv";
    private static readonly string[] SupportedExtensions = { ".js", ".php", ".html" };

    private List<string> allRelativePaths = new List<string>();

    public void ToCSV()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string csvFilePath = Path.Combine(currentDirectory, CsvFileName);
        int totalFiles = 0;

        allRelativePaths = Directory.EnumerateFiles(currentDirectory, "*", SearchOption.AllDirectories)
          .Select(filePath => Path.GetRelativePath(currentDirectory, filePath).Replace('\\', '/'))
          .ToList();

        Console.WriteLine("Loading... Creating CSV file with relative paths.");
        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            foreach (string relativeFilePath in allRelativePaths)
            {
                writer.WriteLine(relativeFilePath);
                totalFiles++;
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Total: {totalFiles} files. Relative file paths saved to: " + csvFilePath);
        Console.ResetColor();
    }

    public void ExtractAjaxUrls(string csvPath)
    {
        int totalChecked = 0;
        int foundCount = 0;
        int notFoundCount = 0;
        int fixedCount = 0;

        var relativePaths = File.ReadAllLines(csvPath);

        Console.WriteLine("Loading... Extracting AJAX URLs from files.");
        using (StreamWriter urlWriter = new StreamWriter(ExtractedUrlsCsvFileName))
        {
            urlWriter.WriteLine("File,Extracted URL,Status,Suggestion");

            foreach (var relativePath in relativePaths)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

                if (File.Exists(fullPath) && IsSupportedExtension(fullPath))
                {
                    ExtractUrlsFromFile(fullPath, urlWriter, ref totalChecked, ref foundCount, ref notFoundCount, ref fixedCount);
                }
                if (totalChecked % 10 == 0) 
                {
                    Console.WriteLine($"Checked {totalChecked} files...");
                }
            }
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Total files checked: {totalChecked}");
        Console.WriteLine($"Correct Path: {foundCount}");
        Console.WriteLine($"Not Found: {notFoundCount}");
        Console.WriteLine($"Fixed: {fixedCount}");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Cant Fixed : {notFoundCount-fixedCount} :( ");
        Console.ResetColor();
    }

    private void ExtractUrlsFromFile(string fullPath, StreamWriter urlWriter, ref int totalChecked, ref int foundCount, ref int notFoundCount, ref int fixedCount)
    {
        string[] lines = File.ReadAllLines(fullPath);
        bool fileUpdated = false;

        Console.WriteLine($"Loading... Processing file: {fullPath}");
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            var matches = Regex.Matches(line, @"url\s*:\s*['\']([^'\']+)['\']|require\(\s*['\']([^'\']+)['\']\)|<link\s+.*?href=\'([^\']+)\'|<script\s+.*?src=\'([^\']+)\'|require_once\(\s*['\']([^'\']+)['\']\)|require\(\s*['\']([^'\']+)['\']\)", RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string extractedPath = match.Groups[1].Value;
                if (string.IsNullOrEmpty(extractedPath))
                    extractedPath = match.Groups[2].Value;
                if (string.IsNullOrEmpty(extractedPath))
                    extractedPath = match.Groups[3].Value;
                if (string.IsNullOrEmpty(extractedPath))
                    extractedPath = match.Groups[4].Value;
                if (string.IsNullOrEmpty(extractedPath))
                    extractedPath = match.Groups[5].Value;
                if (string.IsNullOrEmpty(extractedPath))
                    extractedPath = match.Groups[6].Value;

                string status = allRelativePaths.Contains(extractedPath) ? "Found" : "Not Found";
                string suggestion = "";

                if (status == "Found")
                {
                    foundCount++;
                }
                else
                {
                    notFoundCount++;
                    var similarPath = allRelativePaths.FirstOrDefault(p => p.Equals(extractedPath, StringComparison.OrdinalIgnoreCase));
                    if (similarPath != null && similarPath != extractedPath)
                    {
                        suggestion = $"{similarPath}";
                        line = line.Replace(extractedPath, suggestion);
                        fileUpdated = true;
                        fixedCount++;
                    }
                }

                urlWriter.WriteLine($"{fullPath},{extractedPath},{status},{suggestion}");
            }

            lines[i] = line;
        }

        totalChecked++;
        
        if (fileUpdated)
        {
            File.WriteAllLines(fullPath, lines);
        }
    }

    private bool IsSupportedExtension(string filePath)
    {
        return SupportedExtensions.Any(ext => ext.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase));
    }
}
