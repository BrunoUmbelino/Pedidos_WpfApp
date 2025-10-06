using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Pedidos_WpfApp.Services
{
    public static class DataService<T> where T : class
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static List<T> CarregarDados(string fileName)
        {
            try
            {
                var filePath = ObterCaminhoArquivo(fileName);

                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    return new List<T>();
                }

                var json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<T>();

                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados de {fileName}: {ex.Message}");
                return new List<T>();
            }
        }

        public static void SalvarDados(List<T> dados, string fileName)
        {
            try
            {
                var filePath = ObterCaminhoArquivo(fileName);
                var json = JsonConvert.SerializeObject(dados, _settings);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar dados em {fileName}: {ex.Message}");
                throw;
            }
        }

        private static string ObterCaminhoArquivo(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", fileName);
        }
    }
}

