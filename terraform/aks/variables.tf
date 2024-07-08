variable "environment" {}

variable "file_environment" {}

variable "postgres_version" { default = 14 }

variable "app_docker_image" {}

# Key Vault variables
variable "azure_credentials" { default = null }

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

locals {
  azure_credentials       = try(jsondecode(var.azure_credentials), null)
  app_resource_group_name = "${var.azure_resource_prefix}-${var.service_short}-${var.config_short}-rg"
  app_secrets = {
    PG_CONN_STR    = module.postgres.dotnet_connection_string
    REDIS_CONN_STR = module.redis-cache.connection_string
  }
}
