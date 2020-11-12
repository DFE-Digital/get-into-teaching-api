Data Source for Influx

Currently I have no way of configuring the influxdb data source. as it is not often needed it can be carried out manually.

You will need access to cloud foundry CLI and be able to 'cf env <prometheus application>'

The vaules you need are:

1. The URL of the Database i.e. https://prod-lon-70d73563-f5f9-4b94-9b93-d95ffe1a4a2d-paas-cf-prod.aivencloud.com:19676
2. The Username
3. The Password

These all go in the logical places for the datasource, which should be called InfluxDB
There is one other setting that MUST be right and this is the database, this must be _internal

