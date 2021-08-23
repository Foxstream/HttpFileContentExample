using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CommandLine;

namespace HttpFileContentExampleConsole
{
    public class Options
    {
        /*
         * file-content-result
         * file-stream-result
         * physical-file
         */
        [Option('u', "url", Required = false, HelpText = "url. For example 'http://172.16.66.20:5000/api/file/file-content-result'", Default = "http://172.16.66.20:5000/api/file/file-content-result")]
        public string Url { get; set; }

        [Option('p', "path", Required = true, HelpText = "File path")]
        public string FilePath { get; set; }

        [Option('r', "rangeSize", Required = false, HelpText = "Range size", Default = 1024 * 1024 * 1)]
        public int RangeSize { get; set; }
    }

    class Program
    {
        private static DateTime _startDate;

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async options =>
            {
                var url = $"{options.Url}?path={options.FilePath}";
                var rangeSize = options.RangeSize;
                long currentPosition = 0;
                Console.WriteLine($"url={url} | rangeSize={rangeSize / 1024 } KBytes");

                using var httpClient = new HttpClient();
                var res = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                var contentLength = res.Content.Headers.ContentLength ?? throw new Exception("Content-Length should not be null");
                Console.WriteLine($"Content-Length={contentLength}");

                _startDate = DateTime.Now;

                while (currentPosition <= contentLength)
                {
                    var (start, end) = CalculateRange(currentPosition, rangeSize, contentLength);
                    httpClient.DefaultRequestHeaders.Range = new RangeHeaderValue(start, end);
                    currentPosition = end + 1;

                    res = await httpClient.GetAsync(url);
                    var stream = await res.Content.ReadAsStreamAsync();
                    var diffTimeInSeconds = (DateTime.Now - _startDate).TotalMilliseconds / 1000;
                    Console.WriteLine($"Received data. Range={start}-{end} | size={stream.Length / 1024} KBytes | bandwidth={currentPosition / 1024 / diffTimeInSeconds} KBytes/s | {currentPosition * 100 / contentLength}%");
                }
            });

            Console.WriteLine($"It took {(DateTime.Now - _startDate).TotalMilliseconds / 1000}s");
            Console.ReadKey();
        }

        private static (long, long) CalculateRange(long currentPosition, int rangeSize, long contentLength)
        {
            var start = currentPosition;
            var end = start + rangeSize;
            if (end > contentLength)
                end = contentLength;

            return (start, end);
        }
    }
}
