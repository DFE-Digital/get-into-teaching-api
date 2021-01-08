resource "cloudfoundry_network_policy" "api-policy" {
  count = var.monitoring
  policy {
    destination_app = cloudfoundry_app.api_application.id_bg
    source_app      = module.prometheus[0].app_id
    port            = 8080
  }
}
