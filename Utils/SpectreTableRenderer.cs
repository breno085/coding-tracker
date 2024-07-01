using Spectre.Console;
using coding_tracker.Models;

namespace coding_tracker.Utils
{
    public class SpectreTableRenderer
    {
        public static void RenderTable(List<CodingTracker> tableData)
        {
            var table = new Table();

            table.AddColumn("Id");
            table.AddColumn("Date");
            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Duration");

            foreach (var dw in tableData)
            {
                table.AddRow(
                    dw.Id.ToString(),
                    dw.Date.ToString("dd-MM-yyyy"),
                    dw.StartTime.ToString("hh\\:mm"),
                    dw.EndTime.ToString("hh\\:mm"),
                    dw.Duration.ToString("hh\\:mm")
                );
            }
            // Render the table to the console
            AnsiConsole.Write(table);
        }
    }
}