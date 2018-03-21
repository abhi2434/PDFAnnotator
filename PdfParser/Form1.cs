using IdeaBridge.Data.Base;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfParser
{
    public partial class Form1 : Form
    {
        OpenFileDialog dlg = null; 
        public Form1()
        {
            InitializeComponent(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dlg = new OpenFileDialog();
            dlg.Filter = "Pdf Files|*.pdf|All Files|*.*";
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                this.txtPDFFile.Text = dlg.FileName; 
            }
        }
        /// <summary>
        /// Highlights the PDF with Yellow annotation
        /// </summary>
        /// <param name="inputFile">File to read from</param>
        /// <param name="highLightFile">File to write to</param>
        /// <param name="pageno">Which page no to read</param>
        /// <param name="textToAnnotate">Texts to annotate</param>
        private void HighlightPDFAnnotation(string inputFile, string highLightFile, int pageno, params string[] textToAnnotate)
        {
            PdfReader reader = new PdfReader(inputFile);
            using (FileStream fs = new FileStream(highLightFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    MyLocationTextExtractionStrategy strategy = new MyLocationTextExtractionStrategy();
                    strategy.UndercontentHorizontalScaling = 100;

                    string currentText = PdfTextExtractor.GetTextFromPage(reader, pageno, strategy);
                    for (int i = 0; i < textToAnnotate.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(textToAnnotate[i])) { continue; }
                        var lstMatches = strategy.GetTextLocations(textToAnnotate[i].Trim(), StringComparison.CurrentCultureIgnoreCase);
                        if (!this.chkAnnotation.Checked)
                            lstMatches = lstMatches.Take(1).ToList();
                        foreach (iTextSharp.text.Rectangle rectangle in lstMatches)
                        {
                            float[] quadPoints = { rectangle.Left - 3.0f,
                                             rectangle.Bottom,
                                             rectangle.Right,
                                             rectangle.Bottom,
                                             rectangle.Left - 3.0f,
                                             rectangle.Top + 1.0f,
                                             rectangle.Right,
                                             rectangle.Top + 1.0f
                                          };

                          
                            PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer
                                                            , rectangle, null
                                                            , PdfAnnotation.MARKUP_HIGHLIGHT, quadPoints);
                            highlight.Color = BaseColor.YELLOW;


                            PdfGState state = new PdfGState();
                            state.BlendMode = new PdfName("Multiply");


                            PdfAppearance appearance = PdfAppearance.CreateAppearance(stamper.Writer, rectangle.Width, rectangle.Height);
                            
                            appearance.SetGState(state);
                            appearance.Rectangle(0, 0, rectangle.Width, rectangle.Height);
                            appearance.SetColorFill(BaseColor.YELLOW);
                            appearance.Fill();

                            highlight.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, appearance);

                            //Add the annotation
                            stamper.AddAnnotation(highlight, pageno);
                        }
                    }
                }
            }
            reader.Close();
        }
        public string ReadPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dlg != null)
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                var extension = System.IO.Path.GetExtension(dlg.FileName);

                var filePath = System.IO.Path.GetDirectoryName(dlg.FileName);

                var finalPath = System.IO.Path.Combine(filePath, $"{fileName}_annotated.{extension}");


                this.HighlightPDFAnnotation(dlg.FileName, finalPath, 1, this.txtAnnotatedText.Text, this.txtAnnotate2.Text);
            }
        }
    }
}
