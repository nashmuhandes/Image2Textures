using System;
using System.Drawing;
using System.IO;

namespace Image2Textures
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                uint processedCount = 0;
                string outputFileName = "Output.txt";

                using (StreamWriter writer = new StreamWriter(outputFileName))
                {
                    // Start from the directory of the executive.
                    processedCount = Image2Textures(writer, Environment.CurrentDirectory);
                }

                if (processedCount == 0)
                {
                    Console.WriteLine("No file processed.");
                }
                else
                {
                    Console.WriteLine(String.Format("{0} file(s) processed, file \"{1}\" output.", processedCount, outputFileName));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot create output file. Application terminated.");
            }

            Console.ReadKey();
        }

        public static uint Image2Textures(StreamWriter writer, string directory)
        {
            uint processedCount = 0;

            // Get files under current directory.
            string[] files = Directory.GetFiles(directory);
            foreach (string fileName in files)
            {
                // Is this a file with extension "png"?
                if (Path.GetExtension(fileName) == ".png")
                {
                    try
                    {
                        // Load image.
                        Image image = Image.FromFile(fileName);

                        // Output information to TEXTURES lump.
                        string lumpName = Path.GetFileNameWithoutExtension(fileName);
                        writer.WriteLine(String.Format("Texture {0}, {1}, {2}", lumpName, image.Width, image.Height));
                        writer.WriteLine(              "{");
                        writer.WriteLine(              "    XScale 1.0");
                        writer.WriteLine(              "    YScale 1.0");
                        writer.WriteLine(String.Format("    Patch {0}, 0, 0", lumpName));
                        writer.WriteLine(              "    {");
                        writer.WriteLine(              "    }");
                        writer.WriteLine(              "}");
                        writer.WriteLine(              ""); // Pad one more line.

                        // Free image.
                        image.Dispose();

                        processedCount++;
                    }
                    catch(Exception)
                    {
                        // Fail to process this image.
                        // We do nothing.
                    }
                }
            }

            // Get directories under current directory.
            string[] directories = Directory.GetDirectories(directory);
            foreach (string dir in directories)
            {
                // Process through subfolders.
                processedCount += Image2Textures(writer, dir);
            }

            return processedCount;
        }
    }
}
