module "dfe_analytics" {
  count  = 0  # Disabled by default, can be enabled in specific environments
  source = "./vendor/modules/aks//aks/dfe_analytics"

  namespace                  = var.namespace
  environment                = var.environment
  service_name               = var.service_name
  google_analytics_id        = ""
  google_tag_manager_id      = ""
  google_tag_manager_enabled = false
}
