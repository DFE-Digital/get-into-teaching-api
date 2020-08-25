using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace GetIntoTeachingApiContractTests.Servers
{
    public class MockCrmServer : WireMockServer
    {
        public MockCrmServer(string crmServiceUrl) 
            :base(new FluentMockServerSettings { Urls = new [] { $"{crmServiceUrl}" } })
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            Start();

            var authenticationServiceUrl =
                $"{crmServiceUrl}/fad277c9-c60a-4da1-b5f3-b3b8b34a82f9/oauth2/authorize";
            var unauthorisedResponseHeader =
                $"Bearer authorization_uri={authenticationServiceUrl}, resource_id={crmServiceUrl}";
            AddMockXmlSoapApiUnauthorisedRedirect(unauthorisedResponseHeader);
            
            AddMockOAuthApi();

            const string retrieveCurrentOrganizationRequestMatcher = "/s:Envelope/s:Body/Execute/request/a:RequestName[text()=\"RetrieveCurrentOrganization\"]";
            const string retrieveCurrentOrganizationResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><ExecuteResponse xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\"><ExecuteResult xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" i:type=\"b:RetrieveCurrentOrganizationResponse\"><a:ResponseName>RetrieveCurrentOrganization</a:ResponseName><a:Results xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\"><a:KeyValuePairOfstringanyType><c:key>Detail</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/xrm/2014/Contracts\" i:type=\"d:OrganizationDetail\"><d:Endpoints><d:KeyValuePairOfEndpointTypestringyDL0RVHi><c:key>WebApplication</c:key><c:value>https://localhost:8080/Test/</c:value></d:KeyValuePairOfEndpointTypestringyDL0RVHi><d:KeyValuePairOfEndpointTypestringyDL0RVHi><c:key>OrganizationService</c:key><c:value>https://localhost:8080/Test/XRMServices/2011/Organization.svc</c:value></d:KeyValuePairOfEndpointTypestringyDL0RVHi><d:KeyValuePairOfEndpointTypestringyDL0RVHi><c:key>OrganizationDataService</c:key><c:value>https://localhost:8080/Test/XRMServices/2011/OrganizationData.svc</c:value></d:KeyValuePairOfEndpointTypestringyDL0RVHi></d:Endpoints><d:EnvironmentId>c7265fee-0279-492a-ac3f-f7c9140e50ae</d:EnvironmentId><d:FriendlyName>GITIS Mock</d:FriendlyName><d:Geo>EMEA</d:Geo><d:OrganizationId>820d7afa-a6a6-44ea-8279-ba5621810971</d:OrganizationId><d:OrganizationVersion>9.1.0.21048</d:OrganizationVersion><d:State>Enabled</d:State><d:TenantId>fad277c9-c60a-4da1-b5f3-b3b8b34a82f9</d:TenantId><d:UniqueName>orgee42d4e9</d:UniqueName><d:UrlName>gitis-mock</d:UrlName></c:value></a:KeyValuePairOfstringanyType></a:Results></ExecuteResult></ExecuteResponse></s:Body></s:Envelope>";
            //.WithHeader("Set-Cookie", "ARRAffinity=042269a208d2db1ec3fce7e5fca737a0f6f466ea075b4a014f0a3213ec73550c; domain=localhost; path=/; secure; HttpOnly")
            //.WithHeader("Set-Cookie", "ReqClientId=551ac6bb-fbc9-4a3a-9697-f4da527f739d; expires=Wed, 19-Dec-2070 00:00:00 GMT; path=/; secure; HttpOnly; samesite=none")
            //.WithHeader("Set-Cookie", "orgId=820d7afa-a6a6-44ea-8279-ba5621810971; expires=Wed, 19-Dec-2070 00:00:00 GMT; path=/; secure; HttpOnly; samesite=none")
            AddMockXmlSoapApi(retrieveCurrentOrganizationRequestMatcher, retrieveCurrentOrganizationResponse);
            
            const string whoAmIRequestMatcher = "/s:Envelope/s:Body/Execute/request/a:RequestName[text()=\"WhoAmI\"]";
            const string whoAmIResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><ExecuteResponse xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\"><ExecuteResult xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" i:type=\"b:WhoAmIResponse\"><a:ResponseName>WhoAmI</a:ResponseName><a:Results xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\"><a:KeyValuePairOfstringanyType><c:key>UserId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">0ec243d4-1aaf-ea11-a812-000d3a44a2a9</c:value></a:KeyValuePairOfstringanyType><a:KeyValuePairOfstringanyType><c:key>BusinessUnitId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">b78fd0c3-ae25-e811-a83f-000d3a2af321</c:value></a:KeyValuePairOfstringanyType><a:KeyValuePairOfstringanyType><c:key>OrganizationId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">820d7afa-a6a6-44ea-8279-ba5621810971</c:value></a:KeyValuePairOfstringanyType></a:Results></ExecuteResult></ExecuteResponse></s:Body></s:Envelope>";
            AddMockXmlSoapApi(whoAmIRequestMatcher, whoAmIResponse);

            const string eventsRequestMatcher = "/s:Envelope/s:Body/Execute/request/a:Parameters/a:KeyValuePairOfstringanyType/b:value/a:EntityName[text()=\"msevtmgt_event\"]";
            const string eventsResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><ExecuteResponse xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\"><ExecuteResult xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" i:type=\"b:WhoAmIResponse\"><a:ResponseName>WhoAmI</a:ResponseName><a:Results xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\"><a:KeyValuePairOfstringanyType><c:key>UserId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">0ec243d4-1aaf-ea11-a812-000d3a44a2a9</c:value></a:KeyValuePairOfstringanyType><a:KeyValuePairOfstringanyType><c:key>BusinessUnitId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">b78fd0c3-ae25-e811-a83f-000d3a2af321</c:value></a:KeyValuePairOfstringanyType><a:KeyValuePairOfstringanyType><c:key>OrganizationId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">820d7afa-a6a6-44ea-8279-ba5621810971</c:value></a:KeyValuePairOfstringanyType></a:Results></ExecuteResult></ExecuteResponse></s:Body></s:Envelope>";
            // .WithHeader("Set-Cookie", "ARRAffinity=042269a208d2db1ec3fce7e5fca737a0f6f466ea075b4a014f0a3213ec73550c; domain=localhost; path=/; secure; HttpOnly")
            AddMockXmlSoapApi(eventsRequestMatcher, eventsResponse);
            
            const string privacyPoliciesRequestMatcher = "/s:Envelope/s:Body/Execute/request/a:Parameters/a:KeyValuePairOfstringanyType/b:value/a:EntityName[text()=\"dfe_privacypolicy\"]";
            const string privacyPolicyResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><ExecuteResponse xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\"><ExecuteResult xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" i:type=\"b:WhoAmIResponse\"><a:ResponseName>WhoAmI</a:ResponseName><a:Results xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\"><a:KeyValuePairOfstringanyType><c:key>UserId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">0ec243d4-1aaf-ea11-a812-000d3a44a2a9</c:value></a:KeyValuePairOfstringanyType><a:KeyValuePairOfstringanyType><c:key>BusinessUnitId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">b78fd0c3-ae25-e811-a83f-000d3a2af321</c:value></a:KeyValuePairOfstringanyType><a:KeyValuePairOfstringanyType><c:key>OrganizationId</c:key><c:value xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\" i:type=\"d:guid\">820d7afa-a6a6-44ea-8279-ba5621810971</c:value></a:KeyValuePairOfstringanyType></a:Results></ExecuteResult></ExecuteResponse></s:Body></s:Envelope>";
            // .WithHeader("Set-Cookie", "ARRAffinity=8b957b80dd2680f65d1ad0a82703c12cfdb5ad0fe272999b4c1db53c21896dea; domain=localhost; path=/; secure; HttpOnly")
            AddMockXmlSoapApi(privacyPoliciesRequestMatcher, privacyPolicyResponse);
        }

        private void AddMockOAuthApi()
        {
            Given(Request.Create()
                    .WithPath("/common/discovery/instance")
                    .UsingGet())
                .AtPriority(1)
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithHeader("P3P", "CP=\"DSP CUR OTPi IND OTRi ONL FIN\"")
                        //.WithHeader("Set-Cookie", "fpc=AklsyOB1Ur1MlNHMzer7FaA; expires=Sat, 19-Dec-2020 00:00:00 GMT; path=/; secure; HttpOnly; SameSite=None")
                        //.WithHeader("Set-Cookie", "esctx=AQABAAAAAAAGV_bv21oQQ4ROqh0_1-tA5DCeu1MArQPGj106L63K1wvpJIcMyIThyCj2OVsXl9zLveVl04PS-p_RIHTr5z19JIRiJUMwuzUcrh-RADOgSwOu0z82HX1Ytprrs3jYkgucksiHFPhhnYQIWcQ5Lo2ahtzEuDfWZflQ7ERFOBnZtQtKMkAS5_ztQEhjfxR3sFogAA; domain=localhost; path=/; secure; HttpOnly; SameSite=None")
                        //.WithHeader("Set-Cookie", "x-ms-gateway-slice=prod; path=/; secure; httponly")
                        //.WithHeader("Set-Cookie", "stsservicecookie=ests; path=/; secure; httponly")
                        .WithBody(
                            "{\"api-version\":\"1.1\",\"metadata\":[{\"aliases\":[\"localhost:8080\"],\"preferred_cache\":\"localhost:8080\",\"preferred_network\":\"localhost:8080\"}],\"tenant_discovery_endpoint\":\"https://localhost:8080/Test/fad277c9-c60a-4da1-b5f3-b3b8b34a82f9/.well-known/openid-configuration\"}")
                );

            Given(Request.Create()
                    .WithPath("/fad277c9-c60a-4da1-b5f3-b3b8b34a82f9/oauth2/token")
                    .UsingPost())
                .AtPriority(1)
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithHeader("P3P", "CP=\"DSP CUR OTPi IND OTRi ONL FIN\"")
                        //.WithHeader("Set-Cookie", "fpc=AryuOkilk7NGk-xAwV8OmTa6bpr7AQAAAJS60NYOAAAA; expires=Sat, 19-Dec-2070 00:00:00 GMT; path=/; secure; HttpOnly; SameSite=None")
                        //.WithHeader("Set-Cookie", "x-ms-gateway-slice=estsfd; path=/; secure; httponly")
                        //.WithHeader("Set-Cookie", "stsservicecookie=estsfd; path=/; secure; httponly")
                        .WithBody(
                            "{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImppYk5ia0ZTU2JteFBZck45Q0ZxUms0SzRndyIsImtpZCI6ImppYk5ia0ZTU2JteFBZck45Q0ZxUms0SzRndyJ9.eyJhdWQiOiJodHRwczovL2dpdGlzLWRldi5hcGkuY3JtNC5keW5hbWljcy5jb20vIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZmFkMjc3YzktYzYwYS00ZGExLWI1ZjMtYjNiOGIzNGE4MmY5LyIsImlhdCI6MTU5Nzk0ODUyMSwibmJmIjoxNTk3OTQ4NTIxLCJleHAiOjE1OTc5NTI0MjEsImFpbyI6IkUyQmdZUGg1aVduU2R1T2FXZGxYNVY2dytxNmJCQUE9IiwiYXBwaWQiOiI5NjViN2EzYS1kYzBhLTRmNGYtYjc1Yy0xNGFkNmRiYzU3ZjIiLCJhcHBpZGFjciI6IjEiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9mYWQyNzdjOS1jNjBhLTRkYTEtYjVmMy1iM2I4YjM0YTgyZjkvIiwib2lkIjoiODNmOGI5NDgtMmNiNy00NGYzLWI0NGQtZmNmZTQyZmU4MTIzIiwicmgiOiIwLkFTOEF5WGZTLWdyR29VMjE4N080czBxQy1UcDZXNVlLM0U5UHQxd1VyVzI4Vl9JdkFBQS4iLCJzdWIiOiI4M2Y4Yjk0OC0yY2I3LTQ0ZjMtYjQ0ZC1mY2ZlNDJmZTgxMjMiLCJ0aWQiOiJmYWQyNzdjOS1jNjBhLTRkYTEtYjVmMy1iM2I4YjM0YTgyZjkiLCJ1dGkiOiJYaC1jakJTcDJVcXBsdHhUVmU4ZUFBIiwidmVyIjoiMS4wIn0.pjEanUHeecJK5nUUEc5SPLyQK5ggeG_pLLWHXWYDO4Q-xa3KZcq8tkHs7jVa9AxAEEEPZY4fD3o8hF6GuaCWAc3Vl1b4TsK2NzwVazcCKz1A3U5VYNhTP_gRYbEZK4TV2JvCoMLNOh1SvyaB9Me9_ghi07kksIhjJeJEJrbyAlx79mrsTwwnqQsjM_qtzx1_YISglRAul1cLTet_Wkc4MfnRnFYanRhJ5ulrwar6IdSr671bPF9POYJA4bkOeEnJSBrrxlAOo_E_NeObBkXpm84ha2JdApfLv1EYt4nApdRQNxpodyigpIMpji9OcSxKFB6KbsGOfo7B3hefTV4lRA\",\"expires_in\":\"3599\",\"expires_on\":\"1597952421\",\"ext_expires_in\":\"3599\",\"not_before\":\"1597948521\",\"resource\":\"https://localhost:8080/Test\",\"token_type\":\"Bearer\"}")
                );
        }

        private void AddMockXmlSoapApiUnauthorisedRedirect(string responseHeader)
        {// priority 10 means this will match if no other XRM request does
            Given(Request.Create()
                    .WithPath("/XRMServices/2011/Organization.svc/web")
                    .UsingGet())
                .AtPriority(10) 
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(401)
                        //.WithHeader("Set-Cookie", "ARRAffinity=042269a208d2db1ec3fce7e5fca737a0f6f466ea075b4a014f0a3213ec73550c; domain=localhost; path=/; secure; HttpOnly")
                        .WithHeader("WWW-Authenticate", responseHeader)
                );
        }

        private void AddMockXmlSoapApi(string requestMatcher, string response)
        {
            Given(Request.Create()
                    .WithPath("/XRMServices/2011/Organization.svc/web")
                    .WithBody(new XPathMatcher(requestMatcher))
                    .UsingGet())
                .AtPriority(1)
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "text/xml; charset=utf-8")
                        .WithBody(response)
                );
        }
    }
}