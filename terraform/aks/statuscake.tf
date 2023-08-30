module "statuscake" {
  count = var.enable_statuscake_alerts ? 1 : 0

  source = "./vendor/modules/aks/monitoring/statuscake"

  uptime_urls = [module.api_application.probe_url]

  contact_groups = [185037]
}
