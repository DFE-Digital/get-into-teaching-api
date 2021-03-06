# These settings are for the sandbox and should mainly be overriden by TF_VARS 
# or set with environment variables TF_VAR_xxxx

variable "api_url" {
  default = "https://api.london.cloud.service.gov.uk"
}

variable "AZURE_CREDENTIALS" {}
variable "azure_key_vault" {}
variable "azure_resource_group" {}

variable "logging" {
  default = 1
}

variable "strategy" {
  default = "blue-green-v2"
}

variable "application_instances" {
  default = 1
}

variable "application_stopped" {
  default = false
}

variable "application_memory" {
  default = "1024"
}

variable "application_disk" {
  default = "1024"
}

variable "paas_space" {
  default = "sandbox"
}

variable "environment" {
  default = "sb"
}

variable "paas_org_name" {
  default = "dfe"
}

variable "paas_logging_name" {
  default = "logit-ssl-drain"
}

variable "paas_database_common_name" {
  default = "dfe-teacher-services-sb-common-pg-svc"
}

variable "database_plan" {
  default = "small-11"
}

variable "paas_redis_1_name" {
  default = "dfe-teacher-services-sb-redis-svc"
}

variable "redis_1_plan" {
  default = "small-ha-5_x"
}

variable "influxdb_1_plan" {
  default = "tiny-1_x"
}

variable "paas_api_application_name" {
  default = "dfe-teacher-services-api"
}

variable "paas_api_docker_image" {
  default = "dfedigital/get-into-teaching-api:latest"
}

variable "monitoring" {
  default = 0
}

variable "influx" {
  default = 0
}

variable "monitor_space" {
  default = "get-into-teaching"
}

variable "alerts" {
  type = map(any)
}

### Monitoring
variable "monitor_scrape_applications" {
  default = []
}

variable "prometheus" {
  type = map(any)
  default = {
    "name"        = "get-into-teaching"
    "username"    = "username"
    "password"    = "password"
    "alert_rules" = "../../monitoring/prometheus/alert.rules"
    "scrape_file" = "../../monitoring/prometheus/scrapes.yml.tmpl"
  }
}

variable "grafana" {
  type = map(any)
  default = {
    "name"                 = "get-into-teaching"
    "dashboard_directory"  = "../../monitoring/grafana/dashboards"
    "datasource_directory" = "../../monitoring/grafana/datasources"
    "configuration_file"   = "../../monitoring/grafana/grafana.ini"
  }
}

variable "alertmanager" {
  type = map(any)
  default = {
    "name"   = "get-into-teaching"
    "config" = "../../monitoring/alertmanager/alertmanager.yml.tmpl"
  }
}
