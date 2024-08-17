using AutorizationMicroService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AutorizationMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizationController : ControllerBase
    {
        private readonly IConfiguration _config;

        private readonly static Dictionary<string, int> _Two_A_mem = new();

        public AutorizationController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("Occ/{id}")]
        public async Task<ActionResult<OccDTO>> GetAdminOcc(int id)
        {

            using HttpClient client = new();
            string uri_base = @$"{_config.GetConnectionString("CRUD_Server_Conn")}/Occ/{id}";
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new(HttpMethod.Get, uri_base);
            using HttpResponseMessage resp = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (resp.IsSuccessStatusCode)
            {
                string result = await resp.Content.ReadAsStringAsync();
                OccDTO _temp = JsonSerializer.Deserialize<OccDTO>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return _temp;
            }
            else
            {
                return null;
            }
        }

        [HttpGet("admins")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<bool> Autorize([FromQuery] int id, [FromQuery] string hash)
        {
            using HttpClient client = new();
            string uri_base = @$"{_config.GetConnectionString("CRUD_Server_Conn")}/{id}";

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new(HttpMethod.Get, uri_base);
            using HttpResponseMessage resp = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (resp.IsSuccessStatusCode)
            {
                string result = await resp.Content.ReadAsStringAsync();
                Admin _tempAdm = JsonSerializer.Deserialize<Admin>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                if (String.Equals(_tempAdm.Shapass, hash, StringComparison.Ordinal))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        [HttpPut("SetAdminOffline/{id}")]
        public async Task<ActionResult> SetAdminOffline(int id)
        {
            using (HttpClient client = new())
            {
                string uri_base = @$"{_config.GetConnectionString("AdminDTO_Conn")}/{id}";

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request = new(HttpMethod.Get, uri_base);
                using HttpResponseMessage resp = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                if (resp.IsSuccessStatusCode)
                {
                    string result = await resp.Content.ReadAsStringAsync();
                    Admin _tempAdm = JsonSerializer.Deserialize<Admin>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (_tempAdm == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        if (_tempAdm.Stat != false)
                        {
                            _tempAdm.Stat = false;

                            string jObject = JsonSerializer.Serialize<Admin>(_tempAdm);
                            StringContent content = new(jObject, Encoding.UTF8, "application/json");

                            HttpResponseMessage _putResponse = await client.PutAsync(uri_base, content);

                            if (_putResponse.IsSuccessStatusCode)
                            {
                                return Ok(_putResponse);
                            }
                            else
                            {
                                Console.WriteLine(jObject.ToString());
                                return BadRequest(_putResponse.Content);
                            }

                        }
                    }
                }
                else
                {
                    return BadRequest(resp);
                }
            }

            return NoContent();
        }

        [HttpPut("SetAdminOnline/{id}")]
        public async Task<ActionResult> SetAdminOnline(int id)
        {
            using (HttpClient client = new())
            {
                string uri_base = @$"{_config.GetConnectionString("AdminDTO_Conn")}/{id}";

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request = new(HttpMethod.Get, uri_base);
                using HttpResponseMessage resp = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                if (resp.IsSuccessStatusCode)
                {
                    string result = await resp.Content.ReadAsStringAsync();
                    Admin _tempAdm = JsonSerializer.Deserialize<Admin>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (_tempAdm == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        if (_tempAdm.Stat != true)
                        {
                            _tempAdm.Stat = true;

                            string jObject = JsonSerializer.Serialize<Admin>(_tempAdm);
                            StringContent content = new(jObject, Encoding.UTF8, "application/json");

                            HttpResponseMessage _putResponse = await client.PutAsync(uri_base, content);

                            if (_putResponse.IsSuccessStatusCode)
                            {
                                return Ok(_putResponse);
                            }
                            else
                            {
                                Console.WriteLine(jObject.ToString());
                                return BadRequest(_putResponse.Content);
                            }

                        }
                    }
                }
                else
                {
                    return BadRequest(resp);
                }
            }

            return NoContent();
        }

        [HttpGet("Sign/Generate")]
        public async Task<ActionResult> GenerateTwoFactorCode([FromQuery] string PhoneNumber)
        {
            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                Regex r = new(@"\+[0-9]*\(\d{3}\)\d{3}-\d{2}-\d{2}$");

                if (r.IsMatch(PhoneNumber))
                {
                    try
                    {
                        int code = 0;
                        Random rnd = new();
                        code = rnd.Next(1000, 10000);
                        if (await Task.Run(() => _Two_A_mem.ContainsKey(PhoneNumber)))
                        {
                            _Two_A_mem[PhoneNumber] = code;
                        }
                        else
                        {
                            _Two_A_mem.TryAdd(PhoneNumber, code);
                        }
                        

                        if (code != 0)
                        {
                            return Ok($"DEGUB_OPT:Code = {code}");
                        }
                        else
                        {
                            BadRequest();
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    return BadRequest($"{PhoneNumber} is not a correct number of phone");
                }

            }
            else
            {
                return BadRequest("Phone Number is requeired!");
            }

            return NoContent();
        }

        [HttpGet("Sign")]
        public async Task<bool> SingIt([FromQuery] string PhoneNumber, [FromQuery] string Code)
        {
            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                Regex r = new(@"\+[0-9]*\(\d{3}\)\d{3}-\d{2}-\d{2}$");
                if (r.IsMatch(PhoneNumber))
                {
                    if (await Task.Run(() => _Two_A_mem.ContainsKey(PhoneNumber)))
                    {
                        int code = 0;
                        if(!int.TryParse(Code, out code))
                        {
                            return false;
                        }
                        else if (_Two_A_mem[PhoneNumber] == code)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
