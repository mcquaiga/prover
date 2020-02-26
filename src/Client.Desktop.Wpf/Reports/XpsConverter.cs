﻿//using System;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Packaging;
//using System.Windows.Documents;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Xps.Packaging;
//using FixedDocument = PdfSharp.Xps.XpsModel.FixedDocument;
//using FixedPage = PdfSharp.Xps.XpsModel.FixedPage;
//using IOPath = System.IO.Path;

//namespace PdfSharp.Xps
//{
//    /// <summary>
//    ///     Main class that provides the functionallity to convert an XPS file into a PDF file.
//    /// </summary>
//    public class XpsConverter
//    {
//        private PdfDocument pdfDocument;
//        private XpsDocument xpsDocument;

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="XpsConverter" /> class.
//        /// </summary>
//        /// <param name="pdfDocument">The PDF document.</param>
//        /// <param name="xpsDocument">The XPS document.</param>
//        public XpsConverter(PdfDocument pdfDocument, XpsDocument xpsDocument)
//        {
//            if (pdfDocument == null)
//                throw new ArgumentNullException("pdfDocument");
//            if (xpsDocument == null)
//                throw new ArgumentNullException("xpsDocument");

//            this.pdfDocument = pdfDocument;
//            this.xpsDocument = xpsDocument;

//            Initialize();
//        }

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="XpsConverter" /> class.
//        /// </summary>
//        /// <param name="pdfDocument">The PDF document.</param>
//        /// <param name="xpsDocumentPath">The XPS document path.</param>
//        public XpsConverter(PdfDocument pdfDocument, string xpsDocumentPath) // TODO: a constructor with an Uri
//        {
//            if (pdfDocument == null)
//                throw new ArgumentNullException("pdfDocument");
//            if (string.IsNullOrEmpty(xpsDocumentPath))
//                throw new ArgumentNullException("xpsDocumentPath");

//            this.pdfDocument = pdfDocument;
//            xpsDocument = XpsDocument.Open(xpsDocumentPath);

//            Initialize();
//        }

//        private DocumentRenderingContext Context { get; set; }

//        /// <summary>
//        ///     Gets the PDF document of this converter.
//        /// </summary>
//        public PdfDocument PdfDocument => pdfDocument;

//        /// <summary>
//        ///     Gets the XPS document of this converter.
//        /// </summary>
//        public XpsDocument XpsDocument => xpsDocument;

//        public static BitmapSource BitmapSourceFromPage(DocumentPage docPage, double resolution)
//        {
//            var pixelWidth = docPage.Size.Width * resolution / 96;
//            var pixelHeight = docPage.Size.Height * resolution / 96;
//            var renderTarget = new RenderTargetBitmap((int) pixelWidth, (int) pixelHeight, resolution, resolution,
//                PixelFormats.Default);
//            renderTarget.Render(docPage.Visual);

//            return renderTarget;

//            //PngBitmapEncoder encoder = new PngBitmapEncoder();  // Choose type here ie: JpegBitmapEncoder, etc   
//            //encoder.Frames.Add(BitmapFrame.Create(renderTarget));

//            //BitmapSource.Create(pageWidth, pageHeight, resolution, resolution, PixelFormats.)

//            //return encoder.Preview;
//            //encoder.
//            //BitmapSource s = Xps;
//            ////FileStream pageOutStream = new FileStream(xpsFileName + ".Page" + pageNum + ".bmp", FileMode.Create, FileAccess.Write);
//            //MemoryStream memStream = new MemoryStream();
//            //encoder.Save(memStream);
//            //return memStream.ToArray();
//        }

//        /// <summary>
//        ///     Converts the specified PDF file into an XPS file. The new file is stored in the same directory.
//        /// </summary>
//        public static void Convert(string xpsFilename)
//        {
//            if (string.IsNullOrEmpty(xpsFilename))
//                throw new ArgumentNullException("xpsFilename");

//            if (!File.Exists(xpsFilename))
//                throw new FileNotFoundException("File not found.", xpsFilename);

//            var pdfFilename = xpsFilename;
//            if (IOPath.HasExtension(pdfFilename))
//                pdfFilename = pdfFilename.Substring(0, pdfFilename.LastIndexOf('.'));
//            pdfFilename += ".pdf";

//            Convert(xpsFilename, pdfFilename, 0);
//        }

//        /// <summary>
//        ///     Implements the PDF file to XPS file conversion.
//        /// </summary>
//        public static void Convert(string xpsFilename, string pdfFilename, int docInde)
//        {
//            Convert(xpsFilename, pdfFilename, docInde, false);
//        }

//        /// <summary>
//        ///     Implements the PDF file to XPS file conversion.
//        /// </summary>
//        public static void Convert(XpsDocument xpsDocument, string pdfFilename, int docIndex)
//        {
//            if (xpsDocument == null)
//                throw new ArgumentNullException("xpsDocument");

//            if (string.IsNullOrEmpty(pdfFilename))
//                throw new ArgumentNullException("pdfFilename");

//            FixedDocument fixedDocument = xpsDocument.GetDocument();
//            PdfDocument pdfDocument = new PdfDocument();
//            var renderer = new PdfRenderer();

//            var pageIndex = 0;
//            foreach (var page in fixedDocument.Pages)
//            {
//                if (page == null)
//                    continue;
//                Debug.WriteLine("  doc={0}, page={1}", docIndex, pageIndex);
//                PdfPage pdfPage = renderer.CreatePage(pdfDocument, page);
//                renderer.RenderPage(pdfPage, page);
//                pageIndex++;

//#if DEBUG
//                // stop at page...
//                if (pageIndex == 50)
//                    break;
//#endif
//            }

//            pdfDocument.Save(pdfFilename);
//        }

//        /// <summary>
//        ///     Implements the PDF file to XPS file conversion.
//        /// </summary>
//        public static void Convert(string xpsFilename, string pdfFilename, int docIndex, bool createComparisonDocument)
//        {
//            if (string.IsNullOrEmpty(xpsFilename))
//                throw new ArgumentNullException("xpsFilename");

//            if (string.IsNullOrEmpty(pdfFilename))
//            {
//                pdfFilename = xpsFilename;
//                if (IOPath.HasExtension(pdfFilename))
//                    pdfFilename = pdfFilename.Substring(0, pdfFilename.LastIndexOf('.'));
//                pdfFilename += ".pdf";
//            }

//            XpsDocument xpsDocument = null;
//            try
//            {
//                xpsDocument = XpsDocument.Open(xpsFilename);
//                FixedDocument fixedDocument = xpsDocument.GetDocument();
//                PdfDocument pdfDocument = new PdfDocument();
//                var renderer = new PdfRenderer();

//                var pageIndex = 0;
//                foreach (var page in fixedDocument.Pages)
//                {
//                    if (page == null)
//                        continue;
//                    Debug.WriteLine("  doc={0}, page={1}", docIndex, pageIndex);
//                    PdfPage pdfPage = renderer.CreatePage(pdfDocument, page);
//                    renderer.RenderPage(pdfPage, page);
//                    pageIndex++;

//#if DEBUG
//                    // stop at page...
//                    if (pageIndex == 50)
//                        break;
//#endif
//                }

//                pdfDocument.Save(pdfFilename);
//                xpsDocument.Close();
//                xpsDocument = null;

//                if (createComparisonDocument)
//                {
//                    var xpsDoc = new XpsDocument(xpsFilename, FileAccess.Read);
//                    var docSeq = xpsDoc.GetFixedDocumentSequence();
//                    if (docSeq == null)
//                        throw new InvalidOperationException("docSeq");

//                    XPdfForm form = XPdfForm.FromFile(pdfFilename);
//                    PdfDocument pdfComparisonDocument = new PdfDocument();


//                    pageIndex = 0;
//                    foreach (PdfPage page in pdfDocument.Pages)
//                    {
//                        if (page == null)
//                            continue;
//                        Debug.WriteLine("  doc={0}, page={1}", docIndex, pageIndex);

//                        PdfPage pdfPage = /*renderer.CreatePage(pdfComparisonDocument, page);*/
//                            pdfComparisonDocument.AddPage();
//                        double width = page.Width;
//                        double height = page.Height;
//                        pdfPage.Width = page.Width * 2;
//                        pdfPage.Height = page.Height;


//                        var docPage = docSeq.DocumentPaginator.GetPage(pageIndex);
//                        //byte[] png = PngFromPage(docPage, 96);

//                        var bmsource = BitmapSourceFromPage(docPage, 96 * 2);
//                        XImage image = XImage.FromBitmapSource(bmsource);

//                        XGraphics gfx = XGraphics.FromPdfPage(pdfPage);
//                        form.PageIndex = pageIndex;
//                        gfx.DrawImage(form, 0, 0, width, height);
//                        gfx.DrawImage(image, width, 0, width, height);

//                        //renderer.RenderPage(pdfPage, page);
//                        pageIndex++;

//#if DEBUG
//                        // stop at page...
//                        if (pageIndex == 50)
//                            break;
//#endif
//                    }

//                    var pdfComparisonFilename = pdfFilename;
//                    if (IOPath.HasExtension(pdfComparisonFilename))
//                        pdfComparisonFilename =
//                            pdfComparisonFilename.Substring(0, pdfComparisonFilename.LastIndexOf('.'));
//                    pdfComparisonFilename += "-comparison.pdf";

//                    pdfComparisonDocument.ViewerPreferences.FitWindow = true;
//                    //pdfComparisonDocument.PageMode = PdfPageMode.
//                    pdfComparisonDocument.PageLayout = PdfPageLayout.SinglePage;
//                    pdfComparisonDocument.Save(pdfComparisonFilename);
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex.Message);
//                if (xpsDocument != null)
//                    xpsDocument.Close();
//                throw;
//            }
//            finally
//            {
//                if (xpsDocument != null)
//                    xpsDocument.Close();
//            }
//        }

//        /// <summary>
//        ///     HACK
//        /// </summary>
//        public PdfPage CreatePage(int xpsPageIndex)
//        {
//            FixedPage fixedPage = xpsDocument.GetDocument().GetFixedPage(xpsPageIndex);

//            PdfPage page = pdfDocument.AddPage();
//            page.Width = XUnit.FromPresentation(fixedPage.Width);
//            page.Height = XUnit.FromPresentation(fixedPage.Height);
//            return page;
//        }

//        /// <summary>
//        ///     Renders an XPS document page to the specified PDF page.
//        /// </summary>
//        /// <param name="page">The target PDF page. The page must belong to the PDF document of this converter.</param>
//        /// <param name="xpsPageIndex">The zero-based XPS page number.</param>
//        public void RenderPage(PdfPage page, int xpsPageIndex)
//        {
//            if (page == null)
//                throw new ArgumentNullException("page");
//            if (!ReferenceEquals(page.Owner, pdfDocument))
//                throw new InvalidOperationException(PSXSR.PageMustBelongToPdfDocument);
//            // Debug.Assert(xpsPageIndex==0, "xpsPageIndex must be 0 at this stage of implementation.");
//            try
//            {
//                FixedPage fpage = xpsDocument.GetDocument().GetFixedPage(xpsPageIndex);

//                // ZipPackage pack = ZipPackage.Open(xpsFilename) as ZipPackage;
//                var uri = new Uri("/Documents/1/Pages/1.fpage", UriKind.Relative);
//                var part = xpsDocument.Package.GetPart(uri) as ZipPackagePart;
//                if (part != null)
//                    using (var stream = part.GetStream())
//                    using (var sr = new StreamReader(stream))
//                    {
//                        var xml = sr.ReadToEnd();
//#if true && DEBUG
//                        if (!string.IsNullOrEmpty(xpsDocument.Path))
//                        {
//                            var xmlPath =
//                                IOPath.Combine(IOPath.GetDirectoryName(xpsDocument.Path),
//                                    IOPath.GetFileNameWithoutExtension(xpsDocument.Path)) + ".xml";
//                            using (var sw = new StreamWriter(xmlPath))
//                            {
//                                sw.Write(xml);
//                            }
//                        }
//#endif
//                        //XpsElement el = PdfSharp.Xps.Parsing.XpsParser.Parse(xml);
//                        var renderer = new PdfRenderer();
//                        renderer.RenderPage(page, fpage);
//                    }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex.ToString());
//                throw;
//            }
//        }


//        public static void SaveXpsPageToBitmap(string xpsFileName)
//        {
//            var xpsDoc = new XpsDocument(xpsFileName, FileAccess.Read);
//            var docSeq = xpsDoc.GetFixedDocumentSequence();

//            // You can get the total page count from docSeq.PageCount    
//            for (var pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
//            {
//                var docPage = docSeq.DocumentPaginator.GetPage(pageNum);
//                var renderTarget =
//                    new RenderTargetBitmap((int) docPage.Size.Width,
//                        (int) docPage.Size.Height,
//                        96, // WPF (Avalon) units are 96dpi based    
//                        96,
//                        PixelFormats.Default);

//                renderTarget.Render(docPage.Visual);

//                BitmapEncoder encoder = new BmpBitmapEncoder(); // Choose type here ie: JpegBitmapEncoder, etc   
//                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

//                var pageOutStream = new FileStream(xpsFileName + ".Page" + pageNum + ".bmp", FileMode.Create,
//                    FileAccess.Write);
//                encoder.Save(pageOutStream);
//                pageOutStream.Close();
//            }
//        }

//        private void Initialize()
//        {
//            Context = new DocumentRenderingContext(pdfDocument);
//        }

//        //byte[] void PngFromPage(FixedDocument fixedDocument, int pageIndex, double resolution)
//        //{
//        //  if (fixedDocument==null)
//        //    throw new ArgumentNullException("fixedDocument");
//        //  if ( pageIndex<0|| pageIndex>= fixedDocument.PageCount)
//        //    throw new ArgumentOutOfRangeException("pageIndex");

//        //  FixedPage page = fixedDocument.Pages[pageIndex];
//        //  double pageWidth = page.Width;
//        //  double pageHeight= page.Height;

//        //  // Create an appropirate render bitmap
//        //  const int factor = 3;
//        //  int width = (int)(WidthInPoint * factor);
//        //  int height = (int)(HeightInPoint * factor);
//        //  this.image = new RenderTargetBitmap(width, height, 72 * factor, 72 * factor, PixelFormats.Default);
//        //  if (visual is UIElement)
//        //  {
//        //    // Perform layout on UIElement - otherwise nothing gets rendered
//        //    UIElement element = visual as UIElement;
//        //    Size size = new Size(WidthInPU, HeightInPU);
//        //    element.Measure(size);
//        //    element.Arrange(new Rect(new Point(), size));
//        //    element.UpdateLayout();
//        //  }
//        //  this.image.Render(visual);

//        //  // Save image as PNG
//        //  FileStream stream = new FileStream(Path.Combine(OutputDirectory, Name + ".png"), FileMode.Create);
//        //  PngBitmapEncoder encoder = new PngBitmapEncoder();
//        //  //string author = encoder.CodecInfo.Author.ToString();
//        //  encoder.Frames.Add(BitmapFrame.Create(this.image));
//        //  encoder.Save(stream);
//        //  stream.Close();
//        //}
//    }
//}