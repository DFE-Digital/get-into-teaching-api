variable user {
  default = "get-into-teaching-tech@digital.education.gov.uk"
}

variable api_url {
  default = "https://api.london.cloud.service.gov.uk"
}

variable password {}

variable "environment" {
  default = "sb"
}

variable "delivery_space" {
  default = "sandbox"
}

variable "application_space" {
  default = "sandbox"
}

variable "paas_org_name" {
  default = "dfe-teacher-services"
}

variable "domain" {
  default = "london.cloudapps.digital"
}


variable "tta_application_name" {}
variable "app_application_name" {}
variable "api_application_name" {}

variable "paas_exporter_username" {}
variable "paas_exporter_password" {}
variable "grafana_password" {}
variable "redis_service" {}

variable "prometheus" {
  type = map
  default = {
    "name"        = "get-into-teaching"
    "username"    = "username"
    "password"    = "password"
    "alert_rules" = "../../monitoring/prometheus/alert.rules"
    "scrape_file" = "../../monitoring/prometheus/scrapes.yml.tmpl"
  }
}

variable "grafana" {
  type = map
  default = {
    "name"                 = "get-into-teaching"
    "dashboard_directory"  = "../../monitoring/grafana/dashboards"
    "datasource_directory" = "../../monitoring/grafana/datasources"
    "configuration_file"   = "../../monitoring/grafana/grafana.ini"
  }
}

variable "alertmanager" {
  type = map
  default = {
    "name"   = "get-into-teaching"
    "config" = "../../monitoring/alertmanager/alertmanager.yml.tmpl"
  }
}

variable "google_client_id" {}
variable "google_client_secret" {}
variable elasticsearch_user {}
variable elasticsearch_password {}
variable alertmanager_slack_url {}
variable alertmanager_slack_channel {
  default = "getintoteaching_tech"
}
