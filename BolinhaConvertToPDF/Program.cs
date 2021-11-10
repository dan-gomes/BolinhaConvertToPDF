using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BolinhaConvertToPDF
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Seja bem vindo ao BolinhaConvertToPDF");
            Console.WriteLine();
            Console.WriteLine("Informe o que deseja fazer: ");
            Console.WriteLine("1 - Separar Arquivo PDF");
            Console.WriteLine("2 - Converter Arquivo Para PDF");

            switch (ObterOpcaoSelecionada())
            {
                case (1):
                    SepararArquivoPdf();
                    break;
                case (2):
                    ConverterArquivo();
                    break;
                default:
                    Console.WriteLine("Opção inválida, por favor inicie novamente a aplicação.");
                    break;
            }

        }

        private static void SepararArquivoPdf()
        {
            Console.Clear();
            Console.WriteLine("BolinhaConvertToPDF \n");
            Console.WriteLine("Informe qual a saída dos arquivos: ");
            Console.WriteLine("1 - Informe o caminho de cada arquivo");
            Console.WriteLine("2 - Informe o caminho da pasta que contém os arquivos");

            switch (ObterOpcaoSelecionada())
            {
                case (1):
                    OneToOne();
                    break;
                case (2):
                    OneToMany();
                    break;
                default:
                    Console.WriteLine("Opção inválida, por favor inicie novamente a aplicação.");
                    break;
            }
        }

        private static void ConverterArquivo()
        {
            try
            {
                Head();

                iTextSharp.text.Rectangle pageSize = null;

                Console.WriteLine("Informe o caminho do arquivo para conversão");
                string origemArquivo = Console.ReadLine();

                Console.WriteLine("Informe o destino do arquivo convertido");
                string destinoArquivo = Console.ReadLine();

                using (Bitmap srcImage = new Bitmap(origemArquivo))
                {
                    pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
                }
                using var ms = new MemoryStream();
                var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();
                document.Add(iTextSharp.text.Image.GetInstance(origemArquivo));
                document.Close();

                File.WriteAllBytes(destinoArquivo + "\\NovoArquivo.pdf", ms.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Valor inválido");
                ConverterArquivo();
            }
        }

        private static int ObterOpcaoSelecionada()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int opcaoSelecionada))
                    return opcaoSelecionada;
                else
                    Console.WriteLine("Informe uma opção válida");
            }
        }

        /// <summary>
        /// Method for obtaining file paths from a directory.
        /// </summary>
        private static void OneToMany()
        {
            Head();
            Console.WriteLine("Informe a pasta: ");
            string entrada = Console.ReadLine();

            // Class that acess directory
            DirectoryInfo directory = new DirectoryInfo(entrada);

            string[] enderecoArquivos = new string[directory.GetFiles().Count()];

            int i = 0;

            foreach (var item in directory.GetFiles())
            {
                enderecoArquivos[i] = (item.FullName);
                i++;
            }

            CreatePdf(enderecoArquivos, directory.GetFiles().FirstOrDefault().DirectoryName + "\\new.pdf");
            Footer();
        }

        /// <summary>
        /// Console Header.
        /// </summary>
        private static void Head()
        {
            Console.Clear();
            Console.WriteLine("BolinhaConvertToPDF");
            Console.WriteLine();
            Console.WriteLine("-------------------");
        }

        /// <summary>
        /// Footnote .
        /// </summary>
        private static void Footer()
        {
            Console.WriteLine("Arquivo Criado!!! Uhu");
            Console.WriteLine();
            Console.WriteLine("Obrigado por usar o BolinhaConvertToPDF");
            Console.ReadKey();
        }

        /// <summary>
        /// Method for grouping content from n file paths.
        /// </summary>
        private static void OneToOne()
        {
            Head();
            Console.WriteLine("Informe a quantidade de arquivos para agrupar: ");
            int quantidade = int.Parse(Console.ReadLine());

            string[] enderecoArquivos = new string[quantidade];

            for (int i = 0; i < quantidade; i++)
            {
                Console.WriteLine("Informe o caminho dos arquivos para agrupar: ");
                enderecoArquivos[i] = Console.ReadLine();
                Console.Clear();
            }

            Console.WriteLine("Informe o endereço de destino dos arquivos: ");
            string caminhoDestino = Console.ReadLine();

            CreatePdf(enderecoArquivos, caminhoDestino);
            Footer();
        }

        /// <summary>
        /// Is to use the library ITextSharp.
        /// The method creates a new file <paramref name="OutputFile"/> using
        /// the filestream by copying the files entered here <paramref name="PDFfileNames"/>.
        /// "https://www.c-sharpcorner.com/article/merge-multiple-pdf-files-into-single-pdf-using-itextsharp-in-c-sharp/"
        /// </summary>
        /// <param name="PDFfileNames"></param>
        /// <param name="OutputFile"></param>
        public static void CreatePdf(string[] PDFfileNames, string OutputFile)
        {
            // Create document object  
            Document PDFdoc = new Document();
            // Create a object of FileStream which will be disposed at the end  
            using FileStream MyFileStream = new FileStream(OutputFile, FileMode.Create);
            // Create a PDFwriter that is listens to the Pdf document  
            PdfCopy PDFwriter = new PdfCopy(PDFdoc, MyFileStream);
            if (PDFwriter == null)
                return;
            // Open the PDFdocument  
            PDFdoc.Open();
            foreach (string fileName in PDFfileNames)
            {
                // Create a PDFreader for a certain PDFdocument  
                PdfReader PDFreader = new PdfReader(fileName);
                PDFreader.ConsolidateNamedDestinations();
                // Add content for page 
                for (int i = 1; i <= PDFreader.NumberOfPages; i++)
                {
                    PDFwriter.AddPage(PDFwriter.GetImportedPage(PDFreader, i));
                }

                // Close PDFreader  
                PDFreader.Close();
            }
            // Close the PDFdocument and PDFwriter  
            PDFwriter.Close();
            PDFdoc.Close();
        }

    }
}
