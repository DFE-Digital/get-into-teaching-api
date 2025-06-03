#################################################################################################
###
### CURL the deployed containers healthcheck and return the status and SHA.
### check the SHA against a passed in parameter
###
### Input parameters ( not validated )
### 1 HOSTNAME
### 2 SHA
###
### Returns
### 1 on failure
### 0 on sucess
###
#################################################################################################
set -eu

HOSTNAME=${1}
EXPECTED_SHA=${2}

# Example:
# HOSTNAME="getintoteachingapi-test.test.teacherservices.cloud"
# EXPECTED_SHA="730ff0a"

rval=0
FULL_URL="https://${HOSTNAME}/api/operations/health_check"
http_status=$(curl -o /dev/null -s -w "%{http_code}"  ${FULL_URL})
if [ "${http_status}" != "200" ]
then
	echo "HTTP Status ${http_status}"
	rval=1
else
	echo "HTTP Status is Healthy"

        json=$(curl -s -X GET ${FULL_URL})
        status=$( echo ${json} | jq -r .status)
        if [ "${status}" != "healthy" ]
        then
        	echo "Application Status is ${status} (expected: healthy)"
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
