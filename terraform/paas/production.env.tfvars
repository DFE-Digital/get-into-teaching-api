paas_space                = "get-into-teaching-production"
monitor_space             = "get-into-teaching-monitoring"
paas_database_common_name = "get-into-teaching-api-prod-pg-common-svc"
paas_api_application_name = "get-into-teaching-api-prod"
database_plan             = "small-ha-11"
paas_redis_1_name         = "get-into-teaching-prod-redis-svc"
influxdb_1_plan           = "small-1_x"
application_instances     = 2
application_memory        = 2048
monitoring                = 1
environment               = "prod"
azure_key_vault           = "s146p01-kv"
azure_resource_group      = "s146p01-rg"
monitor_scrape_applications = [{ name : "get-into-teaching-api-prod", port : 8080 }, { name : "get-into-teaching-app-prod", port : 3000 }, { name : "get-teacher-training-adviser-service-prod", port : 3000 },
                               { name : "get-into-teaching-api-test", port : 8080 }, { name : "get-into-teaching-app-test", port : 3000 }, { name : "get-teacher-training-adviser-service-test", port : 3000 },
                               { name : "get-into-teaching-app-pagespeed" , port: 3000 }]

alerts = {}
