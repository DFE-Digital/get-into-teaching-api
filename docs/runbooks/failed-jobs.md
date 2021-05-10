#FailedJobs 
HIGH
 
##Description
Alerts when a background job fails. Jobs are retried once per hour for a maximum of 24 attempts/hours before they are deemed as failures. Failed jobs are then deleted permanently. 

##Potential Causes
The API uses background jobs for a few different purposes/tasks:

Updating postcode geolocation data.

Syncing data from the CRM.

Sending data to the CRM.

This alert indicates that one of these tasks has failed (after being retried once per hour for 24 hours).

##Resolutions
Check the Grafana panel for an indication of what’s going on.

We need to determine which type of job is failing as the priority, so head over to Kibana to find a job that has been Started but not finished (it will also log each of the 24 retries).

If the job that failed is a CrmSyncJob or LocationSyncJob or LocationBatchJob then there’s no reason to panic; the API maintains a local cache for these and a failure will only result in data being a little out of data. Check Kibana for an instance of these jobs that succeeded after the failure; if you find one then we’re all good and we can create a ticket to investigate what went wrong at a later date. If the jobs are continually failing then check Sentry, as there’s likely a related error in there. If the CrymSyncJob is continually failing it could mean we are serving outdated events in the GiT app and we may want to notify the events team.

If the UpsertCandidateJob failed it means a Candidate went through a sign up journey but we couldn’t send their information to the CRM. They will have received an email asking them to try signing up again. If it’s only a single job failure we should investigate the cause as a high priority; there should be information in Kibana and perhaps Sentry. It’s also worth checking with the CRM team to see if they had any outages; if we get multiple failures then it’s likely that the CRM is unavailable for some reason.
