locals {
  monitoring_org_name = "${var.environment}-${var.prometheus["name"]}"
  alert_rules         = file("${path.module}/${var.prometheus["alert_rules"]}")

  dashboard_list     = fileset(path.module, "${var.grafana["dashboard_directory"]}/*.json")
  datasource_list    = fileset(path.module, "${var.grafana["datasource_directory"]}/*.yml.tmpl")
  configuration_file = "${path.module}/${var.grafana["configuration_file"]}"

  elasticsearch_credentials = {
    url      = local.monitoring_secrets["LOGIT_URL"]
    username = local.monitoring_secrets["LOGIT_USERID"]
    password = local.monitoring_secrets["LOGIT_PASSWORD"]
  }

  template_variable_map = {
    api                               = local.api_endpoint
    git_api                           = "${cloudfoundry_route.api_route_internal.hostname}.${data.cloudfoundry_domain.internal.name}"
    git_tta                           = "${var.tta_application_name}-internal.${data.cloudfoundry_domain.internal.name}"
    git_app                           = "${var.app_application_name}-internal.${data.cloudfoundry_domain.internal.name}"
    API_ENDPOINT                      = var.api_url
    slack_channel                     = local.monitoring_secrets["SLACK_CHANNEL"]
    slack_url                         = local.monitoring_secrets["SLACK_ALERTMANAGER_HOOK"]
    paas_prometheus_exporter_endpoint = var.monitoring == 1 ? module.paas_prometheus_exporter[0].endpoint : ""
    alertmanager_endpoint             = var.monitoring == 1 ? module.alertmanager[0].endpoint : ""
  }
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
  influxdb_service_instance_id       = var.influx == 1 ? cloudfoundry_service_instance.influxdb[0].id : ""
  memory                             = 5120
  disk_quota                         = 5120
}

module "paas_prometheus_exporter" {
  count                    = var.monitoring
  source                   = "git::https://github.com/DFE-Digital/cf-monitoring.git//paas_prometheus_exporter"
  monitoring_space_id      = data.cloudfoundry_space.monitor.id
  monitoring_instance_name = local.monitoring_org_name
  paas_username            = data.azurerm_key_vault_secret.paas_username.value
  paas_password            = data.azurerm_key_vault_secret.paas_password.value
}

module "redis_prometheus_exporter" {
  count                     = var.monitoring
  source                    = "git::https://github.com/DFE-Digital/cf-monitoring.git//redis_prometheus_exporter"
  monitoring_space_id       = data.cloudfoundry_space.monitor.id
  monitoring_instance_name  = local.monitoring_org_name
  redis_service_instance_id = cloudfoundry_service_instance.redis.id
}

module "grafana" {
  count                     = var.monitoring
  source                    = "git::https://github.com/DFE-Digital/cf-monitoring.git//grafana"
  monitoring_space_id       = data.cloudfoundry_space.monitor.id
  monitoring_instance_name  = "${var.environment}-${var.grafana["name"]}"
  prometheus_endpoint       = module.prometheus[0].endpoint
  runtime_version           = "7.2.2"
  google_client_id          = local.monitoring_secrets["GOOGLE_CLIENT_ID"]
  google_client_secret      = local.monitoring_secrets["GOOGLE_CLIENT_SECRET"]
  admin_password            = local.monitoring_secrets["GRAFANA_ADMIN_PASSWORD"]
  google_jwt                = local.monitoring_secrets["GOOGLE_JWT"]
  influxdb_credentials      = cloudfoundry_service_key.influxdb-key[0].credentials
  elasticsearch_credentials = local.elasticsearch_credentials

  json_dashboards   = [for f in local.dashboard_list : file(f)]
  extra_datasources = [for f in local.datasource_list : templatefile(f, local.template_variable_map)]

}

module "alertmanager" {
  count                    = var.monitoring
  source                   = "git::https://github.com/DFE-Digital/cf-monitoring.git//alertmanager"
  monitoring_space_id      = data.cloudfoundry_space.monitor.id
  monitoring_instance_name = "${var.environment}-${var.alertmanager["name"]}"
  slack_url                = local.monitoring_secrets["SLACK_ALERTMANAGER_HOOK"]
  slack_channel            = local.monitoring_secrets["SLACK_CHANNEL"]
}
