using DRNumberCrunchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DRLPTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DRParserTest : Window
    {
        private Rally rallyData;

        public DRParserTest()
        {
            InitializeComponent();

            rallyData = new Rally();
            DataContext = this;

            label_statusMessage.Content = "";
        }

        // enum for the type of output to print
        public enum PrintType
        {
            Stages,
            Overall,
            Chart
        }

        // binds to the print type combobox
        private Dictionary<PrintType, string> printTypeDict = new Dictionary<PrintType, string>
        {
            { PrintType.Stages, "Stages" },
            { PrintType.Overall, "Overall" },
            { PrintType.Chart, "Chart" }
        };

        public Dictionary<PrintType, string> PrintTypeDict
        {
            get { return printTypeDict; }
        }

        // holds the user selected print type
        public PrintType SelectedPrintType { get; set; }


        private void button_parseStage_Click(object sender, RoutedEventArgs e)
        {
            // TODO: move all parsing code to an external module
            // TODO: input format should not be hardcoded

            // get text from textbox
            var lineCount = textBox_resultsInput.LineCount;

            if (lineCount < 1)
                return;

            var lines = textBox_resultsInput.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            // parse into stage data
            var stageData = new Stage();
            char[] separator = { '\t' };

            foreach (var line in lines)
            {
                var splitLine = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var splitCount = splitLine.Count();

                for (int i = 0; i < splitCount; i++)
                    splitLine[i] = splitLine[i].Trim();

                if (splitCount == 5)
                {
                    stageData.AddDriver(new DriverTime(Int32.Parse(splitLine[0]), splitLine[1], splitLine[2], splitLine[3], splitLine[4]));
                }
                else if (splitCount == 6)
                {
                    stageData.AddDriver(new DriverTime(Int32.Parse(splitLine[0]), splitLine[2], splitLine[3], splitLine[4], splitLine[5], splitLine[1]));
                }
                else if (splitCount == 7)
                {
                    stageData.AddDriver(new DriverTime(Int32.Parse(splitLine[0]), splitLine[3], splitLine[4], splitLine[5], splitLine[6], splitLine[2]));
                }
                else
                {
                    label_statusMessage.Content = "Parse failure";
                    label_statusMessage.Foreground = Brushes.Red;
                    return;
                }
            }

            rallyData.AddStage(stageData);

            textBox_resultsInput.Clear();
            label_statusMessage.Content = String.Format("SS{0} Has been parsed.", rallyData.StageCount);
            label_statusMessage.Foreground = Brushes.Green;
        }

        private void button_crunchNumbers_Click(object sender, RoutedEventArgs e)
        {
            rallyData.CalculateTimes();
            label_statusMessage.Content = "Numbers Crunched";
            label_statusMessage.Foreground = Brushes.Green;
        }

        private void button_clearAllData_Click(object sender, RoutedEventArgs e)
        {
            rallyData = new Rally();
            label_statusMessage.Content = "Data Cleared";
            label_statusMessage.Foreground = Brushes.Green;
            textBox_resultsInput.Clear();
        }

        private void printOverallTimes()
        {
            var outputSB = new StringBuilder();

            int stageCount = 1;
            foreach (Stage stage in rallyData)
            {
                outputSB.AppendLine("SS" + stageCount);
                outputSB.AppendLine("Overall");
                outputSB.AppendLine("Pos, Pos Chng, Name, Vehicle, Time, Diff 1st, Diff Prev");

                List<KeyValuePair<string, DriverTime>> sortedStageData = stage.DriverTimes.ToList();
                sortedStageData.Sort((x, y) =>
                {
                    if (x.Value != null && y.Value == null)
                        return -1;
                    else if (x.Value == null && y.Value != null)
                        return 1;
                    else if (x.Value == null && y.Value == null)
                        return 0;
                    else
                        return x.Value.CalculatedOverallTime.CompareTo(y.Value.CalculatedOverallTime);
                });

                foreach (KeyValuePair<string, DriverTime> driverTimeKvp in sortedStageData)
                {
                    var driverName = driverTimeKvp.Key;
                    var driverTime = driverTimeKvp.Value;

                    if (driverTime != null)
                    {
                        var formatString = @"hh\:mm\:ss\.fff";
                        var line = driverTime.OverallPosition + "," +
                                   driverTime.CalculatedPositionChange + "," +
                                   driverTime.Driver + "," +
                                   driverTime.Vehicle + "," +
                                   driverTime.CalculatedOverallTime.ToString(formatString) + "," +
                                   driverTime.CalculatedOverallDiffFirst.ToString(formatString) + "," +
                                   driverTime.CalculatedOverallDiffPrevious.ToString(formatString);

                        outputSB.AppendLine(line);
                    }
                    else
                    {
                        outputSB.AppendLine(",," + driverName + ",,DNF");
                    }
                }

                outputSB.AppendLine("");
                stageCount++;
            }

            label_statusMessage.Content = "Displaying Overall Times";
            label_statusMessage.Foreground = Brushes.Green;
            textBox_resultsInput.Text = outputSB.ToString();
        }

        private void printStageTimes()
        {
            var outputSB = new StringBuilder();

            int stageCount = 1;

            foreach (Stage stage in rallyData)
            {
                outputSB.AppendLine("SS" + stageCount);
                outputSB.AppendLine("Stage");
                outputSB.AppendLine("Pos, Name, Vehicle, Time, Diff 1st, Diff Prev");

                List<KeyValuePair<string, DriverTime>> sortedStageData = stage.DriverTimes.ToList();
                sortedStageData.Sort((x, y) =>
                {
                    if (x.Value != null && y.Value == null)
                        return -1;
                    else if (x.Value == null && y.Value != null)
                        return 1;
                    else if (x.Value == null && y.Value == null)
                        return 0;
                    else
                        return x.Value.CalculatedStageTime.CompareTo(y.Value.CalculatedStageTime);
                });

                foreach (KeyValuePair<string, DriverTime> driverTimeKvp in sortedStageData)
                {
                    var driverName = driverTimeKvp.Key;
                    var driverTime = driverTimeKvp.Value;

                    if (driverTime != null)
                    {
                        var formatString = @"mm\:ss\.fff";
                        var line = driverTime.CaclulatedStagePosition + "," +
                                   driverTime.Driver + "," +
                                   driverTime.Vehicle + "," +
                                   driverTime.CalculatedStageTime.ToString(formatString) + "," +
                                   driverTime.CalculatedStageDiffFirst.ToString(formatString) + "," +
                                   driverTime.CalculatedStageDiffPrevious.ToString(formatString);

                        outputSB.AppendLine(line);
                    }
                    else
                    {
                        outputSB.AppendLine("," + driverName + ",,DNF");
                    }
                }

                outputSB.AppendLine("");
                stageCount++;
            }
            label_statusMessage.Content = "Displaying Stage Times";
            label_statusMessage.Foreground = Brushes.Green;
            textBox_resultsInput.Text = outputSB.ToString();
        }

        private void printChartOutput()
        {
            var outputSB = new StringBuilder();
            var positionDict = new Dictionary<string, List<int>>();
            List<KeyValuePair<string, DriverTime>> sortedStageData = null;

            foreach (Stage stage in rallyData)
            {
                sortedStageData = stage.DriverTimes.ToList();
                sortedStageData.Sort((x, y) =>
                {
                    if (x.Value != null && y.Value == null)
                        return -1;
                    else if (x.Value == null && y.Value != null)
                        return 1;
                    else if (x.Value == null && y.Value == null)
                        return 0;
                    else
                        return x.Value.CalculatedOverallTime.CompareTo(y.Value.CalculatedOverallTime);
                });

                foreach (KeyValuePair<string, DriverTime> driverTimeKvp in sortedStageData)
                {
                    if (driverTimeKvp.Value == null)
                        continue;

                    var driverKey = driverTimeKvp.Key;
                    var position = driverTimeKvp.Value.OverallPosition;

                    if (!positionDict.ContainsKey(driverKey))
                        positionDict.Add(driverKey, new List<int>());

                    positionDict[driverKey].Add(position);
                }
            }

            label_statusMessage.Content = "Displaying Chart Output";
            label_statusMessage.Foreground = Brushes.Green;


            // sortedStageData should contain the last stage, sorted by overall time
            if (sortedStageData == null)
                return;

            foreach (KeyValuePair<string, DriverTime> driverTimeKvp in sortedStageData)
            {
                var driverKey = driverTimeKvp.Key;
                var positionList = positionDict[driverKey];

                string line = driverKey + "," + String.Join(",", positionList);

                outputSB.AppendLine(line);
            }

            textBox_resultsInput.Text = outputSB.ToString();
        }

        private void button_print_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPrintType == PrintType.Stages)
                printStageTimes();
            else if (SelectedPrintType == PrintType.Overall)
                printOverallTimes();
            else if (SelectedPrintType == PrintType.Chart)
                printChartOutput();
        }

        private void button_copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox_resultsInput.Text);
            label_statusMessage.Content = "Data Copied to Clipboard";
            label_statusMessage.Foreground = Brushes.Green;
        }

        private async void button_parseRacenetApi_Click(object sender, RoutedEventArgs e)
        {
            // check and clean inputs
            var eventId = textBox_eventId.Text;
            if (String.IsNullOrWhiteSpace(eventId))
            {
                label_statusMessage.Content = "No event ID";
                label_statusMessage.Foreground = Brushes.Red;
                return;
            }

            int eventIdInt;
            if (!Int32.TryParse(eventId, out eventIdInt))
            {
                label_statusMessage.Content = "Event ID must be an integer";
                label_statusMessage.Foreground = Brushes.Red;
                return;
            }

            eventId = eventId.Trim();

            // clear all data
            rallyData = new Rally();
            label_statusMessage.Content = "Data Cleared";
            label_statusMessage.Foreground = Brushes.Green;
            textBox_resultsInput.Clear();

            // progess reporting from task
            var progress = new Progress<int>(stagesProcessed =>
            {
                label_statusMessage.Content = "Fetching data from Racenet... " + stagesProcessed + " stages retrieved ";
                label_statusMessage.Foreground = Brushes.Green;
            });

            // run task to get data
            var getDataTask = Task<Rally>.Factory.StartNew(() => RacenetApiParser.GetRallyData(eventId, progress));

            label_statusMessage.Content = "Fetching data from Racenet (be patient, Racenet is slow)...";
            label_statusMessage.Foreground = Brushes.Green;

            await getDataTask;

            rallyData = getDataTask.Result;

            // crunch numbers
            if (rallyData == null)
            {
                label_statusMessage.Content = "Failed to get data from Racenet";
                label_statusMessage.Foreground = Brushes.Red;
            }
            else
            {
                rallyData.CalculateTimes();
                label_statusMessage.Content = rallyData.StageCount + " stages retrieved from Racenet, numbers crunched sucessfully";
                label_statusMessage.Foreground = Brushes.Green;
            }
        }
    }
}
