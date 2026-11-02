locals {
  app_resource_group_name = "${var.azure_resource_prefix}-${var.service_short}-${var.config_short}-rg"

  app_secrets = {
    PG_CONN_STR    = module.postgres.dotnet_connection_string
    REDIS_CONN_STR = module.redis-cache.connection_string
    DATABASE_URL   = module.postgres.url
  }

  postgres_extensions = ["POSTGIS"]
}
