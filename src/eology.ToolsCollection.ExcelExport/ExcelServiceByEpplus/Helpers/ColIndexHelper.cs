using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace KeywordManagementConsole
{

    public enum ParseType
    {

        Contains,
        StartsWith
    }
    public class ColIndexHelper
    {


        public static List<string> GetColIndexList() {
            //List<string> colIndexListDummy = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az" };



            //return colIndexListDummy;
            List<string> list1 = new() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};

            List<string> list2 = new() { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            List<string> used1 = new();
            List<string> used2 = new();

            List<string> colIndexList = new();
            //var count = 0;

            try
            {
                list2.ForEach(letter2 =>
                {
                    string combination = "";
                    if (true)//(!used2.Any(lt => lt == letter2))
                    {

                        list1.ForEach(letter1 =>
                        {
                            if (!used1.Any(lt => lt == letter1 && !used2.Any(lt => lt == letter2)))
                            {
                                combination = letter2 + letter1;
                                colIndexList.Add(combination);
                            }


                            if (list1.Last() == letter1)
                            {

                            }

                            //if (list1.Last() == letter1)
                            //{

                            //    var result = list1.Except(used1).FirstOrDefault();

                            //    if (result != null)
                            //    {
                            //        used1.Add(result);
                            //    }
                            //    else {

                            //        used1 = new List<string>();
                            //    }

                            //}
                        });
                    }

                    //if (list2.Last() == letter2)
                    //{
                    //    var result = list2.Except(used2).First();


                    //    if (result != null)
                    //    {
                    //        used2.Add(result);
                    //    }
                    //    else
                    //    {

                    //        used2 = new List<string>();
                    //    }


                    //}
                });
                return colIndexList;
            }
            catch (Exception ex) {
                var x = ex;
                throw;
            }


            
        }

        //public static List<string> colIndexList = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az" };



        public ColIndexHelper()
        {
        }


        public static int GetColIndex(string colName)
        {
            return (GetColIndexList().IndexOf(colName));
        }

        public static int GetHeaderColumnByName(string colName, ParseType parseType, ExcelWorksheet sheet)
        {
            // Get Index of KeywordColumn
            int headRowColIndex;
            ExcelRangeBase currentReaderCell;


            try
            {


                if (parseType == ParseType.StartsWith)
                {
                    currentReaderCell = sheet.Cells["1:1"].FirstOrDefault(cell =>
                    {
                        if (cell.Value == null) return false;

                        return (bool)cell.Value?.ToString().StartsWith(colName);
                    });
                }
                else
                {
                    currentReaderCell = sheet.Cells["1:1"].FirstOrDefault(cell =>
                    {
                        if (cell.Value == null) return false;
                        return (bool)cell.Value?.ToString().Contains(colName);
                    });
                }
            }
            catch (Exception ex)
            {
                //XConsole.WriteLine("Fehler beim Finden der Zelle: " + colName, LogMode.error);
                //XConsole.WriteLine(ex.Message, LogMode.error);

                if (sheet==null)
                {
                    //XConsole.WriteLine("Sheet not Found",LogMode.error);
                }

                sheet.Cells["1:1"].ToList().ForEach(cell =>
                {
                    if (cell.Value != null)
                    {
                        Console.WriteLine(cell.Value.ToString() + " - " + cell.Value.ToString().StartsWith("Keyword"));
                    }
                    else
                    {
                        Console.WriteLine("Cell is null");
                    }
                });


                currentReaderCell = null;
            }



            if (currentReaderCell != null) headRowColIndex = currentReaderCell.Start.Column;
            else
            {
                //XConsole.WriteLine("Spalte nicht gefunden: " + colName, LogMode.warning);
                return -1;
            }
            return headRowColIndex;
        }


    }
}
