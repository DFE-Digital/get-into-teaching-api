module "prometheus_alerting" {
  count  = var.enable_prometheus_monitoring ? 1 : 0
  source = "./vendor/modules/aks//aks/prometheus_alerting"

  namespace               = var.namespace
  service_name            = var.service_name
  environment             = var.environment
  enable_prometheus_alerts = var.enable_prometheus_monitoring
}
