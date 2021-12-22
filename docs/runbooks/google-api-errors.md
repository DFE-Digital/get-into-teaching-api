# GoogleApiErrors 

MEDIUM 

## Description

Alerts when the Google Geocoding API returns a non-success response.

## Potential Causes

The API is attempting to geolocate a postcode using the Google Geocoding API, but is receiving a non-success response. This indicates one of two things:

The Google API key is no longer valid/has been revoked.

The postcode cannot be geolocated by Google.

## Resolutions

[Check the Grafana panel for an indication of what’s going on](https://grafana-prod-get-into-teaching.london.cloudapps.digital/d/28EURzZGz/get-into-teaching-api?viewPanel=57&orgId=1&var-App=get-into-teaching-api-prod).

We [log the status text response](https://kibana.logit.io/app/kibana#/discover?_g=()&_a=(columns:!(message),filters:!(),index:'8ac115c0-aac1-11e8-88ea-0383c11b333a',interval:auto,query:(language:kuery,query:'%22get-into-teaching-api-prod%22%20AND%20%22Google%20API%20Status%22'),sort:!())) from Google when we make a call to their API; it should give a clear indication of what the error is. If it's only occurring a small number of times we can create a ticket to investigate it. If the HighGoogleApiCalls alert is also firing then we should follow the resolutions outlined for that alert as well.
