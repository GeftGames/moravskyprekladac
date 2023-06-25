using System;
using System.IO;

namespace TranslatorWritter {
    public class Packer {        
        const char delimiter='§';

        public event EventHandler<SampleEventArgs> ProgressChange;
       // public event EventHandler<EventArgs> Done;

        public class SampleEventArgs :EventArgs{
            public SampleEventArgs(float percentage) { Percentage = percentage; }
            public float Percentage; // readonly
        }

        public void CreatePackageAsync(string[] filePaths, string outputFile, IProgress<float> progress = null) {
            int totalFiles = filePaths.Length*2;
            int currentFile = 0; 
            
            // Kontrola delimiter, ve spojovanym sóboro nesmy bet znak '§'!
            foreach (string filePath in filePaths) {        
                using (StreamReader sr = new StreamReader(filePath)) {
                    string content = sr.ReadToEnd();
                    if (content.Contains(delimiter)) throw new InvalidOperationException($"File {filePath} contains the separator '{delimiter}'!");
                }    
                currentFile++;
                float percentage = (float)currentFile / totalFiles; 
                if (progress != null) {    
                    progress.Report(percentage);
                }
                    ProgressChange.Invoke(null, new SampleEventArgs(percentage));
            }
 
            // Zapsat soubor
            using (StreamWriter sw = new StreamWriter(outputFile)) {
                foreach (string filePath in filePaths) {
                    // Název souboru
                    sw.Write(Path.GetFileName(filePath));

                    sw.Write(delimiter);

                    // Zapsat soubor
                    using (StreamReader sr = new StreamReader(filePath)) {
                        sw.Write(sr.ReadToEnd());
                    }

                    // Oddělovač
                    sw.Write(delimiter);

                    currentFile++;
                    // Report progress
                        float percentage = (float)currentFile / totalFiles;
                    if (progress != null) {
                        progress.Report(percentage);
                    }
                        ProgressChange.Invoke(null, new SampleEventArgs(percentage));
                }
            }

        //    Done.Invoke(this, null);
        }   

        public static void ExtractMergedFiles(string inputFile, string outputFolder) {
            string fileContent = File.ReadAllText(inputFile);
            string[] fileContents = fileContent.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

            // Po souborech
            for (int i = 0; i < fileContents.Length; i += 2) {
                string fileName = fileContents[i], 
                    fileText = fileContents[i + 1];

                string filePath = Path.Combine(outputFolder, fileName);

                // Zápis souboru
                using (StreamWriter sw = new StreamWriter(filePath)) sw.Write(fileText);
            }
        }
    }
}

      /*  public static void CreatePackage(string[] filePaths, string outputFile) { 
            using (StreamWriter sw = new StreamWriter(outputFile)) {
                foreach (string filePath in filePaths) {
                    // Název souboru
                    sw.Write(Path.GetFileName(filePath));

                    // Oddělovač
                    sw.Write(delimiter);

                    // Zapsat soubor
                    using (StreamReader sr = new StreamReader(filePath)) sw.Write(sr.ReadToEnd());

                    // Oddělovač
                    sw.Write(delimiter); 
                }
            }
        }*/