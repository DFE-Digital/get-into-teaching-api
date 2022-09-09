paas_space                = "get-into-teaching"
paas_monitoring_app       = "prometheus-dev-get-into-teaching"
monitor_space             = "get-into-teaching"
paas_database_common_name = "get-into-teaching-api-dev-pg-common-svc"
paas_redis_1_name         = "get-into-teaching-dev-redis-svc"
paas_api_application_name = "get-into-teaching-api-dev"
monitor_scrape_applications = ["get-into-teaching-api-dev-internal.apps.internal:8080",
  "get-into-teaching-app-dev-internal.apps.internal:3000",
  "get-teacher-training-adviser-service-dev-internal.apps.internal:3000",
  "school-experience-app-dev-internal.apps.internal:3000",
  "school-experience-app-dev-delayed.apps.internal:3000",
  "school-experience-app-dev-sidekiq.apps.internal:3000"]
application_instances = 1
logging               = 0
monitoring            = 1
influx                = 1
environment           = "dev"
azure_key_vault       = "s146d01-kv"
azure_resource_group  = "s146d01-rg"
alerts                = {}
