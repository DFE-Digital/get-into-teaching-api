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
 
variable "api_application_name" {
    default = "get-into-teaching-api"
}

variable "paas_exporter_username" { }
variable "paas_exporter_password" { }

variable "prometheus" {
    type = map 
    default = {
        "name" = "get-into-teaching"
        "username" = "username"
        "password" = "password"
        "alert_rules" = "../../monitoring/prometheus/alert.rules"
        "config_file" = "../../monitoring/prometheus/prometheus.yml.tmpl"
    }
}

variable "grafana" {
    type = map 
    default = {
        "name" = "get-into-teaching"
        "password" = "a_password"
        "dashboard_directory" = "../../monitoring/grafana/dashboards"
        "datasource_directory" = "../../monitoring/grafana/datasources"
    }
}

variable "alertmanager" {
    type = map 
    default = {
        "name" = "get-into-teaching"
        "config" = "../../monitoring/alertmanager/alertmanager.yml"
    }
}

variable "redis" {
    type = map 
    default = {
        "name" = "get-into-teaching"
        "service" = "get-into-teaching-adviser-dev-red-svc"
    }
}

variable "postgres1" {
    type = map 
    default = {
        "name" = "get-into-teaching-1"
        "service" = "get-into-teaching-api-dev-pg1-svc"
    }
}

variable "postgres2" {
    type = map 
    default = {
        "name" = "get-into-teaching-2"
        "service" = "get-into-teaching-api-dev-pg2-svc"
    }
}

variable "docker" {
    type = map 
    default = {
        "name" = "get-into-teaching"
    }
}

variable elasticsearch_user {}
variable elasticsearch_password {}
