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
   default =  "1024"
}

variable "application_disk" {
   default =  "1024"
}

variable "paas_space" {
   default = "sandbox"
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

variable "paas_postgres_1_name" {
   default = "dfe-teacher-services-sb-pg-svc"
}

variable "paas_postgres_2_name" {
   default = "dfe-teacher-services-sb-pg-svc"
}

variable "paas_redis_1_name" {
   default = "dfe-teacher-services-sb-redis-svc"
}

variable "paas_api_application_name" {
   default = "dfe-teacher-services-api"
}

variable "paas_api_docker_image" {
   default = "dfedigital/get-into-teaching-api:latest"
}

variable "paas_api_route_name" {
   default = "dfe-teacher-services-sb-api"
}

variable "ASPNETCORE_ENVIRONMENT" {
   default = "Staging"
}
variable "CRM_SERVICE_URL"  {}
variable "CRM_CLIENT_ID" {}
variable "CRM_TENANT_ID" {} 
variable "CRM_CLIENT_SECRET" {}
variable "SHARED_SECRET" {}
variable "PEN_TEST_SHARED_SECRET" {
  default = ""
}
variable "NOTIFY_API_KEY" {}
variable "TOTP_SECRET_KEY" {}
variable "SENTRY_URL" {
  default = ""
}
variable "GOOGLE_API_KEY" {
  default = ""
}

