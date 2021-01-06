locals {
  monitoring_org_name = "${var.environment}-${var.prometheus["name"]}"
}

data cloudfoundry_service influxdb {
  name = "influxdb"
}

resource cloudfoundry_service_instance influxdb {
  count        = var.monitoring
  name         = "influxdb-${local.monitoring_org_name}"
  space        = data.cloudfoundry_space.monitor.id
  service_plan = data.cloudfoundry_service.influxdb.service_plans["tiny-1_x"]
}

resource cloudfoundry_service_key influxdb-key {
  count            = var.monitoring
  name             = "prometheus-key"
  service_instance = cloudfoundry_service_instance.influxdb[0].id
}


