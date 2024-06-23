using LogConverter.Entities;
using LogConverter.Utils;

namespace LogConverter.Services
{
    public class ConvertLogService
    {
        public LogEntity Parse(string logLine)
        {
            var fields = logLine.Split('|');

            if (fields.Length != 5)
            {
                throw new InvalidLogFormatException("Log line does not contain the expected number of fields.");
            }

            var requestDetails = fields[3].Split(' ');
            if (requestDetails.Length != 3)
            {
                throw new InvalidLogFormatException("HTTP request part of the log is not in the expected format.");
            }

            return new LogEntity
            {
                RequestId = int.Parse(fields[0]),
                StatusCode = int.Parse(fields[1]),
                CacheStatus = fields[2] == "INVALIDATE" ? "REFRESH_HIT" : fields[2],
                HttpMethod = requestDetails[0].Replace("\"", ""),
                UriPath = requestDetails[1],
                HttpVersion = requestDetails[2].Replace("\"", ""),
                TimeTaken = double.Parse(fields[4])
            };
        }

        public string ConvertLog(string logContent)
        {
            string[] lines = logContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime now = DateTime.Now;
            string header = $"#Version: 1.0\n#Date: {now:dd/MM/yyyy HH:mm:ss}\n#Fields: provider http-method status-code uri-path time-taken response-size cache-status\n";

            List<string> convertedLines = new List<string>();

            foreach (var line in lines)
            {
                try
                {
                    LogEntity entry = Parse(line);
                    convertedLines.Add(entry.ConvertToAgoraFormat());
                }
                catch (InvalidLogFormatException ex)
                {
                    Console.WriteLine($"Warning: Skipping invalid log line. {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: An unexpected error occurred while parsing log line. {ex.Message}");
                }
            }

            return header + string.Join("\n", convertedLines);
        }

        public async Task<string> DownloadLogFile(string url)
        {
            using HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);
        }

        public async Task SaveToFile(string path, string content)
        {
            await File.WriteAllTextAsync(path, content);
        }
    }
}
