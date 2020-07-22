module "alertmanager" {
     source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/alertmanager?ref=devops/get-into-teaching/add-postgres"
     space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.alertmanager[ "name" ]}"
     config = var.alertmanager[ "config" ]
}
