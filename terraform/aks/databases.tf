module "redis-cache" {
  source = "./vendor/modules/aks//aks/redis"
  count  = var.REDIS_ENABLED ? 1 : 0

  namespace             = var.namespace
  environment           = local.environment
  azure_resource_prefix = var.azure_resource_prefix
  service_short         = var.service_short
  config_short          = var.config_short
  service_name          = var.service_name

  cluster_configuration_map = module.cluster_data.configuration_map

  use_azure               = true
  azure_enable_monitoring = var.enable_monitoring
  azure_patch_schedule    = [{ "day_of_week" : "Sunday", "start_hour_utc" : 01 }]
  azure_maxmemory_policy  = "allkeys-lfu"
  server_version          = 6
  azure_capacity          = var.redis_capacity
  azure_family            = var.redis_family
  azure_sku_name          = var.redis_sku_name
}

module "postgres" {
  source = "./vendor/modules/aks//aks/postgres"
  count  = var.POSTGRES_ENABLED ? 1 : 0

  namespace             = var.namespace
  environment           = local.environment
  azure_resource_prefix = var.azure_resource_prefix
  service_short         = var.service_short
  config_short          = var.config_short
  service_name          = var.service_name

  cluster_configuration_map = module.cluster_data.configuration_map

  use_azure               = true
  azure_enable_monitoring = var.enable_monitoring
  azure_extensions        = ["POSTGIS"]
  server_version          = var.postgres_version
  azure_sku_name          = var.postgres_flexible_server_sku

  azure_enable_high_availability = var.postgres_enable_high_availability
  azure_maintenance_window       = var.azure_maintenance_window
  azure_enable_backup_storage    = var.azure_enable_backup_storage
}

# Update local.app_secrets to handle the conditional modules
locals {
  redis_connection_string   = var.REDIS_ENABLED ? module.redis-cache[0].connection_string : ""
  postgres_connection_string = var.POSTGRES_ENABLED ? module.postgres[0].dotnet_connection_string : ""
}
