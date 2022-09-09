paas_space                = "get-into-teaching-production"
paas_monitoring_app       = "prometheus-prod-get-into-teaching"
monitor_space             = "get-into-teaching-monitoring"
paas_database_common_name = "get-into-teaching-api-prod-pg-common-svc"
paas_api_application_name = "get-into-teaching-api-prod"
database_plan             = "small-ha-11"
paas_redis_1_name         = "get-into-teaching-prod-redis-svc"
influxdb_1_plan           = "medium-1_x"
application_instances     = 2
application_memory        = 2048
monitoring                = 1
environment               = "prod"
azure_key_vault           = "s146p01-kv"
azure_resource_group      = "s146p01-rg"
monitor_scrape_applications = ["get-into-teaching-api-prod-internal.apps.internal:8080",
  "get-into-teaching-app-prod-internal.apps.internal:3000",
  "get-teacher-training-adviser-service-prod-internal.apps.internal:3000",
  "get-into-teaching-api-test-internal.apps.internal:8080",
  "get-into-teaching-app-test-internal.apps.internal:3000",
  "get-teacher-training-adviser-service-test-internal.apps.internal:3000",
  "get-into-teaching-app-pagespeed-internal.apps.internal:3000",
  "school-experience-app-production-internal.apps.internal:3000",
  "school-experience-app-production-delayed.apps.internal:3000",
  "school-experience-app-staging-internal.apps.internal:3000",
  "school-experience-app-staging-delayed.apps.internal:3000",
  "school-experience-app-staging-sidekiq.apps.internal:3000"
]
alerts = {}
