using ASDATAHANDLER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASDATAHANDLER.Utils
{
    public class SqlEnginer
    {
        public List<CsvModel> ExecuteQuery(List<CsvModel> data, string query)
        {
            var selectedColumns = GetSelectedColumns(query);
            var whereClause = GetWhereClause(query);
            var orderByClause = GetOrderByClause(query);
            var groupByClause = GetGroupByClause(query);

            Console.WriteLine($"Consulta SQL: {query}");
            Console.WriteLine($"Columnas seleccionadas: {string.Join(", ", selectedColumns)}");
            Console.WriteLine($"Cláusula WHERE: {whereClause}");
            Console.WriteLine($"Cláusula ORDER BY: {orderByClause}");
            Console.WriteLine($"Cláusula GROUP BY: {groupByClause}");

            if (!data.Any())
            {
                Console.WriteLine("No hay datos disponibles para procesar.");
                return new List<CsvModel>();
            }

            var headers = data.First().Values;
            Console.WriteLine($"Encabezados: {string.Join(", ", headers)}");

            var filteredData = data.Where(record => EvaluateCondition(record, whereClause, headers)).ToList();
            Console.WriteLine($"Número de registros después del filtro: {filteredData.Count}");

            if (!string.IsNullOrEmpty(groupByClause))
            {
                filteredData = GroupBy(filteredData, groupByClause, headers);
            }

            if (!string.IsNullOrEmpty(orderByClause))
            {
                filteredData = OrderBy(filteredData, orderByClause, headers);
            }

            var projectedData = filteredData.Select(record => ProjectColumns(record, selectedColumns, headers)).ToList();
            Console.WriteLine("Datos proyectados:");

            foreach (var record in projectedData)
            {
                Console.WriteLine($"Valores proyectados: {string.Join(", ", record.Values)}");
            }

            return projectedData;
        }

        private string[] GetSelectedColumns(string query)
        {
            var match = Regex.Match(query, @"select\s+(.*?)\s+where", RegexOptions.IgnoreCase);
            return match.Groups[1].Value.Split(',').Select(col => col.Trim()).ToArray();
        }

        private string GetWhereClause(string query)
        {
            var match = Regex.Match(query, @"where\s+(.*?)(\s+group\s+by|\s+order\s+by|$)", RegexOptions.IgnoreCase);
            return match.Groups[1].Value.Trim();
        }

        private string GetOrderByClause(string query)
        {
            var match = Regex.Match(query, @"order\s+by\s+(.*)", RegexOptions.IgnoreCase);
            return match.Groups[1].Value.Trim();
        }

        private string GetGroupByClause(string query)
        {
            var match = Regex.Match(query, @"group\s+by\s+(.*)", RegexOptions.IgnoreCase);
            return match.Groups[1].Value.Trim();
        }

        private bool EvaluateCondition(CsvModel record, string condition, string[] headers)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            var andConditions = condition.Split(new[] { " and " }, StringSplitOptions.None);
            return andConditions.Select(cond => EvaluateOrCondition(record, cond.Trim(), headers)).All(r => r);
        }

        private bool EvaluateOrCondition(CsvModel record, string condition, string[] headers)
        {
            var orConditions = condition.Split(new[] { " or " }, StringSplitOptions.None);
            return orConditions.Select(cond => EvaluateSingleCondition(record, cond.Trim(), headers)).Any(r => r);
        }

        private bool EvaluateSingleCondition(CsvModel record, string condition, string[] headers)
        {
            var match = Regex.Match(condition, @"(\w+)\s*(=|>|<|>=|<=|like)\s*(\d+|'.*?')", RegexOptions.IgnoreCase);
            if (!match.Success) return false;

            var columnName = match.Groups[1].Value;
            var @operator = match.Groups[2].Value;
            var value = match.Groups[3].Value.Trim('\'');

            var columnIndex = Array.FindIndex(headers, h => h.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (columnIndex < 0 || columnIndex >= record.Values.Length) return false;

            var recordValue = record.Values[columnIndex];
            bool isInteger = int.TryParse(recordValue, out int recordValueAsInt);
            bool isConditionInteger = int.TryParse(value, out int valueAsInt);

            return @operator switch
            {
                "=" => recordValue.Equals(value, StringComparison.OrdinalIgnoreCase),
                ">" => isInteger && isConditionInteger && recordValueAsInt > valueAsInt,
                "<" => isInteger && isConditionInteger && recordValueAsInt < valueAsInt,
                ">=" => isInteger && isConditionInteger && recordValueAsInt >= valueAsInt,
                "<=" => isInteger && isConditionInteger && recordValueAsInt <= valueAsInt,
                "like" => new Regex("^" + Regex.Escape(value).Replace("%", ".*") + "$", RegexOptions.IgnoreCase).IsMatch(recordValue),
                _ => false,
            };
        }

        private List<CsvModel> OrderBy(List<CsvModel> data, string orderByClause, string[] headers)
        {
            var columns = orderByClause.Split(',').Select(col => col.Trim()).ToArray();
            var columnIndexes = columns.Select(col => Array.FindIndex(headers, h => h.Equals(col, StringComparison.OrdinalIgnoreCase))).ToArray();

            return data.OrderBy(record => string.Join(",", columnIndexes.Select(index => record.Values.ElementAtOrDefault(index)))).ToList();
        }

        private List<CsvModel> GroupBy(List<CsvModel> data, string groupByClause, string[] headers)
        {
            var columns = groupByClause.Split(',').Select(col => col.Trim()).ToArray();
            var columnIndexes = columns.Select(col => Array.FindIndex(headers, h => h.Equals(col, StringComparison.OrdinalIgnoreCase))).ToArray();

            return data
                .GroupBy(record => string.Join(",", columnIndexes.Select(index => record.Values.ElementAtOrDefault(index))))
                .Select(group => new CsvModel(group.First().Values))
                .ToList();
        }

        private CsvModel ProjectColumns(CsvModel record, string[] selectedColumns, string[] headers)
        {
            var projectedValues = selectedColumns.Select(column =>
            {
                var columnIndex = Array.FindIndex(headers, h => h.Equals(column, StringComparison.OrdinalIgnoreCase));
                return columnIndex >= 0 && columnIndex < record.Values.Length ? record.Values[columnIndex] : string.Empty;
            }).ToArray();

            return new CsvModel(projectedValues);
        }
    }
}









