#ClientApproachingRateLimit 
MEDIUM 
##Description
Alerts when the API is reviving a lot of requests to rate limited endpoints from a client (and is in danger of returning a 429 response soon). 

##Potential Causes
See the potential causes in the TooManyRequests alert.

##Resolutions
Check the [Grafana panel](https://grafana-prod-get-into-teaching.london.cloudapps.digital/d/28EURzZGz/get-into-teaching-api?viewPanel=60&orgId=1&var-App=get-into-teaching-api-prod) for an indication of what’s going on.

If it was a caused by a spike that did not get very close to the critical threshold then it can be safely ignored for now and monitored going forward.

If the spike approached the critical threshold and either returned back to normal or is staying consistently above the warning threshold, you should follow the resolutions in the TooManyRequests alert.
