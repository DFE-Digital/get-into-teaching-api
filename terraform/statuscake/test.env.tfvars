alerts =  {
GIT_API_TEST_HEALTHCHECK = {
    website_name = "Get Teacher Training API (Test Space)"
    website_url   = "https://get-into-teaching-api-test.london.cloudapps.digital/api/operations/health_check"
    test_type     = "HTTP"
    check_rate    = 60
    contact_group = [185037]
    trigger_rate  = 0
    timeout       = 80
    custom_header = "{\"Content-Type\": \"application/x-www-form-urlencoded\"}"
    status_codes  = "204, 205, 206, 303, 400, 401, 403, 404, 405, 406, 408, 410, 413, 444, 429, 494, 495, 496, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 521, 522, 523, 524, 520, 598, 599"
  }
GIT_API_PROD_HEALTHCHECK = {
    website_name = "Get Teacher Training API (Production Space)"
    website_url   = "https://get-into-teaching-api-prod.london.cloudapps.digital/api/operations/health_check"
    test_type     = "HTTP"
    check_rate    = 60
    contact_group = [185037]
    trigger_rate  = 0
    timeout       = 80
    custom_header = "{\"Content-Type\": \"application/x-www-form-urlencoded\"}"
    status_codes  = "204, 205, 206, 303, 400, 401, 403, 404, 405, 406, 408, 410, 413, 444, 429, 494, 495, 496, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 521, 522, 523, 524, 520, 598, 599"
  }
}
