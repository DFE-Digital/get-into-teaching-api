resource "cloudfoundry_route" "api_route_cloud" {
  domain   = data.cloudfoundry_domain.cloudapps.id
  hostname = var.paas_api_application_name
  space    = data.cloudfoundry_space.space.id

}

resource "cloudfoundry_route" "api_route_internal" {
  domain   = data.cloudfoundry_domain.internal.id
  hostname = "${var.paas_api_application_name}-internal"
  space    = data.cloudfoundry_space.space.id
}

locals {
  api_endpoint = "${cloudfoundry_route.api_route_cloud.hostname}.${data.cloudfoundry_domain.cloudapps.name}"
}

