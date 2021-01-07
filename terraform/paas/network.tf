resource "cloudfoundry_network_policy" "api-policy" {
  policy {
    destination_app = cloudfoundry_app.api_application.id
    source_app      = module.prometheus[0].app_id
    port            = 8080
  }
}
