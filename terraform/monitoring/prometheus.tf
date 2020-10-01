locals {
  alert_rules = file("${path.module}/${var.prometheus["alert_rules"]}")
  template_variable_map = {
    api        = local.api_endpoint
    redis      = module.redis.endpoint
    postgres-1 = module.postgres-1.endpoint
    postgres-2 = module.postgres-2.endpoint
  }
}


module "prometheus" {
  // source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/prometheus"
  source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/prometheus?ref=master"

  paas_prometheus_exporter_endpoint = module.paas_prometheus_exporter.endpoint
  space_id                          = data.cloudfoundry_space.space.id
  alertmanager_endpoint             = ""
  additional_variable_map           = local.template_variable_map
  name                              = "${var.environment}-${var.prometheus["name"]}"
  config_file                       = var.prometheus["config_file"]
  alert_rules                       = local.alert_rules
}

module "influx" {
  //     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/influxdb"
  source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/influxdb?ref=master"

  space_id = data.cloudfoundry_space.space.id
  name     = "${var.environment}-${var.prometheus["name"]}"
}

module "paas_prometheus_exporter" {
  //     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/paas_prometheus_exporter"
  source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/paas_prometheus_exporter?ref=master"

  space_id = data.cloudfoundry_space.space.id
  name     = "${var.environment}-${var.prometheus["name"]}"
  username = var.paas_exporter_username
  password = var.paas_exporter_password
}

