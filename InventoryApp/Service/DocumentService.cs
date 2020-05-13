using InventoryApp.ViewModels.Base;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace InventoryApp.Service
{
    class DocumentService : ViewModelsBase
    {
        private string FileDirectory = DateTime.Now.ToString("dd-MMM-yyyy");
        private string CurrentDirectory = Environment.CurrentDirectory;
        private string PathToFile = null;

        public string ExportInformationToFile<T>(T instanse, string documentType) where T : class
        {
            if (Properties.Settings.Default.SaveDocsAutomaticly)
            {
                if (!Directory.Exists(Path.Combine(CurrentDirectory, FileDirectory)))
                {
                    Directory.CreateDirectory(Path.Combine(CurrentDirectory, documentType, FileDirectory));
                }
                PathToFile = Path.Combine(CurrentDirectory, documentType, FileDirectory, BaseService.GenerateRandomString());
            }
            else if (!Properties.Settings.Default.SaveDocsAutomaticly)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.ShowDialog();
                var newFileDirectoryName = Path.GetDirectoryName(fileDialog.FileName);
                if (!Directory.Exists(Path.Combine(newFileDirectoryName, FileDirectory)))
                {
                    Directory.CreateDirectory(Path.Combine(newFileDirectoryName, documentType, FileDirectory));
                }
                PathToFile = Path.Combine(newFileDirectoryName, documentType, FileDirectory, Path.GetFileName(fileDialog.FileName));
            }
            bool isCompleted = CreateDocument(PathToFile, instanse);
            if (isCompleted)
            {
                return "Инфо: Экспорт в файл успешно завершен.";
            }
            else
            {
                return "Ошибка: Экспорт в файл завершен с ошибкой.";
            }
        }

        public List<string> GetFromDocument(string path)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Document doc = new Document();
            object fileName = path;
            object missing = System.Type.Missing;
            doc = word.Documents.Open(ref fileName,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing);
            List<string> data = new List<string>();
            for (int i = 0; i < doc.Paragraphs.Count; i++)
            {
                string temp = doc.Paragraphs[i + 1].Range.Text.Trim().Replace("\r\a", string.Empty).Replace("\a", string.Empty);
                if (temp != string.Empty)
                    data.Add(temp.Trim());
            }
            doc.Close();
            word.Quit();
            return data;
        }

        private bool CreateDocument<T>(string path, T firstLevelInstanse) where T : class
        {
            List<string> Headers = new List<string>();
            List<string> Rows = new List<string>();
            bool isCompleted = false;
            try
            {
                Microsoft.Office.Interop.Word.Application winword = new Microsoft.Office.Interop.Word.Application();
                winword.ShowAnimation = false;
                winword.Visible = false;
                object missing = System.Reflection.Missing.Value;

                Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                foreach (Section section in document.Sections)
                {
                    Range headerRange = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, WdFieldType.wdFieldPage);
                    headerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = WdColorIndex.wdBlack;
                    headerRange.Font.Size = 12;
                    headerRange.Text = "Отчет за: " + DateTime.Now;
                }

                document.Content.SetRange(0, 0);
                foreach (var firstLevelFields in firstLevelInstanse.GetType().GetProperties().OrderBy(orderBy => orderBy.MetadataToken))
                {
                    var firstLevelProperty = firstLevelInstanse.GetType().GetProperty(firstLevelFields.Name);
                    if (firstLevelFields.PropertyType.FullName.Contains("InventoryApp"))
                    {
                        var secondLevelInstance = firstLevelProperty.GetValue(firstLevelInstanse);
                        foreach (var secondLevelFields in secondLevelInstance.GetType().GetProperties().OrderBy(orderBy => orderBy.MetadataToken))
                        {
                            var secondLevelProperty = secondLevelInstance.GetType().GetProperty(secondLevelFields.Name);
                            if (secondLevelProperty.PropertyType.FullName.Contains("InventoryApp"))
                            {
                                var thirdLevelInstance = secondLevelProperty.GetValue(secondLevelInstance);
                                foreach (var thirdLevelFields in thirdLevelInstance.GetType().GetProperties().OrderBy(orderBy => orderBy.MetadataToken))
                                {
                                    var thirdLevelProperty = thirdLevelInstance.GetType().GetProperty(thirdLevelFields.Name);
                                    var thirdLevelPropertyValue = thirdLevelProperty.GetValue(thirdLevelInstance).ToString();
                                    if (!thirdLevelFields.Name.Contains("Id"))
                                    {
                                        Headers.Add(firstLevelFields.Name + " " + thirdLevelProperty.Name);
                                        Rows.Add(thirdLevelPropertyValue);
                                    }
                                }
                            }
                            else
                            {
                                var secondLevelPropertyValue = secondLevelProperty.GetValue(secondLevelInstance).ToString();
                                if (!secondLevelFields.Name.Contains("Id"))
                                {
                                    Headers.Add(firstLevelFields.Name + " " + secondLevelProperty.Name);
                                    Rows.Add(secondLevelPropertyValue);
                                }
                            }
                        }
                    }
                    else
                    {
                        var firstLevelPropertyValue = firstLevelProperty.GetValue(firstLevelInstanse).ToString();
                        if (!firstLevelFields.Name.Contains("Id"))
                        {
                            Headers.Add(firstLevelProperty.Name);
                            Rows.Add(firstLevelPropertyValue);
                        }
                    }
                }

                Paragraph mainParagraph = document.Content.Paragraphs.Add(ref missing);

                Table firstTable = document.Tables.Add(mainParagraph.Range, Headers.Count / 2, 2, ref missing, ref missing);
                int x = 0;

                firstTable.Borders.Enable = 1;
                foreach (Row row in firstTable.Rows)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        if (cell.RowIndex == 1)
                        {
                            cell.Range.Font.Bold = 1;
                            cell.Range.Font.Name = "Times New Roman";
                            cell.Range.Font.Size = 12;
                            cell.Range.Font.ColorIndex = WdColorIndex.wdGray25;
                            cell.Shading.BackgroundPatternColor = WdColor.wdColorBlack;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        else
                        {
                            cell.Range.Text += Headers[x];
                            cell.Range.Text += Rows[x];
                            x++;
                        }
                    }
                }

                object filename = path + "-" + DateTime.Now.ToString("HH-mm") + ".docx";
                document.SaveAs2(ref filename);
                document.Close(ref missing, ref missing, ref missing);
                winword.Quit(ref missing, ref missing, ref missing);
                isCompleted = true;
            }
            catch (Exception e)
            {
                isCompleted = false;
            }
            return isCompleted;
        }
    }
}
