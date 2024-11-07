namespace DirectoryScraper;

class ConnectionDangerChecker
{
  private static readonly string[] DangerousStrings = 
  {
    "mail.smtp2go.com",
    "fmlxprotrax2",
    "y0xK5hXaI6Y5IS0c",
    "fmlx14022",
    "10.250.5.23",
    "10.250.0.229",
    "10.250.0.53",
    "10.250.0.77",
    "smtp.gmail.com",
    "smtp.google.com",
    "10.250.8.93",
    "10.100.0.202",
    "10.100.0.252",
    "10.100.0.202"
  };

  public void CheckForDangerousConnections(string csvPath)
  {
    var relativePaths = File.ReadAllLines(csvPath);
    string dangerousConnectionsCsv = "dangerous_connections.csv";
    int count = 0;
    Console.WriteLine("This program will check for this string : ");
    foreach( string connection in DangerousStrings )
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine(" - " +connection);
      Console.ResetColor();
    }
    Console.WriteLine("Press Enter To Continue... ");
    Console.ReadLine();
    using (StreamWriter writer = new StreamWriter(dangerousConnectionsCsv))
    {
      writer.WriteLine("File,Line,Contained String");
      foreach (var relativePath in relativePaths)
      {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine( "Processing = " + count );
        count++;
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        if (File.Exists(fullPath) && (Path.GetExtension(fullPath).Equals(".js", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(fullPath).Equals(".php", StringComparison.OrdinalIgnoreCase)))
        {
          string[] lines = File.ReadAllLines(fullPath);
          for (int i = 0; i < lines.Length; i++)
          {
            foreach (string dangerousString in DangerousStrings)
            {
              if (lines[i].Contains(dangerousString))
              {
                writer.WriteLine($"{relativePath},{i + 1},{dangerousString}");
              }
            }
          }
        }
      }
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Dangerous connections have been checked and saved to: dangerous_connections.csv");
    Console.ResetColor();
  }
}
