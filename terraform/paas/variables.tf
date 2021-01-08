# These settings are for the sandbox and should mainly be overriden by TF_VARS 
# or set with environment variables TF_VAR_xxxx

variable user {
  default = "get-into-teaching-tech@digital.education.gov.uk"
}

variable api_url {
  default = "https://api.london.cloud.service.gov.uk"
}

variable password {}


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
  default = "dfe-teacher-services"
}

variable "paas_logging_name" {
  default = "logit-ssl-drain"
}

variable "paas_logging_endpoint_port" {
  default = ""
}

variable "paas_database_1_name" {
  default = "dfe-teacher-services-sb-ms-svc"
}

variable "paas_database_2_name" {
  default = "dfe-teacher-services-sb-pg-svc"
}

variable "paas_redis_1_name" {
  default = "dfe-teacher-services-sb-redis-svc"
}

variable "paas_api_application_name" {
  default = "dfe-teacher-services-api"
}

variable "tta_application_name" {
  default = ""
}

variable "app_application_name" {
  default = ""
}

variable "paas_api_docker_image" {
  default = "dfedigital/get-into-teaching-api:latest"
}

variable "docker_username" {
  default = ""
}

variable "docker_password" {
  default = ""
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

variable "ASPNETCORE_ENVIRONMENT" {
  default = "Staging"
}
variable "CRM_SERVICE_URL" {
  default = ""
}
variable "CRM_CLIENT_ID" {
  default = ""
}
variable "CRM_TENANT_ID" {
  default = ""
}
variable "CRM_CLIENT_SECRET" {
  default = ""
}
variable "SHARED_SECRET" {
  default = ""
}
variable "NOTIFY_API_KEY" {
  default = ""
}
variable "TOTP_SECRET_KEY" {
  default = ""
}
variable "SENTRY_DSN" {
  default = ""
}
variable "GOOGLE_API_KEY" {
  default = ""
}

### Status Cake 
variable "sc_username" {
  default = ""
}
variable "sc_api_key" {
  default = ""
}
variable "alerts" {
  type = map
}

### Monitoring
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

variable "paas_exporter_username" {
  default = ""
}
variable "paas_exporter_password" {
  default = ""
}
variable "grafana_password" {
  default = ""
}
variable "redis_service" {
  default = ""
}

variable "google_client_id" {
  default = ""
}
variable "google_client_secret" {
  default = ""
}
variable elasticsearch_user {
  default = ""
}
variable elasticsearch_password {
  default = ""
}
variable alertmanager_slack_url {
  default = ""
}
variable alertmanager_slack_channel {
  default = "getintoteaching_tech"
}
