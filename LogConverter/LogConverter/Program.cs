using LogConverter.Services;

namespace LogConverter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Digite a URL do log source:");
            string sourceUrl = Console.ReadLine();

            if (!IsValidUrl(sourceUrl))
            {
                Console.WriteLine("Erro: A URL fornecida não é válida.");
                return;
            }

            Console.WriteLine("Digite o caminho do arquivo de destino:");
            string targetPath = Console.ReadLine();

            ConvertLogService convertLogService = new ConvertLogService();

            try
            {
                
                string logContent = await convertLogService.DownloadLogFile(sourceUrl);
                string convertedContent = convertLogService.ConvertLog(logContent);
                await convertLogService.SaveToFile(targetPath, convertedContent);
                Console.WriteLine("Conversão concluída com sucesso.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro ao baixar o arquivo de log: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Erro ao salvar o arquivo: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
        static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri validatedUri)
                && (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
