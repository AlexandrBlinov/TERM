using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace YstProject.Services
{
    public static class ExcelParser
    {
        private class CellDTO
        {
            public uint Row { get; set; }
            public uint Column { get; set; }
            public string Text { get; set; }
        }

        private static uint GetCellRow(string cellAddress)
        {
            uint rowIndex = uint.Parse(Regex.Match(cellAddress, @"[0-9]+").Value);
            return rowIndex;
        }

        private static uint GetCellColumn(string cellAddress)
        {
            uint initialAddress = 65;  // equals 'А'
            string rowIndex = Regex.Match(cellAddress, @"(\D)+").Value;
            int charCode = getCharCode(rowIndex.ToCharArray()[0]);
            return (uint)charCode - initialAddress;
        }

        private static int getCharCode(char character)
        {
            UTF32Encoding encoding = new UTF32Encoding();
            byte[] bytes = encoding.GetBytes(character.ToString().ToCharArray());
            return BitConverter.ToInt32(bytes, 0);
        }


        public static IDictionary<int, int> ParseTwoColumns(string fileName)
        {
            // put values to dictionary (code,quantity)
            var resultDict = new Dictionary<int, int>();

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                {
                    throw new ArgumentException("sheetName");
                }
                WorksheetPart worksheetPart = wbPart.WorksheetParts.First();


                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                //  CellDTO[] values;
                var link = document.WorkbookPart.SharedStringTablePart;
                Func<Cell, string> selector = (cell) => cell.InnerText;

                if (link != null)
                {
                    SharedStringTable sharedStringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                    selector = (cell) => cell.DataType == null ? cell.InnerText : cell.DataType == CellValues.SharedString ? sharedStringTable.ElementAt(Int32.Parse(cell.InnerText)).InnerText : cell.InnerText;

                }

                var values = wsPart.Worksheet.Descendants<Cell>().Where(cell => GetCellColumn(cell.CellReference) < 2).Select(cell => new CellDTO
                 {
                     Row = GetCellRow(cell.CellReference),
                     Column = GetCellColumn(cell.CellReference),
                     Text = selector(cell)
                 }).ToArray();
                // group by rows
                var values2 = values.GroupBy(x => x.Row).ToArray();


                foreach (var childcells in values2)
                {
                    //var firstCellInRow = cell.First();
                    try
                    {
                        var firstCellInRow = childcells.First();
                        var lastCellInRow = childcells.Skip(1).Single(p => p.Column == 1);
                        int codeint = Convert.ToInt32(firstCellInRow.Text);
                        int quantityint = Convert.ToInt32(lastCellInRow.Text);
                        if (resultDict.ContainsKey(codeint))
                            resultDict[codeint] = quantityint;
                        else

                            resultDict.Add(codeint, quantityint);
                    }
                    catch
                    {
                        continue;
                    }

                }
            }
            return resultDict;
        }

        public static int[] ParseOneColumn(string fileName)
        {


            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
            {
                // SharedStringTable sharedStringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;

                WorkbookPart wbPart = document.WorkbookPart;

                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                {
                    throw new ArgumentException("sheetName");
                }
                WorksheetPart worksheetPart = wbPart.WorksheetParts.First();

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                //
                var link = document.WorkbookPart.SharedStringTablePart;
                Func<Cell, string> selector = (cell) => cell.InnerText;

                try
                {
                    if (link != null)
                    {
                        SharedStringTable sharedStringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                        selector = (cell) => cell.DataType == null ? cell.InnerText : cell.DataType == CellValues.SharedString ? sharedStringTable.ElementAt(Int32.Parse(cell.InnerText)).InnerText : cell.InnerText;

                    }
                    var values = wsPart.Worksheet.Descendants<Cell>().Where(cell => GetCellColumn(cell.CellReference) == 0).Select(cell => selector(cell)).ToArray();

                    var valuesint = Array.ConvertAll(values, delegate(string s) { int result; return int.TryParse(s, out result) ? result : 0; }).Where(p => p != 0).ToArray();

                    return valuesint;


                }
                catch (Exception exc)
                {
                    ExceptionUtility.LogException(exc, "Error from parsing excel document");
                    throw;
                }



            }

        }
    }
}