using ImporterPOS.WPF.Modals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.Helpers
{
    public class Extensions
    {
        public static decimal GetDecimal(string value)
        {
            decimal decimalValue;
            if (value == "" || value == null)
                decimalValue = decimal.Parse("0");
            else
                decimalValue = decimal.Parse(value);

            return decimalValue;
        }

        public static string SetOleDbConnection(string excelfile)
        {
            string con =
       @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + excelfile + ";" +
       @"Extended Properties='Excel 12.0;HDR=No;IMEX=1'";
            return con;
        }

        public static DiscountColumnsViewModel SelectedColumns(DiscountColumnsViewModel mColumnModel, List<string> columnNamesList, ConcurrentDictionary<string, string> _myDictionary)
        {
            for (int i = 0; i < columnNamesList.Count(); i++)
            {
                if (columnNamesList[i].Contains("Name"))
                    mColumnModel.Name = columnNamesList[i];

                if (columnNamesList[i].Contains("Barcode"))
                    mColumnModel.BarCode = columnNamesList[i];

                if (columnNamesList[i].Contains("Full price €"))
                    mColumnModel.Price = columnNamesList[i];

                if (columnNamesList[i].Contains("Category"))
                    mColumnModel.Category = columnNamesList[i];

                if (columnNamesList[i].Equals("Discount"))
                    mColumnModel.Discount = columnNamesList[i];

                if (columnNamesList[i].Contains("Discounted price"))
                    mColumnModel.NewPrice = columnNamesList[i];

                mColumnModel.Storage = "Articles";

            }

            return mColumnModel;
        }

        public static string DisplayDiscountInPercentage(string discount)
        {
            bool success = double.TryParse(discount, out double result);
            string finalResult;

            if (success)
            {
                var percentage = result * (-100);
                finalResult = percentage.ToString() + "%";
            }
            else
            {
                finalResult = "NO DISCOUNT";
            }


            return finalResult;
        }

        public static decimal GetBasePrice(decimal price, decimal v)
        {
            decimal basePrice = price / (1 + (v / 100));

            return basePrice;

        }
    }
}
