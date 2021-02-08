#!/bin/bash

name=$RANDOM
name=STEVE
url='https://alertmanager-dev-get-into-teaching.london.cloudapps.digital:/api/v1/alerts'

echo "firing up alert $name" 

# change url o
curl -XPOST $url -d "[{ 
	\"status\": \"firing\",
	\"labels\": {
		\"severity\":\"medium\"
	},
	\"annotations\": {
		\"summary\": \"This is a test of the alerting system on development, this test can be ignored.\",
		\"runbook\": \"https://dfedigital.atlassian.net/wiki/spaces/GGIT/pages/2152595459/Test+Page\",
		\"dashboard\": \"https://grafana-dev-get-into-teaching.london.cloudapps.digital/d/qZjcqcpGz/csp-violations?orgId=1\",
		\"description\": \"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\"
	},
	\"generatorURL\": \"http://prometheus.int.example.net/${name}\"
}]"

