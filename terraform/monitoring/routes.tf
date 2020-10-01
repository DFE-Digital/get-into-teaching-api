data "cloudfoundry_route" "api_endpoint" {
  domain   = data.cloudfoundry_domain.education.id
  hostname = var.api_application_name
}

locals {
  api_endpoint = "${data.cloudfoundry_route.api_endpoint.hostname}.${data.cloudfoundry_domain.education.name}"
}


