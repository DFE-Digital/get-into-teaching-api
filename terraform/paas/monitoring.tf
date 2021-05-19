locals {
  monitoring_org_name = "${var.environment}-${var.prometheus["name"]}"
  alert_rules         = file("${path.module}/${var.prometheus["alert_rules"]}")

  dashboard_list     = fileset(path.module, "${var.grafana["dashboard_directory"]}/*.json")
  datasource_list    = fileset(path.module, "${var.grafana["datasource_directory"]}/*.yml.tmpl")
  configuration_file = "${path.module}/${var.grafana["configuration_file"]}"

  elasticsearch_credentials = {
    url      = lookup( local.monitoring_secrets,"LOGIT_URL" , "" )
    username = lookup( local.monitoring_secrets,"LOGIT_USERID" , "" )
    password = lookup( local.monitoring_secrets,"LOGIT_PASSWORD" , "" )
  }

  template_variable_map = {
    api           = local.api_endpoint
    applications  = var.monitor_scrape_applications
    API_ENDPOINT  = var.api_url
  }
}

module "prometheus" {
  count  = var.monitoring
  source                             = "git::https://github.com/DFE-Digital/cf-monitoring.git//prometheus_all"
  monitoring_instance_name          = local.monitoring_org_name
  monitoring_org_name               = var.paas_org_name
  monitoring_space_name             = var.monitor_space
  paas_exporter_username            = data.azurerm_key_vault_secret.paas_username.value
  paas_exporter_password            = data.azurerm_key_vault_secret.paas_password.value
  alertmanager_slack_url            = lookup( local.monitoring_secrets , "SLACK_ALERTMANAGER_HOOK" , "" )
  alertmanager_slack_channel        = lookup( local.monitoring_secrets , "SLACK_CHANNEL" , "" )
  alert_rules                       = local.alert_rules
  grafana_elasticsearch_credentials = local.elasticsearch_credentials
  grafana_google_client_id          = lookup( local.monitoring_secrets , "GOOGLE_CLIENT_ID" , "" )
  grafana_google_client_secret      = lookup( local.monitoring_secrets , "GOOGLE_CLIENT_SECRET" , "" )
  grafana_admin_password            = lookup( local.monitoring_secrets , "GRAFANA_ADMIN_PASSWORD" , "" )
  grafana_json_dashboards           = [for f in local.dashboard_list : file(f)]
  grafana_extra_datasources         = [for f in local.datasource_list : templatefile(f, local.template_variable_map)]
  grafana_google_jwt                = lookup( local.monitoring_secrets , "GOOGLE_JWT" , "" )
  grafana_runtime_version           = "7.2.2"
  prometheus_memory                 = 5120
  prometheus_disk_quota             = 5120
  prometheus_extra_scrape_config    = templatefile("${path.module}/${var.prometheus["scrape_file"]}", local.template_variable_map)
  influxdb_service_plan             = var.influxdb_1_plan
  redis_services                    = ["${var.paas_space}/${var.paas_redis_1_name}"]
  postgres_services                 = ["${var.paas_space}/${var.paas_database_common_name}"]
}

