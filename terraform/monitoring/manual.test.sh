cf install-plugin copyenv
cf copyenv redis-exporter-test-get-into-teaching > /tmp/file$$
. /tmp/file$$
rm /tmp/file$$
host=$(echo $VCAP_SERVICES | jq -r '.redis[0].credentials.host')
port=$(echo $VCAP_SERVICES | jq -r '.redis[0].credentials.port')

PASSWORD=$(echo $VCAP_SERVICES | jq -r '.redis[0].credentials.password')
ADDR="rediss://${host}:${port}"

cf set-env redis-exporter-test-get-into-teaching REDIS_ADDR  ${ADDR} 
cf set-env redis-exporter-test-get-into-teaching REDIS_PASSWORD  ${PASSWORD} 
cf restage redis-exporter-test-get-into-teaching

cf copyenv postgres-exporter-test-get-into-teaching-1 > /tmp/file$$
. /tmp/file$$
rm /tmp/file$$
URI=$(echo $VCAP_SERVICES | jq -r '.postgres[0].credentials.uri')

cf set-env postgres-exporter-test-get-into-teaching-1  DATA_SOURCE_NAME  ${URI} 
cf restage postgres-exporter-test-get-into-teaching-1

cf copyenv postgres-exporter-test-get-into-teaching-2 > /tmp/file$$
. /tmp/file$$
rm /tmp/file$$
URI=$(echo $VCAP_SERVICES | jq -r '.postgres[0].credentials.uri')

cf set-env postgres-exporter-test-get-into-teaching-2  DATA_SOURCE_NAME  ${URI} 
cf restage postgres-exporter-test-get-into-teaching-2
