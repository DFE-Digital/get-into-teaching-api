resource "cloudfoundry_network_policy" "my-policy" {

    policy {
        source_app = data.cloudfoundry_app.app.id
        destination_app = data.cloudfoundry_app.api.id
        port = 8080
    }

    policy {
        source_app = data.cloudfoundry_app.tta.id
        destination_app = data.cloudfoundry_app.api.id
        port = 8080
    }
}
