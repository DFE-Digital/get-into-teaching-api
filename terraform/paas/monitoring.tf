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
    elastic_user                      = local.monitoring_secrets["LOGIT_USERID"]
    elastic_pass                      = local.monitoring_secrets["LOGIT_PASSWORD"]
    API_ENDPOINT                      = var.api_url
    slack_channel                     = local.monitoring_secrets["SLACK_CHANNEL"]
    slack_url                         = local.monitoring_secrets["SLACK_ALERTMANAGER_HOOK"]
  }
}

module "prometheus" {
   count                              = var.monitoring
   source                             = "git::https://github.com/DFE-Digital/cf-monitoring.git//prometheus_all?ref=add_redis_exporter_to_all"
   monitoring_instance_name           = local.monitoring_org_name
   monitoring_org_name                = var.paas_org_name
   monitoring_space_name              = var.monitor_space
   paas_exporter_username             = data.azurerm_key_vault_secret.paas_username.value
   paas_exporter_password             = data.azurerm_key_vault_secret.paas_password.value
   alertmanager_slack_url             = local.monitoring_secrets["SLACK_ALERTMANAGER_HOOK"]
   alertmanager_slack_channel         = local.monitoring_secrets["SLACK_CHANNEL"]
   alert_rules                        = local.alert_rules
   grafana_google_client_id           = local.monitoring_secrets["GOOGLE_CLIENT_ID"]
   grafana_google_client_secret       = local.monitoring_secrets["GOOGLE_CLIENT_SECRET"]
   grafana_admin_password             = local.monitoring_secrets["GRAFANA_ADMIN_PASSWORD"]
   grafana_json_dashboards            = [for f in local.dashboard_list : file(f)]
   grafana_extra_datasources          = [for f in local.datasource_list : templatefile(f, local.template_variable_map)]
   grafana_google_jwt                 = local.monitoring_secrets["GOOGLE_JWT"]
   grafana_runtime_version            = "7.2.2"
   prometheus_memory                  = 5120
   prometheus_disk_quota              = 5120
   prometheus_extra_scrape_config     = templatefile("${path.module}/${var.prometheus["scrape_file"]}", local.template_variable_map)
   influxdb_service_plan              = var.influxdb_1_plan
   redis_service_instance_id          = cloudfoundry_service_instance.redis.id

   depends_on  = [ cloudfoundry_service_instance.redis ]
}
