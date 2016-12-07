# DRLeagueParser
A parser for Dirt Rally league results

Some basic instructions, more detailed docs are coming

**Parsing via Racenet API:**

All results can now be parsed via the Racenet API, this can also pull the stage data from rallies that have expired and are no longer available via dirtgame.com. The event ID can be found in the page source of the event on the event page at dirtgame.com.

Simply put the event ID into the "Parse via Racenet" input and all stages will be pulled and parsed!



**Parsing via text:**

1) Copy the results of a stage from the RaceNet site and paste into the large text box in the app, then click "Parse Stage Data"

2) Repeat step one for each stage in your event (don't combine or split up stages, each parse is expected to be it's own stage)

3) Once all stages are parsed click "Crunch Numbers" to calculate individual stage times from the parsed overall times

4) Click "Print Stage Times" to output the individual stage times and winners, and "Print Overall Times" to output the overall times (overall should be the same as the RaceNet results)

The output is in CSV, I then put this into a spreadsheet for formatting.

Note: I use Firefox and the copied data is tab delimited, but this is not the case for all browsers. The input data must be tab delimited. I also use US locale settings on the RaceNet site, but I don't think other locales will cause an issue. The times are expected to be in the "mm:ss.fff" format, which I believe is an international standard.
