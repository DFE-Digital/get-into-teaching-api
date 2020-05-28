provider "cloudfoundry" {
    store_tokens_path = "./tokens"
    api_url = "https://api.london.cloud.service.gov.uk"
    user = var.user
    sso_passcode =var.key 
}

