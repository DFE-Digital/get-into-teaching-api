module "api_application" {
  source = "./vendor/modules/aks//aks/application"

  is_web = true

  namespace    = var.namespace
  environment  = var.environment
  service_name = var.service_name

  cluster_configuration_map = module.cluster_data.configuration_map

  kubernetes_config_map_name = module.application_configuration.kubernetes_config_map_name
  kubernetes_secret_name     = module.application_configuration.kubernetes_secret_name

  docker_image           = var.app_docker_image
  max_memory             = var.memory_max
  replicas               = var.replicas
  web_external_hostnames = var.gov_uk_host_names
  web_port               = 8080
  enable_logit           = var.enable_logit

  enable_prometheus_monitoring  = var.enable_prometheus_monitoring
}

module "application_configuration" {
  source = "./vendor/modules/aks//aks/application_configuration"

  namespace              = var.namespace
  environment            = var.environment
  azure_resource_prefix  = var.azure_resource_prefix
  service_short          = var.service_short
  config_short           = var.config_short
  config_variables       = { AKS_ENV_NAME = var.file_environment, EnableMetrics = false }
  secret_variables       = local.app_secrets
  secret_key_vault_short = "app"
}
