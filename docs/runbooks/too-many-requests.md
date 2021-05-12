#TooManyRequests 
MEDIUM 

##Description
Alerts when the API has received too many requests from a client and has responded with a 429 status code. 

##Potential Causes
The API is configured to rate limit requests to potentially exploitable endpoints, accepting a maximum number of requests per client per rate limited endpoint. Currently the GiT app and TTA service are considered a single client as they share an API key. Rate limited endpoints are:

POST /api/candidates/access_tokens (250rpm)

POST /api/teacher_training_adviser/candidates (100rpm)

POST /api/mailing_list/members (100rpm)

POST /api/teaching_events/attendees (100rpm)

The likely cause of this alert firing is an unexpected spike in sign ups from the GiT app or TTA service. A possible cause could be targeted advertising sending high volumes of traffic to the website in a short period of time, resulting in abnormally high conversions.

A more sinister cause of this alert would be if the website is the target of an automated attack, creating high levels of false sign ups. The chance of this happening should be low; there is additional rate limiting in the GiT app and TTA service that ensure sign ups from the same IP address are restricted.

##Resolutions
Check the Grafana panel for an indication of what’s going on.

The likelihood of this alert firing multiple times in quick succession is low and, given time, sign ups should return to a normal level without intervention. We also handle this gracefully in the GiT website and TTA service.

If the sign ups that triggered this alert are deemed legitimate, we should look to increase the rate limiting threshold in the API to account for the spike.

If the sign ups were from a malicious source then the rate limiting has done its job and we need to inform the CRM team that there will be a (hopefully small) number of candidates in the CRM from the false sign ups that went through before the rate limiting came into effect; they will want to cleanse the data on their end.
