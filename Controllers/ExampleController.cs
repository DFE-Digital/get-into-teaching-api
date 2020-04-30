using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Controllers {
  [ApiController]
  [Route("[controller]")]
  public class ExampleController : ControllerBase {
    private readonly ILogger<ExampleController> _logger;

    public ExampleController(ILogger<ExampleController> logger) {
      _logger = logger;
    }

    [HttpGet]
    public IEnumerable<Example> Get() {
      return Enumerable.Range(1, 5).Select(rand => new Example {
          RandomNumber = rand
        })
        .ToArray();
    }
  }
}
