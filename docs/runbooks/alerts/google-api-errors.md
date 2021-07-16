#GoogleApiErrors 
MEDIUM 
##Description
Alerts when the Google Geocoding API returns a non-success response.

##Potential Causes
The API is attempting to geolocate a postcode using the Google Geocoding API, but is receiving a non-success response. This indicates one of two things:

The Google API key is no longer valid/has been revoked.

The postcode cannot be geolocated by Google.

##Resolutions
Check the Grafana panel for an indication of what’s going on.

We log the status text response from Google when we make a call to their API; it should give a clear indication of what the error is. If it's only occurring a small number of times we can create a ticket to investigate it. If the HighGoogleApiCalls alert is also firing then we should follow the resolutions outlined for that alert as well.
