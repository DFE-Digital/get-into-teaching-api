paas_space                = "get-into-teaching-production"
monitor_space             = "get-into-teaching-monitoring"
paas_database_common_name = "get-into-teaching-api-prod-pg-common-svc"
database_plan             = "small-ha-11"
paas_redis_1_name         = "get-into-teaching-prod-redis-svc"
paas_api_application_name = "get-into-teaching-api-prod"
app_application_name      = "get-into-teaching-app-prod"
tta_application_name      = "get-teacher-training-adviser-service-prod"
application_instances     = 2
application_memory        = 2048
monitoring                = 1
influx                    = 1
environment               = "prod"
azure_key_vault           = "s146p01-kv"
azure_resource_group      = "s146p01-rg"
alerts                    = {}

