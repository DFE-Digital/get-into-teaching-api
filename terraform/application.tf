resource "cloudfoundry_app" "api_application" {
    name =  var.paas_api_application_name
    space = data.cloudfoundry_space.space.id
    docker_image = "dfedigital/get-into-teaching-api:GITPB-149"
    service_binding {
            service_instance = cloudfoundry_service_instance.postgres.id
        }
    routes {
        route = cloudfoundry_route.api_route.id
    }    
    environment =  {
         CRM_CLIENT_ID     = var.CRM_CLIENT_ID 
         CRM_CLIENT_SECRET = var.CRM_CLIENT_SECRET 
         CRM_SERVICE_URL   = var.CRM_SERVICE_URL  
         CRM_TENANT_ID     = var.CRM_TENANT_ID   
         SHARED_SECRET     = var.SHARED_SECRET }     
}

resource "cloudfoundry_route" "api_route" {
    domain = data.cloudfoundry_domain.cloudapps.id
    space = data.cloudfoundry_space.space.id
    hostname =  var.paas_api_route_name
}


