# Degree Status Inference - Domain Service
The degree status inference is a domain service that is used to determine the degree status of a user based on their year of study. Based on the year of study a user specifies, their degree status will be inferred based on the currently assumed academic year from 1st Sept 20X1 - 31st Aug 20X2 for example, the statuses applied are as follows:


* **HasDegree (crm ref: 222750000)** - if the academic year provided has already occurred in relation to the given academic year;
* **FinalYear (crm ref: 222750001)** - if the academic year provided falls within the current academic year;
* **SecondYear (crm ref: 222750002)** - if the academic year provided falls one year behind the current academic year;
* **FirstYear (crm ref: 222750003)** - if the academic year provided falls two years or more behind the current academic year;
* **NoDegree (crm ref: 222750004)** - if no academic year is provided;
* **Other (crm ref: 222750005)** - default value if no degree status can be inferred or has not been provisioned.

The service has been applied as a temporary means to ensure the CRM continues to recieve the correct degree status values until this approach becomes deprecated. Once this is the case, the service must be unwound to ensure no degree status Id's are sent to the CRM.  However, the domain service must be retained in order to ensure a degree status can be returned from the service endpoint to ensure the front-end can provide the necessary values to third-party support services.

## Service Partial Unwind
In order to ensure we correctly unwind the service from the CRM but retain the correctly inferred degree status response, the **InferDegreeStatus** method under the **MailingListAddMember.cs** must be modified as follows,

```csharp
public int? InferDegreeStatus(
    IDegreeStatusDomainService degreeStatusDomainService,
    ICurrentYearProvider currentYearProvider)
{
    if (GraduationYear != null)
    {
        const int GraduationDay = 31;
        const int GraduationMonth = 8;

        DegreeStatusInferenceRequest degreeStatusInferenceRequest =
            DegreeStatusInferenceRequest.Create(
                new GraduationYear(GraduationYear.Value, currentYearProvider), currentYearProvider);

        return degreeStatusDomainService
            .GetInferredDegreeStatusFromGraduationYear(degreeStatusInferenceRequest);
    }

    return (int)DegreeStatus.NoDegree;
}
```