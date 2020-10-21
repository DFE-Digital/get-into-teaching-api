data "cloudfoundry_app" "prometheus" {
  name_or_id = var.prometheus
  space      = data.cloudfoundry_space.monitor.id
}

resource "cloudfoundry_network_policy" "api-policy" {
  policy {
    destination_app = cloudfoundry_app.api_application.id
    source_app      = data.cloudfoundry_app.prometheus.id
    port            = 8080
  }
}

