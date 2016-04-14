using DRTimeCruncher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            // get text from textbox
            var lineCount = textBox_resultsInput.LineCount;

            if (lineCount < 1)
                return;

            var lines = textBox_resultsInput.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            // parse into stage data
            var stageData = new Stage();

            foreach (var line in lines)
            {
                var splitLine = line.Split('\t');
                var splitCount = splitLine.Count();

                for (int i = 0; i < splitCount; i++)
                    splitLine[i] = splitLine[i].Trim();

                if (splitCount == 6)
                    stageData.AddDriver(new DriverTime(Int32.Parse(splitLine[0]), splitLine[2], splitLine[3], splitLine[4], splitLine[5], splitLine[1]));
                else if (splitCount == 5)
                    stageData.AddDriver(new DriverTime(Int32.Parse(splitLine[0]), splitLine[2], splitLine[3], splitLine[4], splitLine[5]));
                //else
                    //bad parse fail
            }

            rallyData.AddStage(stageData);

			textBox_resultsInput.Text = String.Format("SS{0} Has been parsed.", rallyData.StageCount);
        }

		private void button_crunchNumbers_Click(object sender, RoutedEventArgs e)
		{
			rallyData.CalculateTimes();
			textBox_resultsInput.Text = "Numbers Crunched.";
			
		}

		private void button_clearAllData_Click(object sender, RoutedEventArgs e)
		{
			rallyData = new Rally();
			textBox_resultsInput.Text = "Data Cleared.";
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
						outputSB.AppendLine("," + driverName + ",DNF");
					}
				}

				outputSB.AppendLine("");
				stageCount++;
			}

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
						outputSB.AppendLine(",," + driverName + ",DNF");
					}
				}

				outputSB.AppendLine("");
				stageCount++;
			}

			textBox_resultsInput.Text = outputSB.ToString();
		}
    }
}
