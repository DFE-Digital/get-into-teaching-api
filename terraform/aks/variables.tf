variable "environment" {}

variable "file_environment" {}

variable "postgres_version" { default = 14 }

variable "app_docker_image" {}

variable "rg_name" {
  description = "Resource group name"
}

variable "deploy_azure_backing_services" {
  description = "Enable deployment of Azure backing services"
  type        = bool
  default     = true
}

# Key Vault variables

variable "app_key_vault_name" {}

variable "infra_key_vault_name" {}

variable "gov_uk_host_names" {
  default = []
  type    = list(any)
}

# Kubernetes variables
variable "namespace" {}

variable "cluster" {}

variable "enable_monitoring" { default = true }

variable "enable_statuscake_alerts" { default = false }

variable "azure_resource_prefix" {}

variable "config_short" {}
variable "service_short" {}

variable "service_name" {}

variable "azure_maintenance_window" { default = null }
variable "postgres_flexible_server_sku" { default = "B_Standard_B1ms" }
variable "postgres_enable_high_availability" { default = false }
variable "azure_enable_backup_storage" { default = true }

variable "replicas" { default = 1 }
variable "memory_max" { default = "1Gi" }

variable "redis_capacity" {
  type    = number
  default = 1
}
variable "redis_family" {
  type    = string
  default = "C"
}
variable "redis_sku_name" {
  type    = string
  default = "Standard"
}
variable "enable_logit" {
  type    = bool
  default = false
}

variable "enable_prometheus_monitoring" {
  type    = bool
  default = false
}

variable "pr_number" {
  description = "PR number for review app"
  default     = ""
}
