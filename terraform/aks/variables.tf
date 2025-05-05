variable "environment" {
  description = "Environment name"
}

variable "file_environment" {
  description = "Environment name used in configuration files"
}

variable "pr_number" {
  type        = string
  default     = ""
  description = "PR number for review apps"
}

variable "rg_name" {
  description = "Resource group name"
}

variable "postgres_version" {
  description = "PostgreSQL version"
  default     = 14
}

variable "app_docker_image" {
  description = "Docker image URL to be deployed"
}

# Key Vault variables
variable "app_key_vault_name" {
  description = "Application key vault name"
}

variable "infra_key_vault_name" {
  description = "Infrastructure key vault name"
}

variable "gov_uk_host_names" {
  description = "List of gov.uk hostnames"
  default     = []
  type        = list(any)
}

# Kubernetes variables
variable "namespace" {
  description = "Kubernetes namespace"
}

variable "cluster" {
  description = "Kubernetes cluster name"
}

variable "enable_monitoring" {
  description = "Enable Azure monitoring"
  default     = true
}

variable "enable_statuscake_alerts" {
  description = "Enable StatusCake alerts"
  default     = false
}

variable "azure_resource_prefix" {
  description = "Azure resource prefix"
}

variable "config_short" {
  description = "Config short name (e.g. 'dv', 'ts', 'pd')"
}

variable "service_short" {
  description = "Service short name"
}

variable "service_name" {
  description = "Service full name"
}

variable "azure_maintenance_window" {
  description = "Azure maintenance window"
  default     = null
}

variable "postgres_flexible_server_sku" {
  description = "PostgreSQL flexible server SKU"
  default     = "B_Standard_B1ms"
}

variable "postgres_enable_high_availability" {
  description = "Enable PostgreSQL high availability"
  default     = false
}

variable "azure_enable_backup_storage" {
  description = "Enable Azure backup storage"
  default     = true
}

variable "replicas" {
  description = "Number of application replicas"
  default     = 1
}

variable "memory_max" {
  description = "Maximum memory allocation"
  default     = "1Gi"
}

variable "redis_capacity" {
  description = "Redis capacity"
  type        = number
  default     = 1
}

variable "redis_family" {
  description = "Redis family"
  type        = string
  default     = "C"
}

variable "redis_sku_name" {
  description = "Redis SKU name"
  type        = string
  default     = "Standard"
}

variable "enable_logit" {
  description = "Enable Logit logging"
  type        = bool
  default     = false
}

variable "enable_prometheus_monitoring" {
  description = "Enable Prometheus monitoring"
  type        = bool
  default     = false
}

variable "POSTGRES_ENABLED" {
  description = "Enable PostgreSQL database"
  type        = bool
  default     = false
}

variable "REDIS_ENABLED" {
  description = "Enable Redis cache"
  type        = bool
  default     = false
}

variable "CRM_SERVICE_URL" {
  description = "CRM service URL"
  default     = ""
}

variable "CRM_TEST_MODE" {
  description = "Enable CRM test mode"
  type        = bool
  default     = false
}

locals {
  app_resource_group_name = "${var.azure_resource_prefix}-${var.service_short}-${var.config_short}-rg"
  app_secrets = {
    PG_CONN_STR    = var.POSTGRES_ENABLED ? local.postgres_connection_string : ""
    REDIS_CONN_STR = var.REDIS_ENABLED ? local.redis_connection_string : ""
  }
}
