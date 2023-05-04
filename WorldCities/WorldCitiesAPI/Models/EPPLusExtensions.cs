using OfficeOpenXml;
using System.Reflection;

namespace WorldCitiesAPI.Models
{
    public static class EPPLusExtensions
    {
        public static IEnumerable<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet, bool defaultHeader = true) where T : new()
        {

            var properties = (new T())
                .GetType()
                .GetProperties()
                .ToList();

            var groups = worksheet.Cells
                .GroupBy(cell => cell.Start.Row)
                .ToList();

            var headers = groups
                .First()
                .Select((header, index) => new { Name = header.Value.ToString(), indexColumn = index })
                .Where(o => properties.Select(p => p.Name).Contains(o.Name))
                .ToList();

            if (!defaultHeader)
                headers = groups
                .First()
                .Select((header, index) => new { Name = header.Value.ToString(), indexColumn = index })
                .Take(properties.Count)
                .ToList();

            var rows = groups
                .Skip(1)
                .Select(row => row.ToList());

            var collection = rows
                .Select(row =>
                {
                    var tnew = new T();
                    headers.ForEach(header =>
                    {
                        var cell = row[header.indexColumn];
                        var property = defaultHeader ? properties.First(p => p.Name == header.Name) : properties[header.indexColumn];

                        if (cell.Value == null)
                        {
                            property.SetValue(tnew, null);
                            return;
                        }
                        if (property.PropertyType == typeof(Int32))
                        {
                            property.SetValue(tnew, cell.GetValue<int>());
                            return;
                        }
                        if (property.PropertyType == typeof(Decimal))
                        {
                            property.SetValue(tnew, cell.GetValue<decimal>());
                            return;
                        }
                        if (property.PropertyType == typeof(Double))
                        {
                            property.SetValue(tnew, cell.GetValue<double>());
                            return;
                        }
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(tnew, cell.GetValue<DateTime>());
                            return;
                        }
                        property.SetValue(tnew, cell.GetValue<string>());
                    });

                    return tnew;
                });


            //Send it back
            return collection;
        }
    }
}
