data cloudfoundry_service postgres {
  name = "postgres"
}

data cloudfoundry_service redis {
  name = "redis"
}

data cloudfoundry_service mysql {
  name = "mysql"
}


resource "cloudfoundry_service_instance" "hangfire" {
  name         = var.paas_database_1_name
  space        = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.postgres.service_plans["small-11"]
}

resource "cloudfoundry_service_instance" "postgres2" {
  name         = var.paas_database_2_name
  space        = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.postgres.service_plans["small-11"]
  json_params  = "{\"enable_extensions\": [\"postgis\"] }"
}

resource "cloudfoundry_user_provided_service" "logging" {
  count            = var.logging
  name             = var.paas_logging_name
  space            = data.cloudfoundry_space.space.id
  syslog_drain_url = var.paas_logging_endpoint_port
}

resource "cloudfoundry_service_instance" "redis" {
  name         = var.paas_redis_1_name
  space        = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.redis.service_plans["small-ha-5_x"]
  json_params  = "{\"maxmemory_policy\": \"allkeys-lfu\" }"
}

