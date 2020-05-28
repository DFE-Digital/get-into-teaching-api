# These settings are for the sandbox and should mainly be overriden by TF_VARS 
# or set with environment variables TF_VAR_xxxx

variable user {
    default = "get-into-teaching-tech@digital.education.gov.uk"
}

variable key {}

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

variable "CRM_CLIENT_ID" {
   default = "c55ecf07-7059-410a-8595-a65ea7948177"
}
variable "CRM_SERVICE_URL" {
   default = "https://gitis-dev.api.crm4.dynamics.com"
}
variable "CRM_TENANT_ID" {
   default = "fad277c9-c60a-4da1-b5f3-b3b8b34a82f9"
}

variable "CRM_CLIENT_SECRET" { }
variable "SHARED_SECRET" { }
