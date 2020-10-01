data "cloudfoundry_app" "app" {
  name_or_id = var.app_application_name
  space      = data.cloudfoundry_space.space.id
}

data "cloudfoundry_app" "api" {
  name_or_id = var.api_application_name
  space      = data.cloudfoundry_space.space.id
}

data "cloudfoundry_app" "tta" {
  name_or_id = var.tta_application_name
  space      = data.cloudfoundry_space.space.id
}
