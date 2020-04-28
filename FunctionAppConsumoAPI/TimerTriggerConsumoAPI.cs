using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FunctionAppConsumoAPI.Models;

namespace FunctionAppConsumoAPI
{
    public static class TimerTriggerConsumoAPI
    {
        [FunctionName("TimerTriggerConsumoAPI")]
        public static void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(
                    Environment.GetEnvironmentVariable("UrlAPIContagem"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response =
                    client.GetAsync(String.Empty).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string conteudo = response.Content.ReadAsStringAsync().Result;
                    log.LogInformation($"Retorno JSON: {conteudo}");
                    
                    ResultadoContador resultado = null;
                    resultado = JsonSerializer
                        .Deserialize<ResultadoContador>(conteudo,
                            new JsonSerializerOptions()
                            {
                                PropertyNameCaseInsensitive = true
                            });
                    log.LogInformation($"Valor do contador: {resultado.ValorAtual}");
                }
                else
                    log.LogError($"Erro durante chamada à API de contagem: {response.StatusCode}");
            }

            log.LogInformation($"TimerTriggerConsumoAPI executada em: {DateTime.Now}");
        }
    }
}