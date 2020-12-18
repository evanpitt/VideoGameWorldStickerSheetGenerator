using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using CsvHelper;
using VGWStickerSheetGenerator_EP.Constants;
using VGWStickerSheetGenerator_EP.Models;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace VGWStickerSheetGenerator_EP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog folderBrowserDialog;
        private OpenFileDialog openFileDialog;

        public MainWindow()
        {
            InitializeComponent();

            VersionLabel.Content = AppConstants.APP_VERSION;

            openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Multiselect = false
            };

            folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Choose where you want the results to go homie",
                ShowNewFolderButton = true
            };
        }

        private void Input_Select_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)openFileDialog.ShowDialog())
            {
                InputTextBox.Text = openFileDialog.FileName.ToString();
            }
        }

        private void Output_Select_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                OutputTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text == "")
            {
                MessageBox.Show(ErrorMessages.NULL_INPUT_FILE);
                InputTextBox.BorderBrush = Brushes.Red;
            }
            else if (OutputTextBox.Text == "")
            {
                MessageBox.Show(ErrorMessages.NULL_OUTPUT_LOCATION);
                OutputTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                var logOutputFile = "";
                Stopwatch stopwatch = new Stopwatch();
                StringBuilder sb = new StringBuilder();

                try
                {
                    stopwatch.Start();

                    sb.AppendLine(String.Format("{0} - {1}", AppFlowStates.RUN_STARTED, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                    var inputRecords = new List<VideoGameRowInput>();

                    sb.AppendLine(String.Format("{0} {1} - {2}", AppFlowStates.LOADING_INPUT_FILE, InputTextBox.Text, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                    using (var reader = new StreamReader(InputTextBox.Text))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        sb.AppendLine(String.Format("{0} - {1}", AppFlowStates.INPUT_FILE_LOADED_SUCCESS, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                        csv.Read();
                        csv.ReadHeader();

                        sb.AppendLine(String.Format("{0} - {1}", AppFlowStates.READING_INPUT_FILE, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                        while (csv.Read())
                        {
                            var record = new VideoGameRowInput
                            {
                                ProductName = csv.GetField<string>("Product Name"),
                                Category = csv.GetField<string>("Category"),
                                SellPrice = csv.GetField<string>("Sell Price"),
                                Barcode = csv.GetField<string>("Barcode"),
                                TotalQty = csv.GetField<int>("Total Qty")
                            };

                            inputRecords.Add(record);
                        }

                        sb.AppendLine(String.Format("{0} ~~ {1} Rows of data (aka distinct games) - {2}", AppFlowStates.INPUT_FILE_READ_SUCCESS, inputRecords.Count, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));
                    }

                    sb.AppendLine(String.Format("{0} - {1}", AppFlowStates.CREATE_STICKERS_STARTING, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                    var outputFile = string.Format("{0}\\results_{1}.csv", OutputTextBox.Text, DateTime.Now.ToString(AppConstants.OUTPUT_FILE_DATE_FORMAT));
                    using (var writer = new StreamWriter(outputFile))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        var outputRecords = new List<VideoGameRowOutput>();

                        int totalStickersCalculation = inputRecords.Where(item => item.TotalQty > 0).Sum(item => item.TotalQty);

                        sb.AppendLine(String.Format("{0}: {1} - {2}", AppFlowStates.STICKERS_CALCULATED, totalStickersCalculation, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                        foreach (VideoGameRowInput record in inputRecords)
                        {
                            if (record.TotalQty <= 0)
                            {
                                sb.AppendLine(String.Format("Game Ignored: ProductName~{0} | Category~{1} | SellPrice~{2} | Barcode~{3} | Quantity~{4} ~~ {5}",
                                    record.ProductName,
                                    record.Category,
                                    record.SellPrice,
                                    record.Barcode,
                                    record.TotalQty,
                                    DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));
                            }

                            for (int i = 0; i < record.TotalQty; i++)
                            {
                                totalStickersCalculation += (i + 1);
                                outputRecords.Add(new VideoGameRowOutput
                                {
                                    ProductName = record.ProductName,
                                    Category = record.Category,
                                    SellPrice = record.SellPrice,
                                    Barcode = record.Barcode
                                });
                            }
                        }

                        var sortedOutputRecords = outputRecords.OrderBy(d => d.Category);

                        sb.AppendLine(String.Format("{0}: {1} Stickers created - {2}", AppFlowStates.FINISHED_CREATING_STICKERS, outputRecords.Count, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                        sb.AppendLine(String.Format("{0} - {1}", AppFlowStates.EXPORTING_RESULTS, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                        csv.WriteRecords(sortedOutputRecords);

                        sb.AppendLine(String.Format("{0}: {1}  - {2}", AppFlowStates.EXPORTING_RESULTS_SUCCESS, outputFile, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));
                    }

                    logOutputFile = string.Format("{0}\\Log_{1}.txt", OutputTextBox.Text, DateTime.Now.ToString(AppConstants.OUTPUT_FILE_DATE_FORMAT));

                    stopwatch.Stop();

                    sb.AppendLine(String.Format("{0} - Total Elapsed time: {1} -- {2}", AppFlowStates.APPLICATION_FINISHED_SUCCESSFULLY, stopwatch.Elapsed, DateTime.Now.ToString(AppConstants.LOG_DATE_FORMAT)));

                    File.WriteAllText(logOutputFile, sb.ToString());

                    Stream str = Properties.Resources.Success;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();

                    MessageBox.Show(AppFlowStates.APPLICATION_FINISHED_SUCCESSFULLY);
                }
                catch (Exception exception)
                {
                    logOutputFile = string.Format("{0}\\Log_{1}.txt", OutputTextBox.Text, DateTime.Now.ToString(AppConstants.OUTPUT_FILE_DATE_FORMAT));

                    sb.AppendLine(String.Format("Exception Occurred: {0} \nStackTrace: {1}", exception.Message, exception.StackTrace));

                    File.WriteAllText(logOutputFile, sb.ToString());

                    Stream str = Properties.Resources.Failure;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();

                    MessageBox.Show(ErrorMessages.EXCEPTION);
                }
            }
        }

        private void InputTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (InputTextBox.Text != "")
            {
                if (File.Exists(InputTextBox.Text))
                {
                    InputTextBox.BorderBrush = Brushes.Green;
                    ValidInputImage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show(ErrorMessages.NULL_INPUT_FILE);
                InputTextBox.BorderBrush = Brushes.Red;
            }
        }

        private void OutputTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (OutputTextBox.Text != "")
            {
                if (Directory.Exists(OutputTextBox.Text))
                {
                    OutputTextBox.BorderBrush = Brushes.Green;
                    ValidOutputImage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show(ErrorMessages.NULL_OUTPUT_LOCATION);
                OutputTextBox.BorderBrush = Brushes.Red;
            }
        }
    }
}