locals {
  dashboard_directory  = "${path.module}/${var.grafana["dashboard_directory"]}"
  datasource_directory = "${path.module}/${var.grafana["datasource_directory"]}"
  configuration_file   = "${path.module}/${var.grafana["configuration_file"]}"
  alert_rules          = file("${path.module}/${var.prometheus["alert_rules"]}")
  monitoring_org_name  = "${var.environment}-${var.prometheus["name"]}"
  template_variable_map = {
    api                  = local.api_endpoint
    git_app              = "${data.cloudfoundry_route.app_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    git_api              = "${data.cloudfoundry_route.api_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    git_tta              = "${data.cloudfoundry_route.tta_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    elastic_user         = var.elasticsearch_user
    elastic_pass         = var.elasticsearch_password
    google_client_id     = var.google_client_id
    google_client_secret = var.google_client_secret
    USERNAME             = var.paas_exporter_username
    PASSWORD             = var.paas_exporter_password
    API_ENDPOINT         = var.api_url
  }
}

module "prometheus" {
  source                            = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/prometheus?ref=monitoring-terraform-0_13"
  paas_prometheus_exporter_endpoint = module.paas_prometheus_exporter.endpoint
  monitoring_space_id               = data.cloudfoundry_space.space.id
  monitoring_org_name               = local.monitoring_org_name
  alertmanager_endpoint             = module.alertmanager.endpoint
  additional_variable_map           = local.template_variable_map
  config_file                       = var.prometheus["config_file"]
  alert_rules                       = local.alert_rules
  influxdb_service_instance_id      = module.influx.service_instance_id
  memory                            = 5120
}

module "influx" {
  source              = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/influxdb?ref=monitoring-terraform-0_13"
  monitoring_space_id = data.cloudfoundry_space.space.id
  monitoring_org_name = local.monitoring_org_name
}

module "paas_prometheus_exporter" {
  source                   = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/paas_prometheus_exporter?ref=monitoring-terraform-0_13"
  monitoring_space_id      = data.cloudfoundry_space.space.id
  monitoring_org_name      = local.monitoring_org_name
  environment_variable_map = local.template_variable_map
}


module "grafana" {
  source                  = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/grafana?ref=monitoring-terraform-0_13"
  monitoring_space_id     = data.cloudfoundry_space.space.id
  monitoring_org_name     = "${var.environment}-${var.grafana["name"]}"
  graphana_admin_password = var.grafana_password
  dashboard_directory     = local.dashboard_directory
  datasource_directory    = local.datasource_directory
  configuration_file      = local.configuration_file
  prometheus_endpoint     = module.prometheus.endpoint
  additional_variable_map = local.template_variable_map
  runtime_version         = "7.2.2"
}

module "alertmanager" {
   source              = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/alertmanager?ref=monitoring-terraform-0_13"
   monitoring_space_id = data.cloudfoundry_space.space.id
   monitoring_org_name = "${var.environment}-${var.alertmanager["name"]}"
   config              = file( var.alertmanager["config"] )
}
