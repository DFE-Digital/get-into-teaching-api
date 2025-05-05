output "app_url" {
  value = module.api_application.application_url
}

output "app_resource_group_name" {
  value = local.app_resource_group_name
}

output "postgres_server_name" {
  value = var.POSTGRES_ENABLED ? module.postgres[0].server_name : "postgres-not-enabled"
}

output "redis_cache_name" {
  value = var.REDIS_ENABLED ? module.redis-cache[0].redis_cache_name : "redis-not-enabled"
}

output "postgresql_database_name" {
  value = var.POSTGRES_ENABLED ? module.postgres[0].database_name : "postgres-not-enabled"
}

output "redis_connection_string" {
  value     = var.REDIS_ENABLED ? local.redis_connection_string : "redis-not-enabled"
  sensitive = true
}

output "storage_account_name" {
  value = var.POSTGRES_ENABLED ? module.postgres[0].storage_account_name : "postgres-not-enabled"
}
