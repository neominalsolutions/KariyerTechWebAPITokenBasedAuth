using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAppClient.Models;

namespace WebAppClient.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    static HttpClient client = new HttpClient();
    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }


    public async Task<IActionResult> ResSharpIndex()
    {



      var postData = new LoginInputModel();
      postData.Email = "test@test.com";
      postData.Password = "123456";

      var payload = JsonSerializer.Serialize(postData);

      RestClient client = new RestClient("http://localhost:48315/api");

      RestRequest req1 = new RestRequest("auth/token", Method.POST);
      req1.AddJsonBody(payload);
      IRestResponse res2 = client.Execute(req1);

      string token = res2.Content.Replace(@"\","");



      string value = $"Bearer {token}";

      var dic = new Dictionary<string, string>();
      dic.Add("Authorization", value);

      client.AddDefaultHeaders(dic);
      RestRequest request = new RestRequest("product",
      Method.GET);
      var response = client.Execute<List<ProductViewModel>>(request);


      return View();


    }

    public async Task<IActionResult> Index()
    {
      // Bearer Authentication Implementation witch HttpClient

      using (HttpClient client = new HttpClient())
      {

        client.BaseAddress = new Uri("https://localhost:44372");
        client.DefaultRequestHeaders.Accept.Clear();

        var postData = new LoginInputModel();
        postData.Email = "test@test.com";
        postData.Password = "123456";

        var payload = JsonSerializer.Serialize(postData);
        StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage tokenResponse = await client.PostAsync("api/auth/token", content);

        string token = await tokenResponse.Content.ReadAsStringAsync();
        // you can set this token session

        client.DefaultRequestHeaders.Authorization
               = new AuthenticationHeaderValue("Bearer", token);

        //client.SetBearerToken(token);

        var response = await client.GetAsync("api/product");


        if (response.IsSuccessStatusCode)
        {
          var productDto = await response.Content.ReadAsAsync<List<ProductViewModel>>();
        }

      }

      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }


    public IActionResult JqueryAjaxRequest()
    {
      return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
