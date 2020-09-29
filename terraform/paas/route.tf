resource "cloudfoundry_route" "api_route_cloud" {
    domain = data.cloudfoundry_domain.cloudapps.id
    hostname =  var.paas_api_route_name
    space = data.cloudfoundry_space.space.id
    target {
          app = cloudfoundry_app.api_application.id
    }

}

resource "cloudfoundry_route" "api_route_internal" {
    domain = data.cloudfoundry_domain.internal.id
    hostname =  "${var.paas_api_route_name}-internal"
    space = data.cloudfoundry_space.space.id
    target {
          app = cloudfoundry_app.api_application.id
    }
}
