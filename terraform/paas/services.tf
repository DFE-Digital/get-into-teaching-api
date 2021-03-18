data cloudfoundry_service postgres {
  name = "postgres"
}

data cloudfoundry_service redis {
  name = "redis"
}

data cloudfoundry_service mysql {
  name = "mysql"
}

data cloudfoundry_service influxdb {
  name = "influxdb"
}

locals {
  logstash_endpoint = local.infrastructure_secrets["LOGSTASH_ENDPOINT"]
}

resource cloudfoundry_service_instance postgres_common {
  name         = var.paas_database_common_name
  space        = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.postgres.service_plans[var.database_plan]
  json_params  = "{\"enable_extensions\": [\"postgis\"] }"
}

resource cloudfoundry_user_provided_service logging {
  count            = var.logging
  name             = var.paas_logging_name
  space            = data.cloudfoundry_space.space.id
  syslog_drain_url = "syslog-tls://${local.logstash_endpoint}"
}

resource cloudfoundry_service_instance redis {
  name         = var.paas_redis_1_name
  space        = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.redis.service_plans[var.redis_1_plan]
  json_params  = "{\"maxmemory_policy\": \"allkeys-lfu\" }"
}

resource cloudfoundry_service_instance influxdb {
  count        = var.influx
  name         = "influxdb-${local.monitoring_org_name}"
  space        = data.cloudfoundry_space.monitor.id
  service_plan = data.cloudfoundry_service.influxdb.service_plans[var.influxdb_1_plan]
}

resource cloudfoundry_service_key influxdb-key {
  count            = var.influx
  name             = "influxdb-prometheus-key"
  service_instance = cloudfoundry_service_instance.influxdb[0].id
}

