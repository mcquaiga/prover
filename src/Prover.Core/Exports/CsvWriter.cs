using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Prover.Core.Exports
{
    public static class CsvWriter
    {
        public static async Task Write<T>(string fileName, string rowFormat, List<T> recordsList,
            bool writeHeader = false)
        {
            var fs = new StreamWriter(fileName);
            await Write(fs, rowFormat, recordsList, writeHeader);
        }

        public static async Task Write<T>(StreamWriter streamWriter, string rowFormat, List<T> recordsList,
            bool writeHeader = false)
        {
            using (streamWriter)
            {
                foreach (var value in recordsList)
                    await streamWriter.WriteLineAsync(GetRow(rowFormat, value));
            }
        }

        private static string GetRow<T>(string rowFormat, T record)
        {
            var properties = record.GetType().GetProperties()
                .Select(p => p.Name).ToList();

            foreach (var property in properties)
            {
                try
                {
                    if (rowFormat.Contains($"[{property}]"))
                    {
                        var value = record.GetType().GetProperty(property)?.GetValue(record)?.ToString();
                        if (value != null)
                            rowFormat = rowFormat.Replace($"[{property}]", value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);                   
                }               
            }

            return rowFormat;
        }
    }
}