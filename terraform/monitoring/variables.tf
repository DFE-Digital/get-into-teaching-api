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

variable "paas_space" {
  default = "sandbox"
}

variable "paas_org_name" {
  default = "dfe-teacher-services"
}

variable "domain" {
  default = "london.cloudapps.digital"
}

variable "api_application_name" {}
variable "app_application_name" {}
variable "tta_application_name" {}

variable "paas_exporter_username" {}
variable "paas_exporter_password" {}

variable "google_client_id" {}
variable "google_client_secret" {}


variable "prometheus" {
  type = map
  default = {
    "name"        = "get-into-teaching"
    "username"    = "username"
    "password"    = "password"
    "alert_rules" = "../../monitoring/prometheus/alert.rules"
    "config_file" = "../../monitoring/prometheus/prometheus.yml.tmpl"
  }
}

variable "grafana" {
  type = map
  default = {
    "name"                 = "get-into-teaching"
    "password"             = "a_password"
    "dashboard_directory"  = "../../monitoring/grafana/dashboards"
    "datasource_directory" = "../../monitoring/grafana/datasources"
    "configuration_file"   = "../../monitoring/grafana/grafana.ini"
  }
}

variable "alertmanager" {
  type = map
  default = {
    "name"   = "get-into-teaching"
    "config" = "../../monitoring/alertmanager/alertmanager.yml"
  }
}

variable "redis" {
  type = map
  default = {
    "name"    = "get-into-teaching"
    "space"   = "get-into-teaching"
    "service" = "get-into-teaching-dev-redis-svc"
  }
}

variable "postgres1" {
  type = map
  default = {
    "name"    = "get-into-teaching-1"
    "space"   = "get-into-teaching"
    "service" = "get-into-teaching-api-dev-ms1-svc"
  }
}

variable "postgres2" {
  type = map
  default = {
    "name"    = "get-into-teaching-2"
    "space"   = "get-into-teaching"
    "service" = "get-into-teaching-api-dev-pg2-svc"
  }
}


variable elasticsearch_user {}
variable elasticsearch_password {}
