# These settings are for the sandbox and should mainly be overriden by TF_VARS 
# or set with environment variables TF_VAR_xxxx

variable user {
    default = "get-into-teaching-tech@digital.education.gov.uk"
}

variable api_url {
     default = "https://api.london.cloud.service.gov.uk"
}

variable password {}


variable "paas_space" {
   default = "sandbox"
}

variable "paas_org_name" {
   default = "dfe-teacher-services"
}

variable "paas_postgres_name" {
   default = "dfe-teacher-services-sb-pg-svc"
}

variable "paas_api_application_name" {
   default = "dfe-teacher-services-api"
}

variable "paas_api_docker_image" {
   default = "dfedigital/get-into-teaching-api:GITPB-149"
}

variable "paas_api_route_name" {
   default = "dfe-teacher-services-sb-api"
}

# Environment variables

variable "CRM_SERVICE_URL" {
   default = "https://gitis-dev.api.crm4.dynamics.com"
}
variable "CRM_CLIENT_ID" {}
variable "CRM_TENANT_ID" {} 
variable "CRM_CLIENT_SECRET" {}
variable "SHARED_SECRET" {}
