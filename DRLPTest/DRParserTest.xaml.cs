using DRTimeCruncher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

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

		private void button_printOverallTimes_Click(object sender, RoutedEventArgs e)
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
						var formatString = @"mm\:ss\.ff";
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

		private void button_printStageTimes_Click(object sender, RoutedEventArgs e)
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
						var formatString = @"mm\:ss\.ff";
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
    }
}
