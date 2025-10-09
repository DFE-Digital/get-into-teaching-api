output "app_url" {
  value = module.api_application.url
}

output "postgres_db_url" {
  value = module.postgres.url
}

output "postgres_server_docker_image" {
  value = module.postgres.server_docker_image
}

output "postgres_server_database_type" {
  value = module.postgres.server_database_type
}

output "azure_extensions" {
  value = module.postgres.azure_extensions
}