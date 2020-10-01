data "cloudfoundry_domain" "education" {
  name = var.domain
}

data "cloudfoundry_domain" "internal" {
  name = "apps.internal"
}
