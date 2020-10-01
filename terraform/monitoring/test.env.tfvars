paas_space           = "get-into-teaching-monitoring"
api_application_name = "get-into-teaching-api-test"
environment          = "test"

redis = {
  "name"    = "get-into-teaching"
  "space"   = "get-into-teaching-test"
  "service" = "get-into-teaching-adviser-test-red-svc"
}

postgres1 = {
  "name"    = "get-into-teaching-1"
  "space"   = "get-into-teaching-test"
  "service" = "get-into-teaching-api-test-pg1-svc"
}

postgres2 = {
  "name"    = "get-into-teaching-2"
  "space"   = "get-into-teaching-test"
  "service" = "get-into-teaching-api-test-pg2-svc"
}
