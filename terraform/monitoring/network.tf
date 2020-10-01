resource "cloudfoundry_network_policy" "my-policy" {

  policy {
    destination_app = data.cloudfoundry_app.app.id
    source_app      = module.prometheus.app_id
    port            = var.port
  }

  policy {
    destination_app = data.cloudfoundry_app.tta.id
    source_app      = module.prometheus.app_id
    port            = var.port
  }

  policy {
    destination_app = data.cloudfoundry_app.api.id
    source_app      = module.prometheus.app_id
    port            = var.port
  }
}

