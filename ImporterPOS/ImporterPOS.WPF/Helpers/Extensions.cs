using DocumentFormat.OpenXml.Office2010.Excel;
using ImporterPOS.Domain.Models;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static Document UpdatePdfDocument(Document pdfDocument, List<string> columns, ObservableCollection<InventoryDocumentsViewModel> values)
        {

            // Dodavanje tablice u PDF
            PdfPTable table = new PdfPTable(7);
            Font font = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);
            Font font1 = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);
            table.WidthPercentage = 100;


            // Dodavanje naziva stupaca u tablicu
            foreach (string column in columns)
            {
                PdfPCell nameHeader = new PdfPCell(new Phrase(column, font1));
                nameHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                nameHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                nameHeader.Padding = 10;
                table.AddCell(nameHeader);
            }

            foreach (var doc in values)
            {
                PdfPCell dateCell = new PdfPCell(new Phrase(doc.DateCreated, font));
                dateCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(dateCell);

                PdfPCell idCell = new PdfPCell(new Phrase(doc.Name, font1));
                idCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                idCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(idCell);

                PdfPCell purchaseCell = new PdfPCell(new Phrase(doc.PurchasePrice, font));
                purchaseCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(purchaseCell);

                PdfPCell soldPriceCell = new PdfPCell(new Phrase(doc.SoldPrice, font));
                soldPriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(soldPriceCell);

                PdfPCell basePriceCell = new PdfPCell(new Phrase(doc.BasePrice, font));
                basePriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(basePriceCell);

                PdfPCell taxesCell = new PdfPCell(new Phrase(doc.Taxes, font));
                taxesCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(taxesCell);

                PdfPCell rucCell = new PdfPCell(new Phrase(doc.Ruc, font));
                rucCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(rucCell);
            }


            pdfDocument.Add(table);
            pdfDocument.Close();

            return pdfDocument;
        }
    }
}
