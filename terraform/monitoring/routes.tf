data "cloudfoundry_route" "api_endpoint" {
  domain   = data.cloudfoundry_domain.education.id
  hostname = var.api_application_name
}

locals {
  api_endpoint = "${data.cloudfoundry_route.api_endpoint.hostname}.${data.cloudfoundry_domain.education.name}"
}

data "cloudfoundry_route" "api_internal" {
  domain   = data.cloudfoundry_domain.internal.id
  hostname = "${var.api_application_name}-internal"
}


data "cloudfoundry_route" "app_internal" {
  domain   = data.cloudfoundry_domain.internal.id
  hostname = "${var.app_application_name}-internal"
}

data "cloudfoundry_route" "tta_internal" {
  domain   = data.cloudfoundry_domain.internal.id
  hostname = "${var.tta_application_name}-internal"
}


