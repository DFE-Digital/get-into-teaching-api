# HighGoogleApiCalls 

HIGH 

## Description

Alerts when the API makes more than 5 requests to the Google Geocoding API in the space of 10 minutes.

## Potential Causes

The API calls out to the Google Geocoding API when it encounters a postcode that it does not have the geolocation for in its local cache. This can happen from two potential scenarios:

A teaching event is returned from the CRM that contains a postcode the API has not yet geocoded.

A user searches for an event in the GiT app using a postcode that the API has not yet geocoded.

As postcode geolocations get cached in the API its unlikely that this is going to be triggered from a user searching for an event (postcodes are validated, so when a valid postcode comes through it will thereafter be cached meaning a spike in Google API calls should not be possible).

The most likely cause of this is that a teaching event has been added to the CRM that contains a postcode we are not able to geolocate (either by using our local cache or the Google Geocoding API). As we don’t cache the geocoding failures the API will call out to Google repeatedly; once per event that uses the postcode per sync (which is ran every 5 minutes).

## Resolutions

[Check the Grafana panel for an indication of what’s going on](https://grafana-prod-get-into-teaching.london.cloudapps.digital/d/28EURzZGz/get-into-teaching-api?viewPanel=57&orgId=1&var-App=get-into-teaching-api-prod).

If a postcode is repeatedly failing to geolocate then in 99.9% of cases it’s going to be invalid and we should ask the CRM team to correct it in Dynamics. Once that’s done the calls out to Google’s Geocoding API should stop.

If somehow the postcode is in fact valid and Google is not able to geocode it we would need to manually add it to our cached data set. We could either write a one-time script to do this and deploy it (safest method) or shell into the production database and manually add a record for it.

If neither of those is an option and we need to prevent this from racking up a big bill with Google, it is perfectly safe to revoke the Google API key. The only thing to keep in mind is that the event using the valid postcode that we are not capable of being geolocating will not be searchable by distance from the GiT website.
