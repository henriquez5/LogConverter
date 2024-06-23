using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogConverter.Services;
using LogConverter.Utils;

namespace LogConverter.Tests
{
    [TestClass]
    public class ConvertLogServiceTests
    {
        [TestMethod]
        public void Parse_ValidLogLine_ShouldReturnLogEntity()
        {
            var service = new ConvertLogService();
            string logLine = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";

            var result = service.Parse(logLine);

            Assert.IsNotNull(result);
            Assert.AreEqual(312, result.RequestId);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("HIT", result.CacheStatus);
            Assert.AreEqual("GET", result.HttpMethod);
            Assert.AreEqual("/robots.txt", result.UriPath);
            Assert.AreEqual("HTTP/1.1", result.HttpVersion);
            Assert.AreEqual(100.2, result.TimeTaken);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidLogFormatException))]
        public void Parse_InvalidLogLine_ShouldThrowException()
        {
            var service = new ConvertLogService();
            string logLine = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"";

            service.Parse(logLine);
        }

        [TestMethod]
        public void ConvertLog_ValidContent_ShouldReturnConvertedContent()
        {
            var service = new ConvertLogService();
            string logContent = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2\n101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4";

            string result = service.ConvertLog(logContent);
            DateTime now = DateTime.Now;

            string expectedHeader = $"#Version: 1.0\n#Date: {now:dd/MM/yyyy HH:mm:ss}\n#Fields: provider http-method status-code uri-path time-taken response-size cache-status\n";
            string expectedBody = "\"MINHA CDN\" GET 200 /robots.txt 100 312 HIT\n\"MINHA CDN\" POST 200 /myImages 319 101 MISS";

            string expected = expectedHeader + expectedBody;

            Assert.AreEqual(RemoveDatePart(expected), RemoveDatePart(result));
        }

        private string RemoveDatePart(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "#Date: .*\\n", "");
        }
    }
}
