locals {
  review_hostname = var.pr_number != "" ? ["getintoteachingapi-review-${var.pr_number}.${var.cluster}.teacherservices.cloud"] : []
  app_hostnames = var.environment == "review" ? local.review_hostname : var.gov_uk_host_names
  deployment_environment = var.environment == "review" && var.pr_number != "" ? "review-${var.pr_number}" : var.environment
  aks_env_name = var.environment == "review" && var.pr_number != "" ? "review-${var.pr_number}" : var.file_environment
}

module "api_application" {
  source = "./vendor/modules/aks//aks/application"

  is_web = true

  namespace    = var.namespace
  environment  = local.deployment_environment
  service_name = var.service_name

  cluster_configuration_map = module.cluster_data.configuration_map

  kubernetes_config_map_name = module.application_configuration.kubernetes_config_map_name
  kubernetes_secret_name     = module.application_configuration.kubernetes_secret_name

  docker_image           = var.app_docker_image
  max_memory             = var.memory_max
  replicas               = var.replicas
  web_external_hostnames = local.app_hostnames
  web_port               = 8080
  enable_logit           = var.enable_logit

  enable_prometheus_monitoring = var.enable_prometheus_monitoring
}

module "application_configuration" {
  source = "./vendor/modules/aks//aks/application_configuration"

  namespace              = var.namespace
  environment            = local.deployment_environment
  azure_resource_prefix  = var.azure_resource_prefix
  service_short          = var.service_short
  config_short           = var.config_short
  config_variables       = { AKS_ENV_NAME = local.aks_env_name, EnableMetrics = false }
  secret_variables       = local.app_secrets
  secret_key_vault_short = "app"
}
