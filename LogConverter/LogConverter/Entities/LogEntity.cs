namespace LogConverter.Entities
{
    public class LogEntity
    {
        public int RequestId { get; set; }

        public int StatusCode { get; set; }

        public string CacheStatus { get; set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public string HttpVersion { get; set; }

        public double TimeTaken { get; set; }

        public string ConvertToAgoraFormat()
        {
            string provider = "\"MINHA CDN\"";
            string timeTakenRounded = Math.Round(TimeTaken).ToString();
            return $"{provider} {HttpMethod} {StatusCode} {UriPath} {timeTakenRounded} {RequestId} {CacheStatus}";
        }
    }

}
