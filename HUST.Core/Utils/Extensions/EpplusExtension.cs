using HUST.Core.Models;
using HUST.Core.Models.ServerObject;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils.Extensions
{
    public static class EpplusExtension
    {
        public static IEnumerable<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet) where T : BaseImport, new()
        {

            Func<CustomAttributeData, bool> columnOnly = y => y.AttributeType == typeof(ImportColumn);

            var columns = typeof(T)
                    .GetProperties()
                    .Where(x => x.CustomAttributes.Any(columnOnly))
                    .Select(p => new
                    {
                        Property = p,
                        Column = p.GetCustomAttributes<ImportColumn>().First().ColumnIndex
                    }).ToList();

            //var isRowEmpty = cellRange.All(c => c.Value == null);

            var rows = worksheet.Cells
                .Select(cell => cell.Start.Row)
                .Distinct()
                .OrderBy(x => x);


            var collection = rows.Skip(1)
                .Select(row =>
                {
                    var tnew = new T();
                    tnew.RowIndex = row;
                    Parallel.ForEach(columns, col =>
                    {
                        var val = worksheet.Cells[row, col.Column];
                        if (val.Value == null)
                        {
                            col.Property.SetValue(tnew, null);
                            return;
                        }
                        if (col.Property.PropertyType == typeof(Int32))
                        {
                            col.Property.SetValue(tnew, val.GetValue<int>());
                            return;
                        }
                        if (col.Property.PropertyType == typeof(double))
                        {
                            col.Property.SetValue(tnew, val.GetValue<double>());
                            return;
                        }
                        if (col.Property.PropertyType == typeof(DateTime))
                        {
                            col.Property.SetValue(tnew, val.GetValue<DateTime>());
                            return;
                        }

                        col.Property.SetValue(tnew, val.GetValue<string>()?.Trim());
                    });

                    return tnew;
                });


            return collection.Where(x => columns.Any(c => c.Property.GetValue(x) != null && c.Property.GetValue(x)?.ToString() != string.Empty));
        }
    }
}
