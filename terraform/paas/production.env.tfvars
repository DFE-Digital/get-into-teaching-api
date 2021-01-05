paas_space                = "get-into-teaching-production"
monitor_space             = "get-into-teaching-monitoring"
prometheus                = "prometheus-prod-get-into-teaching"
paas_database_1_name      = "get-into-teaching-api-prod-ms1-svc"
paas_database_2_name      = "get-into-teaching-api-prod-pg2-svc"
paas_redis_1_name         = "get-into-teaching-prod-redis-svc"
paas_api_application_name = "get-into-teaching-api-prod"
paas_api_route_name       = "get-into-teaching-api-prod"
ASPNETCORE_ENVIRONMENT    = "Production"
application_instances     = 2
application_memory        = 2048


alerts = {
  GIT_API_PROD_HEALTHCHECK = {
    website_name  = "Get Teacher Training API (Production Space)"
    website_url   = "https://get-into-teaching-api-prod.london.cloudapps.digital/api/operations/health_check"
    test_type     = "HTTP"
    check_rate    = 60
    contact_group = [185037]
    trigger_rate  = 0
    timeout       = 80
    custom_header = "{\"Content-Type\": \"application/x-www-form-urlencoded\"}"
  }
}
