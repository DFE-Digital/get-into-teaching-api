# HighCpu
 MEDIUM 
## Description
Alerts when any of the API instances exceed 70% CPU utilisation.

## Potential Causes
The API has likely received a spike in traffic that it is struggling to keep up with.

## Resolutions
Check the Grafana panel for an indication of what’s going on.

If it was a short spike that has since returned to normal it should be nothing to worry about and we can monitor it going forward. Its also worth investigating if it has had or is having an impact on response times (there are a number of graphs under the performance row in the API dashboard that will be useful here).

If the spike is not subsiding/returning to normal, then we should investigate further what is causing it and if it is effecting a single or multiple instances. An additional instance of the API could also be spun up to to help alleviate some of the burden and hopefully return the CPU utilisation back to normal.
