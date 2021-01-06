paas_space                = "get-into-teaching-test"
monitor_space             = "get-into-teaching-monitoring"
prometheus                = "prometheus-prod-get-into-teaching"
paas_database_1_name      = "get-into-teaching-api-test-pg1-svc"
paas_database_2_name      = "get-into-teaching-api-test-pg2-svc"
paas_redis_1_name         = "get-into-teaching-test-redis-svc"
paas_api_application_name = "get-into-teaching-api-test"
paas_api_route_name       = "get-into-teaching-api-test"
ASPNETCORE_ENVIRONMENT    = "Staging"
application_instances     = 2
application_memory        = 2048


alerts = {
  GIT_API_TEST_HEALTHCHECK = {
    website_name  = "Get Teacher Training API (Test Space)"
    website_url   = "https://get-into-teaching-api-test.london.cloudapps.digital/api/operations/health_check"
    test_type     = "HTTP"
    check_rate    = 60
    contact_group = [185037]
    trigger_rate  = 0
    timeout       = 80
    custom_header = "{\"Content-Type\": \"application/x-www-form-urlencoded\"}"
  }
}
