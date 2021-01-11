data cloudfoundry_app app_application {
  name_or_id = var.app_application_name
  space      = data.cloudfoundry_space.space.id
}

data cloudfoundry_app tta_application {
  name_or_id = var.tta_application_name
  space      = data.cloudfoundry_space.space.id
}

resource "cloudfoundry_network_policy" "api-policy" {
  count = var.monitoring
  policy {
    destination_app = cloudfoundry_app.api_application.id
    source_app      = module.prometheus[0].app_id
    port            = 8080
  }
  policy {
    destination_app = data.cloudfoundry_app.app_application.id
    source_app      = module.prometheus[0].app_id
    port            = 3000
  }
  policy {
    destination_app = data.cloudfoundry_app.tta_application.id
    source_app      = module.prometheus[0].app_id
    port            = 3000
  }
}
