locals {
  dashboard_list      = fileset(path.module, "${var.grafana["dashboard_directory"]}/*.json")
  datasource_list     = fileset(path.module, "${var.grafana["datasource_directory"]}/*.yml.tmpl")
  prometheus_file     = "${path.module}/${var.prometheus["config_file"]}"
  configuration_file  = "${path.module}/${var.grafana["configuration_file"]}"
  alert_rules         = file("${path.module}/${var.prometheus["alert_rules"]}")
  monitoring_org_name = "${var.environment}-${var.prometheus["name"]}"
  template_variable_map = {
    api                               = local.api_endpoint
    git_app                           = "${data.cloudfoundry_route.app_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    git_api                           = "${data.cloudfoundry_route.api_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    git_tta                           = "${data.cloudfoundry_route.tta_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    elastic_user                      = var.elasticsearch_user
    elastic_pass                      = var.elasticsearch_password
    API_ENDPOINT                      = var.api_url
    slack_channel                     = var.alertmanager_slack_channel
    slack_url                         = var.alertmanager_slack_url
    paas_prometheus_exporter_endpoint = module.paas_prometheus_exporter.endpoint
    alertmanager_endpoint             = module.alertmanager.endpoint
  }
}

module "prometheus" {
  source                            = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/prometheus"
  paas_prometheus_exporter_endpoint = module.paas_prometheus_exporter.endpoint
  monitoring_space_id               = data.cloudfoundry_space.space.id
  monitoring_instance_name          = local.monitoring_org_name
  alertmanager_endpoint             = module.alertmanager.endpoint
  config_file                       = templatefile(local.prometheus_file, local.template_variable_map)
  alert_rules                       = local.alert_rules
  influxdb_service_instance_id      = module.influx.service_instance_id
  memory                            = 5120
  disk_quota                        = 5120
}

module "influx" {
  source                   = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/influxdb"
  monitoring_space_id      = data.cloudfoundry_space.space.id
  monitoring_instance_name = local.monitoring_org_name
}

module "paas_prometheus_exporter" {
  source                   = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/paas_prometheus_exporter"
  monitoring_space_id      = data.cloudfoundry_space.space.id
  monitoring_instance_name = local.monitoring_org_name
  paas_username            = var.paas_exporter_username
  paas_password            = var.paas_exporter_password
}


module "grafana" {
  source                   = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/grafana"
  monitoring_space_id      = data.cloudfoundry_space.space.id
  monitoring_instance_name = "${var.environment}-${var.grafana["name"]}"
  prometheus_endpoint      = module.prometheus.endpoint
  runtime_version          = "7.2.2"
  google_client_id         = var.google_client_id
  google_client_secret     = var.google_client_secret
  admin_password           = var.grafana_password

  json_dashboards   = [for f in local.dashboard_list : file(f)]
  extra_datasources = [for f in local.datasource_list : templatefile(f, local.template_variable_map)]

}

module "alertmanager" {
  source                   = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/alertmanager"
  monitoring_space_id      = data.cloudfoundry_space.space.id
  monitoring_instance_name = "${var.environment}-${var.alertmanager["name"]}"
  slack_url                =  var.alertmanager_slack_url
  slack_channel            =  var.alertmanager_slack_channel
}
