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

#URL="get-into-teaching-api-dev"
#EXPECTED_SHA="730ff0a"

rval=0
FULL_URL="https://${URL}.london.cloudapps.digital/api/operations/health_check"
http_status=$(curl -o /dev/null -s -w "%{http_code}"  ${FULL_URL}) 
if [ "${http_status}" != "200" ] 
then
	echo "HTTP Status ${http_status}"
	rval=1
else
	echo "HTTP Status is Healthy"

        json=$(curl -s -X GET ${FULL_URL})
        status=$( echo ${json} | jq -r .status)
        if [ "${status}" == "unhealthy" ] 
        then
        	echo "Application Status is Unhealthy"
        	rval=1
        else
        	echo "Application Status is ${status}"
        fi

        sha=$( echo ${json} | jq -r .gitCommitSha)
        if [ "${sha}" != "${EXPECTED_SHA}"  ] 
        then
        	echo "SHA is not ${EXPECTED_SHA}"
        	rval=1
        else
                echo "SHA is correct"
        fi
fi
exit ${rval}

