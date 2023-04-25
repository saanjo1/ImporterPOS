using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.Helpers
{
    public class CustomPdfPageEvent : PdfPageEventHelper
    {
        private bool _isLastPage;

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            // provjeri je li ovo posljednja stranica
            if (writer.PageNumber == document.PageNumber)
            {
                _isLastPage = true;

                // dodaj prazan prostor na dnu stranice
                PdfContentByte cb = writer.DirectContent;
                cb.MoveTo(0, 30);
                cb.LineTo(800, 30);
                cb.Stroke();
            }
            else
            {
                _isLastPage = false;
            }
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            // provjeri je li ovo posljednja stranica
            if (_isLastPage)
            {
                // dodaj tekst za potpis i pečat
                PdfContentByte cb = writer.DirectContent;
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Potpis: _________________________", 50, 10, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Pečat: __________________________", 50, 30, 0);
                cb.EndText();
            }
        }
    }

}
