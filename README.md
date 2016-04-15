# DRLeagueParser
A parser for Dirt Rally league results

Some basic instructions, more detailed docs are coming

1) Copy the results of a stage from the RaceNet site and paste into the large text box in the app, then click "Parse Stage Data"

2) Repeat step one for each stage in your event (don't combine or split up stages, each parse is expected to be it's own stage)

3) Once all stages are parsed click "Crunch Numbers" to calculate individual stage times from the parsed overall times

4) Click "Print Stage Times" to output the individual stage times and winners, and "Print Overall Times" to output the overall times (overall should be the same as the RaceNet results)

The output is in CSV, I then put this into a spreadsheet for formatting.

Note: I use Firefox and the copied data is tab delimited, but this is not the case for all browsers. The input data must be tab delimited. I also use US locale settings on the RaceNet site, but I don't think other locales will cause an issue. The times are expected to be in the "mm:ss.fff" format, which I believe is an international standard.
