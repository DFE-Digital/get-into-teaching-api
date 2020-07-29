#################################################################################################
###
### CURL the deployed containers healthcheck and return the status and SHA.
### check the SHA against a passed in parameter
###
### Input parameters ( not validated )
### 1 URL
### 2 SHA
###
### Returns
### 1 on failure
### 0 on sucess
###
#################################################################################################
URL=${1}
EXPECTED_SHA=${2}

# URL="get-into-teaching-api-dev"
# EXPECTED_SHA="730ff0a"

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m' 

rval=0
FULL_URL="https://${URL}.london.cloudapps.digital/api/operations/health_check"
http_status=$(curl -s -I ${FULL_URL} | head -1 | cut -d$' ' -f2)
if [ "${http_status}" != "405" ] 
then
	echo "${RED}HTTP Status ${http_status}${NC}"
	rval=1
else
	echo "${GREEN}HTTP Status is Healthy${NC}"

        json=$(curl -s -X GET ${FULL_URL})
        status=$( echo ${json} | jq -r .status)
        if [ "${status}" != "healthy" ] 
        then
        	echo "${RED}Application Status is not Healthy${NC}"
        	rval=1
        else
        	echo "${GREEN}Application Status is Healthy${NC}"
        fi

        sha=$( echo ${json} | jq -r .gitCommitSha)
        if [ "${sha}" != "${EXPECTED_SHA}"  ] 
        then
        	echo "${RED}SHA is not ${EXPECTED_SHA} ${NC}"
        	rval=1
        else
                echo "${GREEN}SHA is correct${NC}"
        fi
fi
exit ${rval}

