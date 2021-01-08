locals {
  monitoring_org_name = "${var.environment}-${var.prometheus["name"]}"
  alert_rules         = file("${path.module}/${var.prometheus["alert_rules"]}")

  dashboard_list     = fileset(path.module, "${var.grafana["dashboard_directory"]}/*.json")
  datasource_list    = fileset(path.module, "${var.grafana["datasource_directory"]}/*.yml.tmpl")
  configuration_file = "${path.module}/${var.grafana["configuration_file"]}"

  template_variable_map = {
    api                               = local.api_endpoint
    git_api                           = "${cloudfoundry_route.api_route_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    git_tta                           = "${var.tta_application_name}-internal.${data.cloudfoundry_domain.internal.name}"
    git_app                           = "${var.app_application_name}-internal.${data.cloudfoundry_domain.internal.name}"
    elastic_user                      = var.elasticsearch_user
    elastic_pass                      = var.elasticsearch_password
    API_ENDPOINT                      = var.api_url
    slack_channel                     = var.alertmanager_slack_channel
    slack_url                         = var.alertmanager_slack_url
    paas_prometheus_exporter_endpoint = var.monitoring == 1 ? module.paas_prometheus_exporter[0].endpoint : ""
    alertmanager_endpoint             = var.monitoring == 1 ? module.alertmanager[0].endpoint : ""
  }
}

data cloudfoundry_service influxdb {
  name = "influxdb"
}

resource cloudfoundry_service_instance influxdb {
  count        = var.influx
  name         = "influxdb-${local.monitoring_org_name}"
  space        = data.cloudfoundry_space.monitor.id
  service_plan = data.cloudfoundry_service.influxdb.service_plans["tiny-1_x"]
}

resource cloudfoundry_service_key influxdb-key {
  count            = var.influx
  name             = "influxdb-prometheus-key"
  service_instance = cloudfoundry_service_instance.influxdb[0].id
}

module "prometheus" {
  count                              = var.monitoring
  source                             = "git::https://github.com/DFE-Digital/cf-monitoring.git//prometheus"
  paas_prometheus_exporter_endpoint  = module.paas_prometheus_exporter[0].endpoint
  redis_prometheus_exporter_endpoint = module.redis_prometheus_exporter[0].endpoint
  alertmanager_endpoint              = module.alertmanager[0].endpoint
  monitoring_space_id                = data.cloudfoundry_space.monitor.id
  monitoring_instance_name           = local.monitoring_org_name
  extra_scrape_config                = templatefile("${path.module}/${var.prometheus["scrape_file"]}", local.template_variable_map)
  alert_rules                        = local.alert_rules
  influxdb_service_instance_id       = cloudfoundry_service_instance.influxdb[0].id
  memory                             = 5120
  disk_quota                         = 5120
}

module "paas_prometheus_exporter" {
  count                    = var.monitoring
  source                   = "git::https://github.com/DFE-Digital/cf-monitoring.git//paas_prometheus_exporter"
  monitoring_space_id      = data.cloudfoundry_space.monitor.id
  monitoring_instance_name = local.monitoring_org_name
  paas_username            = var.paas_exporter_username
  paas_password            = var.paas_exporter_password
}

module "redis_prometheus_exporter" {
  count                     = var.monitoring
  source                    = "git::https://github.com/DFE-Digital/cf-monitoring.git//redis_prometheus_exporter"
  monitoring_space_id       = data.cloudfoundry_space.monitor.id
  monitoring_instance_name  = local.monitoring_org_name
  redis_service_instance_id = cloudfoundry_service_instance.redis.id
}

module "grafana" {
  count                    = var.monitoring
  source                   = "git::https://github.com/DFE-Digital/cf-monitoring.git//grafana"
  monitoring_space_id      = data.cloudfoundry_space.monitor.id
  monitoring_instance_name = "${var.environment}-${var.grafana["name"]}"
  prometheus_endpoint      = module.prometheus[0].endpoint
  runtime_version          = "7.2.2"
  google_client_id         = var.google_client_id
  google_client_secret     = var.google_client_secret
  admin_password           = var.grafana_password
  influxdb_credentials     = cloudfoundry_service_key.influxdb-key[0].credentials

  json_dashboards   = [for f in local.dashboard_list : file(f)]
  extra_datasources = [for f in local.datasource_list : templatefile(f, local.template_variable_map)]

}

module "alertmanager" {
  count                    = var.monitoring
  source                   = "git::https://github.com/DFE-Digital/cf-monitoring.git//alertmanager"
  monitoring_space_id      = data.cloudfoundry_space.monitor.id
  monitoring_instance_name = "${var.environment}-${var.alertmanager["name"]}"
  slack_url                = var.alertmanager_slack_url
  slack_channel            = var.alertmanager_slack_channel
}
