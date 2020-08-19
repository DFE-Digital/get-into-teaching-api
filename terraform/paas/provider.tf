terraform {
 required_version = "~> 0.13"

  required_providers {
    cloudfoundry = {
      source  = "terraform-registry.us-east.philips-healthsuite.com/philips-forks/cloudfoundry"
      version = "0.12.2-202008131826"
    }
  }
}

provider "cloudfoundry" {
    api_url = var.api_url
    user = var.user
    password =var.password 
}

